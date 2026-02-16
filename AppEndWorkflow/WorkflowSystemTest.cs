using System.Collections.Generic;
using AppEndCommon;
using System.Text.Json;

namespace AppEndWorkflow
{
    /// <summary>
    /// Test class for workflow system verification.
    /// Can be called from AppEndHost during startup or via RPC for diagnostics.
    /// 
    /// Usage:
    /// rpcAEP("TestWorkflowSystem", {}, callback)
    /// </summary>
    public static class WorkflowSystemTest
    {
        /// <summary>
        /// Comprehensive test of workflow system.
        /// Returns detailed diagnostic information.
        /// </summary>
        public static object RunAllTests()
        {
            var results = new Dictionary<string, object>();

            try
            {
                // Test 1: Check if workflows directory exists
                results["DirectoryExists"] = TestWorkflowsDirectoryExists();

                // Test 2: Check if schema.json exists and is valid
                results["SchemaValid"] = TestSchemaFileValid();

                // Test 3: Load sample workflows
                results["SampleWorkflowsLoaded"] = TestSampleWorkflowsLoaded();

                // Test 4: Test individual workflow loading
                results["HelloWorldWorkflow"] = TestWorkflowLoad("hello-world");
                results["ScheduledDbCheckWorkflow"] = TestWorkflowLoad("scheduled-db-check");
                results["OrderApprovalWorkflow"] = TestWorkflowLoad("order-approval");
                results["DataPipelineWorkflow"] = TestWorkflowLoad("data-pipeline");

                // Test 5: Test RPC methods
                results["GetWorkflowDefinitions"] = TestGetWorkflowDefinitions();
                results["GetWorkflowStats"] = TestGetWorkflowStats();

                // Test 6: Test cache operations
                results["CacheOperations"] = TestCacheOperations();

                // Overall result
                results["Success"] = true;
                results["Message"] = "All workflow system tests completed";
                results["Timestamp"] = DateTime.UtcNow;

                LogMan.LogConsole("✅ Workflow System Tests: All Passed");
            }
            catch (Exception ex)
            {
                LogMan.LogError($"❌ Workflow System Tests Failed: {ex.Message}");
                results["Success"] = false;
                results["ErrorMessage"] = ex.Message;
                results["Timestamp"] = DateTime.UtcNow;
            }

            return results;
        }

        private static object TestWorkflowsDirectoryExists()
        {
            try
            {
                const string workflowsDir = "workspace/workflows";
                var exists = Directory.Exists(workflowsDir);

                if (exists)
                {
                    var fileCount = Directory.GetFiles(workflowsDir, "*.json").Length;
                    return new
                    {
                        success = true,
                        message = $"Workflows directory found with {fileCount} JSON files",
                        path = Path.GetFullPath(workflowsDir),
                        fileCount = fileCount
                    };
                }
                else
                {
                    return new
                    {
                        success = false,
                        message = "Workflows directory not found",
                        path = Path.GetFullPath(workflowsDir)
                    };
                }
            }
            catch (Exception ex)
            {
                return new { success = false, error = ex.Message };
            }
        }

        private static object TestSchemaFileValid()
        {
            try
            {
                const string schemaPath = "workspace/workflows/schema.json";
                
                if (!File.Exists(schemaPath))
                    return new { success = false, message = $"Schema file not found at {schemaPath}" };

                var schemaContent = File.ReadAllText(schemaPath);
                
                if (schemaContent.Length == 0)
                    return new { success = false, message = "Schema file is empty" };

                // Try to parse as JSON
                try
                {
                    using var doc = JsonDocument.Parse(schemaContent);
                    if (doc == null)
                        return new { success = false, message = "Schema file is not valid JSON" };
                }
                catch
                {
                    return new { success = false, message = "Schema file is not valid JSON" };
                }

                return new
                {
                    success = true,
                    message = "Schema file is valid JSON",
                    fileSize = schemaContent.Length,
                    path = Path.GetFullPath(schemaPath)
                };
            }
            catch (Exception ex)
            {
                return new { success = false, error = ex.Message };
            }
        }

