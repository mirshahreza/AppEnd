namespace Zzz
{
    public static partial class AppEndProxy
    {
        #region Scheduler API
        public static object? SchedulerGetAllTasks(AppEndUser? Actor, object? Enabled = null, string? SearchTerm = null)
        {
            try
            {
                // Parse the Enabled parameter - it might come as JsonElement, bool, or string
                bool? enabled = null;
                
                if (Enabled != null)
                {
                    if (Enabled is JsonElement je)
                    {
                        if (je.ValueKind == System.Text.Json.JsonValueKind.True)
                            enabled = true;
                        else if (je.ValueKind == System.Text.Json.JsonValueKind.False)
                            enabled = false;
                        else if (je.ValueKind == System.Text.Json.JsonValueKind.String)
                        {
                            var str = je.GetString();
                            if (bool.TryParse(str, out var boolValue))
                                enabled = boolValue;
                        }
                    }
                    else if (Enabled is bool b)
                    {
                        enabled = b;
                    }
                    else if (Enabled is string s)
                    {
                        if (bool.TryParse(s, out var boolValue))
                            enabled = boolValue;
                    }
                }

                var manager = GetSchedulerManager();
                return manager.GetAllTasks(enabled, SearchTerm);
            }
            catch (Exception ex)
            {
                return new { error = ex.Message };
            }
        }

        public static object? SchedulerGetTask(AppEndUser? Actor, string TaskId)
        {
            try
            {
                var manager = GetSchedulerManager();
                var task = manager.GetTask(TaskId);
                if (task == null)
                    return new { error = "Task not found" };
                return task;
            }
            catch (Exception ex)
            {
                return new { error = ex.Message };
            }
        }

        public static object? SchedulerCreateTask(AppEndUser? Actor, JsonElement TaskData)
        {
            try
            {
                var task = JsonSerializer.Deserialize<AppEndServer.ScheduledTask>(TaskData.GetRawText());
                if (task == null)
                    return new { error = "Invalid task data" };

                task.CreatedBy = Actor?.UserName;
                var manager = GetSchedulerManager();
                return manager.CreateTask(task);
            }
            catch (Exception ex)
            {
                return new { error = ex.Message };
            }
        }

        public static object? SchedulerUpdateTask(AppEndUser? Actor, JsonElement TaskData)
        {
            try
            {
                var task = JsonSerializer.Deserialize<AppEndServer.ScheduledTask>(TaskData.GetRawText());
                if (task == null)
                    return new { error = "Invalid task data" };

                var manager = GetSchedulerManager();
                return manager.UpdateTask(task);
            }
            catch (Exception ex)
            {
                return new { error = ex.Message };
            }
        }

        public static object? SchedulerDeleteTask(AppEndUser? Actor, string TaskId)
        {
            try
            {
                var manager = GetSchedulerManager();
                return manager.DeleteTask(TaskId);
            }
            catch (Exception ex)
            {
                return new { error = ex.Message };
            }
        }

        public static object? SchedulerStartTask(AppEndUser? Actor, string TaskId)
        {
            try
            {
                var manager = GetSchedulerManager();
                return manager.StartTask(TaskId);
            }
            catch (Exception ex)
            {
                return new { error = ex.Message };
            }
        }

        public static object? SchedulerStopTask(AppEndUser? Actor, string TaskId)
        {
            try
            {
                var manager = GetSchedulerManager();
                return manager.StopTask(TaskId);
            }
            catch (Exception ex)
            {
                return new { error = ex.Message };
            }
        }

        public static object? SchedulerPauseTask(AppEndUser? Actor, string TaskId)
        {
            try
            {
                var manager = GetSchedulerManager();
                return manager.PauseTask(TaskId);
            }
            catch (Exception ex)
            {
                return new { error = ex.Message };
            }
        }

        public static object? SchedulerResumeTask(AppEndUser? Actor, string TaskId)
        {
            try
            {
                var manager = GetSchedulerManager();
                return manager.ResumeTask(TaskId);
            }
            catch (Exception ex)
            {
                return new { error = ex.Message };
            }
        }

        public static object? SchedulerExecuteNow(AppEndUser? Actor, string TaskId)
        {
            try
            {
                var manager = GetSchedulerManager();
                return manager.ExecuteNow(TaskId);
            }
            catch (Exception ex)
            {
                return new { error = ex.Message };
            }
        }

        public static object? SchedulerToggleTask(AppEndUser? Actor, string TaskId, bool Enable)
        {
            try
            {
                var manager = GetSchedulerManager();
                return manager.ToggleTask(TaskId, Enable);
            }
            catch (Exception ex)
            {
                return new { error = ex.Message };
            }
        }

        public static object? SchedulerGetStatistics(AppEndUser? Actor)
        {
            try
            {
                var manager = GetSchedulerManager();
                return manager.GetStatistics();
            }
            catch (Exception ex)
            {
                return new { error = ex.Message };
            }
        }

        public static object? SchedulerGetHistory(AppEndUser? Actor, string? TaskId = null, int MaxEntries = 100)
        {
            try
            {
                var manager = GetSchedulerManager();
                return manager.GetHistory(TaskId, MaxEntries);
            }
            catch (Exception ex)
            {
                return new { error = ex.Message };
            }
        }

        public static object? SchedulerGetAvailableMethods(AppEndUser? Actor)
        {
            try
            {
                return AppEndServer.MethodDiscovery.GetAllMethods();
            }
            catch (Exception ex)
            {
                return new { error = ex.Message };
            }
        }

        public static object? SchedulerReloadTasks(AppEndUser? Actor)
        {
            try
            {
                var manager = GetSchedulerManager();
                return manager.ReloadTasksFromSettings();
            }
            catch (Exception ex)
            {
                return new { error = ex.Message };
            }
        }

        private static AppEndServer.SchedulerManager GetSchedulerManager()
        {
            return AppEndServer.SchedulerService.GetManager()
                ?? throw new InvalidOperationException("Scheduler not initialized");
        }
        #endregion
    }
}
