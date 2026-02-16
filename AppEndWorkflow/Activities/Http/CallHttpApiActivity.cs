using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Text.Json;

namespace AppEndWorkflow.Activities.Http;

/// <summary>
/// Makes HTTP requests to REST API endpoints.
/// Supports GET, POST, PUT, DELETE, PATCH with custom headers and body.
/// </summary>
[Activity(
    Category = "HTTP",
    Description = "Calls a REST API endpoint",
    DisplayName = "Call HTTP API"
)]
public class CallHttpApiActivity : CodeActivity
{
    /// <summary>
    /// Target URL
    /// </summary>
    [Input(Description = "Target URL")]
    public Input<string> Url { get; set; } = default!;

    /// <summary>
    /// HTTP method: GET, POST, PUT, DELETE, PATCH
    /// </summary>
    [Input(Description = "HTTP method: GET, POST, PUT, DELETE, PATCH")]
    public Input<string> Method { get; set; } = new("GET");

    /// <summary>
    /// JSON object of headers
    /// </summary>
    [Input(Description = "JSON object of headers (optional)")]
    public Input<string?> Headers { get; set; }

    /// <summary>
    /// Request body (for POST/PUT/PATCH)
    /// </summary>
    [Input(Description = "Request body (optional)")]
    public Input<string?> Body { get; set; }

    /// <summary>
    /// Content type (default: "application/json")
    /// </summary>
    [Input(Description = "Content type")]
    public Input<string> ContentType { get; set; } = new("application/json");

    /// <summary>
    /// Request timeout in seconds (default: 30)
    /// </summary>
    [Input(Description = "Request timeout in seconds")]
    public Input<int> TimeoutSeconds { get; set; } = new(30);

    /// <summary>
    /// HTTP status code
    /// </summary>
    [Output(Description = "HTTP status code")]
    public Output<int> StatusCode { get; set; } = default!;

    /// <summary>
    /// Response body as string
    /// </summary>
    [Output(Description = "Response body as string")]
    public Output<string> ResponseBody { get; set; } = default!;

    /// <summary>
    /// JSON object of response headers
    /// </summary>
    [Output(Description = "JSON object of response headers")]
    public Output<string> ResponseHeaders { get; set; } = default!;

    /// <summary>
    /// Whether status code is 2xx
    /// </summary>
    [Output(Description = "Whether status code is 2xx")]
    public Output<bool> Success { get; set; } = default!;

    /// <summary>
    /// Error message if request failed
    /// </summary>
    [Output(Description = "Error message if request failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var url = context.Get(Url);
            var method = context.Get(Method);
            var headersJson = context.Get(Headers);
            var body = context.Get(Body);
            var contentType = context.Get(ContentType);
            var timeoutSeconds = context.Get(TimeoutSeconds);

            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("'Url' is required");

            if (string.IsNullOrWhiteSpace(method))
                throw new ArgumentException("'Method' is required");

            // Execute HTTP request
            var (statusCode, responseBody, responseHeaders) = MakeHttpRequest(
                url, method, headersJson, body, contentType, timeoutSeconds);

            context.Set(StatusCode, statusCode);
            context.Set(ResponseBody, responseBody);
            context.Set(ResponseHeaders, responseHeaders);
            context.Set(Success, statusCode >= 200 && statusCode < 300);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(StatusCode, 0);
            context.Set(ResponseBody, "");
            context.Set(ResponseHeaders, "{}");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }

    private (int statusCode, string responseBody, string responseHeaders) MakeHttpRequest(
        string url, string method, string? headersJson, string? body, string contentType, int timeoutSeconds)
    {
        using var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromSeconds(timeoutSeconds);

        // Parse and add custom headers
        if (!string.IsNullOrWhiteSpace(headersJson))
        {
            try
            {
                using var doc = JsonDocument.Parse(headersJson);
                foreach (var property in doc.RootElement.EnumerateObject())
                {
                    httpClient.DefaultRequestHeaders.Add(property.Name, property.Value.GetString());
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to parse headers: {ex.Message}");
            }
        }

        // Create request
        HttpRequestMessage request = method.ToUpper() switch
        {
            "GET" => new HttpRequestMessage(HttpMethod.Get, url),
            "POST" => new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(body ?? "", System.Text.Encoding.UTF8, contentType)
            },
            "PUT" => new HttpRequestMessage(HttpMethod.Put, url)
            {
                Content = new StringContent(body ?? "", System.Text.Encoding.UTF8, contentType)
            },
            "DELETE" => new HttpRequestMessage(HttpMethod.Delete, url)
            {
                Content = body != null ? new StringContent(body, System.Text.Encoding.UTF8, contentType) : null
            },
            "PATCH" => new HttpRequestMessage(HttpMethod.Patch, url)
            {
                Content = new StringContent(body ?? "", System.Text.Encoding.UTF8, contentType)
            },
            _ => throw new NotSupportedException($"HTTP method '{method}' is not supported")
        };

        // Send request
        var response = httpClient.SendAsync(request).Result;
        var responseBody = response.Content.ReadAsStringAsync().Result;

        // Convert response headers to JSON
        var responseHeadersDict = new Dictionary<string, string>();
        foreach (var header in response.Headers)
        {
            responseHeadersDict[header.Key] = string.Join(",", header.Value);
        }
        var responseHeaders = JsonSerializer.Serialize(responseHeadersDict);

        return ((int)response.StatusCode, responseBody, responseHeaders);
    }
}
