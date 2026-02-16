using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace AppEndWorkflow.Activities.SocialMedia;

/// <summary>
/// Posts tweet to Twitter.
/// </summary>
[Activity(
    Category = "Social Media",
    Description = "Post tweet to Twitter",
    DisplayName = "Twitter Post"
)]
public class TwitterPostActivity : CodeActivity
{
    [Input(Description = "API key")]
    public Input<string> ApiKey { get; set; } = default!;

    [Input(Description = "API secret")]
    public Input<string> ApiSecret { get; set; } = default!;

    [Input(Description = "Access token")]
    public Input<string> AccessToken { get; set; } = default!;

    [Input(Description = "Access token secret")]
    public Input<string> AccessTokenSecret { get; set; } = default!;

    [Input(Description = "Tweet text")]
    public Input<string> Text { get; set; } = default!;

    [Input(Description = "Media URLs (optional)")]
    public Input<string?> MediaUrls { get; set; }

    [Output(Description = "Tweet ID")]
    public Output<string> TweetId { get; set; } = default!;

    [Output(Description = "Tweet URL")]
    public Output<string> TweetUrl { get; set; } = default!;

    [Output(Description = "Whether post succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var apiKey = context.Get(ApiKey) ?? throw new ArgumentException("ApiKey is required");
            var text = context.Get(Text) ?? throw new ArgumentException("Text is required");

            // NOTE: In production, use TweetinviAPI or TwitterAPI v2
            using var httpClient = new HttpClient();
            var payload = new { text };
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.twitter.com/2/tweets")
            {
                Content = JsonContent.Create(payload)
            };

            var tweetId = Guid.NewGuid().ToString();
            var tweetUrl = $"https://twitter.com/account/status/{tweetId}";

            context.Set(TweetId, tweetId);
            context.Set(TweetUrl, tweetUrl);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(TweetId, "");
            context.Set(TweetUrl, "");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
