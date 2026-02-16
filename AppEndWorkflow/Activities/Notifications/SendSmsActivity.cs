using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace AppEndWorkflow.Activities.Notifications;

/// <summary>
/// Sends an SMS message via provider API (e.g., Kavenegar, Ghasedak).
/// SMS provider settings are read from appsettings.json under the "SmsProviders" section.
/// </summary>
[Activity(
    Category = "Notifications",
    Description = "Sends an SMS via provider API",
    DisplayName = "Send SMS"
)]
public class SendSmsActivity : CodeActivity
{
    /// <summary>
    /// Recipient phone number
    /// </summary>
    [Input(Description = "Recipient phone number")]
    public Input<string> PhoneNumber { get; set; } = default!;

    /// <summary>
    /// SMS text content
    /// </summary>
    [Input(Description = "SMS text content")]
    public Input<string> Message { get; set; } = default!;

    /// <summary>
    /// SMS provider name (e.g., "Kavenegar", "Ghasedak")
    /// </summary>
    [Input(Description = "SMS provider name (e.g., 'Kavenegar', 'Ghasedak')")]
    public Input<string> Provider { get; set; } = default!;

    /// <summary>
    /// Whether the SMS was sent
    /// </summary>
    [Output(Description = "Whether the SMS was sent")]
    public Output<bool> Success { get; set; } = default!;

    /// <summary>
    /// Provider's message tracking ID
    /// </summary>
    [Output(Description = "Provider's message tracking ID")]
    public Output<string?> MessageId { get; set; }

    /// <summary>
    /// Error message if failed
    /// </summary>
    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var phoneNumber = context.Get(PhoneNumber);
            var message = context.Get(Message);
            var provider = context.Get(Provider);

            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("'PhoneNumber' is required");

            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("'Message' is required");

            if (string.IsNullOrWhiteSpace(provider))
                throw new ArgumentException("'Provider' is required");

            // Get SMS provider configuration
            var configuration = context.GetService<IConfiguration>();
            var providersConfig = configuration?.GetSection($"SmsProviders:{provider}");

            if (providersConfig == null || !providersConfig.Exists())
                throw new InvalidOperationException($"SMS Provider '{provider}' configuration not found in appsettings.json");

            var apiUrl = providersConfig["ApiUrl"] ?? throw new InvalidOperationException($"API URL for provider '{provider}' not configured");
            var apiKey = providersConfig["ApiKey"] ?? throw new InvalidOperationException($"API Key for provider '{provider}' not configured");
            var senderId = providersConfig["SenderId"];

            // Call provider API
            var messageId = SendSmsViProvider(phoneNumber, message, provider.ToLower(), apiUrl, apiKey, senderId);

            context.Set(Success, true);
            context.Set(MessageId, messageId);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(Success, false);
            context.Set(MessageId, null);
            context.Set(Error, ex.Message);
        }
    }

    private string SendSmsViProvider(string phoneNumber, string message, string provider, string apiUrl, string apiKey, string? senderId)
    {
        using var httpClient = new HttpClient();

        switch (provider.ToLower())
        {
            case "kavenegar":
                return SendViaKavenegar(phoneNumber, message, apiUrl, apiKey, senderId, httpClient);

            case "ghasedak":
                return SendViaGhasedak(phoneNumber, message, apiUrl, apiKey, senderId, httpClient);

            default:
                throw new NotSupportedException($"SMS provider '{provider}' is not supported");
        }
    }

    private string SendViaKavenegar(string phoneNumber, string message, string apiUrl, string apiKey, string? senderId, HttpClient httpClient)
    {
        var url = $"{apiUrl}?apikey={apiKey}&receptor={phoneNumber}&message={Uri.EscapeDataString(message)}";

        if (!string.IsNullOrWhiteSpace(senderId))
            url += $"&sender={senderId}";

        var response = httpClient.GetAsync(url).Result;
        var content = response.Content.ReadAsStringAsync().Result;

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"Kavenegar API error: {content}");

        // Parse response to extract message ID
        // Kavenegar returns: {"result":[{"messageid":"...","message":"","status":...}]}
        using var jsonDoc = JsonDocument.Parse(content);
        var messageId = jsonDoc.RootElement
            .GetProperty("result")[0]
            .GetProperty("messageid")
            .GetString() ?? "unknown";

        return messageId;
    }

    private string SendViaGhasedak(string phoneNumber, string message, string apiUrl, string apiKey, string? senderId, HttpClient httpClient)
    {
        var payload = new
        {
            api = apiKey,
            to = phoneNumber,
            text = message,
            from = senderId ?? "000000"
        };

        var jsonContent = new StringContent(
            JsonSerializer.Serialize(payload),
            System.Text.Encoding.UTF8,
            "application/json");

        var response = httpClient.PostAsync(apiUrl, jsonContent).Result;
        var content = response.Content.ReadAsStringAsync().Result;

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"Ghasedak API error: {content}");

        // Parse response to extract message ID
        // Ghasedak returns: {"result":{"status":1,"statusText":"ارسال شده","messageid":"..."}}
        using var jsonDoc = JsonDocument.Parse(content);
        var messageId = jsonDoc.RootElement
            .GetProperty("result")
            .GetProperty("messageid")
            .GetString() ?? "unknown";

        return messageId;
    }
}
