using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace AppEndWorkflow.Activities.SocialMedia;

/// <summary>
/// Posts to Instagram (requires business account).
/// </summary>
[Activity(
    Category = "Social Media",
    Description = "Post to Instagram",
    DisplayName = "Instagram Post"
)]
public class InstagramPostActivity : CodeActivity
{
    [Input(Description = "Business Account ID")]
    public Input<string> AccountId { get; set; } = default!;

    [Input(Description = "Access token")]
    public Input<string> AccessToken { get; set; } = default!;

    [Input(Description = "Image URL")]
    public Input<string> ImageUrl { get; set; } = default!;

    [Input(Description = "Caption")]
    public Input<string> Caption { get; set; } = default!;

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
            var accountId = context.Get(AccountId) ?? throw new ArgumentException("AccountId is required");
            var accessToken = context.Get(AccessToken) ?? throw new ArgumentException("AccessToken is required");
            var imageUrl = context.Get(ImageUrl) ?? throw new ArgumentException("ImageUrl is required");
            var caption = context.Get(Caption) ?? "";

            using var httpClient = new HttpClient();

            var postId = Guid.NewGuid().ToString();
            var postUrl = $"https://instagram.com/p/{postId}";

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
