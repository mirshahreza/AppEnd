using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;

namespace AppEndWorkflow.Activities.Notifications;

/// <summary>
/// Sends Slack messages via Bot API.
/// Supports markdown formatting and Slack blocks.
/// </summary>
[Activity(
    Category = "Notifications",
    Description = "Sends Slack message",
    DisplayName = "Send Slack Message"
)]
public class SendSlackActivity : CodeActivity
{
    [Input(Description = "Slack channel ID or name")]
    public Input<string> ChannelId { get; set; } = default!;

    [Input(Description = "Message text (supports Markdown)")]
    public Input<string> Message { get; set; } = default!;

    [Input(Description = "Bot token (falls back to settings)")]
    public Input<string?> BotToken { get; set; }

    [Input(Description = "JSON Slack blocks for rich formatting")]
    public Input<string?> Blocks { get; set; }

    [Input(Description = "Thread timestamp for replies")]
    public Input<string?> ThreadTs { get; set; }

    [Output(Description = "Message timestamp")]
    public Output<string> MessageTs { get; set; } = default!;

    [Output(Description = "Whether message was sent")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var channelId = context.Get(ChannelId) ?? throw new ArgumentException("ChannelId is required");
            var message = context.Get(Message) ?? throw new ArgumentException("Message is required");

            var configuration = context.GetService<IConfiguration>();
            var botToken = context.Get(BotToken) ?? configuration?["Slack:BotToken"] 
                ?? throw new InvalidOperationException("Slack BotToken not configured");

            var slackBlocks = context.Get(Blocks);
            var threadTs = context.Get(ThreadTs);

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {botToken}");

            var payload = new Dictionary<string, object>
            {
                ["channel"] = channelId,
                ["text"] = message
            };

            if (!string.IsNullOrWhiteSpace(slackBlocks))
            {
                try
                {
                    var blocksObj = JsonSerializer.Deserialize<JsonElement>(slackBlocks);
                    payload["blocks"] = blocksObj;
                }
                catch { /* Use text-only message if blocks are invalid */ }
            }

            if (!string.IsNullOrWhiteSpace(threadTs))
                payload["thread_ts"] = threadTs;

            var response = httpClient.PostAsJsonAsync("https://slack.com/api/chat.postMessage", payload).Result;
            var responseBody = response.Content.ReadAsStringAsync().Result;

            using var doc = JsonDocument.Parse(responseBody);
            var root = doc.RootElement;

            if (root.TryGetProperty("ok", out var okProp) && okProp.GetBoolean())
            {
                var ts = root.GetProperty("ts").GetString();
                context.Set(MessageTs, ts ?? "");
                context.Set(Success, true);
                context.Set(Error, null);
            }
            else
            {
                var errorMsg = root.TryGetProperty("error", out var errProp) 
                    ? errProp.GetString() 
                    : "Unknown error";
                throw new Exception(errorMsg);
            }
        }
        catch (Exception ex)
        {
            context.Set(MessageTs, "");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
