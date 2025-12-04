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
        private static string CopilotBase => "https://api.githubcopilot.com";
        private static string GoogleBase => "https://generativelanguage.googleapis.com";
        private static int DefaultTimeoutSeconds => 60;

        private static JsonObject? GetAiSection()
        {
            var root = AppEndSettings.AppSettings;
            var appEnd = root?[AppEndSettings.ConfigSectionName]?.AsObject();
            return appEnd?["Ai"]?.AsObject();
        }
        private static JsonObject? GetGitHubSection() => GetAiSection()?["GitHub"]?.AsObject();
        private static JsonObject? GetGoogleSection() => GetAiSection()?["Google"]?.AsObject();

        private static string? GetGoogleApiKey() => GetGoogleSection()?["ApiKey"]?.ToString();
        private static string? GetGitHubApiKey() => GetGitHubSection()?["ApiKey"]?.ToString();
        private static string GetGoogleBaseUrl() => GetGoogleSection()?["BaseUrl"]?.ToStringEmpty().FixNullOrEmpty(GoogleBase) ?? GoogleBase;
        private static string GetCopilotBaseUrl() => GetGitHubSection()?["BaseUrl"]?.ToStringEmpty().FixNullOrEmpty(CopilotBase) ?? CopilotBase;
        private static int GetTimeoutSeconds() => GetAiSection()?["TimeoutSeconds"]?.ToStringEmpty().ToIntSafe(DefaultTimeoutSeconds) ?? DefaultTimeoutSeconds;

        // Main entry: prefer explicit provider if given, otherwise auto
        public static async Task<string?> GenerateTextAsync(string prompt, string model, string? provider = null, CancellationToken cancellationToken = default)
        {
            string p = provider?.ToStringEmpty().Trim() ?? string.Empty;
            if (p.Equals("Google", StringComparison.OrdinalIgnoreCase))
            {
                var gkey = GetGoogleApiKey();
                if (!string.IsNullOrWhiteSpace(gkey)) return await GenerateWithGoogleAsync(prompt, model, gkey, cancellationToken);
                return "Google provider not configured";
            }
            if (p.Equals("GitHub", StringComparison.OrdinalIgnoreCase))
            {
                var ghKey = GetGitHubApiKey();
                if (!string.IsNullOrWhiteSpace(ghKey)) return await GenerateWithCopilotAsync(prompt, model, ghKey, cancellationToken);
                return "GitHub provider not configured";
            }
            // Auto: prefer Google if configured else GitHub
            var autoG = GetGoogleApiKey();
            if (!string.IsNullOrWhiteSpace(autoG)) return await GenerateWithGoogleAsync(prompt, model, autoG, cancellationToken);
            var autoGh = GetGitHubApiKey();
            if (!string.IsNullOrWhiteSpace(autoGh)) return await GenerateWithCopilotAsync(prompt, model, autoGh, cancellationToken);
            return null;
        }

        private static async Task<string?> GenerateWithGoogleAsync(string prompt, string model, string apiKey, CancellationToken ct)
        {
            using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(GetTimeoutSeconds()) };
            http.BaseAddress = new Uri(GetGoogleBaseUrl());
            http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            http.DefaultRequestHeaders.UserAgent.ParseAdd("AppEnd/1.0");

            var path = $"/v1beta/models/{Uri.EscapeDataString(model)}:generateContent?key={Uri.EscapeDataString(apiKey)}";
            var body = new { contents = new object[] { new { role = "user", parts = new object[] { new { text = prompt } } } } };
            var jsonBody = JsonSerializer.Serialize(body, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var response = await http.PostAsync(path, content, ct);
            var json = await response.Content.ReadAsStringAsync(ct);
            if (!response.IsSuccessStatusCode) return json;
            try
            {
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("candidates", out var candidates) && candidates.ValueKind == JsonValueKind.Array && candidates.GetArrayLength() > 0)
                {
                    var cand = candidates[0];
                    if (cand.TryGetProperty("content", out var contentEl) && contentEl.TryGetProperty("parts", out var parts) && parts.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var part in parts.EnumerateArray())
                        {
                            if (part.TryGetProperty("text", out var t)) return t.GetString();
                        }
                    }
                }
            }
            catch { }
            return json;
        }

        private static async Task<string?> GenerateWithCopilotAsync(string prompt, string model, string apiKey, CancellationToken ct)
        {
            using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(GetTimeoutSeconds()) };
            http.BaseAddress = new Uri(GetCopilotBaseUrl());
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            http.DefaultRequestHeaders.TryAddWithoutValidation("Editor-Version", "vscode/1.95.0");
            http.DefaultRequestHeaders.TryAddWithoutValidation("Editor-Plugin-Version", "copilot-chat/0.22.0");
            http.DefaultRequestHeaders.TryAddWithoutValidation("Openai-Organization", "github-copilot");
            http.DefaultRequestHeaders.TryAddWithoutValidation("Openai-Intent", "conversation-panel");
            http.DefaultRequestHeaders.UserAgent.ParseAdd("GitHubCopilotChat/0.22.0");
            http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var body = new { model, messages = new[] { new { role = "user", content = prompt } } };
            var jsonBody = JsonSerializer.Serialize(body, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var response = await http.PostAsync("/chat/completions", content, ct);
            var json = await response.Content.ReadAsStringAsync(ct);
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
            return json;
        }
    }
}
