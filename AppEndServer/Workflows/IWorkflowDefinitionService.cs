namespace AppEndServer.Workflows
{
    /// <summary>
    /// Service for managing workflow definitions.
    /// </summary>
    public interface IWorkflowDefinitionService
    {
        /// <summary>
        /// Gets a workflow definition by ID.
        /// </summary>
        Task<WorkflowDefinitionDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a workflow definition by name.
        /// </summary>
        Task<WorkflowDefinitionDto?> GetByNameAsync(string name, string? tenantId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Lists all workflow definitions.
        /// </summary>
        Task<IEnumerable<WorkflowDefinitionDto>> ListAsync(string? tenantId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new workflow definition.
        /// </summary>
        Task<WorkflowDefinitionDto> CreateAsync(CreateWorkflowDefinitionRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing workflow definition.
        /// </summary>
        Task<WorkflowDefinitionDto> UpdateAsync(string id, UpdateWorkflowDefinitionRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Publishes a workflow definition version.
        /// </summary>
        Task<bool> PublishAsync(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a workflow definition.
        /// </summary>
        Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Data transfer object for workflow definitions.
    /// </summary>
    public class WorkflowDefinitionDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        public int Version { get; set; }
        public int? PublishedVersion { get; set; }
        public bool IsPublished { get; set; }
        public bool IsPaused { get; set; }
        public string DefinitionFormat { get; set; } = "Json";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public string? TenantId { get; set; }
    }

    /// <summary>
    /// Request to create a new workflow definition.
    /// </summary>
    public class CreateWorkflowDefinitionRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        public string? DefinitionData { get; set; }
        public string DefinitionFormat { get; set; } = "Json";
        public string? TenantId { get; set; }
        public string? CreatedBy { get; set; }
    }

    /// <summary>
    /// Request to update an existing workflow definition.
    /// </summary>
    public class UpdateWorkflowDefinitionRequest
    {
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        public string? DefinitionData { get; set; }
        public bool? IsPaused { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
