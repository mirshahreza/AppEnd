namespace AppEndServer.Workflows.Samples
{
    /// <summary>
    /// Template for code-first workflow definitions.
    /// 
    /// This is a CODE TEMPLATE showing how to structure AppEnd workflows
    /// Once Elsa 3.0 packages are installed, uncomment the Elsa-specific code.
    /// 
    /// Example usage:
    /// 1. Create a new class inheriting from AppEndWorkflowBase
    /// 2. Override WorkflowName property
    /// 3. Implement Build() method using Elsa activities
    /// 4. Register in the Elsa workflow registry
    /// </summary>
    public static class WorkflowTemplates
    {
        /// <summary>
        /// Template: Simple Approval Workflow
        /// 
        /// Demonstrates:
        /// - Basic workflow structure
        /// - Sequential activities
        /// - Logging in workflows
        /// </summary>
        public class SimpleApprovalWorkflowTemplate
        {
            public string WorkflowName => "simple-approval-process";
            public string WorkflowDisplayName => "Simple Approval Process";
            public string WorkflowDescription => "A basic workflow demonstrating approval steps";

            /*
            // UNCOMMENT WHEN ELSA PACKAGES ARE INSTALLED:
            
            using Elsa.Workflows.Core;
            using Elsa.Workflows.Core.Activities;
            using Elsa.Workflows.Core.Models;

            public class SimpleApprovalWorkflow : AppEndWorkflowBase
            {
                public override string WorkflowName => "simple-approval-process";
                public override string WorkflowDisplayName => "Simple Approval Process";
                public override string WorkflowDescription => "A basic workflow demonstrating approval steps";

                protected override void Build(IWorkflowBuilder builder)
                {
                    // Start: WriteLine activity
                    builder.Root = new Sequence
                    {
                        Activities =
                        [
                            // Activity 1: Log workflow start
                            new WriteLine
                            {
                                Text = new("Workflow started: Simple Approval Process")
                            },

                            // Activity 2: Wait for user input (placeholder - will be replaced with actual approval)
                            // In Phase 3, this will be replaced with custom AppEnd approval activity
                            new WriteLine
                            {
                                Text = new("Awaiting approval...")
                            },

                            // Activity 3: Log completion
                            new WriteLine
                            {
                                Text = new("Workflow completed successfully")
                            }
                        ]
                    };
                }
            }
            */
        }
    }

    /// <summary>
    /// Base class for all AppEnd workflow definitions.
    /// 
    /// UNCOMMENT THE ELSA INHERITANCE ONCE PACKAGES ARE INSTALLED:
    /// public abstract class AppEndWorkflowBase : WorkflowBase
    /// 
    /// For now, this is a template/documentation class.
    /// </summary>
    public abstract class AppEndWorkflowBase
    {
        /// <summary>
        /// Gets the unique name of this workflow.
        /// Must be overridden by derived classes.
        /// 
        /// Example: "purchase-order-approval", "invoice-processing", etc.
        /// </summary>
        public abstract string WorkflowName { get; }

        /// <summary>
        /// Gets the display name for the workflow UI.
        /// Optional - defaults to WorkflowName if not set.
        /// </summary>
        public virtual string? WorkflowDisplayName => WorkflowName;

        /// <summary>
        /// Gets the workflow description shown in the designer.
        /// </summary>
        public virtual string? WorkflowDescription => null;

        /// <summary>
        /// Gets the workflow version for tracking changes.
        /// Increment when making significant changes.
        /// </summary>
        public virtual int Version => 1;

        /// <summary>
        /// Gets the tenant ID for multi-tenant deployments.
        /// Leave null for single-tenant workflows.
        /// </summary>
        public virtual string? TenantId => null;

        /// <summary>
        /// Helper: Create a log activity for informational messages.
        /// Used in Build() method.
        /// </summary>
        protected string LogInfoMessage(string message) => $"[INFO] {message}";

        /// <summary>
        /// Helper: Create a log activity for warning messages.
        /// </summary>
        protected string LogWarningMessage(string message) => $"[WARN] {message}";

        /// <summary>
        /// Helper: Create a log activity for error messages.
        /// </summary>
        protected string LogErrorMessage(string message) => $"[ERROR] {message}";

        /// <summary>
        /// UNCOMMENT WHEN ELSA PACKAGES ARE INSTALLED:
        /// 
        /// protected abstract void Build(IWorkflowBuilder builder);
        /// 
        /// This method defines the workflow activities and flow.
        /// Example:
        /// 
        /// protected override void Build(IWorkflowBuilder builder)
        /// {
        ///     builder.Root = new Sequence
        ///     {
        ///         Activities =
        ///         [
        ///             new WriteLine { Text = new(LogInfoMessage("Starting workflow")) },
        ///             new MyCustomActivity(),
        ///             new WriteLine { Text = new(LogInfoMessage("Done")) }
        ///         ]
        ///     };
        /// }
        /// </summary>
    }

    /// <summary>
    /// Documentation: Common Workflow Patterns
    /// 
    /// 1. SEQUENTIAL PROCESS
    ///    Workflow step 1 → Step 2 → Step 3
    ///    Use: Sequence activity
    ///
    /// 2. CONDITIONAL LOGIC
    ///    If condition → Branch A, else Branch B
    ///    Use: If/Then/Else activities
    ///
    /// 3. PARALLEL PROCESSING
    ///    Start task 1 and task 2 in parallel, wait for both
    ///    Use: Parallel activity
    ///
    /// 4. HUMAN APPROVAL
    ///    Create approval request, wait for response
    ///    Use: Custom AppEndApproval activity (Phase 3)
    ///
    /// 5. SCHEDULED TASKS
    ///    Trigger workflow at specific time
    ///    Use: Timer trigger + Scheduler integration (Phase 2)
    ///
    /// 6. EVENT-DRIVEN
    ///    Trigger workflow on external event
    ///    Use: Event trigger (Phase 2)
    /// </summary>
}
