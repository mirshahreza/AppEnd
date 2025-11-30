using AppEndCommon;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace AppEndServer
{
    public static class AiServices
    {
        // GitHub Models base endpoint (no /openai segment)
        private static string DefaultBaseUrl => "https://api.github.com";
        private static int DefaultTimeoutSeconds => 30;
        private static string CompletionsPath => "/v1/chat/completions";

        private static JsonObject? GetAiSection()
        {
            var root = AppEndSettings.AppSettings;
            var appEnd = root?[AppEndSettings.ConfigSectionName]?.AsObject();
            return appEnd?["Ai"]?.AsObject();
        }
        private static JsonObject? GetGitHubSection() => GetAiSection()?["GitHub"]?.AsObject();
        private static string? GetGitHubApiKey() => GetGitHubSection()?["ApiKey"]?.ToString();
        private static string GetBaseUrl() => GetGitHubSection()?["BaseUrl"]?.ToStringEmpty().FixNullOrEmpty(DefaultBaseUrl) ?? DefaultBaseUrl;
        private static int GetTimeoutSeconds()
        {
            // Fix: null conditional chain produced int?; coalesce to default
            return GetGitHubSection()?["TimeoutSeconds"]?.ToStringEmpty().ToIntSafe(DefaultTimeoutSeconds) ?? DefaultTimeoutSeconds;
        }

        public static async Task<string?> GenerateTextAsync(string prompt, string model, CancellationToken cancellationToken = default)
        {
            var apiKey = GetGitHubApiKey();
            if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(model) || string.IsNullOrWhiteSpace(prompt)) return null;

            using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(GetTimeoutSeconds()) };
            http.BaseAddress = new Uri(GetBaseUrl());
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            http.DefaultRequestHeaders.UserAgent.ParseAdd("AppEnd/1.0");
            http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            // Optional recommended header for GitHub Models (versioning)
            http.DefaultRequestHeaders.TryAddWithoutValidation("X-GitHub-Api-Version", "2023-07-01");

            var body = new { model, messages = new[] { new { role = "user", content = prompt } } };
            var jsonBody = JsonSerializer.Serialize(body, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await http.PostAsync(CompletionsPath, content, cancellationToken);
            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!response.IsSuccessStatusCode) return json;

            try
            {
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("choices", out var choices) && choices.ValueKind == JsonValueKind.Array && choices.GetArrayLength() > 0)
                {
                    var first = choices[0];
                    if (first.TryGetProperty("message", out var msg) && msg.TryGetProperty("content", out var contentEl))
                        return contentEl.GetString();
                }
            }
            catch { }
            return json; // fallback raw
        }
    }
}
