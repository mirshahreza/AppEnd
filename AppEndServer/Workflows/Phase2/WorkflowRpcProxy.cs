using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AppEndCommon;

namespace AppEndServer.Workflows
{
    /// <summary>
    /// RPC Proxy for Workflow Management
    /// Exposes workflow operations through AppEnd's RpcNet framework
    /// Methods are callable from client applications via RPC
    /// </summary>
    public class WorkflowRpcProxy
    {
        private readonly IWorkflowService _workflowService;
        private readonly IWorkflowDefinitionService _definitionService;
        private readonly IWorkflowInstanceService _instanceService;
        private readonly ILogger<WorkflowRpcProxy> _logger;

        public WorkflowRpcProxy(
            IWorkflowService workflowService,
            IWorkflowDefinitionService definitionService,
            IWorkflowInstanceService instanceService,
            ILogger<WorkflowRpcProxy> logger)
        {
            _workflowService = workflowService;
            _definitionService = definitionService;
            _instanceService = instanceService;
            _logger = logger;
        }

        // ============================================================================
        // WORKFLOW EXECUTION OPERATIONS
        // ============================================================================

        /// <summary>
        /// Executes a workflow definition by ID with optional input parameters
        /// </summary>
        /// <param name="workflowDefinitionId">The workflow definition ID</param>
        /// <param name="input">Optional input parameters for the workflow</param>
        /// <param name="correlationId">Optional correlation ID to link related instances</param>
        /// <returns>The created workflow instance ID</returns>
        public async Task<string> ExecuteWorkflowAsync(
            string workflowDefinitionId,
            Dictionary<string, object>? input = null,
            string? correlationId = null)
        {
            try
            {
                _logger.LogInformation(
                    "RPC: Executing workflow {WorkflowId} with correlationId {CorrelationId}",
                    workflowDefinitionId, correlationId ?? "N/A");

                var instanceId = await _workflowService.ExecuteWorkflowAsync(
                    workflowDefinitionId,
                    input,
                    correlationId ?? Guid.NewGuid().ToString()
                );

                _logger.LogInformation(
                    "RPC: Workflow {WorkflowId} executed successfully. Instance: {InstanceId}",
                    workflowDefinitionId, instanceId);

                return instanceId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "RPC: Failed to execute workflow {WorkflowId}",
                    workflowDefinitionId);
                throw;
            }
        }

        /// <summary>
        /// Resumes a suspended workflow instance
        /// </summary>
        /// <param name="workflowInstanceId">The workflow instance ID to resume</param>
        /// <param name="input">Optional input/resumption data</param>
        public async Task ResumeWorkflowAsync(
            string workflowInstanceId,
            Dictionary<string, object>? input = null)
        {
            try
            {
                _logger.LogInformation(
                    "RPC: Resuming workflow instance {InstanceId}",
                    workflowInstanceId);

                await _workflowService.ResumeWorkflowAsync(workflowInstanceId, input);

                _logger.LogInformation(
                    "RPC: Workflow instance {InstanceId} resumed successfully",
                    workflowInstanceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "RPC: Failed to resume workflow instance {InstanceId}",
                    workflowInstanceId);
                throw;
            }
        }

        /// <summary>
        /// Suspends a running workflow instance
        /// </summary>
        /// <param name="workflowInstanceId">The workflow instance ID to suspend</param>
        /// <param name="reason">The reason for suspension</param>
        public async Task SuspendWorkflowAsync(string workflowInstanceId, string reason)
        {
            try
            {
                _logger.LogInformation(
                    "RPC: Suspending workflow instance {InstanceId} (reason: {Reason})",
                    workflowInstanceId, reason);

                await _workflowService.SuspendWorkflowAsync(workflowInstanceId, reason);

                _logger.LogInformation(
                    "RPC: Workflow instance {InstanceId} suspended successfully",
                    workflowInstanceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "RPC: Failed to suspend workflow instance {InstanceId}",
                    workflowInstanceId);
                throw;
            }
        }

