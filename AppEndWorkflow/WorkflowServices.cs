using AppEndCommon;
using AppEndDbIO;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;

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
                var all = WorkflowDefinitionProvider.GetAll().ToList();
                var published = all.Count(w => w.IsPublished);

                return new
                {
                    totalWorkflows = all.Count,
                    publishedWorkflows = published,
                    unpublishedWorkflows = all.Count - published,
                    workflows = all.Select(w => new { w.Id, w.Name, w.IsPublished })
                };
            }
            catch (Exception ex)
            {
                LogMan.LogError($"Failed to get workflow stats: {ex.Message}");
                return new { error = ex.Message };
            }
        }

        /// <summary>
        /// Gets paginated list of workflow instances with optional filtering.
        /// 
        /// RPC Call:
        /// rpcAEP("GetWorkflowInstances", { 
        ///     Filter: "order",
        ///     Status: "Running",
        ///     Page: 1,
        ///     PageSize: 25
        /// }, callback)
        /// </summary>
        public static object GetWorkflowInstances(string? Filter = null, string? Status = null, int Page = 1, int PageSize = 25)
        {
            try
            {
                var services = GetServices();
                var storeType = FindType("Elsa.Workflows.Runtime.IWorkflowInstanceStore") 
                    ?? FindType("Elsa.Workflows.IWorkflowInstanceStore");

                if (storeType == null)
                {
                    LogMan.LogWarning("IWorkflowInstanceStore type not found");
                    return new { success = false, error = "Workflow instance store not available" };
                }

                var store = services.GetService(storeType);
                if (store == null)
                {
                    LogMan.LogWarning("IWorkflowInstanceStore service not available");
                    return new { success = false, error = "Workflow instance store service not available" };
                }

                // Find FindAsync or FindManyAsync method
                var findMethod = storeType.GetMethods()
                    .FirstOrDefault(m => m.Name is "FindManyAsync" or "FindAsync" && m.GetParameters().Length > 0);

                if (findMethod == null)
                {
                    LogMan.LogWarning("FindManyAsync method not found on IWorkflowInstanceStore");
                    return new { success = false, error = "Query method not available" };
                }

                // Build query filter
                var filterType = FindType("Elsa.Workflows.Runtime.Filters.WorkflowInstanceFilter")
                    ?? FindType("Elsa.Workflows.Filters.WorkflowInstanceFilter");

                object? filterObj = null;
                if (filterType != null)
                {
                    filterObj = Activator.CreateInstance(filterType);
                    if (!string.IsNullOrEmpty(Status))
                        SetPropertyIfExists(filterObj!, "WorkflowStatus", Status);
                    if (!string.IsNullOrEmpty(Filter))
                        SetPropertyIfExists(filterObj!, "DefinitionId", Filter);
                }

                // Invoke query
                var args = new object?[] { filterObj, CancellationToken.None };
                var queryResult = findMethod.Invoke(store, args);
                var instances = ResolveTaskResult(queryResult);

                if (instances == null)
                {
                    return new { 
                        success = true, 
                        instances = new List<InstanceSummary>(), 
                        totalCount = 0,
                        page = Page,
                        pageSize = PageSize
                    };
                }

                // Map to summary list
                var instanceList = new List<InstanceSummary>();
                var enumerableType = instances.GetType();
                var enumerator = ((System.Collections.IEnumerable)instances).GetEnumerator();

                while (enumerator.MoveNext())
                {
                    var instance = enumerator.Current;
                    if (instance == null) continue;

                    var instType = instance.GetType();
                    instanceList.Add(new InstanceSummary
                    {
                        InstanceId = GetPropertyValue(instType, instance, "Id")?.ToString() ?? string.Empty,
                        DefinitionId = GetPropertyValue(instType, instance, "DefinitionId")?.ToString() ?? string.Empty,
                        Status = GetPropertyValue(instType, instance, "WorkflowStatus")?.ToString() 
                            ?? GetPropertyValue(instType, instance, "Status")?.ToString() ?? "Unknown",
                        StartedAt = GetPropertyValue(instType, instance, "CreatedAt") as DateTime?,
                        FinishedAt = GetPropertyValue(instType, instance, "FinishedAt") as DateTime?,
                        LastExecutedAt = GetPropertyValue(instType, instance, "LastExecutedAt") as DateTime?,
                        IncidentCount = GetPropertyValue(instType, instance, "IncidentCount") as int? ?? 0
                    });
                }

                // Apply pagination
                var totalCount = instanceList.Count;
                var pagedList = instanceList
                    .Skip((Page - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();

                // Enrich with definition names
                foreach (var inst in pagedList)
                {
                    var def = WorkflowDefinitionProvider.Get(inst.DefinitionId);
                    if (def != null)
                        inst.DefinitionName = def.Name;
                }

                return new
                {
                    success = true,
                    instances = pagedList,
                    totalCount,
                    page = Page,
                    pageSize = PageSize,
                    totalPages = (int)Math.Ceiling(totalCount / (double)PageSize)
                };
            }
            catch (Exception ex)
            {
                LogMan.LogError($"GetWorkflowInstances failed: {ex.Message}");
                return new { success = false, error = ex.Message };
            }
        }

        /// <summary>
        /// Gets details of a single workflow instance including execution log.
        /// 
        /// RPC Call:
        /// rpcAEP("GetWorkflowInstance", { InstanceId: "guid-here" }, callback)
        /// </summary>
        public static object? GetWorkflowInstance(string InstanceId)
        {
            try
            {
                var services = GetServices();
                var storeType = FindType("Elsa.Workflows.Runtime.IWorkflowInstanceStore")
                    ?? FindType("Elsa.Workflows.IWorkflowInstanceStore");

                if (storeType == null)
                    return new { success = false, error = "Workflow instance store not available" };

                var store = services.GetService(storeType);
                if (store == null)
                    return new { success = false, error = "Workflow instance store service not available" };

                var findMethod = storeType.GetMethods()
                    .FirstOrDefault(m => m.Name is "FindAsync" && m.GetParameters().Length == 2);

                if (findMethod == null)
                    return new { success = false, error = "Find method not available" };

                var args = new object[] { InstanceId, CancellationToken.None };
                var queryResult = findMethod.Invoke(store, args);
                var instance = ResolveTaskResult(queryResult);

                if (instance == null)
                    return new { success = false, error = "Instance not found" };

                var instType = instance.GetType();
                return new
                {
                    success = true,
                    instance = new
                    {
                        InstanceId = GetPropertyValue(instType, instance, "Id")?.ToString(),
                        DefinitionId = GetPropertyValue(instType, instance, "DefinitionId")?.ToString(),
                        Status = GetPropertyValue(instType, instance, "WorkflowStatus")?.ToString()
                            ?? GetPropertyValue(instType, instance, "Status")?.ToString(),
                        StartedAt = GetPropertyValue(instType, instance, "CreatedAt"),
                        FinishedAt = GetPropertyValue(instType, instance, "FinishedAt"),
                        LastExecutedAt = GetPropertyValue(instType, instance, "LastExecutedAt"),
                        IncidentCount = GetPropertyValue(instType, instance, "IncidentCount")
                    }
                };
            }
            catch (Exception ex)
            {
                LogMan.LogError($"GetWorkflowInstance failed: {ex.Message}");
                return new { success = false, error = ex.Message };
            }
        }

        /// <summary>
        /// Cancels a running workflow instance.
        /// 
        /// RPC Call:
        /// rpcAEP("CancelWorkflowInstance", { InstanceId: "guid-here" }, callback)
        /// </summary>
        public static object CancelWorkflowInstance(string InstanceId)
        {
            try
            {
                var services = GetServices();
                var runtimeType = FindType("Elsa.Workflows.Runtime.IWorkflowRuntime")
                    ?? FindType("Elsa.Workflows.IWorkflowRuntime");

                if (runtimeType == null)
                    return new { success = false, error = "Workflow runtime not available" };

                var runtime = services.GetService(runtimeType);
                if (runtime == null)
                    return new { success = false, error = "Workflow runtime service not available" };

                var cancelMethod = runtimeType.GetMethods()
                    .FirstOrDefault(m => m.Name is "CancelWorkflowAsync" or "CancelAsync");

                if (cancelMethod == null)
                    return new { success = false, error = "Cancel method not available" };

                var args = new object[] { InstanceId, CancellationToken.None };
                var cancelResult = cancelMethod.Invoke(runtime, args);
                ResolveTaskResult(cancelResult);

                LogMan.LogConsole($"Workflow instance canceled: {InstanceId}");

                return new
                {
                    success = true,
                    message = "Workflow instance canceled successfully",
                    instanceId = InstanceId,
                    timestamp = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                LogMan.LogError($"CancelWorkflowInstance failed: {ex.Message}");
                return new { success = false, error = ex.Message };
            }
        }

        /// <summary>
        /// Gets current user's pending workflow tasks (kartabl).
        /// 
        /// RPC Call:
        /// rpcAEP("GetMyWorkflowTasks", { 
        ///     Status: "Pending",
        ///     Page: 1,
        ///     PageSize: 25
        /// }, callback)
        /// </summary>
        public static object GetMyWorkflowTasks(string? Status = null, int Page = 1, int PageSize = 25, string? CurrentUser = null)
        {
            try
            {
                // Use provided user or default placeholder (in production, get from HttpContext via proxy)
                var userId = string.IsNullOrEmpty(CurrentUser) ? "CurrentUser" : CurrentUser;

                using var dbIO = AppEndDbIO.DbIO.Instance();
                
                // Query tasks using the stored procedure
                var parameters = new List<System.Data.Common.DbParameter>
                {
                    dbIO.CreateParameter("@UserId", "NVARCHAR", 100, userId),
                    dbIO.CreateParameter("@Status", "NVARCHAR", 50, string.IsNullOrEmpty(Status) ? (object)DBNull.Value : Status),
                    dbIO.CreateParameter("@Page", "INT", null, Page),
                    dbIO.CreateParameter("@PageSize", "INT", null, PageSize)
                };

                var sql = "EXEC [dbo].[ElsaGetMyWorkflowTasks] @UserId, @Status, @Page, @PageSize";
                var resultSet = dbIO.ToDataSet(sql, parameters);

                if (resultSet == null || resultSet.Count == 0)
                {
                    return new
                    {
                        success = true,
                        tasks = new List<TaskSummary>(),
                        totalCount = 0,
                        page = Page,
                        pageSize = PageSize,
                        totalPages = 0
                    };
                }

                var tasks = new List<TaskSummary>();
                var tasksTable = resultSet["T0"];

                foreach (System.Data.DataRow row in tasksTable.Rows)
                {
                    tasks.Add(new TaskSummary
                    {
                        TaskId = row["Id"]?.ToString() ?? "",
                        WorkflowInstanceId = row["InstanceId"]?.ToString() ?? "",
                        WorkflowDefinitionId = row["DefinitionId"]?.ToString() ?? "",
                        Title = row["Title"]?.ToString() ?? "",
                        Description = row["Description"]?.ToString(),
                        AssignedTo = row["AssignedTo"]?.ToString(),
                        AssignedRole = row["AssignedRole"]?.ToString(),
                        Priority = row["Priority"]?.ToString() ?? "Normal",
                        Status = row["Status"]?.ToString() ?? "Pending",
                        DueDate = row["DueDate"] != DBNull.Value ? (DateTime?)row["DueDate"] : null,
                        CreatedAt = row["CreatedAt"] != DBNull.Value ? (DateTime)row["CreatedAt"] : DateTime.UtcNow,
                        ContextData = row["ContextData"]?.ToString()
                    });
                }

                // Get total count from second result set
                int totalCount = 0;
                if (resultSet.ContainsKey("T1"))
                {
                    var countTable = resultSet["T1"];
                    if (countTable.Rows.Count > 0 && int.TryParse(countTable.Rows[0][0].ToString(), out int count))
                    {
                        totalCount = count;
                    }
                }

                return new
                {
                    success = true,
                    tasks = tasks,
                    totalCount = totalCount,
                    page = Page,
                    pageSize = PageSize,
                    totalPages = totalCount > 0 ? (int)Math.Ceiling(totalCount / (double)PageSize) : 0
                };
            }
            catch (Exception ex)
            {
                LogMan.LogError($"GetMyWorkflowTasks failed: {ex.Message}");
                return new { success = false, error = ex.Message };
            }
        }

        /// <summary>
        /// Completes a workflow task and resumes the workflow.
        /// 
        /// RPC Call:
        /// rpcAEP("CompleteWorkflowTask", { 
        ///     TaskId: "guid-here",
        ///     Outcome: "Approve",
        ///     OutputParams: { comment: "Looks good" }
        /// }, callback)
        /// </summary>
        public static object CompleteWorkflowTask(string TaskId, string Outcome, Dictionary<string, object>? OutputParams = null, string? CurrentUser = null)
        {
            try
            {
                // Use provided user or default placeholder (in production, get from HttpContext via proxy)
                var userId = string.IsNullOrEmpty(CurrentUser) ? "CurrentUser" : CurrentUser;
                var comment = OutputParams?["comment"]?.ToString();

                using var dbIO = AppEndDbIO.DbIO.Instance();

                // Call stored procedure to complete task
                var parameters = new List<System.Data.Common.DbParameter>
                {
                    dbIO.CreateParameter("@TaskId", "UNIQUEIDENTIFIER", null, new Guid(TaskId)),
                    dbIO.CreateParameter("@UserId", "NVARCHAR", 100, userId),
                    dbIO.CreateParameter("@Outcome", "NVARCHAR", 100, Outcome),
                    dbIO.CreateParameter("@Comment", "NVARCHAR", -1, string.IsNullOrEmpty(comment) ? (object)DBNull.Value : comment)
                };

                var sql = "EXEC [dbo].[ElsaCompleteWorkflowTask] @TaskId, @UserId, @Outcome, @Comment";
                var resultSet = dbIO.ToDataSet(sql, parameters);

                if (resultSet == null || !resultSet.ContainsKey("T0") || resultSet["T0"].Rows.Count == 0)
                {
                    return new
                    {
                        success = false,
                        error = "Task not found or already completed",
                        taskId = TaskId
                    };
                }

                var resultRow = resultSet["T0"].Rows[0];
                var bookmarkId = resultRow["BookmarkId"]?.ToString();
                var completedAt = resultRow["CompletedAt"] != DBNull.Value ? (DateTime)resultRow["CompletedAt"] : DateTime.UtcNow;

                // Resume workflow via bookmark if available
                if (!string.IsNullOrEmpty(bookmarkId))
                {
                    try
                    {
                        var services = GetServices();
                        var bookmarkManagerType = FindType("Elsa.Workflows.Runtime.IBookmarkManager")
                            ?? FindType("Elsa.Workflows.IBookmarkManager");

                        if (bookmarkManagerType != null)
                        {
                            var bookmarkManager = services.GetService(bookmarkManagerType);
                            var resumeMethod = bookmarkManagerType.GetMethods()
                                .FirstOrDefault(m => m.Name == "ResumeAsync" && m.GetParameters().Length > 0);

                            if (resumeMethod != null)
                            {
                                var resumeParams = new object[] 
                                { 
                                    bookmarkId, 
                                    new { Outcome, Result = OutputParams },
                                    CancellationToken.None 
                                };
                                
                                var resumeResult = resumeMethod.Invoke(bookmarkManager, resumeParams);
                                if (resumeResult is Task task)
                                {
                                    task.Wait();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogMan.LogWarning($"Failed to resume workflow via bookmark: {ex.Message}");
                    }
                }

                return new
                {
                    success = true,
                    message = "Task completed successfully",
                    taskId = TaskId,
                    outcome = Outcome,
                    completedAt = completedAt,
                    completedBy = userId,
                    bookmarkResumed = !string.IsNullOrEmpty(bookmarkId)
                };
            }
            catch (Exception ex)
            {
                LogMan.LogError($"CompleteWorkflowTask failed: {ex.Message}");
                return new { success = false, error = ex.Message };
            }
        }

        /// <summary>
        /// Gets workflow execution log entries for debugging.
        /// 
        /// RPC Call:
        /// rpcAEP("GetWorkflowExecutionLog", { InstanceId: "guid-here" }, callback)
        /// </summary>
        public static object GetWorkflowExecutionLog(string InstanceId)
        {
            try
            {
                var services = GetServices();
                var logStoreType = FindType("Elsa.Workflows.Runtime.IWorkflowExecutionLogStore")
                    ?? FindType("Elsa.Workflows.IWorkflowExecutionLogStore");

                if (logStoreType == null)
                    return new { success = false, error = "Execution log store not available" };

                var logStore = services.GetService(logStoreType);
                if (logStore == null)
                    return new { success = false, error = "Execution log store service not available" };

                var findMethod = logStoreType.GetMethods()
                    .FirstOrDefault(m => m.Name.Contains("Find") && m.GetParameters().Any(p => p.Name == "workflowInstanceId"));

                if (findMethod == null)
                    return new { success = false, error = "Log query method not available" };

                var args = new object[] { InstanceId, CancellationToken.None };
                var queryResult = findMethod.Invoke(logStore, args);
                var logs = ResolveTaskResult(queryResult);

                if (logs == null)
                    return new { success = true, logs = new List<object>() };

                var logList = new List<object>();
                var enumerator = ((System.Collections.IEnumerable)logs).GetEnumerator();

                while (enumerator.MoveNext())
                {
                    var log = enumerator.Current;
                    if (log == null) continue;

                    var logType = log.GetType();
                    logList.Add(new
                    {
                        ActivityId = GetPropertyValue(logType, log, "ActivityId")?.ToString(),
                        EventName = GetPropertyValue(logType, log, "EventName")?.ToString(),
                        Timestamp = GetPropertyValue(logType, log, "Timestamp"),
                        Payload = GetPropertyValue(logType, log, "Payload")
                    });
                }

                return new
                {
                    success = true,
                    logs = logList,
                    instanceId = InstanceId
                };
            }
            catch (Exception ex)
            {
                LogMan.LogError($"GetWorkflowExecutionLog failed: {ex.Message}");
                return new { success = false, error = ex.Message };
            }
        }
    }
}