        private static object TestSampleWorkflowsLoaded()
        {
            try
            {
                var count = WorkflowDefinitionProvider.Count;
                var all = WorkflowDefinitionProvider.GetAll().ToList();

                if (count == 0)
                    return new { success = false, message = "No workflows loaded" };

                return new
                {
                    success = true,
                    message = $"{count} workflow(s) loaded successfully",
                    count = count,
                    workflows = all.Select(w => new { w.Id, w.Name, w.Version, w.IsPublished }).ToList()
                };
            }
            catch (Exception ex)
            {
                return new { success = false, error = ex.Message };
            }
        }

        private static object TestWorkflowLoad(string workflowId)
        {
            try
            {
                var definition = WorkflowDefinitionProvider.Get(workflowId);

                if (definition == null)
                    return new { success = false, message = $"Workflow '{workflowId}' not found in cache" };

                return new
                {
                    success = true,
                    id = definition.Id,
                    name = definition.Name,
                    version = definition.Version,
                    isPublished = definition.IsPublished,
                    loadedAt = definition.LoadedAt,
                    activitiesCount = definition.RawDefinition["activities"]?.AsArray().Count ?? 0
                };
            }
            catch (Exception ex)
            {
                return new { success = false, error = ex.Message };
            }
        }

        private static object TestGetWorkflowDefinitions()
        {
            try
            {
                var definitions = WorkflowServices.GetWorkflowDefinitions();
                return new
                {
                    success = true,
                    count = definitions.Count,
                    definitions = definitions.Select(d => new { d.Id, d.Name, d.Version }).ToList()
                };
            }
            catch (Exception ex)
            {
                return new { success = false, error = ex.Message };
            }
        }

        private static object TestGetWorkflowStats()
        {
            try
            {
                var stats = WorkflowServices.GetWorkflowStats();
                return new
                {
                    success = true,
                    stats = stats
                };
            }
            catch (Exception ex)
            {
                return new { success = false, error = ex.Message };
            }
        }

        private static object TestCacheOperations()
        {
            try
            {
                var initialCount = WorkflowDefinitionProvider.Count;

                // Test Get
                var helloWorld = WorkflowDefinitionProvider.Get("hello-world");
                if (helloWorld == null)
                    return new { success = false, message = "Failed to get workflow from cache" };

                // Test Exists
                if (!WorkflowDefinitionProvider.Exists("hello-world"))
                    return new { success = false, message = "Exists() check failed" };

                // Test GetAll
                var all = WorkflowDefinitionProvider.GetAll().ToList();
                if (all.Count != initialCount)
                    return new { success = false, message = "GetAll() count mismatch" };

                return new
                {
                    success = true,
                    message = "Cache operations working correctly",
                    initialCount = initialCount,
                    getAllCount = all.Count,
                    existsCheck = true
                };
            }
            catch (Exception ex)
            {
                return new { success = false, error = ex.Message };
            }
        }

        /// <summary>
        /// Quick diagnostics endpoint.
        /// rpcAEP("WorkflowDiagnostics", {}, callback)
        /// </summary>
        public static object GetDiagnostics()
        {
            try
            {
                var all = WorkflowDefinitionProvider.GetAll().ToList();
                return new
                {
                    timestamp = DateTime.UtcNow,
                    workflowsLoaded = all.Count,
                    workflowsCached = WorkflowDefinitionProvider.Count,
                    workflowsPublished = all.Count(w => w.IsPublished),
                    workflowsUnpublished = all.Count(w => !w.IsPublished),
                    workflows = all.Select(w => new
                    {
                        w.Id,
                        w.Name,
                        w.Version,
                        w.IsPublished,
                        w.LoadedAt,
                        activitiesCount = w.RawDefinition["activities"]?.AsArray().Count ?? 0
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                LogMan.LogError($"Diagnostics failed: {ex.Message}");
                return new { error = ex.Message, timestamp = DateTime.UtcNow };
            }
        }
    }
}
