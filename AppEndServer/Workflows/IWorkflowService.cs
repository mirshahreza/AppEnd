namespace AppEndServer.Workflows
{
    /// <summary>
    /// Main service interface for workflow management.
    /// Provides abstraction between AppEnd and Elsa engine.
    /// </summary>
    public interface IWorkflowService
    {
        /// <summary>
        /// Gets a workflow definition service.
        /// </summary>
        IWorkflowDefinitionService Definitions { get; }

        /// <summary>
        /// Gets a workflow instance service.
        /// </summary>
        IWorkflowInstanceService Instances { get; }

        /// <summary>
        /// Executes a workflow by definition ID and returns the instance ID.
        /// </summary>
        Task<string> ExecuteWorkflowAsync(
            string workflowDefinitionId,
            Dictionary<string, object>? input = null,
            string? correlationId = null,
            string? tenantId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Resumes a suspended workflow instance.
        /// </summary>
        Task<bool> ResumeWorkflowAsync(
            string workflowInstanceId,
            Dictionary<string, object>? input = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Suspends a running workflow instance.
        /// </summary>
        Task<bool> SuspendWorkflowAsync(
            string workflowInstanceId,
            string reason,
            string? suspendedBy = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Cancels a workflow instance.
        /// </summary>
        Task<bool> CancelWorkflowAsync(
            string workflowInstanceId,
            CancellationToken cancellationToken = default);
    }
}
