using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Nodes;
using AppEndCommon;
using System.Reflection;

namespace AppEndWorkflow
{
    /// <summary>
    /// Manages workflow definitions loaded from workspace/workflows/ directory.
    /// Handles loading, caching, reloading, and in-memory access without restart.
    /// </summary>
    public static class WorkflowDefinitionProvider
    {
        private static readonly ConcurrentDictionary<string, WorkflowDefinition> _cache 
            = new(StringComparer.OrdinalIgnoreCase);

        private const string WorkflowFilePattern = "*.json";

        /// <summary>
        /// Represents a workflow definition loaded from JSON.
        /// </summary>
        public class WorkflowDefinition
        {
            public string Id { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public int Version { get; set; } = 1;
            public bool IsPublished { get; set; } = false;
            public JsonObject RawDefinition { get; set; } = new();
            public DateTime LoadedAt { get; set; }
        }

        /// <summary>
        /// Loads all workflow definitions from workspace/workflows/ directory.
        /// Called once on application startup.
        /// </summary>
        public static void LoadAll()
        {
            _cache.Clear();

            var workflowsDirectory = AppEndSettings.WorkflowsPath;
            if (!Directory.Exists(workflowsDirectory))
            {
                LogMan.LogWarning($"Workflows directory not found: {workflowsDirectory}");
                return;
            }

            var workflowFiles = Directory.GetFiles(workflowsDirectory, WorkflowFilePattern)
                .Where(f => !Path.GetFileName(f).StartsWith("schema", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            LogMan.LogConsole($"Loading {workflowFiles.Length} workflow(s) from {workflowsDirectory}");

            foreach (var filePath in workflowFiles)
            {
                try
                {
                    LoadFromFile(filePath);
                }
                catch (Exception ex)
                {
                    LogMan.LogError($"Failed to load workflow from {filePath}: {ex.Message}");
                }
            }

            LogMan.LogConsole($"Successfully loaded {_cache.Count} workflow(s)");
        }

        /// <summary>
        /// Loads a single workflow definition from file.
        /// Validates against schema and caches in memory.
        /// </summary>
        public static WorkflowDefinition LoadFromFile(string filePath)
        {
            var workflowId = Path.GetFileNameWithoutExtension(filePath);

            // Validate file exists
            if (!File.Exists(filePath))
                throw new AppEndException("WorkflowFileNotFound", MethodBase.GetCurrentMethod())
                    .AddParam("Path", filePath)
                    .GetEx();

            // Read and validate JSON
            var json = File.ReadAllText(filePath);
            WorkflowValidator.ValidateJson(json, workflowId);

            // Parse and cache
            var rawDefinition = JsonNode.Parse(json)?.AsObject()
                ?? throw new AppEndException("FailedToParseWorkflowJson", MethodBase.GetCurrentMethod())
                    .AddParam("WorkflowId", workflowId)
                    .GetEx();

            var definition = new WorkflowDefinition
            {
                Id = rawDefinition["id"]?.GetValue<string>() ?? workflowId,
                Name = rawDefinition["name"]?.GetValue<string>() ?? "Unnamed",
                Description = rawDefinition["description"]?.GetValue<string>() ?? "",
                Version = rawDefinition["version"]?.GetValue<int>() ?? 1,
                IsPublished = rawDefinition["isPublished"]?.GetValue<bool>() ?? false,
                RawDefinition = rawDefinition,
                LoadedAt = DateTime.UtcNow
            };

            _cache[definition.Id] = definition;
            LogMan.LogConsole($"Loaded workflow: {definition.Id} (v{definition.Version})");

            return definition;
        }

        /// <summary>
        /// Gets a workflow definition by ID from cache.
        /// </summary>
        public static WorkflowDefinition? Get(string workflowId)
        {
            return _cache.TryGetValue(workflowId, out var definition) ? definition : null;
        }

        /// <summary>
        /// Gets all loaded workflow definitions.
        /// </summary>
        public static IEnumerable<WorkflowDefinition> GetAll()
        {
            return _cache.Values.OrderBy(w => w.Name);
        }

        /// <summary>
        /// Reloads all workflow definitions from disk.
        /// Zero-downtime: new definitions loaded before cache is cleared.
        /// </summary>
        public static void ReloadAll()
        {
            LogMan.LogConsole("Reloading all workflows from disk...");

            try
            {
                var tempCache = new Dictionary<string, WorkflowDefinition>(StringComparer.OrdinalIgnoreCase);
                var workflowsDirectory = AppEndSettings.WorkflowsPath;

                if (!Directory.Exists(workflowsDirectory))
                {
                    LogMan.LogWarning($"Workflows directory not found: {workflowsDirectory}");
                    return;
                }

                var workflowFiles = Directory.GetFiles(workflowsDirectory, WorkflowFilePattern)
                    .Where(f => !Path.GetFileName(f).StartsWith("schema", StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                foreach (var filePath in workflowFiles)
                {
                    try
                    {
                        var definition = LoadFromFile(filePath);
                        tempCache[definition.Id] = definition;
                    }
                    catch (Exception ex)
                    {
                        LogMan.LogError($"Failed to reload workflow from {filePath}: {ex.Message}");
                    }
                }

                _cache.Clear();
                foreach (var kvp in tempCache)
                {
                    _cache[kvp.Key] = kvp.Value;
                }

                LogMan.LogConsole($"Successfully reloaded {_cache.Count} workflow(s)");
            }
            catch (Exception ex)
            {
                LogMan.LogError($"Workflow reload failed: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Reloads a single workflow definition by ID.
        /// </summary>
        public static WorkflowDefinition Reload(string workflowId)
        {
            var workflowsDirectory = AppEndSettings.WorkflowsPath;
            var filePath = Path.Combine(workflowsDirectory, $"{workflowId}.json");

            LogMan.LogConsole($"Reloading workflow: {workflowId}");
            var definition = LoadFromFile(filePath);

            _cache[workflowId] = definition;
            LogMan.LogConsole($"Successfully reloaded workflow: {workflowId}");

            return definition;
        }

        /// <summary>
        /// Unloads a workflow definition from cache.
        /// </summary>
        public static bool Unload(string workflowId)
        {
            var removed = _cache.TryRemove(workflowId, out _);
            if (removed)
                LogMan.LogConsole($"Unloaded workflow: {workflowId}");
            return removed;
        }

        /// <summary>
        /// Gets total count of loaded workflows.
        /// </summary>
        public static int Count => _cache.Count;

        /// <summary>
        /// Checks if a workflow is loaded.
        /// </summary>
        public static bool Exists(string workflowId)
        {
            return _cache.ContainsKey(workflowId);
        }

        /// <summary>
        /// Clears all cached workflows. (For testing/reset scenarios)
        /// </summary>
        public static void Clear()
        {
            _cache.Clear();
            LogMan.LogConsole("Cleared all cached workflows");
        }
    }
}
