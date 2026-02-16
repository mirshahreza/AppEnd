using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Text;

namespace AppEndWorkflow.Activities.Http;

/// <summary>
/// Calls a SOAP web service.
/// Supports legacy SOAP endpoints (common in Iranian banking/government APIs).
/// </summary>
[Activity(
    Category = "HTTP",
    Description = "Calls a SOAP web service",
    DisplayName = "Call SOAP Service"
)]
public class CallSoapServiceActivity : CodeActivity
{
    /// <summary>
    /// WSDL endpoint URL
    /// </summary>
    [Input(Description = "WSDL endpoint URL")]
    public Input<string> WsdlUrl { get; set; } = default!;

    /// <summary>
    /// SOAP action name
    /// </summary>
    [Input(Description = "SOAP action name")]
    public Input<string> Action { get; set; } = default!;

    /// <summary>
    /// SOAP envelope body XML
    /// </summary>
    [Input(Description = "SOAP envelope body XML")]
    public Input<string> Body { get; set; } = default!;

    /// <summary>
    /// JSON object of HTTP headers
    /// </summary>
    [Input(Description = "JSON object of HTTP headers (optional)")]
    public Input<string?> Headers { get; set; }

    /// <summary>
    /// Request timeout (default: 30)
    /// </summary>
    [Input(Description = "Request timeout in seconds")]
    public Input<int> TimeoutSeconds { get; set; } = new(30);

    /// <summary>
    /// Full SOAP response XML
    /// </summary>
    [Output(Description = "Full SOAP response XML")]
    public Output<string> ResponseXml { get; set; } = default!;

    /// <summary>
    /// Extracted SOAP body content
    /// </summary>
    [Output(Description = "Extracted SOAP body content")]
    public Output<string> ResponseBody { get; set; } = default!;

    /// <summary>
    /// HTTP status code
    /// </summary>
    [Output(Description = "HTTP status code")]
    public Output<int> StatusCode { get; set; } = default!;

    /// <summary>
    /// Whether call succeeded
    /// </summary>
    [Output(Description = "Whether call succeeded")]
    public Output<bool> Success { get; set; } = default!;

    /// <summary>
    /// Error message if failed
    /// </summary>
    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var wsdlUrl = context.Get(WsdlUrl);
            var action = context.Get(Action);
            var body = context.Get(Body);
            var headersJson = context.Get(Headers);
            var timeoutSeconds = context.Get(TimeoutSeconds);

            if (string.IsNullOrWhiteSpace(wsdlUrl))
                throw new ArgumentException("'WsdlUrl' is required");

            if (string.IsNullOrWhiteSpace(action))
                throw new ArgumentException("'Action' is required");

            if (string.IsNullOrWhiteSpace(body))
                throw new ArgumentException("'Body' is required");

            // Execute SOAP request
            var (statusCode, responseXml, responseBody) = CallSoapService(
                wsdlUrl, action, body, headersJson, timeoutSeconds);

            context.Set(StatusCode, statusCode);
            context.Set(ResponseXml, responseXml);
            context.Set(ResponseBody, responseBody);
            context.Set(Success, statusCode >= 200 && statusCode < 300);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(StatusCode, 0);
            context.Set(ResponseXml, "");
            context.Set(ResponseBody, "");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }

    private (int statusCode, string responseXml, string responseBody) CallSoapService(
        string wsdlUrl, string action, string bodyXml, string? headersJson, int timeoutSeconds)
    {
        using var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromSeconds(timeoutSeconds);

        // Build SOAP envelope
        var soapEnvelope = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
    {bodyXml}
</soap:Envelope>";

        // Create request
        var request = new HttpRequestMessage(HttpMethod.Post, wsdlUrl)
        {
            Content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml")
        };

        // Add SOAPAction header
        request.Headers.Add("SOAPAction", action);

        // Parse and add custom headers if provided
        if (!string.IsNullOrWhiteSpace(headersJson))
        {
            try
            {
                using var doc = System.Text.Json.JsonDocument.Parse(headersJson);
                foreach (var property in doc.RootElement.EnumerateObject())
                {
                    request.Headers.Add(property.Name, property.Value.GetString());
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to parse headers: {ex.Message}");
            }
        }

        // Send request
        var response = httpClient.SendAsync(request).Result;
        var responseXml = response.Content.ReadAsStringAsync().Result;

        // Try to extract body content (simplified extraction)
        var responseBody = ExtractSoapBodyContent(responseXml);

        return ((int)response.StatusCode, responseXml, responseBody);
    }

    private string ExtractSoapBodyContent(string soapResponse)
    {
        try
        {
            // Simple XML extraction of body content
            // This is a basic implementation; a full XML parser would be better
            var startIndex = soapResponse.IndexOf("<soap:Body", StringComparison.OrdinalIgnoreCase);
            if (startIndex < 0)
                return soapResponse;

            startIndex = soapResponse.IndexOf(">", startIndex) + 1;
            var endIndex = soapResponse.IndexOf("</soap:Body>", StringComparison.OrdinalIgnoreCase);

            if (endIndex < 0)
                return soapResponse;

            return soapResponse.Substring(startIndex, endIndex - startIndex).Trim();
        }
        catch
        {
            return soapResponse;
        }
    }
}
