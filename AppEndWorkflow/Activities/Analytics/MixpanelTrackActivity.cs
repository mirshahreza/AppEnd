using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace AppEndWorkflow.Activities.Analytics;

/// <summary>
/// Sends events to Mixpanel for analytics.
/// </summary>
[Activity(
    Category = "Analytics",
    Description = "Track event in Mixpanel",
    DisplayName = "Mixpanel Track"
)]
public class MixpanelTrackActivity : CodeActivity
{
    [Input(Description = "Mixpanel project token")]
    public Input<string> ProjectToken { get; set; } = default!;

    [Input(Description = "User ID (or distinct ID)")]
    public Input<string> UserId { get; set; } = default!;

    [Input(Description = "Event name")]
    public Input<string> EventName { get; set; } = default!;

    [Input(Description = "Event properties JSON")]
    public Input<string?> Properties { get; set; }

    [Output(Description = "Whether event was sent")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var projectToken = context.Get(ProjectToken) ?? throw new ArgumentException("ProjectToken is required");
            var userId = context.Get(UserId) ?? throw new ArgumentException("UserId is required");
            var eventName = context.Get(EventName) ?? throw new ArgumentException("EventName is required");
            var properties = context.Get(Properties) ?? "{}";

            using var httpClient = new HttpClient();

            var payload = new
            {
                data = new
                {
                    token = projectToken,
                    distinct_id = userId,
                    e = eventName,
                    @event = eventName,
                    properties = JsonSerializer.Deserialize<object>(properties)
                }
            };

            var url = "https://api.mixpanel.com/track";
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = JsonContent.Create(payload)
            };

            var response = httpClient.SendAsync(request).Result;

            context.Set(Success, response.IsSuccessStatusCode);
            context.Set(Error, response.IsSuccessStatusCode ? null : response.ReasonPhrase);
        }
        catch (Exception ex)
        {
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
