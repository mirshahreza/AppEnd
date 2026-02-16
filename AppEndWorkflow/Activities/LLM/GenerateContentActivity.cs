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
/// Generates various types of content using LLM.
/// Supports article, email, blog post, social post, and code generation.
/// </summary>
[Activity(
    Category = "AI/LLM",
    Description = "Generate content using LLM",
    DisplayName = "Generate Content"
)]
public class GenerateContentActivity : CodeActivity
{
    [Input(Description = "Content generation prompt")]
    public Input<string> Prompt { get; set; } = default!;

    [Input(Description = "Type: Article, Email, BlogPost, SocialPost, Code")]
    public Input<string> ContentType { get; set; } = default!;

    [Input(Description = "Tone: Professional, Casual, Formal")]
    public Input<string?> Tone { get; set; }

    [Input(Description = "Length: Short, Medium, Long")]
    public Input<string?> Length { get; set; }

    [Input(Description = "LLM provider")]
    public Input<string> Provider { get; set; } = default!;

    [Input(Description = "Model name")]
    public Input<string?> Model { get; set; }

    [Output(Description = "Generated content")]
    public Output<string> GeneratedContent { get; set; } = default!;

    [Output(Description = "Whether generation succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var prompt = context.Get(Prompt) ?? throw new ArgumentException("Prompt is required");
            var contentType = context.Get(ContentType) ?? throw new ArgumentException("ContentType is required");
            var tone = context.Get(Tone) ?? "Professional";
            var length = context.Get(Length) ?? "Medium";
            var provider = context.Get(Provider) ?? "OpenAI";
            var model = context.Get(Model) ?? "gpt-3.5-turbo";

            var configuration = context.GetService<IConfiguration>();

            var enhancedPrompt = BuildPrompt(prompt, contentType, tone, length);

            var content = GenerateContent(configuration, provider, model, enhancedPrompt);

            context.Set(GeneratedContent, content);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(GeneratedContent, "");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }

    private string BuildPrompt(string prompt, string contentType, string tone, string length)
    {
        var lengthGuidance = length switch
        {
            "Short" => "Keep it concise (100-200 words)",
            "Medium" => "Keep it moderate (300-500 words)",
            "Long" => "Provide detailed content (800-1200 words)",
            _ => ""
        };

        var typeGuidance = contentType switch
        {
            "Article" => "Write as a professional article",
            "Email" => "Write as a professional email",
            "BlogPost" => "Write as an engaging blog post",
            "SocialPost" => "Write as a social media post (max 280 characters)",
            "Code" => "Generate clean, well-documented code",
            _ => ""
        };

        return $"{typeGuidance} with a {tone} tone. {lengthGuidance}\n\n{prompt}";
    }

    private string GenerateContent(IConfiguration? configuration, string provider, string model, string prompt)
    {
        if (provider.Equals("OpenAI", StringComparison.OrdinalIgnoreCase))
        {
            return GenerateWithOpenAI(configuration, model, prompt);
        }
        else if (provider.Equals("Azure", StringComparison.OrdinalIgnoreCase))
        {
            return GenerateWithAzure(configuration, model, prompt);
        }
        else
        {
            throw new NotSupportedException($"Provider '{provider}' not supported");
        }
    }

    private string GenerateWithOpenAI(IConfiguration? configuration, string model, string prompt)
    {
        var apiKey = configuration?["OpenAI:ApiKey"]
            ?? throw new InvalidOperationException("OpenAI ApiKey not configured");

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        var payload = new
        {
            model = model,
            messages = new[] { new { role = "user", content = prompt } },
            temperature = 0.7f
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

    private string GenerateWithAzure(IConfiguration? configuration, string model, string prompt)
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
            temperature = 0.7f,
            max_tokens = 2048
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
}
