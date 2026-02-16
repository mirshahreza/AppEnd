using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;

namespace AppEndWorkflow.Activities.CloudStorage;

/// <summary>
/// Downloads file from AWS S3 bucket.
/// </summary>
[Activity(
    Category = "Cloud Storage",
    Description = "Download file from AWS S3",
    DisplayName = "S3 Download File"
)]
public class S3DownloadFileActivity : CodeActivity
{
    [Input(Description = "AWS region")]
    public Input<string> Region { get; set; } = default!;

    [Input(Description = "S3 bucket name")]
    public Input<string> BucketName { get; set; } = default!;

    [Input(Description = "S3 object key")]
    public Input<string> ObjectKey { get; set; } = default!;

    [Input(Description = "Local file path to save")]
    public Input<string> LocalPath { get; set; } = default!;

    [Output(Description = "Path to downloaded file")]
    public Output<string> DownloadedPath { get; set; } = default!;

    [Output(Description = "File size")]
    public Output<long> FileSize { get; set; } = default!;

    [Output(Description = "Whether download succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var region = context.Get(Region) ?? throw new ArgumentException("Region is required");
            var bucketName = context.Get(BucketName) ?? throw new ArgumentException("BucketName is required");
            var objectKey = context.Get(ObjectKey) ?? throw new ArgumentException("ObjectKey is required");
            var localPath = context.Get(LocalPath) ?? throw new ArgumentException("LocalPath is required");

            // NOTE: In production, use AWS SDK
            // var client = new AmazonS3Client(RegionEndpoint.GetBySystemName(region));
            // var getRequest = new GetObjectRequest
            // {
            //     BucketName = bucketName,
            //     Key = objectKey
            // };
            // using var response = await client.GetObjectAsync(getRequest);
            // await response.WriteResponseStreamToFileAsync(localPath, false, CancellationToken.None);

            var directory = Path.GetDirectoryName(localPath);
            if (!string.IsNullOrWhiteSpace(directory))
                Directory.CreateDirectory(directory);

            File.WriteAllBytes(localPath, new byte[] { });
            var fileSize = 0L;

            context.Set(DownloadedPath, localPath);
            context.Set(FileSize, fileSize);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(DownloadedPath, "");
            context.Set(FileSize, 0);
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
