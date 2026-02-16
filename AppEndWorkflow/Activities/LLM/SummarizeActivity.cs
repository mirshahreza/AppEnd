using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace AppEndWorkflow.Activities.LLM;

/// <summary>
/// Summarizes text using LLM.
/// Supports OpenAI, Anthropic, Azure, and Ollama providers.
/// </summary>
[Activity(
    Category = "AI/LLM",
    Description = "Summarize text using LLM",
    DisplayName = "Summarize Text"
)]
public class SummarizeActivity : CodeActivity
{
    [Input(Description = "Text to summarize")]
    public Input<string> Content { get; set; } = default!;

    [Input(Description = "Maximum summary length in characters")]
    public Input<int?> MaxLength { get; set; }

    [Input(Description = "Content language (for better summarization)")]
    public Input<string?> Language { get; set; }

    [Input(Description = "LLM provider (default: OpenAI)")]
    public Input<string> Provider { get; set; } = default!;

    [Input(Description = "Model name (default: gpt-3.5-turbo)")]
    public Input<string?> Model { get; set; }

    [Output(Description = "Generated summary")]
    public Output<string> Summary { get; set; } = default!;

    [Output(Description = "Original text length")]
    public Output<int> OriginalLength { get; set; } = default!;

    [Output(Description = "Summary length")]
    public Output<int> SummaryLength { get; set; } = default!;

    [Output(Description = "Whether summarization succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var content = context.Get(Content) ?? throw new ArgumentException("Content is required");
            var maxLength = context.Get(MaxLength);
            var language = context.Get(Language) ?? "English";
            var provider = context.Get(Provider) ?? "OpenAI";
            var model = context.Get(Model) ?? "gpt-3.5-turbo";

            var configuration = context.GetService<IConfiguration>();

            var prompt = $"Summarize the following text in {language}.";
            if (maxLength.HasValue)
                prompt += $" Keep the summary under {maxLength} characters.";

            prompt += $"\n\nText to summarize:\n{content}";

            string summaryText;

            if (provider.Equals("OpenAI", StringComparison.OrdinalIgnoreCase))
            {
                summaryText = SummarizeWithOpenAI(configuration, model, prompt);
            }
            else if (provider.Equals("Azure", StringComparison.OrdinalIgnoreCase))
            {
                summaryText = SummarizeWithAzure(configuration, model, prompt);
            }
            else if (provider.Equals("Anthropic", StringComparison.OrdinalIgnoreCase))
            {
                summaryText = SummarizeWithAnthropic(configuration, model, prompt);
            }
            else
            {
                throw new NotSupportedException($"Provider '{provider}' not supported for summarization");
            }

            context.Set(Summary, summaryText);
            context.Set(OriginalLength, content.Length);
            context.Set(SummaryLength, summaryText.Length);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(Summary, "");
            context.Set(OriginalLength, 0);
            context.Set(SummaryLength, 0);
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }

    private string SummarizeWithOpenAI(IConfiguration? configuration, string model, string prompt)
    {
        var apiKey = configuration?["OpenAI:ApiKey"] 
            ?? throw new InvalidOperationException("OpenAI ApiKey not configured");

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        var payload = new
        {
            model = model,
            messages = new[] { new { role = "user", content = prompt } },
            temperature = 0.5f
        };

        var response = httpClient.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", payload).Result;
        var responseBody = response.Content.ReadAsStringAsync().Result;

        using var doc = JsonDocument.Parse(responseBody);
        var root = doc.RootElement;

        if (root.TryGetProperty("choices", out var choicesProp))
        {
            var firstChoice = choicesProp.EnumerateArray().FirstOrDefault();
            if (firstChoice.TryGetProperty("message", out var msgProp) &&
                msgProp.TryGetProperty("content", out var contentProp))
            {
                return contentProp.GetString() ?? "";
            }
        }

        throw new Exception("Invalid response from OpenAI API");
    }

    private string SummarizeWithAzure(IConfiguration? configuration, string model, string prompt)
    {
        var endpoint = configuration?["Azure:OpenAI:Endpoint"]
            ?? throw new InvalidOperationException("Azure OpenAI Endpoint not configured");
        var apiKey = configuration?["Azure:OpenAI:ApiKey"]
            ?? throw new InvalidOperationException("Azure OpenAI ApiKey not configured");

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("api-key", apiKey);

        var payload = new
        {
            messages = new[] { new { role = "user", content = prompt } },
            temperature = 0.5f,
            max_tokens = 512
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
                return contentProp.GetString() ?? "";
            }
        }

        throw new Exception("Invalid response from Azure OpenAI API");
    }

    private string SummarizeWithAnthropic(IConfiguration? configuration, string model, string prompt)
    {
        var apiKey = configuration?["Anthropic:ApiKey"]
            ?? throw new InvalidOperationException("Anthropic ApiKey not configured");

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey);
        httpClient.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");

        var payload = new
        {
            model = model,
            max_tokens = 512,
            messages = new[] { new { role = "user", content = prompt } },
            temperature = 0.5f
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
                return textProp.GetString() ?? "";
            }
        }

        throw new Exception("Invalid response from Anthropic API");
    }
}
