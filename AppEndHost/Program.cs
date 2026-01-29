using AppEndCommon;
using AppEndServer;
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
