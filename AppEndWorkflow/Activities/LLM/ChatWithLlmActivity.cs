using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;

namespace AppEndWorkflow.Activities.LLM;

/// <summary>
/// Chat with LLM models (GPT-4, Claude, etc.)
/// Supports multiple providers: OpenAI, Anthropic, Azure, Ollama
/// </summary>
[Activity(
    Category = "AI/LLM",
    Description = "Chat with LLM model",
    DisplayName = "Chat with LLM"
)]
public class ChatWithLlmActivity : CodeActivity
{
    [Input(Description = "User message/prompt")]
    public Input<string> Message { get; set; } = default!;

    [Input(Description = "LLM model (e.g., 'gpt-4', 'gpt-3.5-turbo', 'claude-3')")]
    public Input<string> Model { get; set; } = default!;

    [Input(Description = "Provider: 'OpenAI', 'Anthropic', 'Azure', 'Ollama'")]
    public Input<string> Provider { get; set; } = default!;

    [Input(Description = "API key (falls back to settings)")]
    public Input<string?> ApiKey { get; set; }

    [Input(Description = "Randomness 0-1 (default: 0.7)")]
    public Input<float?> Temperature { get; set; }

    [Input(Description = "Max response tokens")]
    public Input<int?> MaxTokens { get; set; }

    [Input(Description = "System instruction")]
    public Input<string?> SystemPrompt { get; set; }

    [Input(Description = "JSON conversation history for context")]
    public Input<string?> Context { get; set; }

    [Output(Description = "LLM response message")]
    public Output<string> Response { get; set; } = default!;

    [Output(Description = "Tokens consumed")]
    public Output<int?> TokensUsed { get; set; }

