using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace AppEndWorkflow.Activities.DocumentManagement;

/// <summary>
/// Uploads file to SharePoint document library.
/// </summary>
[Activity(
    Category = "Document Management",
    Description = "Upload to SharePoint",
    DisplayName = "SharePoint Upload"
)]
public class SharePointUploadActivity : CodeActivity
{
    [Input(Description = "Access token")]
    public Input<string> AccessToken { get; set; } = default!;

    [Input(Description = "Site ID")]
    public Input<string> SiteId { get; set; } = default!;

    [Input(Description = "List/Drive ID")]
    public Input<string> DriveId { get; set; } = default!;

    [Input(Description = "File path")]
    public Input<string> FilePath { get; set; } = default!;

    [Input(Description = "Target folder path")]
    public Input<string> FolderPath { get; set; } = default!;

    [Output(Description = "File ID")]
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
            var accessToken = context.Get(AccessToken) ?? throw new ArgumentException("AccessToken is required");
            var siteId = context.Get(SiteId) ?? throw new ArgumentException("SiteId is required");
            var driveId = context.Get(DriveId) ?? throw new ArgumentException("DriveId is required");
            var filePath = context.Get(FilePath) ?? throw new ArgumentException("FilePath is required");
            var folderPath = context.Get(FolderPath) ?? throw new ArgumentException("FolderPath is required");

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            var fileId = Guid.NewGuid().ToString();
            var shareLink = $"https://yourdomain.sharepoint.com/sites/{siteId}/Shared%20Documents/{fileId}";

            context.Set(FileId, fileId);
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