        /// <summary>
        /// Cancels a running or suspended workflow instance
        /// </summary>
        /// <param name="workflowInstanceId">The workflow instance ID to cancel</param>
        public async Task CancelWorkflowAsync(string workflowInstanceId)
        {
            try
            {
                _logger.LogInformation(
                    "RPC: Cancelling workflow instance {InstanceId}",
                    workflowInstanceId);

                await _workflowService.CancelWorkflowAsync(workflowInstanceId);

                _logger.LogInformation(
                    "RPC: Workflow instance {InstanceId} cancelled successfully",
                    workflowInstanceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "RPC: Failed to cancel workflow instance {InstanceId}",
                    workflowInstanceId);
                throw;
            }
        }

        // ============================================================================
        // WORKFLOW DEFINITION OPERATIONS
        // ============================================================================

        /// <summary>
        /// Gets a workflow definition by ID
        /// </summary>
        /// <param name="workflowDefinitionId">The workflow definition ID</param>
        /// <returns>Workflow definition details</returns>
        public async Task<WorkflowDefinitionDto?> GetWorkflowDefinitionAsync(string workflowDefinitionId)
        {
            try
            {
                _logger.LogInformation(
                    "RPC: Getting workflow definition {WorkflowId}",
                    workflowDefinitionId);

                var definition = await _definitionService.GetByIdAsync(workflowDefinitionId);

                if (definition != null)
                {
                    _logger.LogInformation(
                        "RPC: Found workflow definition {WorkflowId}",
                        workflowDefinitionId);
                }
                else
                {
                    _logger.LogWarning(
                        "RPC: Workflow definition {WorkflowId} not found",
                        workflowDefinitionId);
                }

                return definition;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "RPC: Failed to get workflow definition {WorkflowId}",
                    workflowDefinitionId);
                throw;
            }
        }

        /// <summary>
        /// Gets a workflow definition by name
        /// </summary>
        /// <param name="workflowName">The workflow definition name</param>
        /// <returns>Workflow definition details</returns>
        public async Task<WorkflowDefinitionDto?> GetWorkflowDefinitionByNameAsync(string workflowName)
        {
            try
            {
                _logger.LogInformation(
                    "RPC: Getting workflow definition by name {WorkflowName}",
                    workflowName);

                var definition = await _definitionService.GetByNameAsync(workflowName);

                if (definition != null)
                {
                    _logger.LogInformation(
                        "RPC: Found workflow definition {WorkflowName}",
                        workflowName);
                }
                else
                {
                    _logger.LogWarning(
                        "RPC: Workflow definition {WorkflowName} not found",
                        workflowName);
                }

                return definition;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "RPC: Failed to get workflow definition by name {WorkflowName}",
                    workflowName);
                throw;
            }
        }

        /// <summary>
        /// Lists workflow definitions with pagination
        /// </summary>
        /// <param name="pageNumber">Page number (1-based)</param>
        /// <param name="pageSize">Number of results per page</param>
        /// <returns>Paginated list of workflow definitions</returns>
        public async Task<IEnumerable<WorkflowDefinitionDto>> ListWorkflowDefinitionsAsync(
            int pageNumber = 1,
            int pageSize = 20)
        {
            try
            {
                _logger.LogInformation(
                    "RPC: Listing workflow definitions (page {PageNumber}, size {PageSize})",
                    pageNumber, pageSize);

                var result = await _definitionService.ListAsync(tenantId: null);

                _logger.LogInformation(
                    "RPC: Found workflow definitions",
                    pageNumber);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "RPC: Failed to list workflow definitions");
                throw;
            }
        }

        /// <summary>
        /// Creates a new workflow definition
        /// </summary>
        /// <param name="request">Workflow definition creation request</param>
        /// <returns>The created workflow definition</returns>
        public async Task<WorkflowDefinitionDto> CreateWorkflowDefinitionAsync(
            CreateWorkflowDefinitionRequest request)
        {
            try
            {
                _logger.LogInformation(
                    "RPC: Creating workflow definition {WorkflowName}",
                    request.Name);

                var definition = await _definitionService.CreateAsync(request);

                _logger.LogInformation(
                    "RPC: Workflow definition {WorkflowName} created successfully (ID: {WorkflowId})",
                    request.Name, definition.Id);

                return definition;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "RPC: Failed to create workflow definition {WorkflowName}",
                    request.Name);
                throw;
            }
        }

        /// <summary>
        /// Updates an existing workflow definition
        /// </summary>
        /// <param name="workflowDefinitionId">The workflow definition ID</param>
        /// <param name="request">Workflow definition update request</param>
        /// <returns>The updated workflow definition</returns>
        public async Task<WorkflowDefinitionDto> UpdateWorkflowDefinitionAsync(
            string workflowDefinitionId,
            UpdateWorkflowDefinitionRequest request)
        {
            try
            {
                _logger.LogInformation(
                    "RPC: Updating workflow definition {WorkflowId}",
                    workflowDefinitionId);

                var definition = await _definitionService.UpdateAsync(workflowDefinitionId, request);

                _logger.LogInformation(
                    "RPC: Workflow definition {WorkflowId} updated successfully",
                    workflowDefinitionId);

                return definition;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "RPC: Failed to update workflow definition {WorkflowId}",
                    workflowDefinitionId);
                throw;
            }
        }

        /// <summary>
        /// Publishes a workflow definition (marks as ready for execution)
        /// </summary>
        /// <param name="workflowDefinitionId">The workflow definition ID</param>
        public async Task PublishWorkflowDefinitionAsync(string workflowDefinitionId)
        {
            try
            {
                _logger.LogInformation(
                    "RPC: Publishing workflow definition {WorkflowId}",
                    workflowDefinitionId);

                await _definitionService.PublishAsync(workflowDefinitionId);

                _logger.LogInformation(
                    "RPC: Workflow definition {WorkflowId} published successfully",
                    workflowDefinitionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "RPC: Failed to publish workflow definition {WorkflowId}",
                    workflowDefinitionId);
                throw;
            }
        }

        /// <summary>
        /// Deletes a workflow definition
        /// </summary>
        /// <param name="workflowDefinitionId">The workflow definition ID</param>
        public async Task DeleteWorkflowDefinitionAsync(string workflowDefinitionId)
        {
            try
            {
                _logger.LogInformation(
                    "RPC: Deleting workflow definition {WorkflowId}",
                    workflowDefinitionId);

                await _definitionService.DeleteAsync(workflowDefinitionId);

                _logger.LogInformation(
                    "RPC: Workflow definition {WorkflowId} deleted successfully",
                    workflowDefinitionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "RPC: Failed to delete workflow definition {WorkflowId}",
                    workflowDefinitionId);
                throw;
            }
        }

        // ============================================================================
        // WORKFLOW INSTANCE OPERATIONS
        // ============================================================================

        /// <summary>
        /// Gets a workflow instance by ID
        /// </summary>
        /// <param name="workflowInstanceId">The workflow instance ID</param>
        /// <returns>Workflow instance details</returns>
        public async Task<WorkflowInstanceDto?> GetWorkflowInstanceAsync(string workflowInstanceId)
        {
            try
            {
                _logger.LogInformation(
                    "RPC: Getting workflow instance {InstanceId}",
                    workflowInstanceId);

                var instance = await _instanceService.GetByIdAsync(workflowInstanceId);

                if (instance != null)
                {
                    _logger.LogInformation(
                        "RPC: Found workflow instance {InstanceId}",
                        workflowInstanceId);
                }
                else
                {
                    _logger.LogWarning(
                        "RPC: Workflow instance {InstanceId} not found",
                        workflowInstanceId);
                }

                return instance;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "RPC: Failed to get workflow instance {InstanceId}",
                    workflowInstanceId);
                throw;
            }
        }

        /// <summary>
        /// Gets workflow instances by correlation ID
        /// </summary>
        /// <param name="correlationId">The correlation ID to search for</param>
        /// <returns>Workflow instances matching the correlation ID</returns>
        public async Task<IEnumerable<WorkflowInstanceDto>> GetWorkflowInstancesByCorrelationIdAsync(
            string correlationId)
        {
            try
            {
                _logger.LogInformation(
                    "RPC: Getting workflow instances by correlation ID {CorrelationId}",
                    correlationId);

                var instances = await _instanceService.GetByCorrelationIdAsync(correlationId);

                _logger.LogInformation(
                    "RPC: Found {Count} workflow instances for correlation ID {CorrelationId}",
                    instances.Count(), correlationId);

                return instances;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "RPC: Failed to get workflow instances by correlation ID {CorrelationId}",
                    correlationId);
                throw;
            }
        }

        /// <summary>
        /// Lists workflow instances with filtering and pagination
        /// </summary>
        /// <param name="workflowDefinitionId">Optional: filter by workflow definition ID</param>
        /// <param name="status">Optional: filter by execution status</param>
        /// <param name="pageNumber">Page number (1-based)</param>
        /// <param name="pageSize">Number of results per page</param>
        /// <returns>Paginated list of workflow instances</returns>
        public async Task<PagedResult<WorkflowInstanceDto>> ListWorkflowInstancesAsync(
            string? workflowDefinitionId = null,
            string? status = null,
            int pageNumber = 1,
            int pageSize = 20)
        {
            try
            {
                _logger.LogInformation(
                    "RPC: Listing workflow instances (workflow {WorkflowId}, status {Status}, page {PageNumber})",
                    workflowDefinitionId ?? "Any", status ?? "Any", pageNumber);

                var filter = new WorkflowInstanceFilter
                {
                    WorkflowDefinitionId = workflowDefinitionId,
                    Status = status
                };

                var result = await _instanceService.ListAsync(filter, pageNumber, pageSize);

                _logger.LogInformation(
                    "RPC: Found {TotalCount} workflow instances (page {PageNumber})",
                    result.TotalCount, pageNumber);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "RPC: Failed to list workflow instances");
                throw;
            }
        }

        /// <summary>
        /// Gets execution history for a workflow instance
        /// </summary>
        /// <param name="workflowInstanceId">The workflow instance ID</param>
        /// <returns>List of execution events</returns>
        public async Task<IEnumerable<WorkflowInstanceEventDto>> GetWorkflowExecutionHistoryAsync(
            string workflowInstanceId)
        {
            try
            {
                _logger.LogInformation(
                    "RPC: Getting execution history for workflow instance {InstanceId}",
                    workflowInstanceId);

                var history = await _instanceService.GetExecutionHistoryAsync(workflowInstanceId);

                _logger.LogInformation(
                    "RPC: Found {Count} execution events for instance {InstanceId}",
                    history.Count(), workflowInstanceId);

                return history;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "RPC: Failed to get execution history for workflow instance {InstanceId}",
                    workflowInstanceId);
                throw;
            }
        }

        /// <summary>
        /// Gets activity executions for a workflow instance
        /// </summary>
        /// <param name="workflowInstanceId">The workflow instance ID</param>
        /// <returns>List of activity executions</returns>
        public async Task<IEnumerable<ActivityExecutionDto>> GetWorkflowActivityExecutionsAsync(
            string workflowInstanceId)
        {
            try
            {
                _logger.LogInformation(
                    "RPC: Getting activity executions for workflow instance {InstanceId}",
                    workflowInstanceId);

                var activities = await _instanceService.GetActivityExecutionsAsync(workflowInstanceId);

                _logger.LogInformation(
                    "RPC: Found {Count} activity executions for instance {InstanceId}",
                    activities.Count(), workflowInstanceId);

                return activities;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "RPC: Failed to get activity executions for workflow instance {InstanceId}",
                    workflowInstanceId);
                throw;
            }
        }
    }
}
