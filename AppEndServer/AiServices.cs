using AppEndCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.Json.Nodes;

namespace AppEndServer
{
    /// <summary>
    /// Very small helper to call configured LLM providers based on AppEnd:LLMProviders section.
    /// </summary>
    public static class AiServices
    {
        private static readonly HttpClient _http = new HttpClient();

        public static List<object> GetAiProvidersWithModels()
        {
            List<object> res = [];
            try
            {
                // Use strongly-typed accessor that already normalizes/initializes the section
                JsonArray providers = AppEndSettings.LLMProviders;
                foreach (JsonNode? n in providers)
                {
                    if (n is null) continue;
                    JsonObject o = n.AsObject();
                    string name = o["Name"].ToStringEmpty();
                    string apiBaseUrl = o["ApiBaseUrl"].ToStringEmpty();
                    string apiKey = o["ApiKey"].ToStringEmpty();

                    List<string> models = [];
                    if (o["Models"] is JsonArray arr)
                    {
                        foreach (JsonNode? m in arr)
                        {
                            string mv = m.ToStringEmpty();
                            if (!string.IsNullOrWhiteSpace(mv)) models.Add(mv);
                        }
                    }

                    // Skip empty entries
                    if (string.IsNullOrWhiteSpace(name) &&
                        string.IsNullOrWhiteSpace(apiBaseUrl) &&
                        string.IsNullOrWhiteSpace(apiKey) &&
                        models.Count == 0)
                    {
                        continue;
                    }

                    res.Add(new
                    {
                        Name = name,
                        Models = models
                    });
                }
            }
            catch
            {
                // Ignore and return what we have
            }

            return res;
        }

        public static async Task<string> GenerateFromAppSettingsAsync(string prompt, string model)
        {
            // read providers from settings
            var appEnd = AppEndSettings.AppSettings[AppEndSettings.ConfigSectionName];
            if (appEnd == null) return "AppEnd settings not found";

            var providersNode = appEnd?["LLMProviders"] as System.Text.Json.Nodes.JsonArray;
            if (providersNode == null) return "LLMProviders not configured";

            string? apiKey = null;
            string? apiBase = null;

            foreach (var p in providersNode)
            {
                if (p == null) continue;
                var nameNode = p["Name"];
                var modelsNode = p["Models"] as System.Text.Json.Nodes.JsonArray;
                if (modelsNode == null) continue;
                foreach (var m in modelsNode)
                {
                    if (string.Equals(m?.ToString(), model, StringComparison.OrdinalIgnoreCase))
                    {
                        apiKey = p["ApiKey"]?.ToString();
                        apiBase = p["ApiBaseUrl"]?.ToString();
                        break;
                    }
                }
                if (apiKey != null) break;
            }

            if (apiKey == null || apiBase == null) return $"Model '{model}' not found in LLMProviders";

            // naive OpenAI-compatible chat completion call
            var url = apiBase.TrimEnd('/') + "/chat/completions";
            var payload = new
            {
                model = model,
                messages = new[] { new { role = "user", content = prompt } }
            };
            var json = JsonSerializer.Serialize(payload);
            using var req = new HttpRequestMessage(HttpMethod.Post, url);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            req.Content = new StringContent(json, Encoding.UTF8, "application/json");

            using var resp = await _http.SendAsync(req).ConfigureAwait(false);
            var body = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (!resp.IsSuccessStatusCode)
            {
                return $"Error from LLM provider: {(int)resp.StatusCode} {resp.ReasonPhrase} - {body}";
            }

            try
            {
                using var doc = JsonDocument.Parse(body);
                var root = doc.RootElement;
                var choice = root.GetProperty("choices")[0];
                if (choice.TryGetProperty("message", out var msg) && msg.TryGetProperty("content", out var content))
                {
                    return content.GetString() ?? string.Empty;
                }
            }
            catch
            {
                // fall through and return raw body
            }

            return body;
        }
    }
}
