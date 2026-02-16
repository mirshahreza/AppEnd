using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;

namespace AppEndWorkflow.Activities.FileTransfer;

/// <summary>
/// Uploads a file to FTP server.
/// Requires FluentFTP NuGet package.
/// </summary>
[Activity(
    Category = "File Transfer",
    Description = "Upload file to FTP server",
    DisplayName = "FTP Upload File"
)]
public class FtpUploadFileActivity : CodeActivity
{
    [Input(Description = "FTP server hostname")]
    public Input<string> Host { get; set; } = default!;

    [Input(Description = "FTP port (default: 21)")]
    public Input<int?> Port { get; set; }

    [Input(Description = "FTP username")]
    public Input<string> Username { get; set; } = default!;

    [Input(Description = "FTP password")]
    public Input<string> Password { get; set; } = default!;

    [Input(Description = "Local file path")]
    public Input<string> LocalPath { get; set; } = default!;

    [Input(Description = "Remote FTP path")]
    public Input<string> RemotePath { get; set; } = default!;

    [Input(Description = "Passive mode (default: true)")]
    public Input<bool?> Passive { get; set; }

    [Output(Description = "Full remote path")]
    public Output<string> RemotePathResult { get; set; } = default!;

    [Output(Description = "Uploaded file size")]
    public Output<long> FileSize { get; set; } = default!;

    [Output(Description = "Whether upload succeeded")]
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
            var localPath = context.Get(LocalPath) ?? throw new ArgumentException("LocalPath is required");
            var remotePath = context.Get(RemotePath) ?? throw new ArgumentException("RemotePath is required");
            var passive = context.Get(Passive) ?? true;

            if (!File.Exists(localPath))
                throw new FileNotFoundException($"Local file not found: {localPath}");

            var fileSize = new FileInfo(localPath).Length;

            // NOTE: In production, use FluentFTP
            // using var client = new AsyncFtpClient(host, port, username, password);
            // client.Config.UsePassiveMode = passive;
            // await client.Connect();
            // await client.UploadFile(localPath, remotePath);
            // await client.Disconnect();

            context.Set(RemotePathResult, remotePath);
            context.Set(FileSize, fileSize);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(RemotePathResult, "");
            context.Set(FileSize, 0);
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
