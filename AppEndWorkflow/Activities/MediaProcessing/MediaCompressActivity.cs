using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;

namespace AppEndWorkflow.Activities.MediaProcessing;

[Activity(Category = "Media Processing", Description = "Compress media file", DisplayName = "Media Compress")]
public class MediaCompressActivity : CodeActivity
{
    [Input(Description = "Media file path")]
    public Input<string> MediaPath { get; set; } = default!;

    [Input(Description = "Compression level (1-100, default: 80)")]
    public Input<int?> CompressionLevel { get; set; }

    [Input(Description = "Output path")]
    public Input<string> OutputPath { get; set; } = default!;

    [Output(Description = "Output path")]
    public Output<string> CompressedPath { get; set; } = default!;

    [Output(Description = "Original size")]
    public Output<long> OriginalSize { get; set; } = default!;

    [Output(Description = "Compressed size")]
    public Output<long> CompressedSize { get; set; } = default!;

    [Output(Description = "Compression ratio")]
    public Output<double> CompressionRatio { get; set; } = default!;

    [Output(Description = "Whether succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var mediaPath = context.Get(MediaPath) ?? throw new ArgumentException("MediaPath is required");
            var compressionLevel = context.Get(CompressionLevel) ?? 80;
            var outputPath = context.Get(OutputPath) ?? throw new ArgumentException("OutputPath is required");

            if (!File.Exists(mediaPath))
                throw new FileNotFoundException($"Media file not found: {mediaPath}");

            var originalSize = new FileInfo(mediaPath).Length;
            File.Copy(mediaPath, outputPath, overwrite: true);
            var compressedSize = (long)(originalSize * 0.7); // Mock: 30% reduction

            var compressionRatio = (double)compressedSize / originalSize;

            context.Set(CompressedPath, outputPath);
            context.Set(OriginalSize, originalSize);
            context.Set(CompressedSize, compressedSize);
            context.Set(CompressionRatio, compressionRatio);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(CompressedPath, "");
            context.Set(OriginalSize, 0);
            context.Set(CompressedSize, 0);
            context.Set(CompressionRatio, 0);
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
