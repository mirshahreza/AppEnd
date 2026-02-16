using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace AppEndWorkflow.Activities.Notifications;

/// <summary>
/// Sends a message via Telegram Bot API.
/// Default bot token is read from appsettings.json under "Telegram.BotToken".
/// </summary>
[Activity(
    Category = "Notifications",
    Description = "Sends a message via Telegram Bot API",
    DisplayName = "Send Telegram"
)]
public class SendTelegramActivity : CodeActivity
{
    /// <summary>
    /// Telegram chat/group/channel ID
    /// </summary>
    [Input(Description = "Telegram chat/group/channel ID")]
    public Input<string> ChatId { get; set; } = default!;

    /// <summary>
    /// Message text (Markdown supported)
    /// </summary>
    [Input(Description = "Message text (Markdown supported)")]
    public Input<string> Message { get; set; } = default!;

    /// <summary>
    /// Bot token (optional — falls back to settings)
    /// </summary>
    [Input(Description = "Bot token (optional — falls back to settings)")]
    public Input<string?> BotToken { get; set; }

    /// <summary>
    /// Parse mode: "Markdown" or "HTML" (default: "Markdown")
    /// </summary>
    [Input(Description = "Parse mode: 'Markdown' or 'HTML' (default: 'Markdown')")]
    public Input<string> ParseMode { get; set; } = new("Markdown");

    /// <summary>
    /// Whether the message was sent
    /// </summary>
    [Output(Description = "Whether the message was sent")]
    public Output<bool> Success { get; set; } = default!;

    /// <summary>
    /// Telegram message ID
    /// </summary>
    [Output(Description = "Telegram message ID")]
    public Output<int?> MessageId { get; set; }

    /// <summary>
    /// Error message if failed
    /// </summary>
    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var chatId = context.Get(ChatId);
            var message = context.Get(Message);
            var botToken = context.Get(BotToken);
            var parseMode = context.Get(ParseMode);

            if (string.IsNullOrWhiteSpace(chatId))
                throw new ArgumentException("'ChatId' is required");

            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("'Message' is required");

            // Get bot token from input or fallback to configuration
            if (string.IsNullOrWhiteSpace(botToken))
            {
                var configuration = context.GetService<IConfiguration>();
                botToken = configuration?["Telegram:BotToken"];

                if (string.IsNullOrWhiteSpace(botToken))
                    throw new InvalidOperationException("Telegram BotToken not provided and not configured in appsettings.json");
            }

            if (string.IsNullOrWhiteSpace(parseMode))
                parseMode = "Markdown";

            // Validate parseMode
            if (parseMode != "Markdown" && parseMode != "HTML")
                parseMode = "Markdown";

            // Send message via Telegram API
            var messageId = SendTelegramMessage(chatId, message, botToken, parseMode);

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

    private int SendTelegramMessage(string chatId, string message, string botToken, string parseMode)
    {
        using var httpClient = new HttpClient();

        var url = $"https://api.telegram.org/bot{botToken}/sendMessage";

        var payload = new
        {
            chat_id = chatId,
            text = message,
            parse_mode = parseMode,
            disable_web_page_preview = true
        };

        var jsonContent = new StringContent(
            JsonSerializer.Serialize(payload),
            System.Text.Encoding.UTF8,
            "application/json");

        var response = httpClient.PostAsync(url, jsonContent).Result;
        var content = response.Content.ReadAsStringAsync().Result;

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"Telegram API error: {content}");

        // Parse response to extract message ID
        // Telegram returns: {"ok":true,"result":{"message_id":123,...}}
        using var jsonDoc = JsonDocument.Parse(content);
        
        if (!jsonDoc.RootElement.GetProperty("ok").GetBoolean())
        {
            var errorDescription = jsonDoc.RootElement
                .TryGetProperty("description", out var desc) 
                ? desc.GetString() 
                : "Unknown error";
            throw new HttpRequestException($"Telegram API error: {errorDescription}");
        }

        var messageId = jsonDoc.RootElement
            .GetProperty("result")
            .GetProperty("message_id")
            .GetInt32();

        return messageId;
    }
}
