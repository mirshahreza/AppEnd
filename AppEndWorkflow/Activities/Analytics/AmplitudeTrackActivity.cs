using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace AppEndWorkflow.Activities.Analytics;

/// <summary>
/// Sends events to Amplitude for product analytics.
/// </summary>
[Activity(
    Category = "Analytics",
    Description = "Track event in Amplitude",
    DisplayName = "Amplitude Track"
)]
public class AmplitudeTrackActivity : CodeActivity
{
    [Input(Description = "Amplitude API key")]
    public Input<string> ApiKey { get; set; } = default!;

    [Input(Description = "User ID")]
    public Input<string> UserId { get; set; } = default!;

    [Input(Description = "Event type")]
    public Input<string> EventType { get; set; } = default!;

    [Input(Description = "Event properties JSON")]
    public Input<string?> EventProperties { get; set; }

    [Input(Description = "User properties JSON")]
    public Input<string?> UserProperties { get; set; }

    [Output(Description = "Event ID")]
    public Output<string> EventId { get; set; } = default!;

    [Output(Description = "Whether event was sent")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var apiKey = context.Get(ApiKey) ?? throw new ArgumentException("ApiKey is required");
            var userId = context.Get(UserId) ?? throw new ArgumentException("UserId is required");
            var eventType = context.Get(EventType) ?? throw new ArgumentException("EventType is required");

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var payload = new
            {
                events = new[] {
                    new {
                        user_id = userId,
                        event_type = eventType,
                        time = (long)(DateTime.UtcNow - DateTime.UnixEpoch).TotalMilliseconds,
                        event_properties = JsonSerializer.Deserialize<object>(context.Get(EventProperties) ?? "{}"),
                        user_properties = JsonSerializer.Deserialize<object>(context.Get(UserProperties) ?? "{}")
                    }
                }
            };

            var url = "https://api2.amplitude.com/2/httpapi";
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = JsonContent.Create(payload)
            };

            var response = httpClient.SendAsync(request).Result;
            var eventId = Guid.NewGuid().ToString();

            context.Set(EventId, eventId);
            context.Set(Success, response.IsSuccessStatusCode);
            context.Set(Error, response.IsSuccessStatusCode ? null : response.ReasonPhrase);
        }
        catch (Exception ex)
        {
            context.Set(EventId, "");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
