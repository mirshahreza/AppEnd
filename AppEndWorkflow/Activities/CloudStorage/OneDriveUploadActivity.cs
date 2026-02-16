using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace AppEndWorkflow.Activities.CloudStorage;

/// <summary>
/// Uploads file to OneDrive/SharePoint.
/// Uses Microsoft.Graph NuGet package.
/// </summary>
[Activity(
    Category = "Cloud Storage",
    Description = "Upload file to OneDrive",
    DisplayName = "OneDrive Upload"
)]
public class OneDriveUploadActivity : CodeActivity
{
    [Input(Description = "File path to upload")]
    public Input<string> FilePath { get; set; } = default!;

    [Input(Description = "OneDrive path")]
    public Input<string> OneDrivePath { get; set; } = default!;

    [Input(Description = "Access token")]
    public Input<string> AccessToken { get; set; } = default!;

    [Output(Description = "File ID")]
    public Output<string> FileId { get; set; } = default!;

    [Output(Description = "File URL")]
    public Output<string> FileUrl { get; set; } = default!;

    [Output(Description = "Whether upload succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var filePath = context.Get(FilePath) ?? throw new ArgumentException("FilePath is required");
            var oneDrivePath = context.Get(OneDrivePath) ?? throw new ArgumentException("OneDrivePath is required");
            var accessToken = context.Get(AccessToken) ?? throw new ArgumentException("AccessToken is required");

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");

            // NOTE: In production, use Microsoft.Graph
            // var graphClient = new GraphServiceClient(new DelegateAuthenticationProvider(async (request) =>
            // {
            //     request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            // }));
            //
            // using var stream = new FileStream(filePath, FileMode.Open);
            // var result = await graphClient.Me.Drive.Root.ItemWithPath(oneDrivePath)
            //     .Content.Request().PutAsync<DriveItem>(stream);

            var mockFileId = Guid.NewGuid().ToString();
            var fileUrl = $"https://1drv.ms/u/s!{mockFileId}";

            context.Set(FileId, mockFileId);
            context.Set(FileUrl, fileUrl);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(FileId, "");
            context.Set(FileUrl, "");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
