namespace AppEndServer.Workflows
{
    /// <summary>
    /// Service for managing workflow instances.
    /// </summary>
    public interface IWorkflowInstanceService
    {
        /// <summary>
        /// Gets a workflow instance by ID.
        /// </summary>
        Task<WorkflowInstanceDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets workflow instances by correlation ID.
        /// </summary>
        Task<IEnumerable<WorkflowInstanceDto>> GetByCorrelationIdAsync(string correlationId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Lists workflow instances with filtering.
        /// </summary>
        Task<PagedResult<WorkflowInstanceDto>> ListAsync(
            WorkflowInstanceFilter filter,
            int pageNumber = 1,
            int pageSize = 50,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets execution history of a workflow instance.
        /// </summary>
        Task<IEnumerable<WorkflowInstanceEventDto>> GetExecutionHistoryAsync(
            string workflowInstanceId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets activity executions for a workflow instance.
        /// </summary>
        Task<IEnumerable<ActivityExecutionDto>> GetActivityExecutionsAsync(
            string workflowInstanceId,
            CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Data transfer object for workflow instances.
    /// </summary>
    public class WorkflowInstanceDto
    {
        public string Id { get; set; } = string.Empty;
        public string WorkflowDefinitionId { get; set; } = string.Empty;
        public string? WorkflowDefinitionName { get; set; }
        public string? CorrelationId { get; set; }
        public string Status { get; set; } = "Running";
        public string? SubStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? FaultedAt { get; set; }
        public string? ExceptionMessage { get; set; }
        public Dictionary<string, object>? Variables { get; set; }
        public Dictionary<string, object>? Input { get; set; }
        public Dictionary<string, object>? Output { get; set; }
        public string? TenantId { get; set; }
        public string? UserId { get; set; }
    }

    /// <summary>
    /// Filter criteria for listing workflow instances.
    /// </summary>
    public class WorkflowInstanceFilter
    {
        public string? WorkflowDefinitionId { get; set; }
        public string? Status { get; set; }
        public string? CorrelationId { get; set; }
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }
        public string? TenantId { get; set; }
        public string? UserId { get; set; }
    }

    /// <summary>
    /// Data transfer object for workflow events.
    /// </summary>
    public class WorkflowInstanceEventDto
    {
        public string Id { get; set; } = string.Empty;
        public string WorkflowInstanceId { get; set; } = string.Empty;
        public string? ActivityExecutionId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Level { get; set; } = "Information";
        public DateTime Timestamp { get; set; }
        public Dictionary<string, object>? Data { get; set; }
    }

    /// <summary>
    /// Data transfer object for activity executions.
    /// </summary>
    public class ActivityExecutionDto
    {
        public string Id { get; set; } = string.Empty;
        public string WorkflowInstanceId { get; set; } = string.Empty;
        public string ActivityId { get; set; } = string.Empty;
        public string ActivityType { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int? Sequence { get; set; }
        public Dictionary<string, object>? Outputs { get; set; }
        public string? ExceptionMessage { get; set; }
    }

    /// <summary>
    /// Generic paged result wrapper.
    /// </summary>
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = [];
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
    }
}
