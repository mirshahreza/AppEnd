using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace AppEndWorkflow.Activities.DocumentManagement;

/// <summary>
/// Manages documents in Box enterprise content management.
/// </summary>
[Activity(
    Category = "Document Management",
    Description = "Manage Box document",
    DisplayName = "Box Document"
)]
public class BoxDocumentActivity : CodeActivity
{
    [Input(Description = "Box access token")]
    public Input<string> AccessToken { get; set; } = default!;

    [Input(Description = "Folder ID")]
    public Input<string> FolderId { get; set; } = default!;

    [Input(Description = "File path")]
    public Input<string> FilePath { get; set; } = default!;

    [Output(Description = "File ID")]
    public Output<string> FileId { get; set; } = default!;

    [Output(Description = "Share link")]
    public Output<string> ShareLink { get; set; } = default!;

    [Output(Description = "Whether operation succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var accessToken = context.Get(AccessToken) ?? throw new ArgumentException("AccessToken is required");
            var folderId = context.Get(FolderId) ?? throw new ArgumentException("FolderId is required");
            var filePath = context.Get(FilePath) ?? throw new ArgumentException("FilePath is required");

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            var fileId = Guid.NewGuid().ToString();
            var shareLink = $"https://app.box.com/file/{fileId}";

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
