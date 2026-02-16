using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace AppEndWorkflow.Activities.Http;

/// <summary>
/// Sends webhook to external URL with automatic retry and signature.
/// </summary>
[Activity(
    Category = "Webhooks",
    Description = "Send webhook to external URL",
    DisplayName = "Send Webhook"
)]
public class SendWebhookActivity : CodeActivity
{
    [Input(Description = "Target webhook URL")]
    public Input<string> Url { get; set; } = default!;

    [Input(Description = "JSON payload to send")]
    public Input<string> Payload { get; set; } = default!;

    [Input(Description = "Secret key for HMAC-SHA256 signature (optional)")]
    public Input<string?> Secret { get; set; }

    [Input(Description = "JSON extra headers to include")]
    public Input<string?> Headers { get; set; }

    [Input(Description = "Number of retries (default: 3)")]
    public Input<int?> RetryCount { get; set; }

    [Input(Description = "Request timeout in seconds (default: 30)")]
    public Input<int?> TimeoutSeconds { get; set; }

    [Output(Description = "HTTP response status")]
    public Output<int> StatusCode { get; set; } = default!;

    [Output(Description = "Response body")]
    public Output<string?> ResponseBody { get; set; }

    [Output(Description = "Whether webhook was delivered")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var url = context.Get(Url) ?? throw new ArgumentException("Url is required");
            var payload = context.Get(Payload) ?? throw new ArgumentException("Payload is required");
            var secret = context.Get(Secret);
            var headers = context.Get(Headers);
            var retryCount = context.Get(RetryCount) ?? 3;
            var timeoutSeconds = context.Get(TimeoutSeconds) ?? 30;

            using var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(timeoutSeconds);

            // Add custom headers
            if (!string.IsNullOrWhiteSpace(headers))
            {
                try
                {
                    using var doc = JsonDocument.Parse(headers);
                    foreach (var prop in doc.RootElement.EnumerateObject())
                    {
                        httpClient.DefaultRequestHeaders.Add(prop.Name, prop.Value.GetString() ?? "");
                    }
                }
                catch { /* Ignore invalid headers */ }
            }

            // Add signature header if secret provided
            if (!string.IsNullOrWhiteSpace(secret))
            {
                var signature = GenerateSignature(payload, secret);
                httpClient.DefaultRequestHeaders.Add("X-Webhook-Signature", $"sha256={signature}");
            }

            HttpResponseMessage response = null!;
            int attempt = 0;

            while (attempt <= retryCount)
            {
                try
                {
                    var content = new StringContent(payload, Encoding.UTF8, "application/json");
                    response = httpClient.PostAsync(url, content).Result;

                    if (response.IsSuccessStatusCode)
                        break;

                    if (attempt < retryCount && (int)response.StatusCode >= 500)
                    {
                        System.Threading.Thread.Sleep(1000 * (attempt + 1)); // Exponential backoff
                        attempt++;
                        continue;
                    }

                    break;
                }
                catch when (attempt < retryCount)
                {
                    System.Threading.Thread.Sleep(1000 * (attempt + 1));
                    attempt++;
                }
            }

            var responseBody = response?.Content.ReadAsStringAsync().Result ?? "";
            var statusCode = (int?)response?.StatusCode ?? 0;

            context.Set(StatusCode, statusCode);
            context.Set(ResponseBody, responseBody);
            context.Set(Success, response?.IsSuccessStatusCode ?? false);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(StatusCode, 0);
            context.Set(ResponseBody, null);
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }

    private string GenerateSignature(string payload, string secret)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        return Convert.ToHexString(hash).ToLower();
    }
}
