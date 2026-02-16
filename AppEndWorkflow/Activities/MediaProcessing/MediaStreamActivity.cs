using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;

namespace AppEndWorkflow.Activities.MediaProcessing;

[Activity(Category = "Media Processing", Description = "Stream media content", DisplayName = "Media Stream")]
public class MediaStreamActivity : CodeActivity
{
    [Input(Description = "Media source URL or path")]
    public Input<string> Source { get; set; } = default!;

    [Input(Description = "Stream format (hls, dash, smooth, progressive)")]
    public Input<string> Format { get; set; } = default!;

    [Input(Description = "Bitrates (comma-separated)")]
    public Input<string?> Bitrates { get; set; }

    [Output(Description = "Stream URL")]
    public Output<string> StreamUrl { get; set; } = default!;

    [Output(Description = "Manifest URL")]
    public Output<string> ManifestUrl { get; set; } = default!;

    [Output(Description = "Whether succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var source = context.Get(Source) ?? throw new ArgumentException("Source is required");
            var format = context.Get(Format) ?? throw new ArgumentException("Format is required");

            var streamUrl = $"https://stream.example.com/{Guid.NewGuid()}/stream.{format}";
            var manifestUrl = $"https://stream.example.com/{Guid.NewGuid()}/manifest.m3u8";

            context.Set(StreamUrl, streamUrl);
            context.Set(ManifestUrl, manifestUrl);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(StreamUrl, "");
            context.Set(ManifestUrl, "");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
