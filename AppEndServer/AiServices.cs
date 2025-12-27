using AppEndCommon;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.Json.Nodes;

namespace AppEndServer
{
    /// <summary>
    /// Service for calling configured LLM providers based on AppEnd:LLMProviders section.
    /// Supports OpenAI, Gemini (Vertex AI and Direct API), Ollama, and other OpenAI-compatible providers.
    /// </summary>
    public static class AiServices
    {
        private static readonly HttpClient _http = new HttpClient();

        // Constants for provider detection
        private const string OllamaPortIndicator = ":11434";
        private const string GeminiDirectApiHost = "generativelanguage.googleapis.com";
        private const string GeminiDirectApiPath = "/v1beta";
        private const string VertexAiIndicator = "vertexai";
        private const string AiPlatformIndicator = "aiplatform";
        private const string PublishersGoogleModels = "publishers/google/models";

        // Endpoints
        private const string EndpointOllama = "/api/chat";
        private const string EndpointOpenAIChat = "/chat/completions";
        private const string EndpointGeminiDirectFormat = "/models/{0}:generateContent"; // v1beta is in the base URL

        // JSON property names
        private const string JsonPropertyName = "Name";
        private const string JsonPropertyApiBaseUrl = "ApiBaseUrl";
        private const string JsonPropertyApiKey = "ApiKey";
        private const string JsonPropertyModels = "Models";

        /// <summary>
        /// Retrieves all configured LLM providers with their models.
        /// </summary>
        public static List<object> GetAiProvidersWithModels()
        {
            List<object> result = [];
            try
            {
                JsonArray providers = AppEndSettings.LLMProviders;
                foreach (JsonNode? providerNode in providers)
                {
                    if (providerNode is null) continue;

                    JsonObject provider = providerNode.AsObject();
                    string name = provider[JsonPropertyName].ToStringEmpty();
                    string apiBaseUrl = provider[JsonPropertyApiBaseUrl].ToStringEmpty();
                    string apiKey = provider[JsonPropertyApiKey].ToStringEmpty();
                    List<string> models = ExtractModels(provider);

                    // Skip completely empty entries
                    if (IsProviderEmpty(name, apiBaseUrl, apiKey, models))
                    {
                        continue;
                    }

                    result.Add(new
                    {
                        Name = name,
                        ApiBaseUrl = apiBaseUrl,
                        ApiKey = apiKey,
                        Models = models
                    });
                }
            }
            catch
            {
                // Return what we have so far on error
            }

            return result;
        }

        /// <summary>
        /// Generates text using the specified model from configured LLM providers.
        /// </summary>
        public static async Task<string> GenerateFromAppSettingsAsync(string prompt, string model)
        {
            var providersNode = AppEndSettings.LLMProviders;
            if (providersNode == null || providersNode.Count == 0)
            {
                return "LLMProviders not configured";
            }

            (string? apiKey, string? apiBase) = FindProviderForModel(providersNode, model);
            if (apiBase == null)
            {
                return $"Model '{model}' not found in LLMProviders";
            }

            apiBase = apiBase.TrimEnd('/');
            ProviderType providerType = DetermineProviderType(apiBase);
            string endpoint = GetEndpoint(providerType, model, apiBase);
            string url = apiBase + endpoint;
            object payload = BuildPayload(providerType, model, prompt, apiBase);

            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            ApplyAuthentication(request, providerType, apiKey, ref url, apiBase);

            request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            using var response = await _http.SendAsync(request).ConfigureAwait(false);
            string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                // Provide helpful error messages for authentication issues
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    string apiBaseLower = apiBase.ToLowerInvariant();
                    bool isVertexAi = apiBaseLower.Contains(VertexAiIndicator) || 
                                      apiBaseLower.Contains(AiPlatformIndicator) ||
                                      apiBaseLower.Contains(PublishersGoogleModels);
                    
                    if (isVertexAi)
                    {
                        return $"Authentication Error (401): Vertex AI requires an OAuth 2.0 access token, not an API key. " +
                               $"Please use 'gcloud auth print-access-token' to get a token, or use Gemini Direct API instead " +
                               $"(ApiBaseUrl: https://generativelanguage.googleapis.com/v1beta) which accepts API keys. " +
                               $"Response: {responseBody}";
                    }
                }
                
                return $"Error from LLM provider: {(int)response.StatusCode} {response.ReasonPhrase} - {responseBody}";
            }

