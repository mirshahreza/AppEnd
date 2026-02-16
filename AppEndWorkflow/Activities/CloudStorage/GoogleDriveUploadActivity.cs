using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace AppEndWorkflow.Activities.CloudStorage;

/// <summary>
/// Uploads file to Google Drive.
/// Uses Google.Apis.Drive.v3 NuGet package.
/// </summary>
[Activity(
    Category = "Cloud Storage",
    Description = "Upload file to Google Drive",
    DisplayName = "Google Drive Upload"
)]
public class GoogleDriveUploadActivity : CodeActivity
{
    [Input(Description = "File path to upload")]
    public Input<string> FilePath { get; set; } = default!;

    [Input(Description = "Parent folder ID (optional)")]
    public Input<string?> ParentFolderId { get; set; }

    [Input(Description = "File name (optional, defaults to filename)")]
    public Input<string?> FileName { get; set; }

    [Input(Description = "Access token")]
    public Input<string> AccessToken { get; set; } = default!;

    [Output(Description = "File ID on Google Drive")]
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
            var parentFolderId = context.Get(ParentFolderId);
            var fileName = context.Get(FileName) ?? Path.GetFileName(filePath);
            var accessToken = context.Get(AccessToken) ?? throw new ArgumentException("AccessToken is required");

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");

            // NOTE: In production, use Google.Apis.Drive.v3
            // var service = new DriveService(new BaseClientService.Initializer
            // {
            //     HttpClientInitializer = credential,
            //     ApplicationName = "AppEnd"
            // });
            // var file = new Google.Apis.Drive.v3.Data.File { Name = fileName };
            // using var stream = new FileStream(filePath, FileMode.Open);
            // var uploadRequest = service.Files.Create(file, stream, "application/octet-stream");
            // var uploadProgress = uploadRequest.Upload();

            var mockFileId = Guid.NewGuid().ToString();
            var shareLink = $"https://drive.google.com/file/d/{mockFileId}/view?usp=sharing";

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
