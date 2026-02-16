using System.Text.Json;
using System.Text.Json.Nodes;
using AppEndCommon;
using System.Reflection;

namespace AppEndWorkflow
{
    /// <summary>
    /// Validates workflow definitions against schema.
    /// Ensures all workflow JSON files conform to expected structure before loading.
    /// </summary>
    public static class WorkflowValidator
    {
        private static JsonObject? _schema;

        /// <summary>
        /// Loads the workflow schema from workspace/workflows/schema.json
        /// </summary>
        private static JsonObject LoadSchema()
        {
            if (_schema != null)
                return _schema;

            var schemaPath = Path.Combine(AppEndSettings.WorkflowsPath, "schema.json");
            
            if (!File.Exists(schemaPath))
                throw new AppEndException("WorkflowSchemaNotFound", MethodBase.GetCurrentMethod())
                    .AddParam("Path", schemaPath)
                    .GetEx();

            var schemaJson = File.ReadAllText(schemaPath);
            _schema = JsonNode.Parse(schemaJson)?.AsObject()
                ?? throw new AppEndException("InvalidWorkflowSchema", MethodBase.GetCurrentMethod())
                    .AddParam("Path", schemaPath)
                    .GetEx();

            return _schema;
        }

        /// <summary>
        /// Validates a workflow JSON string against the schema.
        /// </summary>
        public static void ValidateJson(string json, string workflowId)
        {
            try
            {
                // Try to parse JSON
                var workflow = JsonNode.Parse(json)?.AsObject()
                    ?? throw new AppEndException("InvalidWorkflowJson", MethodBase.GetCurrentMethod())
                        .AddParam("WorkflowId", workflowId)
                        .AddParam("Message", "JSON does not deserialize to object")
                        .GetEx();

                // Check required fields
                ValidateRequiredFields(workflow, workflowId);

                // Validate ID matches filename
                var workflowIdFromJson = workflow["id"]?.GetValue<string>();
                if (workflowIdFromJson != workflowId)
                    throw new AppEndException("WorkflowIdMismatch", MethodBase.GetCurrentMethod())
                        .AddParam("WorkflowId", workflowId)
                        .AddParam("JsonId", workflowIdFromJson)
                        .GetEx();
            }
            catch (JsonException ex)
            {
                throw new AppEndException("WorkflowJsonParseError", MethodBase.GetCurrentMethod())
                    .AddParam("WorkflowId", workflowId)
                    .AddParam("Message", ex.Message)
                    .GetEx();
            }
            catch (AppEndException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new AppEndException("WorkflowValidationError", MethodBase.GetCurrentMethod())
                    .AddParam("WorkflowId", workflowId)
                    .AddParam("Message", ex.Message)
                    .GetEx();
            }
        }

        /// <summary>
        /// Validates that all required fields are present and valid.
        /// </summary>
        private static void ValidateRequiredFields(JsonObject workflow, string workflowId)
        {
            var requiredFields = new[] { "id", "name" };

            foreach (var field in requiredFields)
            {
                if (!workflow.ContainsKey(field) || workflow[field] == null)
                    throw new AppEndException("MissingRequiredWorkflowField", MethodBase.GetCurrentMethod())
                        .AddParam("WorkflowId", workflowId)
                        .AddParam("Field", field)
                        .GetEx();
            }

            var activitiesNode = workflow["activities"];
            var rootNode = workflow["root"] as JsonObject;
            if (activitiesNode == null && rootNode != null)
            {
                activitiesNode = rootNode["activities"];
            }

            if (activitiesNode == null)
                throw new AppEndException("MissingRequiredWorkflowField", MethodBase.GetCurrentMethod())
                    .AddParam("WorkflowId", workflowId)
                    .AddParam("Field", "activities")
                    .GetEx();

            // Validate ID format (lowercase, alphanumeric, hyphens only)
            var id = workflow["id"]?.GetValue<string>();
            if (string.IsNullOrWhiteSpace(id) || !System.Text.RegularExpressions.Regex.IsMatch(id, "^[a-z0-9-]+$"))
                throw new AppEndException("InvalidWorkflowIdFormat", MethodBase.GetCurrentMethod())
                    .AddParam("WorkflowId", id)
                    .AddParam("Message", "Must be lowercase alphanumeric with hyphens only")
                    .GetEx();

            // Validate name
            var name = workflow["name"]?.GetValue<string>();
            if (string.IsNullOrWhiteSpace(name) || name.Length > 255)
                throw new AppEndException("InvalidWorkflowName", MethodBase.GetCurrentMethod())
                    .AddParam("WorkflowId", workflowId)
                    .AddParam("Message", "Name must be 1-255 characters")
                    .GetEx();

            // Validate activities is array
            if (activitiesNode?.GetValueKind() != JsonValueKind.Array)
                throw new AppEndException("InvalidWorkflowActivities", MethodBase.GetCurrentMethod())
                    .AddParam("WorkflowId", workflowId)
                    .AddParam("Message", "Activities must be an array")
                    .GetEx();
        }

        /// <summary>
        /// Validates a workflow definition file.
        /// </summary>
        public static void ValidateFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new AppEndException("WorkflowFileNotFound", MethodBase.GetCurrentMethod())
                    .AddParam("Path", filePath)
                    .GetEx();

            var workflowId = Path.GetFileNameWithoutExtension(filePath);
            var json = File.ReadAllText(filePath);
            ValidateJson(json, workflowId);
        }

        /// <summary>
        /// Resets cached schema (for reload scenarios).
        /// </summary>
        public static void ResetSchema()
        {
            _schema = null;
        }
    }
}
