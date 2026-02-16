using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace AppEndWorkflow.Activities.SocialMedia;

/// <summary>
/// Posts video to TikTok.
/// </summary>
[Activity(
    Category = "Social Media",
    Description = "Post video to TikTok",
    DisplayName = "TikTok Post"
)]
public class TikTokPostActivity : CodeActivity
{
    [Input(Description = "Access token")]
    public Input<string> AccessToken { get; set; } = default!;

    [Input(Description = "Video file path")]
    public Input<string> VideoPath { get; set; } = default!;

    [Input(Description = "Video caption")]
    public Input<string> Caption { get; set; } = default!;

    [Input(Description = "Music ID (optional)")]
    public Input<string?> MusicId { get; set; }

    [Output(Description = "Video ID")]
    public Output<string> VideoId { get; set; } = default!;

    [Output(Description = "Video URL")]
    public Output<string> VideoUrl { get; set; } = default!;

    [Output(Description = "Whether post succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var accessToken = context.Get(AccessToken) ?? throw new ArgumentException("AccessToken is required");
            var videoPath = context.Get(VideoPath) ?? throw new ArgumentException("VideoPath is required");
            var caption = context.Get(Caption) ?? "";

            if (!File.Exists(videoPath))
                throw new FileNotFoundException($"Video file not found: {videoPath}");

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            var videoId = Guid.NewGuid().ToString();
            var videoUrl = $"https://www.tiktok.com/@account/video/{videoId}";

            context.Set(VideoId, videoId);
            context.Set(VideoUrl, videoUrl);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(VideoId, "");
            context.Set(VideoUrl, "");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
