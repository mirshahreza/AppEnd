using System;
using System.Collections.Generic;

namespace AppEndServer
{
	public class ScheduledTask
	{
		public string TaskId { get; set; } = Guid.NewGuid().ToString("N");
		public string Name { get; set; } = "";
		public string Description { get; set; } = "";
		public bool Enabled { get; set; } = true;
		public string CronExpression { get; set; } = "*/10 * * * *";
		public string MethodFullName { get; set; } = ""; // Must be Namespace.ClassName.MethodName
		public string? MethodParameters { get; set; } // JSON
		
		public DateTime? LastRunTime { get; set; }
		public DateTime? NextRunTime { get; set; }
		public int ExecutionCount { get; set; }
		public int FailureCount { get; set; }
		public string? LastError { get; set; }
		public TaskState State { get; set; } = TaskState.Stopped;
		
		public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
		public string? CreatedBy { get; set; }
	}

	public enum TaskState
	{
		Stopped = 0,
		Running = 1,
		Paused = 2,
		Failed = 3
	}

	public class TaskExecutionHistory
	{
		public string HistoryId { get; set; } = Guid.NewGuid().ToString("N");
		public string TaskId { get; set; } = "";
		public string TaskName { get; set; } = "";
		public DateTime StartTime { get; set; }
		public DateTime? EndTime { get; set; }
		public bool IsSuccessful { get; set; }
		public long DurationMs { get; set; }
		public string? ErrorMessage { get; set; }
		public string? Result { get; set; }
	}

	public class SchedulerStatistics
	{
		public int TotalTasks { get; set; }
		public int EnabledTasks { get; set; }
		public int DisabledTasks { get; set; }
		public int RunningTasks { get; set; }
		public int PausedTasks { get; set; }
		public int FailedTasks { get; set; }
		public int TotalExecutions { get; set; }
		public int TotalFailures { get; set; }
		public DateTime? LastExecutionTime { get; set; }
		public bool SchedulerRunning { get; set; }
	}

	public class SchedulableMethod
	{
		public string MethodFullName { get; set; } = ""; // Namespace.ClassName.MethodName
		public string Namespace { get; set; } = "";
		public string ClassName { get; set; } = "";
		public string MethodName { get; set; } = "";
		public string ReturnType { get; set; } = "";
		public List<MethodParameter> Parameters { get; set; } = [];
	}

	public class MethodParameter
	{
		public string Name { get; set; } = "";
		public string Type { get; set; } = "";
		public bool IsOptional { get; set; }
		public object? DefaultValue { get; set; }
	}

	public class OperationResult
	{
		public bool Success { get; set; }
		public string Message { get; set; } = "";
		public object? Data { get; set; }
	}
}
