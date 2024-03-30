using AppEndCommon;
using AppEndServer;
using Microsoft.AspNetCore.StaticFiles;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    WebRootPath = AppEndSettings.ClientObjectsPath
});
var app = builder.Build();
//app.Urls.Add("http://localhost:3000");
//app.Urls.Add("http://localhost:4000");

app.UseHttpsRedirection();

var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".vue"] = "application/x-msdownload";
FileServerOptions fileServerOptions = new();
fileServerOptions.StaticFileOptions.ContentTypeProvider = provider;
app.UseFileServer(fileServerOptions);
app.UseRpcNet();

app.Run();



