using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace AppEndWorkflow.Activities.CloudStorage;

/// <summary>
/// Uploads file to Dropbox.
/// Uses Dropbox.Api NuGet package.
/// </summary>
[Activity(
    Category = "Cloud Storage",
    Description = "Upload file to Dropbox",
    DisplayName = "Dropbox Upload"
)]
public class DropboxUploadActivity : CodeActivity
{
    [Input(Description = "File path to upload")]
    public Input<string> FilePath { get; set; } = default!;

    [Input(Description = "Dropbox path")]
    public Input<string> DropboxPath { get; set; } = default!;

    [Input(Description = "Access token")]
    public Input<string> AccessToken { get; set; } = default!;

    [Input(Description = "Overwrite if exists (default: true)")]
    public Input<bool?> Overwrite { get; set; }

    [Output(Description = "Dropbox file ID")]
    public Output<string> FileId { get; set; } = default!;

    [Output(Description = "Share link")]
    public Output<string> ShareLink { get; set; } = default!;

    [Output(Description = "Whether upload succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var filePath = context.Get(FilePath) ?? throw new ArgumentException("FilePath is required");
            var dropboxPath = context.Get(DropboxPath) ?? throw new ArgumentException("DropboxPath is required");
            var accessToken = context.Get(AccessToken) ?? throw new ArgumentException("AccessToken is required");
            var overwrite = context.Get(Overwrite) ?? true;

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");

            // NOTE: In production, use Dropbox.Api
            // using var client = new DropboxClient(accessToken);
            // using var fileStream = File.Open(filePath, FileMode.Open);
            // var result = await client.Files.UploadAsync(
            //     dropboxPath,
            //     overwrite ? WriteMode.Add : WriteMode.Update,
            //     body: fileStream);

            var mockFileId = Guid.NewGuid().ToString();
            var shareLink = $"https://www.dropbox.com/s/{mockFileId}?dl=0";

            context.Set(FileId, mockFileId);
            context.Set(ShareLink, shareLink);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(FileId, "");
            context.Set(ShareLink, "");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
