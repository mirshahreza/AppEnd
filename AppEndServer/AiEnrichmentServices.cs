using AppEndCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace AppEndServer
{
    /// <summary>
    /// Specialized AI Enrichment Service: uses LLM providers from appsettings to generate bilingual (EN + Persian)
    /// BaseZetadata fields from schema context, then upserts into BaseZetadata with MD5 StructureId logic.
    /// </summary>
    public static class AiEnrichmentServices
    {
        private const string JsonPropertyName = "Name";
        private const string JsonPropertyApiBaseUrl = "ApiBaseUrl";
        private const string JsonPropertyApiKey = "ApiKey";
        private const string JsonPropertyModels = "Models";

        /// <summary>
        /// Gets the first model for the given provider name from AppEnd:LLMProviders (e.g. "LocalOllama", "Gemini Vertex AI").
        /// </summary>
        public static string? GetModelForProvider(string providerName)
        {
            if (string.IsNullOrWhiteSpace(providerName)) return null;
            try
            {
                var providers = AppEndSettings.LLMProviders;
                foreach (JsonNode? providerNode in providers)
                {
                    if (providerNode == null) continue;
                    JsonObject provider = providerNode.AsObject();
                    string name = provider[JsonPropertyName].ToStringEmpty();
                    if (!name.Equals(providerName, StringComparison.OrdinalIgnoreCase)) continue;
                    if (provider[JsonPropertyModels] is not JsonArray models || models.Count == 0) continue;
                    var first = models[0]?.ToString();
                    return string.IsNullOrWhiteSpace(first) ? null : first;
                }
            }
            catch { }
            return null;
        }

        /// <summary>
        /// Gets the default model for enrichment: prefers LocalOllama (e.g. qwen2.5-coder:7b-instruct), then Gemini Vertex AI, then first available.
        /// </summary>
        public static string? GetDefaultEnrichmentModel()
        {
            return GetModelForProvider("LocalOllama")
                   ?? GetModelForProvider("Gemini Vertex AI")
                   ?? GetFirstAvailableModel();
        }

        private static string? GetFirstAvailableModel()
        {
            try
            {
                var providers = AppEndSettings.LLMProviders;
                foreach (JsonNode? providerNode in providers)
                {
                    if (providerNode == null) continue;
                    JsonObject provider = providerNode.AsObject();
                    if (provider[JsonPropertyModels] is not JsonArray models) continue;
                    foreach (var modelNode in models)
                    {
                        var m = modelNode?.ToString();
                        if (!string.IsNullOrWhiteSpace(m)) return m;
                    }
                }
            }
            catch { }
            return null;
        }

        /// <summary>
        /// Builds the RAG-oriented prompt for the LLM: documents schema so an AI Agent can write accurate SQL from natural language.
        /// Native language for HumanTitleNative, NoteNative, KeywordsNative is read from AppEndSettings.EnrichmentNativeLanguage.
        /// </summary>
        public static string BuildEnrichmentPrompt(SchemaDetailForEnrichment detail)
        {
            string nativeLang = AppEndSettings.EnrichmentNativeLanguage;
            if (string.IsNullOrWhiteSpace(nativeLang)) nativeLang = "Persian";
            return "You are an expert Data Engineer specializing in RAG (Retrieval-Augmented Generation). Your goal is to document database schemas so an AI Agent can later write accurate SQL queries based on natural language questions.\n\n" +
                   "Instructions:\n" +
                   $"- Provide professional, technical descriptions in both English and {nativeLang}.\n" +
                   "- For HumanTitleNative, NoteNative, and KeywordsNative use " + nativeLang + " only.\n" +
                   "- In NoteEn and NoteNative, explain the 'Business Intent' of the object (e.g., for a status code column, explain what each value represents; for a table, summarize what the entity represents).\n" +
                   "- In KeywordsEn and KeywordsNative, include synonyms and alternate terms. Example: for a 'Price' column include 'Cost', 'Amount' in English and equivalent terms in " + nativeLang + ". Use comma- or space-separated terms.\n" +
                   "- Return ONLY a valid JSON object. No prose, no markdown formatting outside the JSON (no ``` or code blocks).\n\n" +
                   $"Schema context:\n{detail.SchemaContextDescription}\n\n" +
                   $"Object to document: Type = {detail.ObjectType}, Name = {detail.ObjectName}.\n\n" +
                   "Output a single JSON object with exactly these keys: HumanTitleEn, HumanTitleNative, NoteEn, NoteNative, KeywordsEn, KeywordsNative.";
        }

        /// <summary>
        /// Parses LLM response into the six BaseZetadata text fields. Returns null if parsing fails.
        /// </summary>
        public static EnrichmentParsedResult? ParseEnrichmentResponse(string responseBody)
        {
            if (string.IsNullOrWhiteSpace(responseBody)) return null;
            var trimmed = responseBody.Trim();
            // Strip markdown code block if present
            if (trimmed.StartsWith("```json", StringComparison.OrdinalIgnoreCase))
                trimmed = trimmed.Substring(7).Trim();
            else if (trimmed.StartsWith("```", StringComparison.Ordinal))
                trimmed = trimmed.Substring(3).Trim();
            if (trimmed.EndsWith("```", StringComparison.Ordinal))
                trimmed = trimmed.Substring(0, trimmed.Length - 3).Trim();
            try
            {
                using var doc = JsonDocument.Parse(trimmed);
                var root = doc.RootElement;
                return new EnrichmentParsedResult
                {
                    HumanTitleEn = root.TryGetProperty("HumanTitleEn", out var v) ? v.GetString() : null,
                    HumanTitleNative = root.TryGetProperty("HumanTitleNative", out v) ? v.GetString() : null,
                    NoteEn = root.TryGetProperty("NoteEn", out v) ? v.GetString() : null,
                    NoteNative = root.TryGetProperty("NoteNative", out v) ? v.GetString() : null,
                    KeywordsEn = root.TryGetProperty("KeywordsEn", out v) ? v.GetString() : null,
                    KeywordsNative = root.TryGetProperty("KeywordsNative", out v) ? v.GetString() : null
                };
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Enriches the given structure IDs: fetches schema context, calls LLM for each, parses bilingual JSON, then upserts into BaseZetadata.
        /// StructureId is computed as MD5(connectionName:schemaName:tableName:objectName). If exists, update; otherwise insert.
        /// UpdatedOn is set to current timestamp; Vector columns are left NULL/empty.
        /// </summary>
        /// <param name="connectionName">Connection name (e.g. DefaultRepo).</param>
        /// <param name="structureIds">List of StructureIds to enrich.</param>
        /// <param name="model">Optional model name; if null, uses default (LocalOllama or Gemini Vertex AI).</param>
        /// <returns>Result with success count, failure count, and per-item errors.</returns>
        public static async Task<EnrichmentRunResult> EnrichStructuresAsync(string connectionName, List<string> structureIds, string? model = null)
        {
            var result = new EnrichmentRunResult();
            if (string.IsNullOrEmpty(connectionName) || structureIds == null || structureIds.Count == 0)
            {
                result.ErrorMessage = "ConnectionName and at least one StructureId are required.";
                return result;
            }
            var modelToUse = !string.IsNullOrWhiteSpace(model) ? model : GetDefaultEnrichmentModel();
            if (string.IsNullOrEmpty(modelToUse))
            {
                result.ErrorMessage = "No LLM model configured. Add LLMProviders (e.g. LocalOllama with qwen2.5-coder:7b-instruct or Gemini Vertex AI) in appsettings.";
                return result;
            }

            List<SchemaDetailForEnrichment> details = DbServices.GetSchemaDetailsForStructureIds(connectionName, structureIds);
            if (details.Count == 0)
            {
                result.ErrorMessage = "No schema details found for the given StructureIds and connection.";
                return result;
            }

            var enrichedIds = DbServices.GetEnrichedStructureIds(connectionName).ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var detail in details)
            {
                try
                {
                    string prompt = BuildEnrichmentPrompt(detail);
                    string response = await AiServices.GenerateFromAppSettingsAsync(prompt, modelToUse).ConfigureAwait(false);
                    if (response.StartsWith("Error", StringComparison.OrdinalIgnoreCase) || response.StartsWith("Model '", StringComparison.Ordinal))
                    {
                        result.FailedCount++;
                        result.Errors.Add($"{detail.StructureId} ({detail.ObjectName}): {response}");
                        continue;
                    }
                    var parsed = ParseEnrichmentResponse(response);
                    if (parsed == null)
                    {
                        result.FailedCount++;
                        result.Errors.Add($"{detail.StructureId} ({detail.ObjectName}): Could not parse LLM response as JSON.");
                        continue;
                    }
                    bool exists = enrichedIds.Contains(detail.StructureId);
                    if (exists)
                    {
                        DbServices.UpdateBaseZetadata(
                            detail.StructureId,
                            parsed.HumanTitleEn,
                            parsed.HumanTitleNative,
                            parsed.NoteEn,
                            parsed.NoteNative,
                            parsed.KeywordsEn,
                            parsed.KeywordsNative);
                    }
                    else
                    {
                        DbServices.CreateBaseZetadata(
                            detail.StructureId,
                            detail.ConnectionName,
                            detail.ObjectName,
                            detail.ObjectType,
                            parsed.HumanTitleEn,
                            parsed.HumanTitleNative,
                            parsed.NoteEn,
                            parsed.NoteNative,
                            parsed.KeywordsEn,
                            parsed.KeywordsNative);
                        enrichedIds.Add(detail.StructureId);
                    }
                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    result.FailedCount++;
                    result.Errors.Add($"{detail.StructureId} ({detail.ObjectName}): {ex.Message}");
                }
            }

            return result;
        }
    }

    /// <summary>
    /// Parsed bilingual fields from LLM JSON response.
    /// </summary>
    public class EnrichmentParsedResult
    {
        public string? HumanTitleEn { get; set; }
        public string? HumanTitleNative { get; set; }
        public string? NoteEn { get; set; }
        public string? NoteNative { get; set; }
        public string? KeywordsEn { get; set; }
        public string? KeywordsNative { get; set; }
    }

    /// <summary>
    /// Result of an enrichment run: success/failure counts and error messages.
    /// </summary>
    public class EnrichmentRunResult
    {
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
        public List<string> Errors { get; set; } = [];
        public string? ErrorMessage { get; set; }
    }
}
