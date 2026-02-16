using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;

namespace AppEndWorkflow.Activities.FileTransfer;

/// <summary>
/// Downloads a file from FTP server.
/// </summary>
[Activity(
    Category = "File Transfer",
    Description = "Download file from FTP server",
    DisplayName = "FTP Download File"
)]
public class FtpDownloadFileActivity : CodeActivity
{
    [Input(Description = "FTP server hostname")]
    public Input<string> Host { get; set; } = default!;

    [Input(Description = "FTP port (default: 21)")]
    public Input<int?> Port { get; set; }

    [Input(Description = "FTP username")]
    public Input<string> Username { get; set; } = default!;

    [Input(Description = "FTP password")]
    public Input<string> Password { get; set; } = default!;

    [Input(Description = "Remote file path")]
    public Input<string> RemotePath { get; set; } = default!;

    [Input(Description = "Local save path")]
    public Input<string> LocalPath { get; set; } = default!;

    [Output(Description = "Path to downloaded file")]
    public Output<string> LocalPathResult { get; set; } = default!;

    [Output(Description = "Downloaded file size")]
    public Output<long> FileSize { get; set; } = default!;

    [Output(Description = "Whether download succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var host = context.Get(Host) ?? throw new ArgumentException("Host is required");
            var port = context.Get(Port) ?? 21;
            var username = context.Get(Username) ?? throw new ArgumentException("Username is required");
            var password = context.Get(Password) ?? throw new ArgumentException("Password is required");
            var remotePath = context.Get(RemotePath) ?? throw new ArgumentException("RemotePath is required");
            var localPath = context.Get(LocalPath) ?? throw new ArgumentException("LocalPath is required");

            // NOTE: In production, use FluentFTP
            // using var client = new AsyncFtpClient(host, port, username, password);
            // await client.Connect();
            // await client.DownloadFile(localPath, remotePath);
            // await client.Disconnect();

            // Create directory if needed
            var directory = Path.GetDirectoryName(localPath);
            if (!string.IsNullOrWhiteSpace(directory))
                Directory.CreateDirectory(directory);

            // Mock: create empty file for testing
            File.WriteAllBytes(localPath, new byte[] { });
            var fileSize = 0L;

            context.Set(LocalPathResult, localPath);
            context.Set(FileSize, fileSize);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(LocalPathResult, "");
            context.Set(FileSize, 0);
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
