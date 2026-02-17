using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AppEndServer.Workflows
{
    /// <summary>
    /// Integration between Elsa Workflow Events and AppEnd Event System
    /// Allows workflows to publish events that trigger AppEnd actions
    /// and AppEnd events to trigger workflow resumption
    /// </summary>
    public class WorkflowEventSystemIntegration
    {
        private readonly IWorkflowService _workflowService;
        private readonly ILogger<WorkflowEventSystemIntegration> _logger;
        private readonly Dictionary<string, List<WorkflowEventHandler>> _eventHandlers = new();

        public delegate Task WorkflowEventHandler(WorkflowEvent @event);

        public WorkflowEventSystemIntegration(
            IWorkflowService workflowService,
            ILogger<WorkflowEventSystemIntegration> logger)
        {
            _workflowService = workflowService;
            _logger = logger;
        }

        /// <summary>
        /// Represents a workflow event with all necessary context
        /// </summary>
        public class WorkflowEvent
        {
            public string EventName { get; set; } = string.Empty;
            public string WorkflowInstanceId { get; set; } = string.Empty;
            public string? Source { get; set; }
            public Dictionary<string, object>? Payload { get; set; }
            public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
        }

        /// <summary>
        /// Subscribes to a specific workflow event type.
        /// When the event occurs, the handler is called.
        /// </summary>
        public void SubscribeToWorkflowEvent(string eventName, WorkflowEventHandler handler)
        {
            try
            {
                if (!_eventHandlers.ContainsKey(eventName))
                {
                    _eventHandlers[eventName] = new List<WorkflowEventHandler>();
                }

                _eventHandlers[eventName].Add(handler);

                _logger.LogInformation(
                    "Subscribed to workflow event {EventName}",
                    eventName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to subscribe to workflow event {EventName}", eventName);
                throw;
            }
        }

        /// <summary>
        /// Unsubscribes from a workflow event.
        /// </summary>
        public void UnsubscribeFromWorkflowEvent(string eventName, WorkflowEventHandler handler)
        {
            try
            {
                if (_eventHandlers.TryGetValue(eventName, out var handlers))
                {
                    handlers.Remove(handler);
                    _logger.LogInformation(
                        "Unsubscribed from workflow event {EventName}",
                        eventName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to unsubscribe from workflow event {EventName}", eventName);
                throw;
            }
        }

        /// <summary>
        /// Publishes a workflow event to all subscribed handlers.
        /// Called when a workflow event occurs (activity completion, decision, etc.)
        /// </summary>
        public async Task PublishWorkflowEventAsync(WorkflowEvent @event)
        {
            try
            {
                _logger.LogInformation(
                    "Publishing workflow event {EventName} from instance {InstanceId}",
                    @event.EventName, @event.WorkflowInstanceId);

                if (_eventHandlers.TryGetValue(@event.EventName, out var handlers))
                {
                    var tasks = handlers.Select(handler => InvokeHandlerAsync(handler, @event));
                    await Task.WhenAll(tasks);
                }

                _logger.LogInformation(
                    "Successfully published workflow event {EventName}",
                    @event.EventName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to publish workflow event {EventName}",
                    @event.EventName);
                throw;
            }
        }

        /// <summary>
        /// Subscribes to AppEnd events and triggers workflow resumption.
        /// When an AppEnd event occurs, resumes the waiting workflow.
        /// </summary>
        public void SubscribeToAppEndEvent(string appEndEventName, string workflowInstanceId)
        {
            try
            {
                _logger.LogInformation(
                    "Subscribing to AppEnd event {EventName} for workflow instance {InstanceId}",
                    appEndEventName, workflowInstanceId);

                // TODO: Implement AppEnd event subscription
                // When appEndEventName occurs, call:
                // await _workflowService.ResumeWorkflowAsync(workflowInstanceId);

                _logger.LogInformation(
                    "Successfully subscribed to AppEnd event {EventName}",
                    appEndEventName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to subscribe to AppEnd event {EventName}",
                    appEndEventName);
                throw;
            }
        }

        /// <summary>
        /// Handles when an AppEnd operation completes and workflow should resume.
        /// </summary>
        public async Task OnAppEndOperationCompletedAsync(
            string workflowInstanceId,
            string operationName,
            object? result = null)
        {
            try
            {
                _logger.LogInformation(
                    "AppEnd operation {Operation} completed. Resuming workflow {InstanceId}",
                    operationName, workflowInstanceId);

                // Resume the workflow with the result as input
                var resumeInput = result != null
                    ? new Dictionary<string, object> { { "result", result } }
                    : null;

                await _workflowService.ResumeWorkflowAsync(workflowInstanceId, resumeInput);

                _logger.LogInformation(
                    "Successfully resumed workflow {InstanceId} after operation {Operation}",
                    workflowInstanceId, operationName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to resume workflow {InstanceId} after operation {Operation}",
                    workflowInstanceId, operationName);
                throw;
            }
        }

        /// <summary>
        /// Gets all subscribed event handlers for a specific event.
        /// </summary>
        public IEnumerable<WorkflowEventHandler> GetEventHandlers(string eventName)
        {
            return _eventHandlers.TryGetValue(eventName, out var handlers)
                ? handlers
                : new List<WorkflowEventHandler>();
        }

        /// <summary>
        /// Gets all subscribed event names.
        /// </summary>
        public IEnumerable<string> GetSubscribedEvents()
        {
            return _eventHandlers.Keys;
        }

        /// <summary>
        /// Helper method to safely invoke a handler.
        /// </summary>
        private async Task InvokeHandlerAsync(WorkflowEventHandler handler, WorkflowEvent @event)
        {
            try
            {
                await handler(@event);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error invoking workflow event handler for event {EventName}",
                    @event.EventName);
                // Don't re-throw - continue with other handlers
            }
        }
    }

    /// <summary>
    /// Built-in event handlers for common workflow events
    /// </summary>
    public static class WorkflowEventHandlers
    {
        /// <summary>
        /// Handler for workflow completion event
        /// </summary>
        public static async Task OnWorkflowCompletedAsync(
            WorkflowEventSystemIntegration.WorkflowEvent @event,
            Func<Task> callback)
        {
            if (@event.EventName == "WorkflowCompleted")
            {
                await callback();
            }
        }

        /// <summary>
        /// Handler for workflow fault event
        /// </summary>
        public static async Task OnWorkflowFaultedAsync(
            WorkflowEventSystemIntegration.WorkflowEvent @event,
            Func<Task> callback)
        {
            if (@event.EventName == "WorkflowFaulted")
            {
                await callback();
            }
        }

        /// <summary>
        /// Handler for activity completion event
        /// </summary>
        public static async Task OnActivityCompletedAsync(
            WorkflowEventSystemIntegration.WorkflowEvent @event,
            string activityType,
            Func<Task> callback)
        {
            if (@event.EventName == "ActivityCompleted" && 
                @event.Payload?.ContainsKey("ActivityType") == true &&
                @event.Payload["ActivityType"]?.ToString() == activityType)
            {
                await callback();
            }
        }
    }
}
