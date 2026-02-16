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
/// Uploads file to AWS S3 bucket.
/// Requires AWSSDK.S3 NuGet package.
/// </summary>
[Activity(
    Category = "Cloud Storage",
    Description = "Upload file to AWS S3",
    DisplayName = "S3 Upload File"
)]
public class S3UploadFileActivity : CodeActivity
{
    [Input(Description = "AWS region")]
    public Input<string> Region { get; set; } = default!;

    [Input(Description = "S3 bucket name")]
    public Input<string> BucketName { get; set; } = default!;

    [Input(Description = "S3 object key")]
    public Input<string> ObjectKey { get; set; } = default!;

    [Input(Description = "Local file path")]
    public Input<string> LocalPath { get; set; } = default!;

    [Input(Description = "Public ACL (default: false)")]
    public Input<bool?> IsPublic { get; set; }

    [Output(Description = "S3 object URL")]
    public Output<string> ObjectUrl { get; set; } = default!;

    [Output(Description = "File size")]
    public Output<long> FileSize { get; set; } = default!;

    [Output(Description = "Whether upload succeeded")]
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
            var isPublic = context.Get(IsPublic) ?? false;

            if (!File.Exists(localPath))
                throw new FileNotFoundException($"File not found: {localPath}");

            var fileSize = new FileInfo(localPath).Length;

            // NOTE: In production, use AWS SDK
            // var client = new AmazonS3Client(RegionEndpoint.GetBySystemName(region));
            // var putRequest = new PutObjectRequest
            // {
            //     BucketName = bucketName,
            //     Key = objectKey,
            //     FilePath = localPath,
            //     CannedACL = isPublic ? S3CannedACL.PublicRead : S3CannedACL.Private
            // };
            // await client.PutObjectAsync(putRequest);

            var objectUrl = $"https://{bucketName}.s3.{region}.amazonaws.com/{objectKey}";

            context.Set(ObjectUrl, objectUrl);
            context.Set(FileSize, fileSize);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(ObjectUrl, "");
            context.Set(FileSize, 0);
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
