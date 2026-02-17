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

	if (AppEndSettings.IsDevelopment)
		EnsureZyncPackages();
	InitializeScheduler(app.Services);

	app.Lifetime.ApplicationStopping.Register(LogMan.Flush);
	app.Lifetime.ApplicationStopped.Register(LogMan.Flush);

	app.UseCors("AppEndPolicy");
	app.UseResponseCompression();
	app.UseForwardedHeaders(GetForwardedHeadersOptions());
	app.UseHttpsRedirection();
	app.UseRpcNet();
	app.UseFileServer(GetFileServerOptions());
	app.UseRouting();
	
	// Phase 4: Configure workflow endpoints
	app.ConfigurePhase4Endpoints();
	
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

	builder.Services.AddSingleton<SchedulerService>();
	builder.Services.AddSingleton<SchedulerManager>();
	builder.Services.AddHostedService(sp => sp.GetRequiredService<SchedulerService>());

	// NEW: Add Elsa Workflow Engine
	// Use DefaultConnection (same as AppEnd's DefaultRepo)
	var workflowDbConnection = builder.Configuration.GetConnectionString("DefaultConnection")
		?? "Server=localhost;Database=AppEnd;Integrated Security=true;";

	builder.Services.AddAppEndWorkflows(workflowDbConnection, builder.Configuration);

	// Phase 4: Add Operations & UI Services
	builder.Services.AddPhase4Services();

	return builder;
}

static void EnsureZyncPackages()
{
	try
	{
		ZyncEnsure.EnsurePackages(AppEndSettings.LoginDbConfName);
	}
	catch (Exception ex)
	{
		Console.WriteLine($"[ZyncEnsure] Startup check failed: {ex.Message}");
	}
}

static void InitializeScheduler(IServiceProvider services)
{
	try
	{
		var schedulerManager = services.GetRequiredService<SchedulerManager>();

		SchedulerService.SetManager(schedulerManager);
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