    [Output(Description = "Whether request succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var message = context.Get(Message) ?? throw new ArgumentException("Message is required");
            var model = context.Get(Model) ?? throw new ArgumentException("Model is required");
            var provider = context.Get(Provider) ?? "OpenAI";

            var configuration = context.GetService<IConfiguration>();

            if (provider.Equals("OpenAI", StringComparison.OrdinalIgnoreCase))
            {
                ChatWithOpenAI(context, configuration, message, model);
            }
            else if (provider.Equals("Azure", StringComparison.OrdinalIgnoreCase))
            {
                ChatWithAzure(context, configuration, message, model);
            }
            else if (provider.Equals("Anthropic", StringComparison.OrdinalIgnoreCase))
            {
                ChatWithAnthropic(context, configuration, message, model);
            }
            else if (provider.Equals("Ollama", StringComparison.OrdinalIgnoreCase))
            {
                ChatWithOllama(context, configuration, message, model);
            }
            else
            {
                throw new NotSupportedException($"Provider '{provider}' is not supported");
            }
        }
        catch (Exception ex)
        {
            context.Set(Response, "");
            context.Set(TokensUsed, null);
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }

    private void ChatWithOpenAI(ActivityExecutionContext context, IConfiguration? configuration, string message, string model)
    {
        var apiKey = context.Get(ApiKey) ?? configuration?["OpenAI:ApiKey"]
            ?? throw new InvalidOperationException("OpenAI ApiKey not configured");
        var temperature = context.Get(Temperature) ?? 0.7f;
        var maxTokens = context.Get(MaxTokens);
        var systemPrompt = context.Get(SystemPrompt);

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        var messages = new List<object>();

        if (!string.IsNullOrWhiteSpace(systemPrompt))
            messages.Add(new { role = "system", content = systemPrompt });

        messages.Add(new { role = "user", content = message });

        var payload = new
        {
            model = model,
            messages = messages,
            temperature = temperature
        };

        if (maxTokens.HasValue)
            ((dynamic)payload).max_tokens = maxTokens.Value;

        var response = httpClient.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", payload).Result;
        var responseBody = response.Content.ReadAsStringAsync().Result;

        using var doc = JsonDocument.Parse(responseBody);
        var root = doc.RootElement;

        if (root.TryGetProperty("choices", out var choicesProp) && choicesProp.ValueKind == JsonValueKind.Array)
        {
            var firstChoice = choicesProp.EnumerateArray().FirstOrDefault();
            if (firstChoice.TryGetProperty("message", out var msgProp) &&
                msgProp.TryGetProperty("content", out var contentProp))
            {
                var responseText = contentProp.GetString();

                int? tokensUsed = null;
                if (root.TryGetProperty("usage", out var usageProp) &&
                    usageProp.TryGetProperty("total_tokens", out var tokensProp))
                {
                    tokensUsed = tokensProp.GetInt32();
                }

                context.Set(Response, responseText ?? "");
                context.Set(TokensUsed, tokensUsed);
                context.Set(Success, true);
                context.Set(Error, null);
                return;
            }
        }

        throw new Exception("Invalid response from OpenAI API");
    }

    private void ChatWithAzure(ActivityExecutionContext context, IConfiguration? configuration, string message, string model)
    {
        var endpoint = configuration?["Azure:OpenAI:Endpoint"]
            ?? throw new InvalidOperationException("Azure OpenAI Endpoint not configured");
        var apiKey = context.Get(ApiKey) ?? configuration?["Azure:OpenAI:ApiKey"]
            ?? throw new InvalidOperationException("Azure OpenAI ApiKey not configured");

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("api-key", apiKey);

        var payload = new
        {
            messages = new[] { new { role = "user", content = message } },
            temperature = context.Get(Temperature) ?? 0.7f,
            max_tokens = context.Get(MaxTokens) ?? 2048
        };

        var url = $"{endpoint}/openai/deployments/{model}/chat/completions?api-version=2024-02-15-preview";
        var response = httpClient.PostAsJsonAsync(url, payload).Result;
        var responseBody = response.Content.ReadAsStringAsync().Result;

        using var doc = JsonDocument.Parse(responseBody);
        var root = doc.RootElement;

        if (root.TryGetProperty("choices", out var choicesProp))
        {
            var firstChoice = choicesProp.EnumerateArray().FirstOrDefault();
            if (firstChoice.TryGetProperty("message", out var msgProp) &&
                msgProp.TryGetProperty("content", out var contentProp))
            {
                context.Set(Response, contentProp.GetString() ?? "");
                context.Set(Success, true);
                context.Set(Error, null);
                return;
            }
        }

        throw new Exception("Invalid response from Azure OpenAI API");
    }

    private void ChatWithAnthropic(ActivityExecutionContext context, IConfiguration? configuration, string message, string model)
    {
        var apiKey = context.Get(ApiKey) ?? configuration?["Anthropic:ApiKey"]
            ?? throw new InvalidOperationException("Anthropic ApiKey not configured");

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey);
        httpClient.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");

        var payload = new
        {
            model = model,
            max_tokens = context.Get(MaxTokens) ?? 1024,
            messages = new[] { new { role = "user", content = message } },
            temperature = context.Get(Temperature) ?? 0.7f
        };

        var response = httpClient.PostAsJsonAsync("https://api.anthropic.com/v1/messages", payload).Result;
        var responseBody = response.Content.ReadAsStringAsync().Result;

        using var doc = JsonDocument.Parse(responseBody);
        var root = doc.RootElement;

        if (root.TryGetProperty("content", out var contentProp) && contentProp.ValueKind == JsonValueKind.Array)
        {
            var firstBlock = contentProp.EnumerateArray().FirstOrDefault();
            if (firstBlock.TryGetProperty("text", out var textProp))
            {
                context.Set(Response, textProp.GetString() ?? "");
                context.Set(Success, true);
                context.Set(Error, null);
                return;
            }
        }

        throw new Exception("Invalid response from Anthropic API");
    }

    private void ChatWithOllama(ActivityExecutionContext context, IConfiguration? configuration, string message, string model)
    {
        var endpoint = configuration?["Ollama:Endpoint"] ?? "http://localhost:11434";

        using var httpClient = new HttpClient();

        var payload = new
        {
            model = model,
            prompt = message,
            temperature = context.Get(Temperature) ?? 0.7f,
            stream = false
        };

        var response = httpClient.PostAsJsonAsync($"{endpoint}/api/generate", payload).Result;
        var responseBody = response.Content.ReadAsStringAsync().Result;

        using var doc = JsonDocument.Parse(responseBody);
        var root = doc.RootElement;

        if (root.TryGetProperty("response", out var responseProp))
        {
            context.Set(Response, responseProp.GetString() ?? "");
            context.Set(Success, true);
            context.Set(Error, null);
            return;
        }

        throw new Exception("Invalid response from Ollama");
    }
}
