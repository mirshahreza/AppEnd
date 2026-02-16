using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;

namespace AppEndWorkflow.Activities.Http;

/// <summary>
/// Sends webhook with exponential backoff retry strategy.
/// </summary>
[Activity(
    Category = "Webhooks",
    Description = "Webhook with exponential backoff retry",
    DisplayName = "Webhook Retry"
)]
public class WebhookRetryActivity : CodeActivity
{
    [Input(Description = "Webhook URL to retry")]
    public Input<string> Url { get; set; } = default!;

    [Input(Description = "JSON payload")]
    public Input<string> Payload { get; set; } = default!;

    [Input(Description = "Maximum retry attempts (default: 5)")]
    public Input<int?> MaxRetries { get; set; }

    [Input(Description = "Initial delay before first retry in seconds (default: 5)")]
    public Input<int?> InitialDelaySeconds { get; set; }

    [Input(Description = "Exponential backoff multiplier (default: 2.0)")]
    public Input<double?> BackoffMultiplier { get; set; }

    [Output(Description = "Total attempts made")]
    public Output<int> AttemptCount { get; set; } = default!;

    [Output(Description = "Whether webhook was delivered")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Last HTTP status received")]
    public Output<int?> LastStatusCode { get; set; }

    [Output(Description = "Error message if all retries failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var url = context.Get(Url) ?? throw new ArgumentException("Url is required");
            var payload = context.Get(Payload) ?? throw new ArgumentException("Payload is required");
            var maxRetries = context.Get(MaxRetries) ?? 5;
            var initialDelay = context.Get(InitialDelaySeconds) ?? 5;
            var backoffMultiplier = context.Get(BackoffMultiplier) ?? 2.0;

            using var httpClient = new HttpClient();
            var attemptCount = 0;
            int? lastStatusCode = null;
            Exception? lastException = null;

            for (int attempt = 0; attempt <= maxRetries; attempt++)
            {
                attemptCount++;

                try
                {
                    var content = new StringContent(payload, System.Text.Encoding.UTF8, "application/json");
                    var response = httpClient.PostAsync(url, content).Result;
                    lastStatusCode = (int)response.StatusCode;

                    if (response.IsSuccessStatusCode)
                    {
                        context.Set(AttemptCount, attemptCount);
                        context.Set(Success, true);
                        context.Set(LastStatusCode, lastStatusCode);
                        context.Set(Error, null);
                        return;
                    }

                    // Don't retry on 4xx errors (except 429)
                    if (response.StatusCode != System.Net.HttpStatusCode.TooManyRequests &&
                        (int)response.StatusCode >= 400 && (int)response.StatusCode < 500)
                    {
                        throw new HttpRequestException($"HTTP {(int)response.StatusCode}: {response.ReasonPhrase}");
                    }

                    lastException = new Exception($"HTTP {(int)response.StatusCode}");
                }
                catch (Exception ex)
                {
                    lastException = ex;
                }

                // Don't sleep after last attempt
                if (attempt < maxRetries)
                {
                    var delayMs = (int)(initialDelay * 1000 * System.Math.Pow(backoffMultiplier, attempt));
                    System.Threading.Thread.Sleep(delayMs);
                }
            }

            context.Set(AttemptCount, attemptCount);
            context.Set(Success, false);
            context.Set(LastStatusCode, lastStatusCode);
            context.Set(Error, lastException?.Message ?? "Max retries exceeded");
        }
        catch (Exception ex)
        {
            context.Set(AttemptCount, 0);
            context.Set(Success, false);
            context.Set(LastStatusCode, null);
            context.Set(Error, ex.Message);
        }
    }
}
