using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using Microsoft.Extensions.Configuration;

namespace AppEndWorkflow.Activities.Notifications;

/// <summary>
/// Sends WhatsApp messages via provider API (Twilio, Meta, etc.)
/// </summary>
[Activity(
    Category = "Notifications",
    Description = "Sends WhatsApp message",
    DisplayName = "Send WhatsApp Message"
)]
public class SendWhatsappActivity : CodeActivity
{
    [Input(Description = "Recipient WhatsApp number (with country code)")]
    public Input<string> PhoneNumber { get; set; } = default!;

    [Input(Description = "Message text")]
    public Input<string> Message { get; set; } = default!;

    [Input(Description = "WhatsApp provider: 'Twilio', 'Meta', etc.")]
    public Input<string> Provider { get; set; } = default!;

    [Input(Description = "Media URL (optional image/video)")]
    public Input<string?> MediaUrl { get; set; }

    [Input(Description = "Media type: 'image', 'video', 'document'")]
    public Input<string?> MediaType { get; set; }

    [Output(Description = "Message ID")]
    public Output<string> MessageId { get; set; } = default!;

    [Output(Description = "Whether message was sent")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var phoneNumber = context.Get(PhoneNumber) ?? throw new ArgumentException("PhoneNumber is required");
            var message = context.Get(Message) ?? throw new ArgumentException("Message is required");
            var provider = context.Get(Provider) ?? "Twilio";

            var configuration = context.GetService<IConfiguration>();

            // For simplicity, using Twilio as default provider
            if (provider.Equals("Twilio", StringComparison.OrdinalIgnoreCase))
            {
                SendViaTwilio(context, configuration, phoneNumber, message);
            }
            else if (provider.Equals("Meta", StringComparison.OrdinalIgnoreCase))
            {
                SendViaMeta(context, configuration, phoneNumber, message);
            }
            else
            {
                throw new NotSupportedException($"WhatsApp provider '{provider}' is not supported");
            }
        }
        catch (Exception ex)
        {
            context.Set(MessageId, "");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }

    private void SendViaTwilio(ActivityExecutionContext context, IConfiguration? configuration, string phoneNumber, string message)
    {
        var accountSid = configuration?["Twilio:AccountSid"] ?? throw new InvalidOperationException("Twilio AccountSid not configured");
        var authToken = configuration?["Twilio:AuthToken"] ?? throw new InvalidOperationException("Twilio AuthToken not configured");
        var fromNumber = configuration?["Twilio:WhatsAppNumber"] ?? throw new InvalidOperationException("Twilio WhatsApp number not configured");

        using var httpClient = new HttpClient();
        var auth = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{accountSid}:{authToken}"));
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {auth}");

        var payload = new Dictionary<string, string>
        {
            ["From"] = $"whatsapp:{fromNumber}",
            ["To"] = $"whatsapp:{phoneNumber}",
            ["Body"] = message
        };

        var content = new FormUrlEncodedContent(payload);
        var response = httpClient.PostAsync($"https://api.twilio.com/2010-04-01/Accounts/{accountSid}/Messages", content).Result;
        var responseBody = response.Content.ReadAsStringAsync().Result;

        using var doc = JsonDocument.Parse(responseBody);
        var root = doc.RootElement;

        if (root.TryGetProperty("sid", out var sidProp))
        {
            var messageId = sidProp.GetString();
            context.Set(MessageId, messageId ?? "");
            context.Set(Success, true);
            context.Set(Error, null);
        }
        else
        {
            throw new Exception("Failed to send message via Twilio");
        }
    }

    private void SendViaMeta(ActivityExecutionContext context, IConfiguration? configuration, string phoneNumber, string message)
    {
        var phoneNumberId = configuration?["Meta:PhoneNumberId"] ?? throw new InvalidOperationException("Meta PhoneNumberId not configured");
        var accessToken = configuration?["Meta:AccessToken"] ?? throw new InvalidOperationException("Meta AccessToken not configured");

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

        var payload = new
        {
            messaging_product = "whatsapp",
            recipient_type = "individual",
            to = phoneNumber,
            type = "text",
            text = new { body = message }
        };

        var response = httpClient.PostAsJsonAsync($"https://graph.instagram.com/v18.0/{phoneNumberId}/messages", payload).Result;
        var responseBody = response.Content.ReadAsStringAsync().Result;

        using var doc = JsonDocument.Parse(responseBody);
        var root = doc.RootElement;

        if (root.TryGetProperty("messages", out var messagesProp) && messagesProp.ValueKind == JsonValueKind.Array)
        {
            var firstMsg = messagesProp.EnumerateArray().FirstOrDefault();
            var messageId = firstMsg.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;

            context.Set(MessageId, messageId ?? "");
            context.Set(Success, true);
            context.Set(Error, null);
        }
        else
        {
            throw new Exception("Failed to send message via Meta");
        }
    }
}
