using AppEndCommon;
using AppEndServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.StaticFiles;
using System.IO.Compression;
using System.Net;

var builder = ConfigServices(CreateBuilder(args));
var app = builder.Build();

app.UseCors("AppEndPolicy");
app.UseResponseCompression();
app.UseForwardedHeaders(GetForwardedHeadersOptions());
app.UseHttpsRedirection();
app.UseRpcNet();
app.UseFileServer(GetFileServerOptions());
app.UseRouting();
app.Run();


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
	return builder;
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


//app.Urls.Add("http://localhost:3000");
//app.Urls.Add("http://localhost:4000");

//fileServerOptions.StaticFileOptions.OnPrepareResponse = ctx =>
//{
//	ctx.Context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.CacheControl] = "public,max-age=" + (60 * 60 * 24);

//	//ctx.Context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.CacheControl] = "s-maxage=1, stale-while-revalidate=59";
//	//var headers = ctx.Context.Response.GetTypedHeaders();
//	//headers.CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue
//	//{
//	//	Public = true,
//	//	MaxAge = TimeSpan.FromDays(30)
//	//};

//	//ctx.Context.Response.Headers["cache-control"] = "public,max-age=31536000";
//	//ctx.Context.Response.Headers["expires"] = DateTime.UtcNow.AddYears(1).ToString("R");

//};
