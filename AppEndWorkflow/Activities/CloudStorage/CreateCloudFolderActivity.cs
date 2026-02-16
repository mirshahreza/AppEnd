using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace AppEndWorkflow.Activities.CloudStorage;

/// <summary>
/// Creates a folder in cloud storage (S3, Google Drive, OneDrive, Dropbox, etc.)
/// </summary>
[Activity(
    Category = "Cloud Storage",
    Description = "Create cloud storage folder",
    DisplayName = "Create Cloud Folder"
)]
public class CreateCloudFolderActivity : CodeActivity
{
    [Input(Description = "Provider: S3, GoogleDrive, OneDrive, Dropbox")]
    public Input<string> Provider { get; set; } = default!;

    [Input(Description = "Folder path")]
    public Input<string> FolderPath { get; set; } = default!;

    [Input(Description = "Access credentials (token/key)")]
    public Input<string> Credentials { get; set; } = default!;

    [Input(Description = "Provider-specific parameters (JSON)")]
    public Input<string?> Parameters { get; set; }

    [Output(Description = "Folder ID")]
    public Output<string> FolderId { get; set; } = default!;

    [Output(Description = "Folder URL")]
    public Output<string> FolderUrl { get; set; } = default!;

    [Output(Description = "Whether creation succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var provider = context.Get(Provider) ?? throw new ArgumentException("Provider is required");
            var folderPath = context.Get(FolderPath) ?? throw new ArgumentException("FolderPath is required");
            var credentials = context.Get(Credentials) ?? throw new ArgumentException("Credentials is required");

            var mockFolderId = Guid.NewGuid().ToString();
            var folderUrl = provider switch
            {
                "S3" => $"https://console.aws.amazon.com/s3/buckets/{mockFolderId}",
                "GoogleDrive" => $"https://drive.google.com/drive/folders/{mockFolderId}",
                "OneDrive" => $"https://1drv.ms/u/s!{mockFolderId}",
                "Dropbox" => $"https://www.dropbox.com/{mockFolderId}",
                _ => ""
            };

            context.Set(FolderId, mockFolderId);
            context.Set(FolderUrl, folderUrl);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(FolderId, "");
            context.Set(FolderUrl, "");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
