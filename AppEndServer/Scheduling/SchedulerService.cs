using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AppEndCommon;

namespace AppEndServer
{
	public class SchedulerService : BackgroundService
	{
		private readonly ILogger<SchedulerService> _logger;
		private readonly Dictionary<string, ScheduledTask> _tasks = [];
		private readonly object _lock = new();
		private Timer? _timer;

		public SchedulerService(ILogger<SchedulerService> logger)
		{
			_logger = logger;
		}

		public void RegisterTask(ScheduledTask task)
		{
			lock (_lock)
			{
				_tasks[task.TaskId] = task;
				task.State = TaskState.Running;
				CalculateNextRunTime(task);
				_logger.LogInformation($"Task registered: {task.Name} ({task.TaskId})");
			}
		}

		public void UnregisterTask(string taskId)
		{
			lock (_lock)
			{
				if (_tasks.TryGetValue(taskId, out var task))
				{
					task.State = TaskState.Stopped;
					task.NextRunTime = null;
					_tasks.Remove(taskId);
					_logger.LogInformation($"Task unregistered: {taskId}");
				}
			}
		}

		public void PauseTask(string taskId)
		{
			lock (_lock)
			{
				if (_tasks.TryGetValue(taskId, out var task))
				{
					task.State = TaskState.Paused;
					_logger.LogInformation($"Task paused: {task.Name}");
				}
			}
		}

		public void ResumeTask(string taskId)
		{
			lock (_lock)
			{
				if (_tasks.TryGetValue(taskId, out var task))
				{
					task.State = TaskState.Running;
					CalculateNextRunTime(task);
					_logger.LogInformation($"Task resumed: {task.Name}");
				}
			}
		}

		public ScheduledTask? GetTask(string taskId)
		{
			lock (_lock)
			{
				return _tasks.TryGetValue(taskId, out var task) ? task : null;
			}
		}

		public List<ScheduledTask> GetAllTasks()
		{
			lock (_lock)
			{
				return _tasks.Values.ToList();
			}
		}

		public bool IsTaskRegistered(string taskId)
		{
			lock (_lock)
			{
				return _tasks.ContainsKey(taskId);
			}
		}

		public async Task ExecuteTaskImmediately(string taskId)
		{
			ScheduledTask? task;
			lock (_lock)
			{
				_tasks.TryGetValue(taskId, out task);
			}

			if (task != null)
				await ExecuteTask(task, CancellationToken.None);
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("Scheduler service started");
			
			_timer = new Timer(
				async _ => await CheckAndExecuteTasks(stoppingToken),
				null,
				TimeSpan.Zero,
				TimeSpan.FromMinutes(1)
			);

			await Task.Delay(Timeout.Infinite, stoppingToken);
		}

		private async Task CheckAndExecuteTasks(CancellationToken cancellationToken)
		{
			var now = DateTime.UtcNow;
			List<ScheduledTask> tasksToExecute;

			lock (_lock)
			{
				tasksToExecute = _tasks.Values
					.Where(t => t.Enabled && 
							   t.State == TaskState.Running && 
							   t.NextRunTime.HasValue && 
							   t.NextRunTime.Value <= now)
					.ToList();
			}

			foreach (var task in tasksToExecute)
			{
				if (!cancellationToken.IsCancellationRequested)
					await ExecuteTask(task, cancellationToken);
			}
		}

		private async Task ExecuteTask(ScheduledTask task, CancellationToken cancellationToken)
		{
			var startTime = DateTime.UtcNow;
			var methodParts = ParseMethodFullName(task.MethodFullName);
			string? inputsJson = task.MethodParameters;
			string? responseStr = null;
			bool isSucceeded = false;
			
			try
			{
				_logger.LogInformation($"Executing: {task.Name}");
				task.LastRunTime = startTime;
				task.ExecutionCount++;

				var result = await Task.Run(() => 
					MethodExecutor.ExecuteMethod(task.MethodFullName, task.MethodParameters),
					cancellationToken
				);

				var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;
				_logger.LogInformation($"Success: {task.Name} ({duration:F0}ms)");
				
				task.LastError = null;
				isSucceeded = true;
				
				// Serialize result for logging
				if (result != null)
				{
					try
					{
						responseStr = System.Text.Json.JsonSerializer.Serialize(result);
						if (responseStr.Length > 4000) responseStr = responseStr.Substring(0, 3997) + "...";
					}
					catch { responseStr = result.ToString(); }
				}

				CalculateNextRunTime(task);
				NotifyExecution(task, startTime, DateTime.UtcNow, true, null, result);

				// Log activity
				LogMan.LogActivity(
					methodParts.namespaceName,
					methodParts.className,
					methodParts.methodName,
					task.TaskId,
					true,
					false,
					inputsJson,
					responseStr,
					(int)duration,
					"Scheduler",
					$"Task:{task.Name}",
					-1,
					"Scheduler"
				);
			}
			catch (Exception ex)
			{
				task.FailureCount++;
				task.LastError = ex.Message;
				_logger.LogError(ex, $"Failed: {task.Name}");

				var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;
				responseStr = ex.Message;
				if (responseStr.Length > 4000) responseStr = responseStr.Substring(0, 3997) + "...";

				if (task.FailureCount >= 5)
					task.State = TaskState.Failed;

				CalculateNextRunTime(task);
				NotifyExecution(task, startTime, DateTime.UtcNow, false, ex.Message, null);

				// Log activity for failed execution
				LogMan.LogActivity(
					methodParts.namespaceName,
					methodParts.className,
					methodParts.methodName,
					task.TaskId,
					false,
					false,
					inputsJson,
					responseStr,
					(int)duration,
					"Scheduler",
					$"Task:{task.Name}",
					-1,
					"Scheduler"
				);
			}
		}

		private static (string namespaceName, string className, string methodName) ParseMethodFullName(string methodFullName)
		{
			if (string.IsNullOrWhiteSpace(methodFullName))
				return ("", "", "");

			var parts = methodFullName.Split('.');
			if (parts.Length == 3)
				return (parts[0], parts[1], parts[2]);
			else if (parts.Length == 2)
				return ("", parts[0], parts[1]);
			else if (parts.Length == 1)
				return ("", "", parts[0]);
			else
				return ("", "", methodFullName);
		}

		private static void CalculateNextRunTime(ScheduledTask task)
		{
			try
			{
				var cron = new CronExpression(task.CronExpression);
				task.NextRunTime = cron.GetNextRunTime(DateTime.UtcNow);
			}
			catch
			{
				task.NextRunTime = null;
			}
		}

		private static void NotifyExecution(ScheduledTask task, DateTime start, DateTime end, bool success, string? error, object? result)
		{
			try
			{
				var manager = GetSchedulerManager();
				manager?.RecordExecution(new TaskExecutionHistory
				{
					TaskId = task.TaskId,
					TaskName = task.Name,
					StartTime = start,
					EndTime = end,
					IsSuccessful = success,
					DurationMs = (long)(end - start).TotalMilliseconds,
					ErrorMessage = error,
					Result = result?.ToString()
				});
			}
			catch { }
		}

		private static SchedulerManager? _managerInstance;
		public static void SetManager(SchedulerManager manager)
		{
			_managerInstance = manager;
		}

		private static SchedulerManager? GetSchedulerManager()
		{
			return _managerInstance;
		}

		public override void Dispose()
		{
			_timer?.Dispose();
			base.Dispose();
		}
	}
}
