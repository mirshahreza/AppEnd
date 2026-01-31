using AppEndCommon;
using AppEndServer;
using AppEndDynaCode;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO.Compression;
using System.Net;

#if DEBUG
// Ensure Development environment when debugging
Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
#endif

try
{
	LogMan.SetupLoggers();
	AppEndDynaCode.DynaCodeGuard.TryRefresh();

	var builder = ConfigServices(CreateBuilder(args));
	var app = builder.Build();

	InitializeScheduler(app.Services);

	app.Lifetime.ApplicationStopping.Register(LogMan.Flush);
	app.Lifetime.ApplicationStopped.Register(LogMan.Flush);

	app.UseCors("AppEndPolicy");
	app.UseResponseCompression();
	app.UseForwardedHeaders(GetForwardedHeadersOptions());
	app.UseHttpsRedirection();
	app.UseSession();
	
	// Google OAuth endpoints
	ConfigureGoogleOAuthEndpoints(app);
	
	app.UseRpcNet();
	app.UseFileServer(GetFileServerOptions());
	app.UseRouting();
	app.Run();
}
catch (Exception ex)
{
	LogMan.LogError(ex);
	throw;
}
finally
{
	LogMan.Flush();
}

static WebApplicationBuilder CreateBuilder(string[] args)
{
	var builder = WebApplication.CreateBuilder(new WebApplicationOptions
	{
		Args = args,
		WebRootPath = AppEndSettings.ClientObjectsPath
	});
	return builder;
}

static WebApplicationBuilder ConfigServices(WebApplicationBuilder builder)
{
	builder.Services.AddCors(o => o.AddPolicy("AppEndPolicy", builder =>
	{
		builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
	}));
	builder.Services.Configure<ForwardedHeadersOptions>(options =>
	{
		options.KnownProxies.Add(IPAddress.Parse("10.0.0.100"));
	});

	builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
	{
		options.Level = CompressionLevel.Optimal;
	});
	builder.Services.AddResponseCompression(options =>
	{
		options.EnableForHttps = true;
		options.Providers.Add<BrotliCompressionProvider>();
	});

	// Add session support for OAuth
	builder.Services.AddDistributedMemoryCache();
	builder.Services.AddSession(options =>
	{
		options.IdleTimeout = TimeSpan.FromMinutes(10);
		options.Cookie.HttpOnly = true;
		options.Cookie.IsEssential = true;
	});

	builder.Services.AddSingleton<SchedulerService>();
	builder.Services.AddSingleton<SchedulerManager>();
	builder.Services.AddHostedService(sp => sp.GetRequiredService<SchedulerService>());

	return builder;
}

static void InitializeScheduler(IServiceProvider services)
{
	try
	{
		var schedulerService = services.GetRequiredService<SchedulerService>();
		var schedulerManager = services.GetRequiredService<SchedulerManager>();
		Zzz.AppEndProxy.InitializeScheduler(schedulerManager);
		Console.WriteLine("[Scheduler] Initialized successfully");
	}
	catch (Exception ex)
	{
		Console.WriteLine($"[Scheduler] Initialization failed: {ex.Message}");
	}
}


static FileServerOptions GetFileServerOptions()
{
	var provider = new FileExtensionContentTypeProvider();
	provider.Mappings[".vue"] = "application/x-msdownload";
	FileServerOptions fileServerOptions = new();
	fileServerOptions.StaticFileOptions.ContentTypeProvider = provider;
	return fileServerOptions;
}

static ForwardedHeadersOptions GetForwardedHeadersOptions()
{
	return new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto };
}

static void ConfigureGoogleOAuthEndpoints(WebApplication app)
{
	if (!AppEndServer.GoogleOAuthServices.IsEnabled)
	{
		return;
	}

	// Get Google OAuth configuration
	var clientId = AppEndServer.GoogleOAuthServices.ClientId;
	if (string.IsNullOrEmpty(clientId))
	{
		return;
	}

	// Start OAuth flow endpoint
	app.MapGet("/auth/google/start", async (HttpContext context) =>
	{
		var state = Guid.NewGuid().ToString();
		context.Session.SetString("oauth_state", state);
		
		// Build redirect URI from current request
		var request = context.Request;
		var scheme = request.Scheme;
		var host = request.Host.Value;
		var redirectUri = $"{scheme}://{host}/auth/google/callback";
		
		var authUrl = $"https://accounts.google.com/o/oauth2/v2/auth?" +
			$"client_id={Uri.EscapeDataString(clientId)}&" +
			$"redirect_uri={Uri.EscapeDataString(redirectUri)}&" +
			$"response_type=code&" +
			$"scope={Uri.EscapeDataString("openid email profile")}&" +
			$"state={Uri.EscapeDataString(state)}&" +
			$"access_type=offline&" +
			$"prompt=consent";

		return Results.Redirect(authUrl);
	});

	// OAuth callback endpoint
	app.MapGet("/auth/google/callback", async (HttpContext context) =>
	{
		var code = context.Request.Query["code"].ToString();
		var state = context.Request.Query["state"].ToString();
		var storedState = context.Session.GetString("oauth_state");

		if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(state) || state != storedState)
		{
			return Results.Redirect("/?error=oauth_failed");
		}

		// Build redirect URI from current request
		var request = context.Request;
		var scheme = request.Scheme;
		var host = request.Host.Value;
		var redirectUri = $"{scheme}://{host}/auth/google/callback";

		// Exchange code for token
		var clientSecret = AppEndServer.GoogleOAuthServices.ClientSecret;
		using var httpClient = new HttpClient();
		
		var tokenRequest = new Dictionary<string, string>
		{
			{ "code", code },
			{ "client_id", clientId ?? "" },
			{ "client_secret", clientSecret ?? "" },
			{ "redirect_uri", redirectUri },
			{ "grant_type", "authorization_code" }
		};

		var tokenResponse = await httpClient.PostAsync("https://oauth2.googleapis.com/token",
			new FormUrlEncodedContent(tokenRequest));

		if (!tokenResponse.IsSuccessStatusCode)
		{
			return Results.Redirect("/?error=token_exchange_failed");
		}

		var tokenContent = await tokenResponse.Content.ReadAsStringAsync();
		var tokenJson = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(tokenContent);
		var idToken = tokenJson.GetProperty("id_token").GetString();

		if (string.IsNullOrEmpty(idToken))
		{
			return Results.Redirect("/?error=no_id_token");
		}

		// Verify and get user info
		var payload = await AppEndServer.GoogleOAuthServices.VerifyTokenAsync(idToken);
		if (payload == null)
		{
			return Results.Redirect("/?error=token_verification_failed");
		}

		// Call LoginWithGoogle method directly
		var loginResult = AppEndDynaCode.DynaCode.InvokeByJsonInputs(
			"Zzz.AppEndProxy.LoginWithGoogle",
			System.Text.Json.JsonSerializer.SerializeToElement(new { IdToken = idToken }),
			AppEndServer.ActorServices.GetNobodyUser(),
			context.Request.GetClientIp(),
			context.Request.GetClientAgent()
		);

		if (loginResult.IsSucceeded == true && loginResult.Result != null)
		{
			var resultDict = loginResult.Result as System.Collections.Hashtable;
			if (resultDict != null && resultDict["Result"]?.ToString() == "True")
			{
				var token = resultDict["token"]?.ToString();
				if (!string.IsNullOrEmpty(token))
				{
					// Store token in session and redirect
					context.Session.SetString("google_auth_token", token);
					return Results.Redirect($"/?google_auth_token={Uri.EscapeDataString(token)}");
				}
			}
		}

		return Results.Redirect("/?error=login_failed");
	});
}
