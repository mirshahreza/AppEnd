using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AppEndServer.Workflows
{
    /// <summary>
    /// Workflow Management API Controller
    /// Provides REST endpoints for workflow operations
    /// Integrates with Phase 2 RPC endpoints and Phase 3 activities
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class WorkflowsController : ControllerBase
    {
        private readonly IWorkflowService _workflowService;
        private readonly WorkflowRpcProxy _rpcProxy;
        private readonly ILogger<WorkflowsController> _logger;

        public WorkflowsController(
            IWorkflowService workflowService,
            WorkflowRpcProxy rpcProxy,
            ILogger<WorkflowsController> logger)
        {
            _workflowService = workflowService;
            _rpcProxy = rpcProxy;
            _logger = logger;
        }

        // ============================================================================
        // WORKFLOW EXECUTION ENDPOINTS
        // ============================================================================

        /// <summary>
        /// Executes a workflow definition and creates a new instance
        /// </summary>
        /// <param name="workflowDefinitionId">Workflow definition ID</param>
        /// <param name="request">Execution request with input parameters</param>
        /// <returns>Created workflow instance ID</returns>
        [HttpPost("execute/{workflowDefinitionId}")]
        [ProducesResponseType(typeof(WorkflowExecutionResponse), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ExecuteWorkflowAsync(
            string workflowDefinitionId,
            [FromBody] WorkflowExecutionRequest request)
        {
            try
            {
                _logger.LogInformation(
                    "Executing workflow {WorkflowId} with correlationId {CorrelationId}",
                    workflowDefinitionId, request.CorrelationId ?? "auto-generated");

                var instanceId = await _rpcProxy.ExecuteWorkflowAsync(
                    workflowDefinitionId,
                    request.Input,
                    request.CorrelationId ?? Guid.NewGuid().ToString()
                );

                return Ok(new WorkflowExecutionResponse
                {
                    Success = true,
                    InstanceId = instanceId,
                    Message = $"Workflow {workflowDefinitionId} executed successfully",
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to execute workflow {WorkflowId}",
                    workflowDefinitionId);

                return StatusCode(500, new ErrorResponse
                {
                    Success = false,
                    Error = "Workflow execution failed",
                    Message = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Resumes a suspended workflow instance
        /// </summary>
        /// <param name="workflowInstanceId">Workflow instance ID</param>
        /// <param name="request">Resumption request with optional input</param>
        /// <returns>Success status</returns>
        [HttpPost("resume/{workflowInstanceId}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ResumeWorkflowAsync(
            string workflowInstanceId,
            [FromBody] WorkflowResumptionRequest? request = null)
        {
            try
            {
                _logger.LogInformation(
                    "Resuming workflow instance {InstanceId}",
                    workflowInstanceId);

                await _rpcProxy.ResumeWorkflowAsync(
                    workflowInstanceId,
                    request?.Input
                );

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = $"Workflow instance {workflowInstanceId} resumed successfully",
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to resume workflow instance {InstanceId}",
                    workflowInstanceId);

                return StatusCode(500, new ErrorResponse
                {
                    Success = false,
                    Error = "Workflow resumption failed",
                    Message = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Suspends a running workflow instance
        /// </summary>
        /// <param name="workflowInstanceId">Workflow instance ID</param>
        /// <param name="request">Suspension request with reason</param>
        /// <returns>Success status</returns>
        [HttpPost("suspend/{workflowInstanceId}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> SuspendWorkflowAsync(
            string workflowInstanceId,
            [FromBody] WorkflowSuspensionRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request?.Reason))
                {
                    return BadRequest(new ErrorResponse
                    {
                        Success = false,
                        Error = "Validation failed",
                        Message = "Suspension reason is required",
                        Timestamp = DateTime.UtcNow
                    });
                }

                _logger.LogInformation(
                    "Suspending workflow instance {InstanceId} (reason: {Reason})",
                    workflowInstanceId, request.Reason);

                await _rpcProxy.SuspendWorkflowAsync(
                    workflowInstanceId,
                    request.Reason
                );

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = $"Workflow instance {workflowInstanceId} suspended successfully",
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to suspend workflow instance {InstanceId}",
                    workflowInstanceId);

                return StatusCode(500, new ErrorResponse
                {
                    Success = false,
                    Error = "Workflow suspension failed",
                    Message = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Cancels a workflow instance
        /// </summary>
        /// <param name="workflowInstanceId">Workflow instance ID</param>
        /// <returns>Success status</returns>
        [HttpPost("cancel/{workflowInstanceId}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> CancelWorkflowAsync(string workflowInstanceId)
        {
            try
            {
                _logger.LogInformation(
                    "Cancelling workflow instance {InstanceId}",
                    workflowInstanceId);

                await _rpcProxy.CancelWorkflowAsync(workflowInstanceId);

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = $"Workflow instance {workflowInstanceId} cancelled successfully",
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to cancel workflow instance {InstanceId}",
                    workflowInstanceId);

                return StatusCode(500, new ErrorResponse
                {
                    Success = false,
                    Error = "Workflow cancellation failed",
                    Message = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        // ============================================================================
        // WORKFLOW DEFINITION ENDPOINTS
        // ============================================================================

        /// <summary>
        /// Gets a workflow definition by ID
        /// </summary>
        /// <param name="workflowDefinitionId">Workflow definition ID</param>
        /// <returns>Workflow definition details</returns>
        [HttpGet("definitions/{workflowDefinitionId}")]
        [ProducesResponseType(typeof(WorkflowDefinitionDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetWorkflowDefinitionAsync(string workflowDefinitionId)
        {
            try
            {
                _logger.LogInformation(
                    "Getting workflow definition {WorkflowId}",
                    workflowDefinitionId);

                var definition = await _rpcProxy.GetWorkflowDefinitionAsync(workflowDefinitionId);

                if (definition == null)
                {
                    return NotFound(new ErrorResponse
                    {
                        Success = false,
                        Error = "Not found",
                        Message = $"Workflow definition {workflowDefinitionId} not found",
                        Timestamp = DateTime.UtcNow
                    });
                }

                return Ok(definition);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to get workflow definition {WorkflowId}",
                    workflowDefinitionId);

                return StatusCode(500, new ErrorResponse
                {
                    Success = false,
                    Error = "Failed to retrieve workflow definition",
                    Message = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Gets a workflow definition by name
        /// </summary>
        /// <param name="workflowName">Workflow definition name</param>
        /// <returns>Workflow definition details</returns>
        [HttpGet("definitions/by-name/{workflowName}")]
        [ProducesResponseType(typeof(WorkflowDefinitionDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetWorkflowDefinitionByNameAsync(string workflowName)
        {
            try
            {
                _logger.LogInformation(
                    "Getting workflow definition by name {WorkflowName}",
                    workflowName);

                var definition = await _rpcProxy.GetWorkflowDefinitionByNameAsync(workflowName);

                if (definition == null)
                {
                    return NotFound(new ErrorResponse
                    {
                        Success = false,
                        Error = "Not found",
                        Message = $"Workflow definition {workflowName} not found",
                        Timestamp = DateTime.UtcNow
                    });
                }

                return Ok(definition);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to get workflow definition by name {WorkflowName}",
                    workflowName);

                return StatusCode(500, new ErrorResponse
                {
                    Success = false,
                    Error = "Failed to retrieve workflow definition",
                    Message = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Lists all workflow definitions with pagination
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 20)</param>
        /// <returns>Paginated list of workflow definitions</returns>
        [HttpGet("definitions")]
        [ProducesResponseType(typeof(IEnumerable<WorkflowDefinitionDto>), 200)]
        public async Task<IActionResult> ListWorkflowDefinitionsAsync(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                _logger.LogInformation(
                    "Listing workflow definitions (page {PageNumber}, size {PageSize})",
                    pageNumber, pageSize);

                var definitions = await _rpcProxy.ListWorkflowDefinitionsAsync(pageNumber, pageSize);

                return Ok(definitions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to list workflow definitions");

                return StatusCode(500, new ErrorResponse
                {
                    Success = false,
                    Error = "Failed to retrieve workflow definitions",
                    Message = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Creates a new workflow definition
        /// </summary>
        /// <param name="request">Creation request with definition details</param>
        /// <returns>Created workflow definition</returns>
        [HttpPost("definitions")]
        [ProducesResponseType(typeof(WorkflowDefinitionDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateWorkflowDefinitionAsync(
            [FromBody] CreateWorkflowDefinitionRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request?.Name))
                {
                    return BadRequest(new ErrorResponse
                    {
                        Success = false,
                        Error = "Validation failed",
                        Message = "Workflow name is required",
                        Timestamp = DateTime.UtcNow
                    });
                }

                _logger.LogInformation(
                    "Creating workflow definition {WorkflowName}",
                    request.Name);

                var definition = await _rpcProxy.CreateWorkflowDefinitionAsync(request);

                return CreatedAtAction(
                    nameof(GetWorkflowDefinitionAsync),
                    new { workflowDefinitionId = definition.Id },
                    definition
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to create workflow definition");

                return StatusCode(500, new ErrorResponse
                {
                    Success = false,
                    Error = "Failed to create workflow definition",
                    Message = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Updates a workflow definition
        /// </summary>
        /// <param name="workflowDefinitionId">Workflow definition ID</param>
        /// <param name="request">Update request with new details</param>
        /// <returns>Updated workflow definition</returns>
        [HttpPut("definitions/{workflowDefinitionId}")]
        [ProducesResponseType(typeof(WorkflowDefinitionDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateWorkflowDefinitionAsync(
            string workflowDefinitionId,
            [FromBody] UpdateWorkflowDefinitionRequest request)
        {
            try
            {
                _logger.LogInformation(
                    "Updating workflow definition {WorkflowId}",
                    workflowDefinitionId);

                var definition = await _rpcProxy.UpdateWorkflowDefinitionAsync(
                    workflowDefinitionId,
                    request
                );

                return Ok(definition);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to update workflow definition {WorkflowId}",
                    workflowDefinitionId);

                return StatusCode(500, new ErrorResponse
                {
                    Success = false,
                    Error = "Failed to update workflow definition",
                    Message = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Publishes a workflow definition
        /// </summary>
        /// <param name="workflowDefinitionId">Workflow definition ID</param>
        /// <returns>Success status</returns>
        [HttpPost("definitions/{workflowDefinitionId}/publish")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PublishWorkflowDefinitionAsync(string workflowDefinitionId)
        {
            try
            {
                _logger.LogInformation(
                    "Publishing workflow definition {WorkflowId}",
                    workflowDefinitionId);

                await _rpcProxy.PublishWorkflowDefinitionAsync(workflowDefinitionId);

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = $"Workflow definition {workflowDefinitionId} published successfully",
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to publish workflow definition {WorkflowId}",
                    workflowDefinitionId);

                return StatusCode(500, new ErrorResponse
                {
                    Success = false,
                    Error = "Failed to publish workflow definition",
                    Message = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Deletes a workflow definition
        /// </summary>
        /// <param name="workflowDefinitionId">Workflow definition ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("definitions/{workflowDefinitionId}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteWorkflowDefinitionAsync(string workflowDefinitionId)
        {
            try
            {
                _logger.LogInformation(
                    "Deleting workflow definition {WorkflowId}",
                    workflowDefinitionId);

                await _rpcProxy.DeleteWorkflowDefinitionAsync(workflowDefinitionId);

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = $"Workflow definition {workflowDefinitionId} deleted successfully",
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to delete workflow definition {WorkflowId}",
                    workflowDefinitionId);

                return StatusCode(500, new ErrorResponse
                {
                    Success = false,
                    Error = "Failed to delete workflow definition",
                    Message = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        // ============================================================================
        // WORKFLOW INSTANCE ENDPOINTS
        // ============================================================================

        /// <summary>
        /// Gets a workflow instance by ID
        /// </summary>
        /// <param name="workflowInstanceId">Workflow instance ID</param>
        /// <returns>Workflow instance details</returns>
        [HttpGet("instances/{workflowInstanceId}")]
        [ProducesResponseType(typeof(WorkflowInstanceDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetWorkflowInstanceAsync(string workflowInstanceId)
        {
            try
            {
                _logger.LogInformation(
                    "Getting workflow instance {InstanceId}",
                    workflowInstanceId);

                var instance = await _rpcProxy.GetWorkflowInstanceAsync(workflowInstanceId);

                if (instance == null)
                {
                    return NotFound(new ErrorResponse
                    {
                        Success = false,
                        Error = "Not found",
                        Message = $"Workflow instance {workflowInstanceId} not found",
                        Timestamp = DateTime.UtcNow
                    });
                }

                return Ok(instance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to get workflow instance {InstanceId}",
                    workflowInstanceId);

                return StatusCode(500, new ErrorResponse
                {
                    Success = false,
                    Error = "Failed to retrieve workflow instance",
                    Message = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Lists workflow instances by correlation ID
        /// </summary>
        /// <param name="correlationId">Correlation ID to search for</param>
        /// <returns>List of workflow instances with matching correlation ID</returns>
        [HttpGet("instances/by-correlation/{correlationId}")]
        [ProducesResponseType(typeof(IEnumerable<WorkflowInstanceDto>), 200)]
        public async Task<IActionResult> GetWorkflowInstancesByCorrelationIdAsync(string correlationId)
        {
            try
            {
                _logger.LogInformation(
                    "Getting workflow instances by correlation ID {CorrelationId}",
                    correlationId);

                var instances = await _rpcProxy.GetWorkflowInstancesByCorrelationIdAsync(correlationId);

                return Ok(instances);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to get workflow instances by correlation ID {CorrelationId}",
                    correlationId);

                return StatusCode(500, new ErrorResponse
                {
                    Success = false,
                    Error = "Failed to retrieve workflow instances",
                    Message = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Lists workflow instances with filtering and pagination
        /// </summary>
        /// <param name="workflowDefinitionId">Optional: Filter by workflow definition ID</param>
        /// <param name="status">Optional: Filter by status</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 20)</param>
        /// <returns>Paginated list of workflow instances</returns>
        [HttpGet("instances")]
        [ProducesResponseType(typeof(PagedResult<WorkflowInstanceDto>), 200)]
        public async Task<IActionResult> ListWorkflowInstancesAsync(
            [FromQuery] string? workflowDefinitionId = null,
            [FromQuery] string? status = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                _logger.LogInformation(
                    "Listing workflow instances (workflow {WorkflowId}, status {Status}, page {PageNumber})",
                    workflowDefinitionId ?? "Any", status ?? "Any", pageNumber);

                var instances = await _rpcProxy.ListWorkflowInstancesAsync(
                    workflowDefinitionId,
                    status,
                    pageNumber,
                    pageSize
                );

                return Ok(instances);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to list workflow instances");

                return StatusCode(500, new ErrorResponse
                {
                    Success = false,
                    Error = "Failed to retrieve workflow instances",
                    Message = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Gets execution history for a workflow instance
        /// </summary>
        /// <param name="workflowInstanceId">Workflow instance ID</param>
        /// <returns>List of execution events</returns>
        [HttpGet("instances/{workflowInstanceId}/history")]
        [ProducesResponseType(typeof(IEnumerable<WorkflowInstanceEventDto>), 200)]
        public async Task<IActionResult> GetWorkflowExecutionHistoryAsync(string workflowInstanceId)
        {
            try
            {
                _logger.LogInformation(
                    "Getting execution history for workflow instance {InstanceId}",
                    workflowInstanceId);

                var history = await _rpcProxy.GetWorkflowExecutionHistoryAsync(workflowInstanceId);

                return Ok(history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to get execution history for workflow instance {InstanceId}",
                    workflowInstanceId);

                return StatusCode(500, new ErrorResponse
                {
                    Success = false,
                    Error = "Failed to retrieve execution history",
                    Message = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Gets activity executions for a workflow instance
        /// </summary>
        /// <param name="workflowInstanceId">Workflow instance ID</param>
        /// <returns>List of activity executions</returns>
        [HttpGet("instances/{workflowInstanceId}/activities")]
        [ProducesResponseType(typeof(IEnumerable<ActivityExecutionDto>), 200)]
        public async Task<IActionResult> GetWorkflowActivityExecutionsAsync(string workflowInstanceId)
        {
            try
            {
                _logger.LogInformation(
                    "Getting activity executions for workflow instance {InstanceId}",
                    workflowInstanceId);

                var activities = await _rpcProxy.GetWorkflowActivityExecutionsAsync(workflowInstanceId);

                return Ok(activities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to get activity executions for workflow instance {InstanceId}",
                    workflowInstanceId);

                return StatusCode(500, new ErrorResponse
                {
                    Success = false,
                    Error = "Failed to retrieve activity executions",
                    Message = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        // ============================================================================
        // HEALTH & STATUS ENDPOINTS
        // ============================================================================

        /// <summary>
        /// Gets workflow engine health status
        /// </summary>
        /// <returns>Health status information</returns>
        [HttpGet("health")]
        [ProducesResponseType(typeof(HealthStatusResponse), 200)]
        public IActionResult GetHealthStatus()
        {
            return Ok(new HealthStatusResponse
            {
                Status = "Healthy",
                Service = "Workflow Engine",
                Version = "3.0.0",
                Timestamp = DateTime.UtcNow,
                DatabaseConnection = "Connected",
                Message = "Workflow engine is operational"
            });
        }
    }

    // ============================================================================
    // REQUEST/RESPONSE MODELS
    // ============================================================================

    /// <summary>
    /// Workflow execution request model
    /// </summary>
    public class WorkflowExecutionRequest
    {
        public Dictionary<string, object>? Input { get; set; }
        public string? CorrelationId { get; set; }
    }

    /// <summary>
    /// Workflow resumption request model
    /// </summary>
    public class WorkflowResumptionRequest
    {
        public Dictionary<string, object>? Input { get; set; }
    }

    /// <summary>
    /// Workflow suspension request model
    /// </summary>
    public class WorkflowSuspensionRequest
    {
        public string? Reason { get; set; }
    }

    /// <summary>
    /// Generic API response model
    /// </summary>
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// Error response model
    /// </summary>
    public class ErrorResponse : ApiResponse
    {
        public string? Error { get; set; }
    }

    /// <summary>
    /// Workflow execution response model
    /// </summary>
    public class WorkflowExecutionResponse : ApiResponse
    {
        public string? InstanceId { get; set; }
    }

    /// <summary>
    /// Health status response model
    /// </summary>
    public class HealthStatusResponse
    {
        public string? Status { get; set; }
        public string? Service { get; set; }
        public string? Version { get; set; }
        public DateTime Timestamp { get; set; }
        public string? DatabaseConnection { get; set; }
        public string? Message { get; set; }
    }
}
