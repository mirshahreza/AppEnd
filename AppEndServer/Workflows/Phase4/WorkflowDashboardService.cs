using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AppEndServer.Workflows
{
    /// <summary>
    /// Dashboard Service
    /// Provides data aggregation for workflow monitoring dashboard
    /// Retrieves summary statistics and real-time metrics
    /// </summary>
    public class WorkflowDashboardService : IWorkflowDashboardService
    {
        private readonly IWorkflowInstanceService _instanceService;
        private readonly IWorkflowDefinitionService _definitionService;
        private readonly ILogger<WorkflowDashboardService> _logger;

        public WorkflowDashboardService(
            IWorkflowInstanceService instanceService,
            IWorkflowDefinitionService definitionService,
            ILogger<WorkflowDashboardService> logger)
        {
            _instanceService = instanceService;
            _definitionService = definitionService;
            _logger = logger;
        }

        /// <summary>
        /// Gets overall dashboard summary with statistics
        /// </summary>
        public async Task<DashboardSummary> GetDashboardSummaryAsync()
        {
            try
            {
                _logger.LogInformation("Getting dashboard summary");

                // Aggregate statistics from instances
                var filter = new WorkflowInstanceFilter();
                var allInstancesResult = await _instanceService.ListAsync(filter, 1, 10000);
                var instances = allInstancesResult?.Items?.ToList() ?? new List<WorkflowInstanceDto>();

                // Calculate statistics from available data
                var totalInstances = instances.Count;
                var completedInstances = instances.Count(x => x.Status == "Completed");
                var runningInstances = instances.Count(x => x.Status == "Running");
                var suspendedInstances = instances.Count(x => x.Status == "Suspended");
                var failedInstances = instances.Count(x => x.Status == "Faulted");
                var cancelledInstances = instances.Count(x => x.Status == "Cancelled");

                var successRate = totalInstances > 0 
                    ? (completedInstances / (double)totalInstances) * 100.0 
                    : 0.0;

                // Calculate average execution time
                var avgExecutionTime = totalInstances > 0 
                    ? (int)instances.Average(x => (x.CompletedAt?.Subtract(x.CreatedAt) ?? TimeSpan.Zero).TotalSeconds)
                    : 0;

                // Get instances from last 24 hours
                var now = DateTime.UtcNow;
                var last24Hours = now.AddHours(-24);
                var instancesLast24 = instances.Count(x => x.CreatedAt >= last24Hours);
                var lastHour = now.AddHours(-1);
                var instancesLastHour = instances.Count(x => x.CreatedAt >= lastHour);

                var summary = new DashboardSummary
                {
                    TotalWorkflows = 0, // TODO: Count unique workflow definitions
                    TotalInstances = totalInstances,
                    CompletedInstances = completedInstances,
                    RunningInstances = runningInstances,
                    SuspendedInstances = suspendedInstances,
                    FailedInstances = failedInstances,
                    CancelledInstances = cancelledInstances,
                    SuccessRate = Math.Round(successRate, 2),
                    AverageExecutionTimeSeconds = avgExecutionTime,
                    InstancesLastHour = instancesLastHour,
                    InstancesLast24Hours = instancesLast24,
                    RecentErrors = new List<ErrorSummary>(), // TODO: Get from error tracking
                    TopWorkflows = new List<WorkflowSummary>(), // TODO: Get from workflow definitions
                    GeneratedAt = DateTime.UtcNow
                };

                _logger.LogInformation(
                    "Dashboard summary: Total={Total}, Completed={Completed}, Running={Running}, Failed={Failed}, SuccessRate={SuccessRate}%",
                    totalInstances, completedInstances, runningInstances, failedInstances, successRate);

                return summary;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get dashboard summary");
                throw;
            }
        }

        /// <summary>
        /// Gets instance statistics grouped by status
        /// </summary>
        public async Task<IEnumerable<StatusStatistic>> GetInstanceStatusStatisticsAsync()
        {
            try
            {
                _logger.LogInformation("Getting instance status statistics");

                var filter = new WorkflowInstanceFilter();
                var allInstancesResult = await _instanceService.ListAsync(filter, 1, 10000);
                var instances = allInstancesResult?.Items?.ToList() ?? new List<WorkflowInstanceDto>();

                var totalCount = instances.Count;
                var running = instances.Count(x => x.Status == "Running");
                var completed = instances.Count(x => x.Status == "Completed");
                var suspended = instances.Count(x => x.Status == "Suspended");
                var failed = instances.Count(x => x.Status == "Faulted");
                var cancelled = instances.Count(x => x.Status == "Cancelled");

                var stats = new List<StatusStatistic>
                {
                    new StatusStatistic 
                    { 
                        Status = "Running", 
                        Count = running,
                        Percentage = totalCount > 0 ? Math.Round((running / (double)totalCount) * 100, 2) : 0
                    },
                    new StatusStatistic 
                    { 
                        Status = "Completed", 
                        Count = completed,
                        Percentage = totalCount > 0 ? Math.Round((completed / (double)totalCount) * 100, 2) : 0
                    },
                    new StatusStatistic 
                    { 
                        Status = "Suspended", 
                        Count = suspended,
                        Percentage = totalCount > 0 ? Math.Round((suspended / (double)totalCount) * 100, 2) : 0
                    },
                    new StatusStatistic 
                    { 
                        Status = "Failed", 
                        Count = failed,
                        Percentage = totalCount > 0 ? Math.Round((failed / (double)totalCount) * 100, 2) : 0
                    },
                    new StatusStatistic 
                    { 
                        Status = "Cancelled", 
                        Count = cancelled,
                        Percentage = totalCount > 0 ? Math.Round((cancelled / (double)totalCount) * 100, 2) : 0
                    }
                };

                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get instance status statistics");
                throw;
            }
        }

        /// <summary>
        /// Gets execution timeline data for chart visualization
        /// </summary>
        public async Task<IEnumerable<ExecutionTimelinePoint>> GetExecutionTimelineAsync(
            int last24Hours = 24)
        {
            try
            {
                _logger.LogInformation(
                    "Getting execution timeline for last {Hours} hours",
                    last24Hours);

                var filter = new WorkflowInstanceFilter();
                var allInstancesResult = await _instanceService.ListAsync(filter, 1, 10000);
                var instances = allInstancesResult?.Items?.ToList() ?? new List<WorkflowInstanceDto>();

                var timeline = new List<ExecutionTimelinePoint>();

                for (int i = last24Hours - 1; i >= 0; i--)
                {
                    var hourStart = DateTime.UtcNow.AddHours(-i).Date.AddHours(DateTime.UtcNow.AddHours(-i).Hour);
                    var hourEnd = hourStart.AddHours(1);

                    var hourInstances = instances.Where(x => x.CreatedAt >= hourStart && x.CreatedAt < hourEnd).ToList();
                    var successCount = hourInstances.Count(x => x.Status == "Completed");
                    var failureCount = hourInstances.Count(x => x.Status == "Faulted" || x.Status == "Cancelled");

                    timeline.Add(new ExecutionTimelinePoint
                    {
                        Hour = hourStart.ToString("HH:00"),
                        Date = hourStart.Date,
                        ExecutionCount = hourInstances.Count,
                        SuccessCount = successCount,
                        FailureCount = failureCount
                    });
                }

                return timeline;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get execution timeline");
                throw;
            }
        }

        /// <summary>
        /// Gets performance metrics (execution times, activity metrics)
        /// </summary>
        public async Task<PerformanceMetrics> GetPerformanceMetricsAsync()
        {
            try
            {
                _logger.LogInformation("Getting performance metrics");

                var filter = new WorkflowInstanceFilter();
                var allInstancesResult = await _instanceService.ListAsync(filter, 1, 10000);
                var instances = allInstancesResult?.Items?.ToList() ?? new List<WorkflowInstanceDto>();

                var completedInstances = instances
                    .Where(x => x.Status == "Completed" && x.CompletedAt.HasValue)
                    .ToList();

                var executionTimes = completedInstances
                    .Select(x => (x.CompletedAt.Value - x.CreatedAt).TotalMilliseconds)
                    .OrderBy(x => x)
                    .ToList();

                var metrics = new PerformanceMetrics
                {
                    AverageExecutionTimeMs = executionTimes.Any() ? executionTimes.Average() : 0,
                    MinExecutionTimeMs = executionTimes.Any() ? executionTimes.Min() : 0,
                    MaxExecutionTimeMs = executionTimes.Any() ? executionTimes.Max() : 0,
                    MedianExecutionTimeMs = executionTimes.Any() 
                        ? executionTimes[executionTimes.Count / 2] 
                        : 0,
                    AverageActivityExecutionTimeMs = 0, // TODO: Get from activity executions table
                    SlowestActivities = new List<ActivityPerformance>(), // TODO: Get from database
                    FastestActivities = new List<ActivityPerformance>(), // TODO: Get from database
                    MostUsedActivities = new List<ActivityPerformance>(), // TODO: Get from database
                    CalculatedAt = DateTime.UtcNow
                };

                return metrics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get performance metrics");
                throw;
            }
        }

        /// <summary>
        /// Gets error summary with recent failures and error types
        /// </summary>
        public async Task<ErrorMetrics> GetErrorMetricsAsync(int topN = 10)
        {
            try
            {
                _logger.LogInformation("Getting error metrics");

                var filter = new WorkflowInstanceFilter();
                var allInstancesResult = await _instanceService.ListAsync(filter, 1, 10000);
                var instances = allInstancesResult?.Items?.ToList() ?? new List<WorkflowInstanceDto>();

                var failedInstances = instances.Where(x => x.Status == "Faulted").ToList();
                var totalFailures = failedInstances.Count;
                var totalInstances = instances.Count;
                var failureRate = totalInstances > 0 
                    ? (totalFailures / (double)totalInstances) * 100.0 
                    : 0.0;

                var recentErrors = failedInstances
                    .OrderByDescending(x => x.CreatedAt)
                    .Take(topN)
                    .Select(x => new ErrorDetail
                    {
                        InstanceId = x.Id,
                        WorkflowDefinitionId = x.WorkflowDefinitionId,
                        ActivityId = null, // TODO: Get from activity executions
                        ErrorMessage = $"Workflow execution failed", // TODO: Get actual error from faults table
                        StackTrace = null,
                        OccurredAt = x.CreatedAt
                    })
                    .ToList();

                var errorMetrics = new ErrorMetrics
                {
                    TotalFailures = totalFailures,
                    FailureRate = Math.Round(failureRate, 2),
                    TopErrorTypes = new List<ErrorTypeStatistic>(), // TODO: Get from error tracking
                    RecentErrors = recentErrors,
                    AffectedWorkflows = failedInstances
                        .Select(x => x.WorkflowDefinitionId)
                        .Distinct()
                        .ToList(),
                    LastErrorTime = failedInstances.Any() ? failedInstances.Max(x => x.CreatedAt) : null,
                    CalculatedAt = DateTime.UtcNow
                };

                _logger.LogInformation(
                    "Error metrics: Total failures={TotalFailures}, Failure rate={FailureRate}%",                
                totalFailures, failureRate);

                return errorMetrics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get error metrics");
                throw;
            }
        }

        /// <summary>
        /// Gets activity execution statistics
        /// </summary>
        public async Task<IEnumerable<ActivityStatistic>> GetActivityStatisticsAsync()
        {
            try
            {
                _logger.LogInformation("Getting activity statistics");

                // TODO: Aggregate activity execution data from ActivityExecutions table
                // - Total executions per activity type
                // - Success/failure rates
                // - Average execution times
                // - Most used activities

                var stats = new List<ActivityStatistic>
                {
                    // Sample data - to be replaced with real data from database
                    new ActivityStatistic
                    {
                        ActivityId = "DatabaseActivity",
                        ActivityType = "Database",
                        TotalExecutions = 0,
                        SuccessCount = 0,
                        FailureCount = 0,
                        SuccessRate = 0.0,
                        AverageExecutionTimeMs = 0
                    }
                };

                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get activity statistics");
                throw;
            }
        }

        /// <summary>
        /// Gets workflow usage statistics
        /// </summary>
        public async Task<IEnumerable<WorkflowStatistic>> GetWorkflowStatisticsAsync(
            int topN = 10)
        {
            try
            {
                _logger.LogInformation(
                    "Getting workflow statistics (top {TopN})",
                    topN);

                // Get all definitions
                var allDefinitions = await _definitionService.ListAsync(null, CancellationToken.None);
                var definitions = (allDefinitions as IEnumerable<WorkflowDefinitionDto>)?.ToList() ?? new List<WorkflowDefinitionDto>();

                // Get all instances
                var filter = new WorkflowInstanceFilter();
                var allInstancesResult = await _instanceService.ListAsync(filter, 1, 10000);
                var instances = allInstancesResult?.Items?.ToList() ?? new List<WorkflowInstanceDto>();

                var stats = new List<WorkflowStatistic>();

                foreach (var definition in definitions.Take(topN))
                {
                    var workflowInstances = instances
                        .Where(x => x.WorkflowDefinitionId == definition.Id)
                        .ToList();

                    var completedCount = workflowInstances.Count(x => x.Status == "Completed");
                    var failedCount = workflowInstances.Count(x => x.Status == "Faulted");
                    var totalCount = workflowInstances.Count;

                    var successRate = totalCount > 0 
                        ? (completedCount / (double)totalCount) * 100.0 
                        : 0.0;

                    var completedWorkflows = workflowInstances
                        .Where(x => x.Status == "Completed" && x.CompletedAt.HasValue)
                        .ToList();

                    var avgExecutionTime = completedWorkflows.Any()
                        ? completedWorkflows.Average(x => (x.CompletedAt.Value - x.CreatedAt).TotalSeconds)
                        : 0;

                    stats.Add(new WorkflowStatistic
                    {
                        WorkflowId = definition.Id,
                        WorkflowName = definition.Name,
                        TotalInstances = totalCount,
                        CompletedInstances = completedCount,
                        FailedInstances = failedCount,
                        SuccessRate = Math.Round(successRate, 2),
                        AverageExecutionTimeSeconds = avgExecutionTime,
                        LastExecutedAt = workflowInstances.Any() ? workflowInstances.Max(x => x.CreatedAt) : DateTime.UtcNow
                    });
                }

                return stats.OrderByDescending(x => x.TotalInstances).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get workflow statistics");
                throw;
            }
        }

        /// <summary>
        /// Gets custom dashboard metrics
        /// </summary>
        public async Task<CustomDashboardMetrics> GetCustomMetricsAsync(
            string metricType)
        {
            try
            {
                _logger.LogInformation(
                    "Getting custom metrics for type {MetricType}",
                    metricType);

                // TODO: Define available custom metrics by type
                // Examples:
                // - "approval-metrics" -> approval statistics
                // - "notification-metrics" -> notification delivery rates
                // - "activity-hotspots" -> most used activities
                // - "workflow-trends" -> execution trends

                var metrics = new CustomDashboardMetrics
                {
                    MetricType = metricType,
                    Data = new Dictionary<string, object>
                    {
                        { "type", metricType },
                        { "status", "not-implemented" },
                        { "message", $"Custom metrics for type '{metricType}' not yet implemented" }
                    },
                    CalculatedAt = DateTime.UtcNow
                };

                return metrics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get custom metrics");
                throw;
            }
        }
    }

    /// <summary>
    /// Interface for workflow dashboard service
    /// </summary>
    public interface IWorkflowDashboardService
    {
        Task<DashboardSummary> GetDashboardSummaryAsync();
        Task<IEnumerable<StatusStatistic>> GetInstanceStatusStatisticsAsync();
        Task<IEnumerable<ExecutionTimelinePoint>> GetExecutionTimelineAsync(int last24Hours = 24);
        Task<PerformanceMetrics> GetPerformanceMetricsAsync();
        Task<ErrorMetrics> GetErrorMetricsAsync(int topN = 10);
        Task<IEnumerable<ActivityStatistic>> GetActivityStatisticsAsync();
        Task<IEnumerable<WorkflowStatistic>> GetWorkflowStatisticsAsync(int topN = 10);
        Task<CustomDashboardMetrics> GetCustomMetricsAsync(string metricType);
    }

    // ============================================================================
    // DATA MODELS
    // ============================================================================

    /// <summary>
    /// Dashboard summary with overall statistics
    /// </summary>
    public class DashboardSummary
    {
        public int TotalWorkflows { get; set; }
        public int TotalInstances { get; set; }
        public int CompletedInstances { get; set; }
        public int RunningInstances { get; set; }
        public int SuspendedInstances { get; set; }
        public int FailedInstances { get; set; }
        public int CancelledInstances { get; set; }
        public double SuccessRate { get; set; }
        public int AverageExecutionTimeSeconds { get; set; }
        public int InstancesLastHour { get; set; }
        public int InstancesLast24Hours { get; set; }
        public List<ErrorSummary> RecentErrors { get; set; } = new();
        public List<WorkflowSummary> TopWorkflows { get; set; } = new();
        public DateTime GeneratedAt { get; set; }
    }

    /// <summary>
    /// Status statistic model
    /// </summary>
    public class StatusStatistic
    {
        public string? Status { get; set; }
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    /// <summary>
    /// Execution timeline point for charts
    /// </summary>
    public class ExecutionTimelinePoint
    {
        public string? Hour { get; set; }
        public DateTime Date { get; set; }
        public int ExecutionCount { get; set; }
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
    }

    /// <summary>
    /// Performance metrics
    /// </summary>
    public class PerformanceMetrics
    {
        public double AverageExecutionTimeMs { get; set; }
        public double MinExecutionTimeMs { get; set; }
        public double MaxExecutionTimeMs { get; set; }
        public double MedianExecutionTimeMs { get; set; }
        public double AverageActivityExecutionTimeMs { get; set; }
        public List<ActivityPerformance> SlowestActivities { get; set; } = new();
        public List<ActivityPerformance> FastestActivities { get; set; } = new();
        public List<ActivityPerformance> MostUsedActivities { get; set; } = new();
        public DateTime CalculatedAt { get; set; }
    }

    /// <summary>
    /// Activity performance data
    /// </summary>
    public class ActivityPerformance
    {
        public string? ActivityId { get; set; }
        public string? ActivityType { get; set; }
        public int ExecutionCount { get; set; }
        public double AverageTimeMs { get; set; }
        public double SuccessRate { get; set; }
    }

    /// <summary>
    /// Error metrics
    /// </summary>
    public class ErrorMetrics
    {
        public int TotalFailures { get; set; }
        public double FailureRate { get; set; }
        public List<ErrorTypeStatistic> TopErrorTypes { get; set; } = new();
        public List<ErrorDetail> RecentErrors { get; set; } = new();
        public List<string> AffectedWorkflows { get; set; } = new();
        public DateTime? LastErrorTime { get; set; }
        public DateTime CalculatedAt { get; set; }
    }

    /// <summary>
    /// Error type statistic
    /// </summary>
    public class ErrorTypeStatistic
    {
        public string? ErrorType { get; set; }
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    /// <summary>
    /// Error detail
    /// </summary>
    public class ErrorDetail
    {
        public string? InstanceId { get; set; }
        public string? WorkflowDefinitionId { get; set; }
        public string? ActivityId { get; set; }
        public string? ErrorMessage { get; set; }
        public string? StackTrace { get; set; }
        public DateTime OccurredAt { get; set; }
    }

    /// <summary>
    /// Activity statistic
    /// </summary>
    public class ActivityStatistic
    {
        public string? ActivityId { get; set; }
        public string? ActivityType { get; set; }
        public int TotalExecutions { get; set; }
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public double SuccessRate { get; set; }
        public double AverageExecutionTimeMs { get; set; }
    }

    /// <summary>
    /// Workflow statistic
    /// </summary>
    public class WorkflowStatistic
    {
        public string? WorkflowId { get; set; }
        public string? WorkflowName { get; set; }
        public int TotalInstances { get; set; }
        public int CompletedInstances { get; set; }
        public int FailedInstances { get; set; }
        public double SuccessRate { get; set; }
        public double AverageExecutionTimeSeconds { get; set; }
        public DateTime LastExecutedAt { get; set; }
    }

    /// <summary>
    /// Error summary
    /// </summary>
    public class ErrorSummary
    {
        public string? InstanceId { get; set; }
        public string? WorkflowName { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime OccurredAt { get; set; }
    }

    /// <summary>
    /// Workflow summary
    /// </summary>
    public class WorkflowSummary
    {
        public string? WorkflowId { get; set; }
        public string? WorkflowName { get; set; }
        public int InstanceCount { get; set; }
    }

    /// <summary>
    /// Custom dashboard metrics
    /// </summary>
    public class CustomDashboardMetrics
    {
        public string? MetricType { get; set; }
        public Dictionary<string, object> Data { get; set; } = new();
        public DateTime CalculatedAt { get; set; }
    }
}
