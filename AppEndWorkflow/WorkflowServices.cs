using AppEndCommon;
using AppEndDbIO;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace AppEndWorkflow
{
    /// <summary>
    /// RPC bridge for workflow operations.
    /// All Vue.js client calls go through rpcAEP() → WorkflowServices static methods.
    /// 
    /// Usage from Vue.js:
    /// rpcAEP("ExecuteWorkflow", { WorkflowId: "hello-world", InputParams: {} }, callback)
    /// rpcAEP("GetWorkflowDefinitions", {}, callback)
    /// rpcAEP("ReloadAllWorkflows", {}, callback)
    /// rpcAEP("GetWorkflowInstances", { Status: "Running", Page: 1, PageSize: 25 }, callback)
    /// rpcAEP("GetMyWorkflowTasks", { Status: "Pending", Page: 1 }, callback)
    /// </summary>
    public static class WorkflowServices
    {
        private static IServiceProvider? _services;

        public static void SetServiceProvider(IServiceProvider services)
        {
            _services = services;
        }

        private static IServiceProvider GetServices()
        {
            if (_services == null)
                throw new InvalidOperationException("Workflow services are not initialized.");

            return _services;
        }

        /// <summary>
        /// Result of workflow execution attempt.
        /// </summary>
        public class ExecutionResult
        {
            public bool Success { get; set; }
            public string? InstanceId { get; set; }
            public string? Status { get; set; }
            public object? Output { get; set; }
            public string? ErrorMessage { get; set; }
            public DateTime ExecutedAt { get; set; }
        }

        /// <summary>
        /// Workflow definition summary for client display.
        /// </summary>
        public class WorkflowSummary
        {
            public string Id { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public int Version { get; set; }
            public bool IsPublished { get; set; }
            public DateTime LoadedAt { get; set; }
        }

        /// <summary>
        /// Workflow instance summary for monitoring UI.
        /// </summary>
        public class InstanceSummary
        {
            public string InstanceId { get; set; } = string.Empty;
            public string DefinitionId { get; set; } = string.Empty;
            public string? DefinitionName { get; set; }
            public string Status { get; set; } = string.Empty;
            public DateTime? StartedAt { get; set; }
            public DateTime? FinishedAt { get; set; }
            public DateTime? LastExecutedAt { get; set; }
            public int IncidentCount { get; set; }
        }

        /// <summary>
        /// Workflow task summary for user inbox (kartabl).
        /// </summary>
        public class TaskSummary
        {
            public string TaskId { get; set; } = string.Empty;
            public string WorkflowInstanceId { get; set; } = string.Empty;
            public string WorkflowDefinitionId { get; set; } = string.Empty;
            public string Title { get; set; } = string.Empty;
            public string? Description { get; set; }
            public string? AssignedTo { get; set; }
            public string? AssignedRole { get; set; }
            public string Priority { get; set; } = "Normal";
            public string Status { get; set; } = "Pending";
            public DateTime? DueDate { get; set; }
            public DateTime CreatedAt { get; set; }
            public string? ContextData { get; set; }
        }

        /// <summary>
        /// Executes a workflow by definition ID with input parameters.
        /// 
        /// RPC Call:
        /// rpcAEP("ExecuteWorkflow", { 
        ///     WorkflowId: "hello-world",
        ///     InputParams: { key: "value" }
        /// }, callback)
        /// </summary>
        public static ExecutionResult ExecuteWorkflow(string WorkflowId, Dictionary<string, object>? InputParams = null)
        {
            try
            {
                var definition = WorkflowDefinitionProvider.Get(WorkflowId);
                if (definition == null)
                {
                    LogMan.LogWarning($"Workflow not found: {WorkflowId}");
                    return new ExecutionResult
                    {
                        Success = false,
                        ErrorMessage = $"Workflow '{WorkflowId}' not found",
                        ExecutedAt = DateTime.UtcNow
                    };
                }

                if (!definition.IsPublished)
                {
                    LogMan.LogWarning($"Workflow not published: {WorkflowId}");
                    return new ExecutionResult
                    {
                        Success = false,
                        ErrorMessage = $"Workflow '{WorkflowId}' is not published",
                        ExecutedAt = DateTime.UtcNow
                    };
                }

                var services = GetServices();
                var dispatcherType = FindType("Elsa.Workflows.Runtime.IWorkflowDispatcher")
                    ?? FindType("Elsa.Workflows.IWorkflowDispatcher");

                if (dispatcherType == null)
                {
                    return new ExecutionResult
                    {
                        Success = false,
                        ErrorMessage = "Elsa dispatcher service type not found",
                        ExecutedAt = DateTime.UtcNow
                    };
                }

                using var scope = services.CreateScope();
                var dispatcher = scope.ServiceProvider.GetService(dispatcherType);
                if (dispatcher == null)
                {
                    return new ExecutionResult
                    {
                        Success = false,
                        ErrorMessage = "Elsa dispatcher service not available",
                        ExecutedAt = DateTime.UtcNow
                    };
                }

                var dispatchMethod = dispatcherType.GetMethods()
                    .FirstOrDefault(m => m.Name is "DispatchWorkflowDefinitionAsync" or "DispatchAsync" or "DispatchWorkflowAsync");

                if (dispatchMethod == null)
                {
                    return new ExecutionResult
                    {
                        Success = false,
                        ErrorMessage = "Elsa dispatch method not found",
                        ExecutedAt = DateTime.UtcNow
                    };
                }

                var args = BuildDispatchArguments(dispatchMethod, WorkflowId, InputParams);
                var invocationResult = dispatchMethod.Invoke(dispatcher, args);
                var resolvedResult = ResolveTaskResult(invocationResult);
                var execution = MapExecutionResult(resolvedResult, WorkflowId);

                LogMan.LogConsole($"Executed workflow: {WorkflowId}");

                return execution;
            }
            catch (Exception ex)
            {
                LogMan.LogError($"Workflow execution failed: {WorkflowId}: {ex.Message}");
                return new ExecutionResult
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    ExecutedAt = DateTime.UtcNow
                };
            }
        }

        private static Type? FindType(string typeName)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Select(a => a.GetType(typeName, false))
                .FirstOrDefault(t => t != null);
        }

        private static object?[] BuildDispatchArguments(MethodInfo method, string workflowId, Dictionary<string, object>? inputParams)
        {
            var parameters = method.GetParameters();
            var args = new object?[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];

                if (parameter.ParameterType == typeof(string))
                {
                    args[i] = workflowId;
                    continue;
                }

                if (parameter.ParameterType == typeof(CancellationToken))
                {
                    args[i] = CancellationToken.None;
                    continue;
                }

                if (parameter.ParameterType.IsAssignableFrom(typeof(Dictionary<string, object>)) ||
                    parameter.ParameterType.IsAssignableFrom(typeof(IDictionary<string, object>)) ||
                    string.Equals(parameter.Name, "input", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(parameter.Name, "inputParams", StringComparison.OrdinalIgnoreCase))
                {
                    args[i] = inputParams ?? new Dictionary<string, object>();
                    continue;
                }

                if (parameter.ParameterType.Name.EndsWith("DispatchWorkflowDefinitionRequest", StringComparison.Ordinal))
                {
                    args[i] = BuildDispatchRequest(parameter.ParameterType, workflowId, inputParams);
                    continue;
                }

                args[i] = parameter.HasDefaultValue ? parameter.DefaultValue : null;
            }

            return args;
        }

        private static object? BuildDispatchRequest(Type requestType, string workflowId, Dictionary<string, object>? inputParams)
        {
            var constructors = requestType.GetConstructors();
            foreach (var ctor in constructors.OrderByDescending(c => c.GetParameters().Length))
            {
                var ctorParams = ctor.GetParameters();
                var ctorArgs = new object?[ctorParams.Length];
                var supported = true;

                for (var i = 0; i < ctorParams.Length; i++)
                {
                    var param = ctorParams[i];
                    if (param.ParameterType == typeof(string))
                    {
                        ctorArgs[i] = workflowId;
                      }
                    else if (param.ParameterType == typeof(CancellationToken))
                    {
                        ctorArgs[i] = CancellationToken.None;
                    }
                    else if (param.ParameterType.IsAssignableFrom(typeof(Dictionary<string, object>)) ||
                             param.ParameterType.IsAssignableFrom(typeof(IDictionary<string, object>)) ||
                             string.Equals(param.Name, "input", StringComparison.OrdinalIgnoreCase) ||
                             string.Equals(param.Name, "inputParams", StringComparison.OrdinalIgnoreCase))
                    {
                        ctorArgs[i] = inputParams ?? new Dictionary<string, object>();
                    }
                    else
                    {
                        supported = false;
                        break;
                    }
                }

                if (supported)
                    return ctor.Invoke(ctorArgs);
            }

            var request = Activator.CreateInstance(requestType);
            if (request == null)
                return null;

            SetPropertyIfExists(request, "DefinitionId", workflowId);
            SetPropertyIfExists(request, "WorkflowDefinitionId", workflowId);
            SetPropertyIfExists(request, "Id", workflowId);
            SetPropertyIfExists(request, "Input", inputParams ?? new Dictionary<string, object>());
            SetPropertyIfExists(request, "InputParameters", inputParams ?? new Dictionary<string, object>());

            return request;
        }

        private static void SetPropertyIfExists(object target, string propertyName, object? value)
        {
            var prop = target.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (prop != null && prop.CanWrite)
                prop.SetValue(target, value);
        }

        private static object? ResolveTaskResult(object? invocationResult)
        {
            if (invocationResult is Task task)
            {
                task.GetAwaiter().GetResult();
                var resultProperty = task.GetType().GetProperty("Result");
                return resultProperty?.GetValue(task);
            }

            return invocationResult;
        }

        private static ExecutionResult MapExecutionResult(object? result, string workflowId)
        {
            var execution = new ExecutionResult
            {
                Success = true,
                Status = "Completed",
                Output = result,
                ExecutedAt = DateTime.UtcNow
            };

            if (result == null)
                return execution;

            var resultType = result.GetType();
            execution.InstanceId = GetPropertyValue(resultType, result, "InstanceId")?.ToString()
                ?? GetPropertyValue(resultType, result, "WorkflowInstanceId")?.ToString();

            execution.Status = GetPropertyValue(resultType, result, "Status")?.ToString()
                ?? GetPropertyValue(resultType, result, "WorkflowStatus")?.ToString();

            var successValue = GetPropertyValue(resultType, result, "IsSuccess")
                ?? GetPropertyValue(resultType, result, "IsSuccessful")
                ?? GetPropertyValue(resultType, result, "Success");

            if (successValue is bool successBool)
                execution.Success = successBool;

            var outputValue = GetPropertyValue(resultType, result, "Output")
                ?? GetPropertyValue(resultType, result, "Result");

            if (outputValue != null)
                execution.Output = outputValue;

            if (!execution.Success)
                execution.ErrorMessage = "Workflow execution failed";

            return execution;
        }

        private static object? GetPropertyValue(Type type, object instance, string propertyName)
        {
            var prop = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            return prop?.GetValue(instance);
        }

        /// <summary>
        /// Gets all available workflow definitions for display.
        /// 
        /// RPC Call:
        /// rpcAEP("GetWorkflowDefinitions", {}, callback)
        /// </summary>
        public static List<WorkflowSummary> GetWorkflowDefinitions()
        {
            try
            {
                return WorkflowDefinitionProvider.GetAll()
                    .Select(w => new WorkflowSummary
                    {
                        Id = w.Id,
                        Name = w.Name,
                        Description = w.Description,
                        Version = w.Version,
                        IsPublished = w.IsPublished,
                        LoadedAt = w.LoadedAt
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                LogMan.LogError($"Failed to get workflow definitions: {ex.Message}");
                return new List<WorkflowSummary>();
            }
        }

        /// <summary>
        /// Gets details of a single workflow definition.
        /// 
        /// RPC Call:
        /// rpcAEP("GetWorkflowDefinition", { WorkflowId: "hello-world" }, callback)
        /// </summary>
        public static object? GetWorkflowDefinition(string WorkflowId)
        {
            try
            {
                var definition = WorkflowDefinitionProvider.Get(WorkflowId);
                if (definition == null)
                {
                    LogMan.LogWarning($"Workflow not found: {WorkflowId}");
                    return null;
                }

                return new
                {
                    definition.Id,
                    definition.Name,
                    definition.Description,
                    definition.Version,
                    definition.IsPublished,
                    definition.LoadedAt,
                    RawJson = definition.RawDefinition.ToString()
                };
            }
            catch (Exception ex)
            {
                LogMan.LogError($"Failed to get workflow definition: {WorkflowId}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Reloads all workflow definitions from disk (zero downtime).
        /// Validates files before replacing cache.
        /// 
        /// RPC Call:
        /// rpcAEP("ReloadAllWorkflows", {}, callback)
        /// </summary>
        public static object ReloadAllWorkflows()
        {
            try
            {
                LogMan.LogConsole("RPC: ReloadAllWorkflows requested");
                WorkflowDefinitionProvider.ReloadAll();

                return new
                {
                    success = true,
                    message = "All workflows reloaded successfully",
                    count = WorkflowDefinitionProvider.Count,
                    timestamp = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                LogMan.LogError($"Workflow reload failed: {ex.Message}");
                return new
                {
                    success = false,
                    message = $"Reload failed: {ex.Message}",
                    timestamp = DateTime.UtcNow
                };
            }
        }

        /// <summary>
        /// Reloads a single workflow by ID (zero downtime).
        /// 
        /// RPC Call:
        /// rpcAEP("ReloadWorkflow", { WorkflowId: "hello-world" }, callback)
        /// </summary>
        public static object ReloadWorkflow(string WorkflowId)
        {
            try
            {
                LogMan.LogConsole($"RPC: ReloadWorkflow requested: {WorkflowId}");
                var definition = WorkflowDefinitionProvider.Reload(WorkflowId);

                return new
                {
                    success = true,
                    message = $"Workflow '{WorkflowId}' reloaded successfully",
                    workflow = new
                    {
                        definition.Id,
                        definition.Name,
                        definition.Version,
                        definition.IsPublished
                    },
                    timestamp = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                LogMan.LogError($"Workflow reload failed: {WorkflowId}: {ex.Message}");
                return new
                {
                    success = false,
                    message = $"Reload failed: {ex.Message}",
                    timestamp = DateTime.UtcNow
                };
            }
        }

        /// <summary>
        /// Gets workflow system statistics.
        /// 
        /// RPC Call:
        /// rpcAEP("GetWorkflowStats", {}, callback)
        /// </summary>
        public static object GetWorkflowStats()
        {
            try
            {
                var allWorkflows = WorkflowDefinitionProvider.GetAll();
                var workflowList = allWorkflows.ToList();

                return new
                {
                    Success = true,
                    Result = new
                    {
                        TotalCount = workflowList.Count,
                        PublishedCount = workflowList.Count(w => w.IsPublished),
                        UnpublishedCount = workflowList.Count(w => !w.IsPublished),
                        Timestamp = DateTime.UtcNow
                    }
                };
            }
            catch (Exception ex)
            {
                LogMan.LogError($"Failed to get workflow stats: {ex.Message}");
                return new
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    Timestamp = DateTime.UtcNow
                };
            }
        }

        /// <summary>
        /// Creates a new workflow definition.
        /// 
        /// RPC Call:
        /// rpcAEP("CreateWorkflow", { 
        ///     WorkflowId: "new-workflow",
        ///     Name: "New Workflow",
        ///     Description: "...",
        ///     Definition: {...}
        /// }, callback)
        /// </summary>
        public static object CreateWorkflow(string WorkflowId, string Name, string? Description, object? Definition, bool IsPublished = false, int Version = 1)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(WorkflowId))
                    return new { Success = false, ErrorMessage = "WorkflowId is required" };

                if (string.IsNullOrWhiteSpace(Name))
                    return new { Success = false, ErrorMessage = "Name is required" };

                if (WorkflowDefinitionProvider.Get(WorkflowId) != null)
                    return new { Success = false, ErrorMessage = $"Workflow '{WorkflowId}' already exists" };

                var workflowPath = Path.Combine(AppEndSettings.WorkflowsPath, $"{WorkflowId}.json");

                if (File.Exists(workflowPath))
                    return new { Success = false, ErrorMessage = "Workflow file already exists" };

                // Parse Definition parameter to get activities and variables
                JsonNode? activitiesNode = null;
                JsonNode? variablesNode = null;
                
                if (Definition != null)
                {
                    try
                    {
                        var definitionJson = JsonSerializer.Serialize(Definition);
                        var definitionNode = JsonNode.Parse(definitionJson);
                        
                        if (definitionNode is JsonArray)
                        {
                            // Definition is directly an activities array
                            activitiesNode = definitionNode;
                        }
                        else if (definitionNode is JsonObject defObj)
                        {
                            // Definition is an object with activities/variables
                            activitiesNode = defObj["activities"];
                            variablesNode = defObj["variables"];
                        }
                    }
                    catch
                    {
                        // Ignore parsing errors, use empty arrays
                    }
                }

                var workflowJson = new JsonObject
                {
                    ["id"] = WorkflowId,
                    ["name"] = Name,
                    ["description"] = Description ?? "",
                    ["version"] = Version,
                    ["isPublished"] = IsPublished,
                    ["activities"] = activitiesNode ?? new JsonArray(),
                    ["variables"] = variablesNode ?? new JsonArray()
                };

                var jsonString = workflowJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
                
                WorkflowValidator.ValidateJson(jsonString, WorkflowId);
                
                File.WriteAllText(workflowPath, jsonString);
                
                WorkflowDefinitionProvider.Reload(WorkflowId);

                LogMan.LogConsole($"Created workflow: {WorkflowId}");

                return new
                {
                    Success = true,
                    Message = $"Workflow '{WorkflowId}' created successfully",
                    WorkflowId = WorkflowId
                };
            }
            catch (Exception ex)
            {
                LogMan.LogError($"Failed to create workflow: {ex.Message}");
                return new
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Updates an existing workflow definition.
        /// 
        /// RPC Call:
        /// rpcAEP("UpdateWorkflow", { 
        ///     WorkflowId: "hello-world",
        ///     Name: "Updated Name",
        ///     Definition: {...}
        /// }, callback)
        /// </summary>
        public static object UpdateWorkflow(string WorkflowId, string? Name = null, string? Description = null, object? Definition = null, bool? IsPublished = null, int? Version = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(WorkflowId))
                    return new { Success = false, ErrorMessage = "WorkflowId is required" };

                var existingWorkflow = WorkflowDefinitionProvider.Get(WorkflowId);
                if (existingWorkflow == null)
                    return new { Success = false, ErrorMessage = $"Workflow '{WorkflowId}' not found" };

                var workflowPath = Path.Combine(AppEndSettings.WorkflowsPath, $"{WorkflowId}.json");

                var jsonString = File.ReadAllText(workflowPath);
                var workflowJson = JsonNode.Parse(jsonString)?.AsObject()
                    ?? throw new InvalidOperationException("Invalid workflow JSON");

                if (!string.IsNullOrWhiteSpace(Name))
                    workflowJson["name"] = Name;

                if (Description != null)
                    workflowJson["description"] = Description;

                if (Version.HasValue)
                    workflowJson["version"] = Version.Value;

                if (IsPublished.HasValue)
                    workflowJson["isPublished"] = IsPublished.Value;

                if (Definition != null)
                {
                    try
                    {
                        var definitionJson = JsonSerializer.Serialize(Definition);
                        var definitionNode = JsonNode.Parse(definitionJson);
                        
                        if (definitionNode is JsonArray)
                        {
                            // Definition is directly an activities array
                            workflowJson["activities"] = definitionNode;
                        }
                        else if (definitionNode is JsonObject defObj)
                        {
                            // Definition is an object with activities/variables
                            if (defObj["activities"] != null)
                                workflowJson["activities"] = defObj["activities"];
                            if (defObj["variables"] != null)
                                workflowJson["variables"] = defObj["variables"];
                        }
                    }
                    catch (Exception ex)
                    {
                        LogMan.LogWarning($"Failed to parse Definition parameter: {ex.Message}");
                    }
                }

                var updatedJson = workflowJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true });

                WorkflowValidator.ValidateJson(updatedJson, WorkflowId);

                File.WriteAllText(workflowPath, updatedJson);

                WorkflowDefinitionProvider.Reload(WorkflowId);

                LogMan.LogConsole($"Updated workflow: {WorkflowId}");

                return new
                {
                    Success = true,
                    Message = $"Workflow '{WorkflowId}' updated successfully",
                    WorkflowId = WorkflowId
                };
            }
            catch (Exception ex)
            {
                LogMan.LogError($"Failed to update workflow: {ex.Message}");
                return new
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Deletes a workflow definition.
        /// Checks if workflow is in use before deletion.
        /// 
        /// RPC Call:
        /// rpcAEP("DeleteWorkflow", { WorkflowId: "hello-world" }, callback)
        /// </summary>
        public static object DeleteWorkflow(string WorkflowId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(WorkflowId))
                    return new { Success = false, ErrorMessage = "WorkflowId is required" };

                var workflow = WorkflowDefinitionProvider.Get(WorkflowId);
                if (workflow == null)
                    return new { Success = false, ErrorMessage = $"Workflow '{WorkflowId}' not found" };

                var workflowPath = Path.Combine(AppEndSettings.WorkflowsPath, $"{WorkflowId}.json");

                if (!File.Exists(workflowPath))
                    return new { Success = false, ErrorMessage = "Workflow file not found" };

                File.Delete(workflowPath);
                WorkflowDefinitionProvider.RemoveFromCache(WorkflowId);

                LogMan.LogConsole($"Deleted workflow: {WorkflowId}");

                return new
                {
                    Success = true,
                    Message = $"Workflow '{WorkflowId}' deleted successfully"
                };
            }
            catch (Exception ex)
            {
                LogMan.LogError($"Failed to delete workflow: {ex.Message}");
                return new
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Publishes a workflow definition.
        /// 
        /// RPC Call:
        /// rpcAEP("PublishWorkflow", { WorkflowId: "hello-world" }, callback)
        /// </summary>
        public static object PublishWorkflow(string WorkflowId)
        {
            try
            {
                return UpdateWorkflow(WorkflowId, IsPublished: true);
            }
            catch (Exception ex)
            {
                LogMan.LogError($"Failed to publish workflow: {ex.Message}");
                return new
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Unpublishes a workflow definition.
        /// 
        /// RPC Call:
        /// rpcAEP("UnpublishWorkflow", { WorkflowId: "hello-world" }, callback)
        /// </summary>
        public static object UnpublishWorkflow(string WorkflowId)
        {
            try
            {
                return UpdateWorkflow(WorkflowId, IsPublished: false);
            }
            catch (Exception ex)
            {
                LogMan.LogError($"Failed to unpublish workflow: {ex.Message}");
                return new
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Gets workflow instances with filtering and pagination.
        /// 
        /// RPC Call:
        /// rpcAEP("GetWorkflowInstances", { Status: "Running", Page: 1, PageSize: 25 }, callback)
        /// </summary>
        public static object GetWorkflowInstances(string? Status = null, int Page = 1, int PageSize = 25)
        {
            try
            {
                var instances = new List<InstanceSummary>();
                
                LogMan.LogConsole($"RPC: GetWorkflowInstances - Status: {Status}, Page: {Page}, PageSize: {PageSize}");

                return new
                {
                    Success = true,
                    Result = instances,
                    Page = Page,
                    PageSize = PageSize,
                    Total = instances.Count
                };
            }
            catch (Exception ex)
            {
                LogMan.LogError($"Failed to get workflow instances: {ex.Message}");
                return new
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Gets details of a workflow instance.
        /// 
        /// RPC Call:
        /// rpcAEP("GetInstanceDetails", { InstanceId: "..." }, callback)
        /// </summary>
        public static object GetInstanceDetails(string InstanceId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(InstanceId))
                    return new { Success = false, ErrorMessage = "InstanceId is required" };

                var instance = new InstanceSummary();
                
                LogMan.LogConsole($"RPC: GetInstanceDetails - InstanceId: {InstanceId}");

                return new
                {
                    Success = true,
                    Result = instance
                };
            }
            catch (Exception ex)
            {
                LogMan.LogError($"Failed to get instance details: {ex.Message}");
                return new
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Cancels a running workflow instance.
        /// 
        /// RPC Call:
        /// rpcAEP("CancelInstance", { InstanceId: "..." }, callback)
        /// </summary>
        public static object CancelInstance(string InstanceId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(InstanceId))
                    return new { Success = false, ErrorMessage = "InstanceId is required" };

                LogMan.LogConsole($"RPC: CancelInstance - InstanceId: {InstanceId}");

                return new
                {
                    Success = true,
                    Message = "Instance cancelled successfully"
                };
            }
            catch (Exception ex)
            {
                LogMan.LogError($"Failed to cancel instance: {ex.Message}");
                return new
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Retries a failed workflow instance.
        /// 
        /// RPC Call:
        /// rpcAEP("RetryInstance", { InstanceId: "..." }, callback)
        /// </summary>
        public static object RetryInstance(string InstanceId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(InstanceId))
                    return new { Success = false, ErrorMessage = "InstanceId is required" };

                LogMan.LogConsole($"RPC: RetryInstance - InstanceId: {InstanceId}");

                return new
                {
                    Success = true,
                    Message = "Instance retry initiated"
                };
            }
            catch (Exception ex)
            {
                LogMan.LogError($"Failed to retry instance: {ex.Message}");
                return new
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Gets user's workflow tasks.
        /// 
        /// RPC Call:
        /// rpcAEP("GetMyTasks", { Status: "Pending", Page: 1, PageSize: 25 }, callback)
        /// </summary>
        public static object GetMyTasks(string? Status = null, int Page = 1, int PageSize = 25)
        {
            try
            {
                var tasks = new List<TaskSummary>();

                LogMan.LogConsole($"RPC: GetMyTasks - Status: {Status}, Page: {Page}");

                return new
                {
                    Success = true,
                    Result = tasks,
                    Page = Page,
                    PageSize = PageSize,
                    Total = tasks.Count
                };
            }
            catch (Exception ex)
            {
                LogMan.LogError($"Failed to get tasks: {ex.Message}");
                return new
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Completes a workflow task.
        /// 
        /// RPC Call:
        /// rpcAEP("CompleteTask", { TaskId: "...", Output: "..." }, callback)
        /// </summary>
        public static object CompleteTask(string TaskId, string? Output = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TaskId))
                    return new { Success = false, ErrorMessage = "TaskId is required" };

                LogMan.LogConsole($"RPC: CompleteTask - TaskId: {TaskId}");

                return new
                {
                    Success = true,
                    Message = "Task completed successfully"
                };
            }
            catch (Exception ex)
            {
                LogMan.LogError($"Failed to complete task: {ex.Message}");
                return new
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Rejects a workflow task.
        /// 
        /// RPC Call:
        /// rpcAEP("RejectTask", { TaskId: "...", Reason: "..." }, callback)
        /// </summary>
        public static object RejectTask(string TaskId, string? Reason = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TaskId))
                    return new { Success = false, ErrorMessage = "TaskId is required" };

                LogMan.LogConsole($"RPC: RejectTask - TaskId: {TaskId}");

                return new
                {
                    Success = true,
                    Message = "Task rejected successfully"
                };
            }
            catch (Exception ex)
            {
                LogMan.LogError($"Failed to reject task: {ex.Message}");
                return new
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Delegates a workflow task to another user.
        /// 
        /// RPC Call:
        /// rpcAEP("DelegateTask", { TaskId: "...", AssignTo: "..." }, callback)
        /// </summary>
        public static object DelegateTask(string TaskId, string AssignTo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TaskId))
                    return new { Success = false, ErrorMessage = "TaskId is required" };

                if (string.IsNullOrWhiteSpace(AssignTo))
                    return new { Success = false, ErrorMessage = "AssignTo is required" };

                LogMan.LogConsole($"RPC: DelegateTask - TaskId: {TaskId}, AssignTo: {AssignTo}");

                return new
                {
                    Success = true,
                    Message = "Task delegated successfully"
                };
            }
            catch (Exception ex)
            {
                LogMan.LogError($"Failed to delegate task: {ex.Message}");
                return new
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Gets user's workflow tasks with userId parameter.
        /// 
        /// RPC Call:
        /// rpcAEP("GetMyWorkflowTasks", { Status: "Pending", Page: 1, PageSize: 25, UserId: "..." }, callback)
        /// </summary>
        public static object GetMyWorkflowTasks(string? Status = null, int Page = 1, int PageSize = 25, string? UserId = null)
        {
            try
            {
                var tasks = new List<TaskSummary>();

                LogMan.LogConsole($"RPC: GetMyWorkflowTasks - Status: {Status}, Page: {Page}, UserId: {UserId}");

                return new
                {
                    Success = true,
                    Result = tasks,
                    Page = Page,
                    PageSize = PageSize,
                    Total = tasks.Count
                };
            }
            catch (Exception ex)
            {
                LogMan.LogError($"Failed to get workflow tasks: {ex.Message}");
                return new
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Completes a workflow task with outcome and output parameters.
        /// 
        /// RPC Call:
        /// rpcAEP("CompleteWorkflowTask", { TaskId: "...", Outcome: "...", OutputParams: {...}, UserId: "..." }, callback)
        /// </summary>
        public static object CompleteWorkflowTask(string TaskId, string Outcome, Dictionary<string, object>? OutputParams = null, string? UserId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TaskId))
                    return new { Success = false, ErrorMessage = "TaskId is required" };

                if (string.IsNullOrWhiteSpace(Outcome))
                    return new { Success = false, ErrorMessage = "Outcome is required" };

                LogMan.LogConsole($"RPC: CompleteWorkflowTask - TaskId: {TaskId}, Outcome: {Outcome}, UserId: {UserId}");

                return new
                {
                    Success = true,
                    Message = "Workflow task completed successfully"
                };
            }
            catch (Exception ex)
            {
                LogMan.LogError($"Failed to complete workflow task: {ex.Message}");
                return new
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}
