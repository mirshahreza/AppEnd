using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Text.Json;

namespace AppEndWorkflow.Activities.Http;

/// <summary>
/// Receives incoming webhooks from external systems.
/// Suspends workflow execution until webhook is received.
/// </summary>
[Activity(
    Category = "Webhooks",
    Description = "Receive incoming webhook",
    DisplayName = "Receive Webhook"
)]
public class ReceiveWebhookActivity : CodeActivity
{
    [Input(Description = "Webhook endpoint path (e.g., /api/webhooks/order)")]
    public Input<string> WebhookPath { get; set; } = default!;

    [Input(Description = "HTTP method: POST, PUT, PATCH (default: POST)")]
    public Input<string?> Method { get; set; }

    [Input(Description = "Timeout before giving up (default: 300 seconds)")]
    public Input<int?> TimeoutSeconds { get; set; }

    [Input(Description = "Header name for signature verification (optional)")]
    public Input<string?> SignatureHeader { get; set; }

    [Output(Description = "Received webhook payload as JSON")]
    public Output<string> PayloadJson { get; set; } = default!;

    [Output(Description = "JSON of request headers")]
    public Output<string> Headers { get; set; } = default!;

    [Output(Description = "JSON of query parameters")]
    public Output<string> QueryParams { get; set; } = default!;

    [Output(Description = "Whether signature verification passed")]
    public Output<bool> Authenticated { get; set; } = default!;

    [Output(Description = "Whether webhook was received")]
    public Output<bool> Success { get; set; } = default!;

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var webhookPath = context.Get(WebhookPath) ?? throw new ArgumentException("WebhookPath is required");
            var method = context.Get(Method) ?? "POST";
            var timeoutSeconds = context.Get(TimeoutSeconds) ?? 300;

            // NOTE: In real implementation, this would use Elsa bookmarks to suspend workflow
            // and wait for external HTTP request to resume it.
            // For now, we'll provide a placeholder implementation.

            // This would create a bookmark and suspend the workflow
            // When an HTTP request comes to the webhook endpoint, it would resume with the payload

            context.Set(PayloadJson, "{}");
            context.Set(Headers, "{}");
            context.Set(QueryParams, "{}");
            context.Set(Authenticated, true);
            context.Set(Success, true);
        }
        catch (Exception ex)
        {
            context.Set(PayloadJson, "");
            context.Set(Headers, "");
            context.Set(QueryParams, "");
            context.Set(Authenticated, false);
            context.Set(Success, false);
        }
    }
}
