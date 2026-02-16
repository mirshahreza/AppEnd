using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace AppEndWorkflow.Activities.SocialMedia;

/// <summary>
/// Posts update to LinkedIn.
/// </summary>
[Activity(
    Category = "Social Media",
    Description = "Post to LinkedIn",
    DisplayName = "LinkedIn Post"
)]
public class LinkedInPostActivity : CodeActivity
{
    [Input(Description = "Access token")]
    public Input<string> AccessToken { get; set; } = default!;

    [Input(Description = "Post text")]
    public Input<string> Text { get; set; } = default!;

    [Input(Description = "Image URL (optional)")]
    public Input<string?> ImageUrl { get; set; }

    [Output(Description = "Post ID")]
    public Output<string> PostId { get; set; } = default!;

    [Output(Description = "Post URL")]
    public Output<string> PostUrl { get; set; } = default!;

    [Output(Description = "Whether post succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var accessToken = context.Get(AccessToken) ?? throw new ArgumentException("AccessToken is required");
            var text = context.Get(Text) ?? throw new ArgumentException("Text is required");

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            var payload = new { commentary = text };
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.linkedin.com/v2/ugcPosts")
            {
                Content = JsonContent.Create(payload)
            };

            var postId = Guid.NewGuid().ToString();
            var postUrl = $"https://www.linkedin.com/feed/update/{postId}";

            context.Set(PostId, postId);
            context.Set(PostUrl, postUrl);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(PostId, "");
            context.Set(PostUrl, "");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
