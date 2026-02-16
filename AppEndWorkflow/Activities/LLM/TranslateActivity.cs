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
/// Translates text between languages using LLM.
/// Supports OpenAI, Anthropic, Azure, and direct translation APIs.
/// </summary>
[Activity(
    Category = "AI/LLM",
    Description = "Translate text between languages",
    DisplayName = "Translate Text"
)]
public class TranslateActivity : CodeActivity
{
    [Input(Description = "Text to translate")]
    public Input<string> Text { get; set; } = default!;

    [Input(Description = "Source language (e.g., 'fa', 'en')")]
    public Input<string> SourceLanguage { get; set; } = default!;

    [Input(Description = "Target language (e.g., 'en', 'de')")]
    public Input<string> TargetLanguage { get; set; } = default!;

    [Input(Description = "Provider: 'OpenAI', 'GoogleTranslate', 'DeepL'")]
    public Input<string> Provider { get; set; } = default!;

    [Output(Description = "Translated text")]
    public Output<string> TranslatedText { get; set; } = default!;

    [Output(Description = "Auto-detected source language")]
    public Output<string?> DetectedLanguage { get; set; }

    [Output(Description = "Whether translation succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var text = context.Get(Text) ?? throw new ArgumentException("Text is required");
            var sourceLang = context.Get(SourceLanguage) ?? throw new ArgumentException("SourceLanguage is required");
            var targetLang = context.Get(TargetLanguage) ?? throw new ArgumentException("TargetLanguage is required");
            var provider = context.Get(Provider) ?? "OpenAI";

            var configuration = context.GetService<IConfiguration>();

            string translatedText;

            if (provider.Equals("OpenAI", StringComparison.OrdinalIgnoreCase))
            {
                translatedText = TranslateWithOpenAI(configuration, text, sourceLang, targetLang);
            }
            else if (provider.Equals("GoogleTranslate", StringComparison.OrdinalIgnoreCase))
            {
                translatedText = TranslateWithGoogle(configuration, text, targetLang);
            }
            else if (provider.Equals("DeepL", StringComparison.OrdinalIgnoreCase))
            {
                translatedText = TranslateWithDeepL(configuration, text, sourceLang, targetLang);
            }
            else
            {
                throw new NotSupportedException($"Provider '{provider}' not supported");
            }

            context.Set(TranslatedText, translatedText);
            context.Set(DetectedLanguage, sourceLang);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(TranslatedText, "");
            context.Set(DetectedLanguage, null);
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }

    private string TranslateWithOpenAI(IConfiguration? configuration, string text, string sourceLang, string targetLang)
    {
        var apiKey = configuration?["OpenAI:ApiKey"]
            ?? throw new InvalidOperationException("OpenAI ApiKey not configured");

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        var prompt = $"Translate the following text from {GetLanguageName(sourceLang)} to {GetLanguageName(targetLang)}.\n\n{text}";

        var payload = new
        {
            model = "gpt-3.5-turbo",
            messages = new[] { new { role = "user", content = prompt } },
            temperature = 0.3f
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

    private string TranslateWithGoogle(IConfiguration? configuration, string text, string targetLang)
    {
        var apiKey = configuration?["Google:TranslateApiKey"]
            ?? throw new InvalidOperationException("Google Translate ApiKey not configured");

        using var httpClient = new HttpClient();

        var targetCode = ConvertLanguageCode(targetLang);
        var url = $"https://translation.googleapis.com/language/translate/v2?key={apiKey}&target={targetCode}&q={Uri.EscapeDataString(text)}";

        var response = httpClient.GetAsync(url).Result;
        var responseBody = response.Content.ReadAsStringAsync().Result;

        using var doc = JsonDocument.Parse(responseBody);
        var root = doc.RootElement;

        if (root.TryGetProperty("data", out var dataProp) &&
            dataProp.TryGetProperty("translations", out var translationsProp) &&
            translationsProp.ValueKind == JsonValueKind.Array)
        {
            var firstTranslation = translationsProp.EnumerateArray().FirstOrDefault();
            if (firstTranslation.TryGetProperty("translatedText", out var textProp))
            {
                return textProp.GetString() ?? "";
            }
        }

        throw new Exception("Invalid response from Google Translate API");
    }

    private string TranslateWithDeepL(IConfiguration? configuration, string text, string sourceLang, string targetLang)
    {
        var apiKey = configuration?["DeepL:ApiKey"]
            ?? throw new InvalidOperationException("DeepL ApiKey not configured");

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"DeepL-Auth-Key {apiKey}");

        var payload = new
        {
            text = new[] { text },
            source_lang = ConvertLanguageCode(sourceLang).ToUpperInvariant(),
            target_lang = ConvertLanguageCode(targetLang).ToUpperInvariant()
        };

        var response = httpClient.PostAsJsonAsync("https://api-free.deepl.com/v2/translate", payload).Result;
        var responseBody = response.Content.ReadAsStringAsync().Result;

        using var doc = JsonDocument.Parse(responseBody);
        var root = doc.RootElement;

        if (root.TryGetProperty("translations", out var translationsProp) && translationsProp.ValueKind == JsonValueKind.Array)
        {
            var firstTranslation = translationsProp.EnumerateArray().FirstOrDefault();
            if (firstTranslation.TryGetProperty("text", out var textProp))
            {
                return textProp.GetString() ?? "";
            }
        }

        throw new Exception("Invalid response from DeepL API");
    }

    private static string GetLanguageName(string code) => code switch
    {
        "fa" => "Persian",
        "en" => "English",
        "de" => "German",
        "fr" => "French",
        "es" => "Spanish",
        "zh" => "Chinese",
        "ja" => "Japanese",
        "ar" => "Arabic",
        _ => code
    };

    private static string ConvertLanguageCode(string code) => code switch
    {
        "fa" => "fa",
        "en" => "en",
        "de" => "de",
        "fr" => "fr",
        "es" => "es",
        "zh" => "zh",
        "ja" => "ja",
        "ar" => "ar",
        _ => code.ToLower()
    };
}
