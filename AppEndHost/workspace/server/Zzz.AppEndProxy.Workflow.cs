namespace Zzz
{
    public static partial class AppEndProxy
    {
        #region Workflow API
        public static object? GetWorkflowDefinitions(AppEndUser? Actor)
        {
            try
            {
                return AppEndWorkflow.WorkflowServices.GetWorkflowDefinitions();
            }
            catch (Exception ex)
            {
                return new { success = false, errorMessage = ex.Message };
            }
        }

        public static object? GetWorkflowDefinition(AppEndUser? Actor, string WorkflowId)
        {
            try
            {
                return AppEndWorkflow.WorkflowServices.GetWorkflowDefinition(WorkflowId);
            }
            catch (Exception ex)
            {
                return new { success = false, errorMessage = ex.Message };
            }
        }

        public static object? ExecuteWorkflow(AppEndUser? Actor, string WorkflowId, object? InputParams = null)
        {
            try
            {
                // Convert JsonElement to Dictionary if needed
                Dictionary<string, object>? parsedParams = null;
                
                if (InputParams != null)
                {
                    if (InputParams is JsonElement je)
                    {
                        if (je.ValueKind == System.Text.Json.JsonValueKind.Object)
                        {
                            parsedParams = new Dictionary<string, object>();
                            foreach (var prop in je.EnumerateObject())
                            {
                                parsedParams[prop.Name] = ParseJsonElementValue(prop.Value);
                            }
                        }
                    }
                    else if (InputParams is Dictionary<string, object> dict)
                    {
                        parsedParams = dict;
                    }
                }

                return AppEndWorkflow.WorkflowServices.ExecuteWorkflow(WorkflowId, parsedParams);
            }
            catch (Exception ex)
            {
                return new { success = false, errorMessage = ex.Message };
            }
        }

        private static object? ParseJsonElementValue(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case System.Text.Json.JsonValueKind.String:
                    return element.GetString();
                case System.Text.Json.JsonValueKind.Number:
                    if (element.TryGetInt32(out var i)) return i;
                    if (element.TryGetInt64(out var l)) return l;
                    if (element.TryGetDouble(out var d)) return d;
                    return element.GetDecimal();
                case System.Text.Json.JsonValueKind.True:
                    return true;
                case System.Text.Json.JsonValueKind.False:
                    return false;
                case System.Text.Json.JsonValueKind.Null:
                    return null;
                case System.Text.Json.JsonValueKind.Object:
                    var dict = new Dictionary<string, object>();
                    foreach (var prop in element.EnumerateObject())
                    {
                        dict[prop.Name] = ParseJsonElementValue(prop.Value) ?? new object();
                    }
                    return dict;
                case System.Text.Json.JsonValueKind.Array:
                    var list = new List<object>();
                    foreach (var item in element.EnumerateArray())
                    {
                        var val = ParseJsonElementValue(item);
                        if (val != null) list.Add(val);
                    }
                    return list;
                default:
                    return null;
            }
        }

        public static object? ReloadWorkflow(AppEndUser? Actor, string WorkflowId)
        {
            try
            {
                return AppEndWorkflow.WorkflowServices.ReloadWorkflow(WorkflowId);
            }
            catch (Exception ex)
            {
                return new { success = false, errorMessage = ex.Message };
            }
        }

        public static object? ReloadAllWorkflows(AppEndUser? Actor)
        {
            try
            {
                return AppEndWorkflow.WorkflowServices.ReloadAllWorkflows();
            }
            catch (Exception ex)
            {
                return new { success = false, errorMessage = ex.Message };
            }
        }

        public static object? GetWorkflowStats(AppEndUser? Actor)
        {
            try
            {
                return AppEndWorkflow.WorkflowServices.GetWorkflowStats();
            }
            catch (Exception ex)
            {
                return new { success = false, errorMessage = ex.Message };
            }
        }

        public static object? GetMyWorkflowTasks(AppEndUser? Actor, string? Status = null, int Page = 1, int PageSize = 25)
        {
            try
            {
                var userId = Actor?.Id.ToString() ?? "Anonymous";
                return AppEndWorkflow.WorkflowServices.GetMyWorkflowTasks(Status, Page, PageSize, userId);
            }
            catch (Exception ex)
            {
                return new { success = false, errorMessage = ex.Message };
            }
        }

        public static object? CompleteWorkflowTask(AppEndUser? Actor, string TaskId, string Outcome, object? OutputParams = null)
        {
            try
            {
                var userId = Actor?.Id.ToString() ?? "Anonymous";
                
                // Convert JsonElement to Dictionary if needed
                Dictionary<string, object>? parsedParams = null;
                
                if (OutputParams != null)
                {
                    if (OutputParams is JsonElement je)
                    {
                        if (je.ValueKind == System.Text.Json.JsonValueKind.Object)
                        {
                            parsedParams = new Dictionary<string, object>();
                            foreach (var prop in je.EnumerateObject())
                            {
                                parsedParams[prop.Name] = ParseJsonElementValue(prop.Value);
                            }
                        }
                    }
                    else if (OutputParams is Dictionary<string, object> dict)
                    {
                        parsedParams = dict;
                    }
                }

                return AppEndWorkflow.WorkflowServices.CompleteWorkflowTask(TaskId, Outcome, parsedParams, userId);
            }
            catch (Exception ex)
            {
                return new { success = false, errorMessage = ex.Message };
            }
        }

        public static object? PublishWorkflow(AppEndUser? Actor, string WorkflowId)
        {
            try
            {
                return AppEndWorkflow.WorkflowServices.PublishWorkflow(WorkflowId);
            }
            catch (Exception ex)
            {
                return new { Success = false, ErrorMessage = ex.Message };
            }
        }

        public static object? UnpublishWorkflow(AppEndUser? Actor, string WorkflowId)
        {
            try
            {
                return AppEndWorkflow.WorkflowServices.UnpublishWorkflow(WorkflowId);
            }
            catch (Exception ex)
            {
                return new { Success = false, ErrorMessage = ex.Message };
            }
        }

        public static object? DeleteWorkflow(AppEndUser? Actor, string WorkflowId)
        {
            try
            {
                return AppEndWorkflow.WorkflowServices.DeleteWorkflow(WorkflowId);
            }
            catch (Exception ex)
            {
                return new { Success = false, ErrorMessage = ex.Message };
            }
        }

        #endregion
    }
}
