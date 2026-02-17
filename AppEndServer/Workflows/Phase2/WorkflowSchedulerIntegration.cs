using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using AppEndCommon;

namespace AppEndServer.Workflows
{
    /// <summary>
    /// Integration between AppEnd Scheduler and Elsa Workflows
    /// Allows workflows to be triggered and executed via AppEnd's SchedulerService
    /// </summary>
    public class WorkflowSchedulerIntegration
    {
        private readonly SchedulerService _schedulerService;
        private readonly IWorkflowService _workflowService;
        private readonly ILogger<WorkflowSchedulerIntegration> _logger;
        private readonly Dictionary<string, string> _workflowTaskMapping = new();

        public WorkflowSchedulerIntegration(
            SchedulerService schedulerService,
            IWorkflowService workflowService,
            ILogger<WorkflowSchedulerIntegration> logger)
        {
            _schedulerService = schedulerService;
            _workflowService = workflowService;
            _logger = logger;
        }

        /// <summary>
        /// Registers a workflow as a scheduled task in AppEnd's scheduler.
        /// Creates a cron-based trigger that executes the workflow periodically.
        /// </summary>
        /// <param name="workflowDefinitionId">Elsa workflow definition ID</param>
        /// <param name="taskId">Unique identifier for the scheduled task</param>
        /// <param name="cronExpression">Cron expression for scheduling (e.g., "*/5 * * * *")</param>
        /// <param name="enabled">Whether the task should be enabled immediately</param>
        /// <param name="input">Optional input parameters for workflow execution</param>
        public async Task RegisterWorkflowAsScheduledTaskAsync(
            string workflowDefinitionId,
            string taskId,
            string cronExpression,
            bool enabled = true,
            Dictionary<string, object>? input = null)
        {
            try
            {
                _logger.LogInformation(
                    "Registering workflow {WorkflowId} as scheduled task {TaskId} with cron {CronExpression}",
                    workflowDefinitionId, taskId, cronExpression);

                // Create parameter dictionary and serialize to JSON
                var parameters = new Dictionary<string, object>
                {
                    { "workflowDefinitionId", workflowDefinitionId },
                    { "input", input ?? new Dictionary<string, object>() }
                };
                var methodParametersJson = JsonSerializer.Serialize(parameters);

                // Create a ScheduledTask for the workflow
                var scheduledTask = new ScheduledTask
                {
                    TaskId = taskId,
                    Name = $"Workflow: {workflowDefinitionId}",
                    Description = $"Scheduled execution of workflow {workflowDefinitionId}",
                    CronExpression = cronExpression,
                    Enabled = enabled,
                    MethodFullName = $"AppEndServer.Workflows.WorkflowScheduledExecutor.ExecuteWorkflowAsync",
                    MethodParameters = methodParametersJson,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = "WorkflowSchedulerIntegration"
                };

                // Register with AppEnd scheduler
                _schedulerService.RegisterTask(scheduledTask);

                // Keep mapping for reference
                _workflowTaskMapping[taskId] = workflowDefinitionId;

                _logger.LogInformation(
                    "Successfully registered workflow {WorkflowId} as scheduled task {TaskId}",
                    workflowDefinitionId, taskId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to register workflow {WorkflowId} as scheduled task {TaskId}",
                    workflowDefinitionId, taskId);
                throw;
            }
        }

        /// <summary>
        /// Unregisters a workflow scheduled task from AppEnd's scheduler.
        /// </summary>
        public async Task UnregisterWorkflowTaskAsync(string taskId)
        {
            try
            {
                _logger.LogInformation("Unregistering workflow scheduled task {TaskId}", taskId);

                _schedulerService.UnregisterTask(taskId);
                _workflowTaskMapping.Remove(taskId);

                _logger.LogInformation("Successfully unregistered workflow scheduled task {TaskId}", taskId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to unregister workflow scheduled task {TaskId}", taskId);
                throw;
            }
        }

        /// <summary>
        /// Pauses a workflow scheduled task.
        /// </summary>
        public async Task PauseWorkflowTaskAsync(string taskId)
        {
            try
            {
                _logger.LogInformation("Pausing workflow scheduled task {TaskId}", taskId);
                _schedulerService.PauseTask(taskId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to pause workflow scheduled task {TaskId}", taskId);
                throw;
            }
        }

        /// <summary>
        /// Resumes a workflow scheduled task.
        /// </summary>
        public async Task ResumeWorkflowTaskAsync(string taskId)
        {
            try
            {
                _logger.LogInformation("Resuming workflow scheduled task {TaskId}", taskId);
                _schedulerService.ResumeTask(taskId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to resume workflow scheduled task {TaskId}", taskId);
                throw;
            }
        }

        /// <summary>
        /// Gets the workflow definition ID for a scheduled task.
        /// </summary>
        public string? GetWorkflowDefinitionForTask(string taskId)
        {
            return _workflowTaskMapping.TryGetValue(taskId, out var workflowId) ? workflowId : null;
        }

        /// <summary>
        /// Lists all workflow scheduled tasks.
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> GetAllWorkflowTasks()
        {
            return _workflowTaskMapping.ToList();
        }
    }

    /// <summary>
    /// Static helper class for executing workflows from AppEnd scheduler.
    /// This is used by the ScheduledTask.MethodFullName to execute the workflow.
    /// </summary>
    public static class WorkflowScheduledExecutor
    {
        private static IWorkflowService? _workflowService;
        private static ILoggerFactory? _loggerFactory;

        /// <summary>
        /// Initialize the executor with DI services.
        /// Called once during application startup.
        /// </summary>
        public static void Initialize(IWorkflowService workflowService, ILoggerFactory loggerFactory)
        {
            _workflowService = workflowService;
            _loggerFactory = loggerFactory;
        }

        /// <summary>
        /// Executes a workflow based on scheduler invocation.
        /// This method is called by AppEnd's scheduler at the scheduled time.
        /// </summary>
        public static async Task ExecuteWorkflowAsync(string workflowDefinitionId, Dictionary<string, object>? input = null)
        {
            if (_workflowService == null || _loggerFactory == null)
            {
                throw new InvalidOperationException("WorkflowScheduledExecutor not initialized");
            }

            var logger = _loggerFactory.CreateLogger("WorkflowScheduledExecutor");

            try
            {
                logger.LogInformation(
                    "Scheduler executing workflow {WorkflowId}",
                    workflowDefinitionId);

                var instanceId = await _workflowService.ExecuteWorkflowAsync(
                    workflowDefinitionId,
                    input,
                    Guid.NewGuid().ToString()
                );

                logger.LogInformation(
                    "Workflow {WorkflowId} executed successfully. Instance: {InstanceId}",
                    workflowDefinitionId, instanceId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Failed to execute workflow {WorkflowId} from scheduler",
                    workflowDefinitionId);
                throw;
            }
        }
    }
}
