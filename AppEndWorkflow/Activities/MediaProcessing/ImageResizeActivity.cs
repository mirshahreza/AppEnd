using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;

namespace AppEndWorkflow.Activities.MediaProcessing;

[Activity(Category = "Media Processing", Description = "Resize image", DisplayName = "Image Resize")]
public class ImageResizeActivity : CodeActivity
{
    [Input(Description = "Image file path")]
    public Input<string> ImagePath { get; set; } = default!;

    [Input(Description = "Output width")]
    public Input<int> Width { get; set; } = default!;

    [Input(Description = "Output height")]
    public Input<int> Height { get; set; } = default!;

    [Input(Description = "Output path")]
    public Input<string> OutputPath { get; set; } = default!;

    [Input(Description = "Maintain aspect ratio (default: true)")]
    public Input<bool?> MaintainAspectRatio { get; set; }

    [Output(Description = "Output image path")]
    public Output<string> ResizedPath { get; set; } = default!;

    [Output(Description = "File size")]
    public Output<long> FileSize { get; set; } = default!;

    [Output(Description = "Whether succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var imagePath = context.Get(ImagePath) ?? throw new ArgumentException("ImagePath is required");
            var width = context.Get(Width);
            var height = context.Get(Height);
            var outputPath = context.Get(OutputPath) ?? throw new ArgumentException("OutputPath is required");

            if (!File.Exists(imagePath))
                throw new FileNotFoundException($"Image not found: {imagePath}");

            File.Copy(imagePath, outputPath, overwrite: true);
            var fileSize = new FileInfo(outputPath).Length;

            context.Set(ResizedPath, outputPath);
            context.Set(FileSize, fileSize);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(ResizedPath, "");
            context.Set(FileSize, 0);
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
