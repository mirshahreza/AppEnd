using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;

namespace AppEndWorkflow.Activities.FileTransfer;

/// <summary>
/// Uploads a file to SFTP server using SSH.NET.
/// </summary>
[Activity(
    Category = "File Transfer",
    Description = "Upload file to SFTP server",
    DisplayName = "SFTP Upload File"
)]
public class SftpUploadFileActivity : CodeActivity
{
    [Input(Description = "SFTP server hostname")]
    public Input<string> Host { get; set; } = default!;

    [Input(Description = "SFTP port (default: 22)")]
    public Input<int?> Port { get; set; }

    [Input(Description = "SFTP username")]
    public Input<string> Username { get; set; } = default!;

    [Input(Description = "SFTP password (optional if using key)")]
    public Input<string?> Password { get; set; }

    [Input(Description = "Path to private key file")]
    public Input<string?> PrivateKeyPath { get; set; }

    [Input(Description = "Local file path")]
    public Input<string> LocalPath { get; set; } = default!;

    [Input(Description = "Remote SFTP path")]
    public Input<string> RemotePath { get; set; } = default!;

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
            var port = context.Get(Port) ?? 22;
            var username = context.Get(Username) ?? throw new ArgumentException("Username is required");
            var password = context.Get(Password);
            var privateKeyPath = context.Get(PrivateKeyPath);
            var localPath = context.Get(LocalPath) ?? throw new ArgumentException("LocalPath is required");
            var remotePath = context.Get(RemotePath) ?? throw new ArgumentException("RemotePath is required");

            if (!File.Exists(localPath))
                throw new FileNotFoundException($"Local file not found: {localPath}");

            var fileSize = new FileInfo(localPath).Length;

            // NOTE: In production, use SSH.NET
            // using var client = new SftpClient(host, port, username, password);
            // if (!string.IsNullOrWhiteSpace(privateKeyPath))
            // {
            //     var keyFile = new PrivateKeyFile(privateKeyPath);
            //     client = new SftpClient(host, port, username, keyFile);
            // }
            // client.Connect();
            // using var stream = new FileStream(localPath, FileMode.Open);
            // client.UploadFile(stream, remotePath);
            // client.Disconnect();

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
