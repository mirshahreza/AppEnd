using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using AppEndCommon;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace AppEndServer
{
	public class SchedulerManager
	{
		private readonly SchedulerService _service;
		private readonly ILogger<SchedulerManager> _logger;
		private readonly Dictionary<string, ScheduledTask> _registry = [];
		private readonly List<TaskExecutionHistory> _history = [];
		private readonly object _lock = new();
		private const int MaxHistory = 1000;
		private const string TasksSettingsKey = "ScheduledTasks";

		public SchedulerManager(SchedulerService service, ILogger<SchedulerManager> logger)
		{
			_service = service;
			_logger = logger;
			LoadTasksFromSettings();
		}

		private void LoadTasksFromSettings()
		{
			try
			{
				lock (_lock)
				{
					var tasksNode = AppEndSettings.AppSettings["AppEnd"]?[TasksSettingsKey];
					if (tasksNode is JsonArray tasksArray)
					{
						foreach (var taskNode in tasksArray)
						{
							try
							{
								var task = JsonSerializer.Deserialize<ScheduledTask>(taskNode.ToJsonString());
								if (task != null && IsValidMethodName(task.MethodFullName))
								{
									_registry[task.TaskId] = task;
									
									if (task.Enabled)
									{
										_service.RegisterTask(task);
										_logger.LogInformation($"Auto-started task: {task.Name} ({task.TaskId})");
									}
									else
									{
										_logger.LogInformation($"Loaded task (disabled): {task.Name} ({task.TaskId})");
									}
								}
							}
							catch (Exception ex)
							{
								_logger.LogError(ex, "Error loading individual task from settings");
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error loading tasks from settings");
			}
		}

		public OperationResult ReloadTasksFromSettings()
		{
			try
			{
				lock (_lock)
				{
					_logger.LogInformation("Reloading tasks from settings...");
					
					// First, unregister all current tasks
					var currentTaskIds = _registry.Keys.ToList();
					foreach (var taskId in currentTaskIds)
					{
						_service.UnregisterTask(taskId);
					}
					
					// Clear the registry
					_registry.Clear();
					
					// Refresh settings from file
					AppEndSettings.RefereshSettings();
					
					// Reload tasks from refreshed settings
					var tasksNode = AppEndSettings.AppSettings["AppEnd"]?[TasksSettingsKey];
					if (tasksNode is JsonArray tasksArray)
					{
						foreach (var taskNode in tasksArray)
						{
							try
							{
								var task = JsonSerializer.Deserialize<ScheduledTask>(taskNode.ToJsonString());
								if (task != null && IsValidMethodName(task.MethodFullName))
								{
									_registry[task.TaskId] = task;
									
									if (task.Enabled)
									{
										_service.RegisterTask(task);
										_logger.LogInformation($"Reloaded and started task: {task.Name} ({task.TaskId})");
									}
									else
									{
										_logger.LogInformation($"Reloaded task (disabled): {task.Name} ({task.TaskId})");
									}
								}
							}
							catch (Exception ex)
							{
								_logger.LogError(ex, $"Error reloading individual task from settings");
							}
						}
					}
					
					_logger.LogInformation($"Successfully reloaded {_registry.Count} tasks from settings");
					return new() { Success = true, Message = $"Successfully reloaded {_registry.Count} tasks" };
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error reloading tasks from settings");
				return new() { Success = false, Message = $"Error reloading tasks: {ex.Message}" };
			}
		}

		private void SaveTasksToSettings()
		{
			try
			{
				lock (_lock)
				{
					var tasksList = _registry.Values.ToList();
					var tasksJson = JsonSerializer.Serialize(tasksList, new JsonSerializerOptions 
					{ 
						WriteIndented = true,
						DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
					});
					
					var tasksNode = JsonNode.Parse(tasksJson);
					
					var appEndNode = AppEndSettings.AppSettings["AppEnd"];
					if (appEndNode == null)
					{
						appEndNode = new JsonObject();
						AppEndSettings.AppSettings["AppEnd"] = appEndNode;
					}
					
					appEndNode[TasksSettingsKey] = tasksNode;
					AppEndSettings.Save();
					
					_logger.LogInformation($"Saved {tasksList.Count} tasks to settings");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error saving tasks to settings");
			}
		}

		private void SyncTaskWithService(ScheduledTask task)
		{
			var serviceTask = _service.GetTask(task.TaskId);
			if (serviceTask != null)
			{
				task.State = serviceTask.State;
				task.LastRunTime = serviceTask.LastRunTime;
				task.NextRunTime = serviceTask.NextRunTime;
				task.ExecutionCount = serviceTask.ExecutionCount;
				task.FailureCount = serviceTask.FailureCount;
				task.LastError = serviceTask.LastError;
			}
		}

		public OperationResult CreateTask(ScheduledTask task)
		{
			try
			{
				lock (_lock)
				{
					if (_registry.ContainsKey(task.TaskId))
						return new() { Success = false, Message = "Task ID already exists" };

					if (!IsValidMethodName(task.MethodFullName))
						return new() { Success = false, Message = "Method must be Namespace.ClassName.MethodName" };

					_registry[task.TaskId] = task;

					if (task.Enabled)
						_service.RegisterTask(task);

					SaveTasksToSettings();
					return new() { Success = true, Message = "Task created", Data = task };
				}
			}
			catch (Exception ex)
			{
				return new() { Success = false, Message = ex.Message };
			}
		}

		public OperationResult UpdateTask(ScheduledTask updatedTask)
		{
			try
			{
				lock (_lock)
				{
					if (!_registry.ContainsKey(updatedTask.TaskId))
						return new() { Success = false, Message = "Task not found" };

					if (!IsValidMethodName(updatedTask.MethodFullName))
						return new() { Success = false, Message = "Method must be Namespace.ClassName.MethodName" };

					bool wasRegistered = _service.IsTaskRegistered(updatedTask.TaskId);

					_registry[updatedTask.TaskId] = updatedTask;

					if (wasRegistered)
						_service.UnregisterTask(updatedTask.TaskId);

					if (updatedTask.Enabled)
						_service.RegisterTask(updatedTask);

					SaveTasksToSettings();
					return new() { Success = true, Message = "Task updated", Data = updatedTask };
				}
			}
			catch (Exception ex)
			{
				return new() { Success = false, Message = ex.Message };
			}
		}

		public OperationResult DeleteTask(string taskId)
		{
			try
			{
				lock (_lock)
				{
					if (!_registry.Remove(taskId))
						return new() { Success = false, Message = "Task not found" };

					_service.UnregisterTask(taskId);
					SaveTasksToSettings();
					return new() { Success = true, Message = "Task deleted" };
				}
			}
			catch (Exception ex)
			{
				return new() { Success = false, Message = ex.Message };
			}
		}

		public ScheduledTask? GetTask(string taskId)
		{
			lock (_lock)
			{
				if (_registry.TryGetValue(taskId, out var task))
				{
					SyncTaskWithService(task);
					return task;
				}
				return null;
			}
		}

		public List<ScheduledTask> GetAllTasks(bool? enabled = null, string? searchTerm = null)
		{
			lock (_lock)
			{
				foreach (var task in _registry.Values)
				{
					SyncTaskWithService(task);
				}

				var query = _registry.Values.AsEnumerable();

				if (enabled.HasValue)
					query = query.Where(t => t.Enabled == enabled.Value);

				if (!string.IsNullOrWhiteSpace(searchTerm))
				{
					var term = searchTerm.ToLower();
					query = query.Where(t => 
						t.Name.ToLower().Contains(term) || 
						t.Description.ToLower().Contains(term) ||
						t.MethodFullName.ToLower().Contains(term));
				}

				return query.ToList();
			}
		}

		public OperationResult StartTask(string taskId)
		{
			try
			{
				lock (_lock)
				{
					if (!_registry.TryGetValue(taskId, out var task))
						return new() { Success = false, Message = "Task not found" };

					if (!task.Enabled)
						return new() { Success = false, Message = "Task is disabled. Enable it first." };

					if (_service.IsTaskRegistered(taskId))
						return new() { Success = false, Message = "Already running" };

					_service.RegisterTask(task);
					return new() { Success = true, Message = "Task started" };
				}
			}
			catch (Exception ex)
			{
				return new() { Success = false, Message = ex.Message };
			}
		}

		public OperationResult StopTask(string taskId)
		{
			try
			{
				lock (_lock)
				{
					if (!_registry.TryGetValue(taskId, out var task))
						return new() { Success = false, Message = "Task not found" };

					_service.UnregisterTask(taskId);
					task.State = TaskState.Stopped;
					task.NextRunTime = null;
					return new() { Success = true, Message = "Task stopped" };
				}
			}
			catch (Exception ex)
			{
				return new() { Success = false, Message = ex.Message };
			}
		}

		public OperationResult PauseTask(string taskId)
		{
			try
			{
				lock (_lock)
				{
					if (!_registry.TryGetValue(taskId, out var task))
						return new() { Success = false, Message = "Task not found" };

					_service.PauseTask(taskId);
					return new() { Success = true, Message = "Task paused" };
				}
			}
			catch (Exception ex)
			{
				return new() { Success = false, Message = ex.Message };
			}
		}

		public OperationResult ResumeTask(string taskId)
		{
			try
			{
				lock (_lock)
				{
					if (!_registry.TryGetValue(taskId, out var task))
						return new() { Success = false, Message = "Task not found" };

					_service.ResumeTask(taskId);
					return new() { Success = true, Message = "Task resumed" };
				}
			}
			catch (Exception ex)
			{
				return new() { Success = false, Message = ex.Message };
			}
		}

		public OperationResult ExecuteNow(string taskId)
		{
			try
			{
				lock (_lock)
				{
					if (!_registry.TryGetValue(taskId, out var task))
						return new() { Success = false, Message = "Task not found" };

					if (!task.Enabled)
						return new() { Success = false, Message = "Task is disabled" };
				}

				_ = _service.ExecuteTaskImmediately(taskId);
				return new() { Success = true, Message = "Task execution triggered" };
			}
			catch (Exception ex)
			{
				return new() { Success = false, Message = ex.Message };
			}
		}

		public OperationResult ToggleTask(string taskId, bool enable)
		{
			try
			{
				lock (_lock)
				{
					if (!_registry.TryGetValue(taskId, out var task))
						return new() { Success = false, Message = "Task not found" };

					task.Enabled = enable;
					
					if (enable)
					{
						_service.RegisterTask(task);
					}
					else
					{
						_service.UnregisterTask(taskId);
						task.State = TaskState.Stopped;
						task.NextRunTime = null;
					}

					SaveTasksToSettings();
					return new() { Success = true, Message = enable ? "Task enabled and started" : "Task disabled and stopped" };
				}
			}
			catch (Exception ex)
			{
				return new() { Success = false, Message = ex.Message };
			}
		}

		public SchedulerStatistics GetStatistics()
		{
			lock (_lock)
			{
				foreach (var task in _registry.Values)
				{
					SyncTaskWithService(task);
				}

				var tasks = _registry.Values.ToList();
				return new()
				{
					TotalTasks = tasks.Count,
					EnabledTasks = tasks.Count(t => t.Enabled),
					DisabledTasks = tasks.Count(t => !t.Enabled),
					RunningTasks = tasks.Count(t => t.State == TaskState.Running),
					PausedTasks = tasks.Count(t => t.State == TaskState.Paused),
					FailedTasks = tasks.Count(t => t.State == TaskState.Failed),
					TotalExecutions = tasks.Sum(t => t.ExecutionCount),
					TotalFailures = tasks.Sum(t => t.FailureCount),
					LastExecutionTime = tasks.Max(t => t.LastRunTime),
					SchedulerRunning = true
				};
			}
		}

		public List<TaskExecutionHistory> GetHistory(string? taskId = null, int max = 100)
		{
			lock (_lock)
			{
				var query = _history.AsEnumerable();
				if (taskId != null)
					query = query.Where(h => h.TaskId == taskId);
				
				return query.OrderByDescending(h => h.StartTime).Take(max).ToList();
			}
		}

		public void RecordExecution(TaskExecutionHistory history)
		{
			lock (_lock)
			{
				_history.Add(history);
				if (_history.Count > MaxHistory)
					_history.RemoveRange(0, _history.Count - MaxHistory);
			}
		}

		private static bool IsValidMethodName(string methodFullName)
		{
			if (string.IsNullOrWhiteSpace(methodFullName))
				return false;

			var parts = methodFullName.Split('.');
			return parts.Length == 3 && parts.All(p => !string.IsNullOrWhiteSpace(p));
		}
	}
}