            return ParseResponse(responseBody, providerType, apiBase);
        }

        #region Private Helper Methods

        private static List<string> ExtractModels(JsonObject provider)
        {
            List<string> models = [];
            if (provider[JsonPropertyModels] is JsonArray modelsArray)
            {
                foreach (JsonNode? modelNode in modelsArray)
                {
                    string modelName = modelNode.ToStringEmpty();
                    if (!string.IsNullOrWhiteSpace(modelName))
                    {
                        models.Add(modelName);
                    }
                }
            }
            return models;
        }

        private static bool IsProviderEmpty(string name, string apiBaseUrl, string apiKey, List<string> models)
        {
            return string.IsNullOrWhiteSpace(name) &&
                   string.IsNullOrWhiteSpace(apiBaseUrl) &&
                   string.IsNullOrWhiteSpace(apiKey) &&
                   models.Count == 0;
        }

        private static (string? apiKey, string? apiBase) FindProviderForModel(JsonArray providers, string modelName)
        {
            foreach (JsonNode? provider in providers)
            {
                if (provider == null) continue;

                JsonArray? modelsNode = provider[JsonPropertyModels] as JsonArray;
                if (modelsNode == null) continue;

                foreach (JsonNode? modelNode in modelsNode)
                {
                    if (string.Equals(modelNode?.ToString(), modelName, StringComparison.OrdinalIgnoreCase))
                    {
                        string? apiKey = provider[JsonPropertyApiKey]?.ToString();
                        string? apiBase = provider[JsonPropertyApiBaseUrl]?.ToString();
                        return (apiKey, apiBase);
                    }
                }
            }

            return (null, null);
        }

        private enum ProviderType
        {
            Ollama,
            GeminiDirect,
            OpenAICompatible
        }

        private static ProviderType DetermineProviderType(string apiBase)
        {
            // Normalize to lowercase for case-insensitive comparison
            string apiBaseLower = apiBase.ToLowerInvariant();

            // Check for Ollama first (most specific)
            if (apiBaseLower.Contains("localhost:11434") ||
                apiBaseLower.Contains("127.0.0.1:11434") ||
                apiBaseLower.Contains(OllamaPortIndicator))
            {
                return ProviderType.Ollama;
            }

            // Check for Vertex AI or AI Platform
            // Vertex AI URLs contain "aiplatform.googleapis.com" and "publishers/google/models"
            // These should use the generateContent endpoint, not OpenAI-compatible endpoint
            if (apiBaseLower.Contains(VertexAiIndicator) || 
                apiBaseLower.Contains(AiPlatformIndicator) ||
                apiBaseLower.Contains(PublishersGoogleModels))
            {
                return ProviderType.OpenAICompatible; // Marked as OpenAICompatible but will use special endpoint
            }

            // Check for Gemini Direct API (only if it's generativelanguage.googleapis.com)
            // This is the direct Gemini API, not Vertex AI
            if (apiBaseLower.Contains(GeminiDirectApiHost))
            {
                return ProviderType.GeminiDirect;
            }

            // All other providers are OpenAI-compatible (OpenAI, Anthropic, Mistral, Azure OpenAI, etc.)
            return ProviderType.OpenAICompatible;
        }

        private static string GetEndpoint(ProviderType providerType, string model, string apiBase)
        {
            // Check if this is Vertex AI (case-insensitive)
            string apiBaseLower = apiBase.ToLowerInvariant();
            bool isVertexAi = apiBaseLower.Contains(VertexAiIndicator) || 
                              apiBaseLower.Contains(AiPlatformIndicator) ||
                              apiBaseLower.Contains(PublishersGoogleModels);
            
            if (isVertexAi && providerType == ProviderType.OpenAICompatible)
            {
                // Vertex AI Gemini format: base_url should end with /models, then append /{model}:generateContent
                // Example: https://us-central1-aiplatform.googleapis.com/v1/projects/PROJECT/locations/us-central1/publishers/google/models/gemini-1.5-pro:generateContent
                // The apiBase should already end with /models
                if (apiBase.EndsWith("/models", StringComparison.OrdinalIgnoreCase))
                {
                    return $"/{model}:generateContent";
                }
                // If base doesn't end with /models (shouldn't happen), append it
                return $"/models/{model}:generateContent";
            }
            
            return providerType switch
            {
                ProviderType.Ollama => EndpointOllama,
                ProviderType.GeminiDirect => string.Format(EndpointGeminiDirectFormat, model),
                ProviderType.OpenAICompatible => EndpointOpenAIChat,
                _ => EndpointOpenAIChat
            };
        }

        private static object BuildPayload(ProviderType providerType, string model, string prompt, string apiBase)
        {
            // Check if this is Vertex AI (case-insensitive)
            string apiBaseLower = apiBase.ToLowerInvariant();
            bool isVertexAi = apiBaseLower.Contains(VertexAiIndicator) || 
                              apiBaseLower.Contains(AiPlatformIndicator) ||
                              apiBaseLower.Contains(PublishersGoogleModels);
            
            if (isVertexAi && providerType == ProviderType.OpenAICompatible)
            {
                // Vertex AI uses the same payload format as Gemini Direct API
                return new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[] { new { text = prompt } }
                        }
                    }
                };
            }
            
            return providerType switch
            {
                ProviderType.Ollama => new
                {
                    model = model,
                    messages = new[] { new { role = "user", content = prompt } },
                    stream = false
                },
                ProviderType.GeminiDirect => new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[] { new { text = prompt } }
                        }
                    }
                },
                _ => new
                {
                    model = model,
                    messages = new[] { new { role = "user", content = prompt } }
                }
            };
        }

        private static void ApplyAuthentication(HttpRequestMessage request, ProviderType providerType, string? apiKey, ref string url, string apiBase)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return;
            }

            // Check if this is Vertex AI (case-insensitive)
            string apiBaseLower = apiBase.ToLowerInvariant();
            bool isVertexAi = apiBaseLower.Contains(VertexAiIndicator) || 
                              apiBaseLower.Contains(AiPlatformIndicator) ||
                              apiBaseLower.Contains(PublishersGoogleModels);
            
            if (providerType == ProviderType.GeminiDirect)
            {
                // Direct Gemini API uses x-goog-api-key header for API key (more secure than query parameter)
                request.Headers.Add("x-goog-api-key", apiKey);
            }
            else if (isVertexAi && providerType == ProviderType.OpenAICompatible)
            {
                // Vertex AI uses OAuth 2.0 Bearer token (access token) in header
                // The apiKey should be an access token from Google Cloud authentication
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            }
            else
            {
                // Most providers (OpenAI, Anthropic, Mistral, Azure OpenAI, etc.) use Bearer token in header
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            }
        }

        private static string ParseResponse(string responseBody, ProviderType providerType, string apiBase)
        {
            try
            {
                using var doc = JsonDocument.Parse(responseBody);
                var root = doc.RootElement;

                // Check if this is Vertex AI (case-insensitive)
                string apiBaseLower = apiBase.ToLowerInvariant();
                bool isVertexAi = apiBaseLower.Contains(VertexAiIndicator) || 
                                  apiBaseLower.Contains(AiPlatformIndicator) ||
                                  apiBaseLower.Contains(PublishersGoogleModels);

                // Try Ollama response structure
                if (providerType == ProviderType.Ollama)
                {
                    string? ollamaContent = TryParseOllamaResponse(root);
                    if (ollamaContent != null)
                    {
                        return ollamaContent;
                    }
                }

                // Try Gemini Direct API or Vertex AI response structure
                if (providerType == ProviderType.GeminiDirect || (isVertexAi && providerType == ProviderType.OpenAICompatible))
                {
                    string? geminiContent = TryParseGeminiDirectResponse(root);
                    if (geminiContent != null)
                    {
                        return geminiContent;
                    }
                }

                // Try OpenAI-compatible response structure (OpenAI, Anthropic, Mistral, etc.)
                string? openAiContent = TryParseOpenAICompatibleResponse(root);
                if (openAiContent != null)
                {
                    return openAiContent;
                }
            }
            catch (Exception ex)
            {
                return $"Error parsing response: {ex.Message}. Response body: {responseBody}";
            }

            return responseBody;
        }

        private static string? TryParseOllamaResponse(JsonElement root)
        {
            if (root.TryGetProperty("message", out var message) &&
                message.TryGetProperty("content", out var content))
            {
                return content.GetString() ?? string.Empty;
            }
            return null;
        }

        private static string? TryParseGeminiDirectResponse(JsonElement root)
        {
            if (root.TryGetProperty("candidates", out var candidates) &&
                candidates.GetArrayLength() > 0)
            {
                var candidate = candidates[0];
                if (candidate.TryGetProperty("content", out var geminiContent) &&
                    geminiContent.TryGetProperty("parts", out var parts) &&
                    parts.GetArrayLength() > 0 &&
                    parts[0].TryGetProperty("text", out var text))
                {
                    return text.GetString() ?? string.Empty;
                }
            }
            return null;
        }

        private static string? TryParseOpenAICompatibleResponse(JsonElement root)
        {
            if (root.TryGetProperty("choices", out var choices) &&
                choices.GetArrayLength() > 0)
            {
                var choice = choices[0];
                if (choice.TryGetProperty("message", out var message) &&
                    message.TryGetProperty("content", out var content))
                {
                    return content.GetString() ?? string.Empty;
                }
            }
            return null;
        }

        #endregion
    }
}