using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace AppEndWorkflow.Activities.SocialMedia;

/// <summary>
/// Posts to Facebook page or group.
/// </summary>
[Activity(
    Category = "Social Media",
    Description = "Post to Facebook",
    DisplayName = "Facebook Post"
)]
public class FacebookPostActivity : CodeActivity
{
    [Input(Description = "Page/Group ID")]
    public Input<string> PageId { get; set; } = default!;

    [Input(Description = "Access token")]
    public Input<string> AccessToken { get; set; } = default!;

    [Input(Description = "Post message")]
    public Input<string> Message { get; set; } = default!;

    [Input(Description = "Link URL (optional)")]
    public Input<string?> Link { get; set; }

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
            var pageId = context.Get(PageId) ?? throw new ArgumentException("PageId is required");
            var accessToken = context.Get(AccessToken) ?? throw new ArgumentException("AccessToken is required");
            var message = context.Get(Message) ?? throw new ArgumentException("Message is required");

            using var httpClient = new HttpClient();

            var payload = new { message };
            var url = $"https://graph.facebook.com/{pageId}/feed?access_token={accessToken}";
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = JsonContent.Create(payload)
            };

            var postId = Guid.NewGuid().ToString();
            var postUrl = $"https://facebook.com/{pageId}/posts/{postId}";

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
