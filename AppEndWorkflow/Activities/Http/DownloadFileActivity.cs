using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Text.Json;

namespace AppEndWorkflow.Activities.Http;

/// <summary>
/// Downloads a file from a URL and saves it locally.
/// Uses streaming download to handle large files.
/// </summary>
[Activity(
    Category = "HTTP",
    Description = "Downloads a file from a URL",
    DisplayName = "Download File"
)]
public class DownloadFileActivity : CodeActivity
{
    /// <summary>
    /// File download URL
    /// </summary>
    [Input(Description = "File download URL")]
    public Input<string> Url { get; set; } = default!;

    /// <summary>
    /// JSON headers (auth, etc.)
    /// </summary>
    [Input(Description = "JSON headers (optional)")]
    public Input<string?> Headers { get; set; }

    /// <summary>
    /// Local path to save downloaded file
    /// </summary>
    [Input(Description = "Local path to save downloaded file")]
    public Input<string> SavePath { get; set; } = default!;

    /// <summary>
    /// Timeout (default: 120)
    /// </summary>
    [Input(Description = "Timeout in seconds")]
    public Input<int> TimeoutSeconds { get; set; } = new(120);

    /// <summary>
    /// Path to saved file
    /// </summary>
    [Output(Description = "Path to saved file")]
    public Output<string> FilePath { get; set; } = default!;

    /// <summary>
    /// Downloaded file size in bytes
    /// </summary>
    [Output(Description = "Downloaded file size in bytes")]
    public Output<long> FileSize { get; set; } = default!;

    /// <summary>
    /// MIME type of the file
    /// </summary>
    [Output(Description = "MIME type of the file")]
    public Output<string> ContentType { get; set; } = default!;

    /// <summary>
    /// Whether download succeeded
    /// </summary>
    [Output(Description = "Whether download succeeded")]
    public Output<bool> Success { get; set; } = default!;

    /// <summary>
    /// Error message if failed
    /// </summary>
    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var url = context.Get(Url);
            var headersJson = context.Get(Headers);
            var savePath = context.Get(SavePath);
            var timeoutSeconds = context.Get(TimeoutSeconds);

            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("'Url' is required");

            if (string.IsNullOrWhiteSpace(savePath))
                throw new ArgumentException("'SavePath' is required");

            // Download file
            var (filePath, fileSize, contentType) = DownloadFileInternal(url, savePath, headersJson, timeoutSeconds);

            context.Set(FilePath, filePath);
            context.Set(FileSize, fileSize);
            context.Set(ContentType, contentType);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(FilePath, "");
            context.Set(FileSize, 0);
            context.Set(ContentType, "");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }

    private (string filePath, long fileSize, string contentType) DownloadFileInternal(
        string url, string savePath, string? headersJson, int timeoutSeconds)
    {
        // Create directory if it doesn't exist
        var directory = Path.GetDirectoryName(savePath);
        if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        using var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromSeconds(timeoutSeconds);

        // Parse and add custom headers
        if (!string.IsNullOrWhiteSpace(headersJson))
        {
            try
            {
                using var doc = JsonDocument.Parse(headersJson);
                foreach (var property in doc.RootElement.EnumerateObject())
                {
                    httpClient.DefaultRequestHeaders.Add(property.Name, property.Value.GetString());
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to parse headers: {ex.Message}");
            }
        }

        // Download with streaming
        var response = httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).Result;

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"Failed to download file: HTTP {(int)response.StatusCode}");

        var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";

        // Save file with streaming
        using (var contentStream = response.Content.ReadAsStreamAsync().Result)
        using (var fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            contentStream.CopyTo(fileStream);
        }

        // Get file size
        var fileInfo = new FileInfo(savePath);
        return (savePath, fileInfo.Length, contentType);
    }
}
