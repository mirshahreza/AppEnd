using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace AppEndWorkflow.Activities.Analytics;

/// <summary>
/// Sends events and tracks data in Google Analytics.
/// </summary>
[Activity(
    Category = "Analytics",
    Description = "Track event in Google Analytics",
    DisplayName = "Google Analytics Track"
)]
public class GoogleAnalyticsTrackActivity : CodeActivity
{
    [Input(Description = "Measurement ID")]
    public Input<string> MeasurementId { get; set; } = default!;

    [Input(Description = "API secret")]
    public Input<string> ApiSecret { get; set; } = default!;

    [Input(Description = "Client ID (or user ID)")]
    public Input<string> ClientId { get; set; } = default!;

    [Input(Description = "Event name")]
    public Input<string> EventName { get; set; } = default!;

    [Input(Description = "Event parameters JSON")]
    public Input<string?> Parameters { get; set; }

    [Output(Description = "Whether event was sent")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var measurementId = context.Get(MeasurementId) ?? throw new ArgumentException("MeasurementId is required");
            var apiSecret = context.Get(ApiSecret) ?? throw new ArgumentException("ApiSecret is required");
            var clientId = context.Get(ClientId) ?? throw new ArgumentException("ClientId is required");
            var eventName = context.Get(EventName) ?? throw new ArgumentException("EventName is required");
            var parameters = context.Get(Parameters) ?? "{}";

            using var httpClient = new HttpClient();

            var payload = new
            {
                client_id = clientId,
                events = new[] {
                    new {
                        name = eventName,
                        @params = JsonSerializer.Deserialize<object>(parameters)
                    }
                }
            };

            var url = $"https://www.google-analytics.com/mp/collect?measurement_id={measurementId}&api_secret={apiSecret}";
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
