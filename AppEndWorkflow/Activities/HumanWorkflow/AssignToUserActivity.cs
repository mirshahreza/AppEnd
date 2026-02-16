using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;

namespace AppEndWorkflow.Activities.HumanWorkflow;

/// <summary>
/// Creates a task and assigns it to a user or role for manual action (کارتابل).
/// The task is accessible via WorkflowInbox.vue (kartabl component).
/// </summary>
[Activity(
    Category = "Human Tasks",
    Description = "Creates a task and assigns it to a user or role",
    DisplayName = "Assign to User"
)]
public class AssignToUserActivity : CodeActivity
{
    /// <summary>
    /// Target user ID or username
    /// </summary>
    [Input(Description = "Target user ID or username")]
    public Input<string> UserId { get; set; } = default!;

    /// <summary>
    /// Assign to a role instead (any user in role can pick up)
    /// </summary>
    [Input(Description = "Assign to a role instead (optional)")]
    public Input<string?> RoleName { get; set; }

    /// <summary>
    /// Title shown in kartabl
    /// </summary>
    [Input(Description = "Title shown in kartabl")]
    public Input<string> TaskTitle { get; set; } = default!;

    /// <summary>
    /// Detailed description
    /// </summary>
    [Input(Description = "Detailed description (optional)")]
    public Input<string?> TaskDescription { get; set; }

    /// <summary>
    /// Optional due date
    /// </summary>
    [Input(Description = "Optional due date")]
    public Input<DateTime?> DueDate { get; set; }

    /// <summary>
    /// Priority: "Low", "Normal", "High", "Critical" (default: "Normal")
    /// </summary>
    [Input(Description = "Priority: 'Low', 'Normal', 'High', 'Critical'")]
    public Input<string> Priority { get; set; } = new("Normal");

    /// <summary>
    /// JSON payload with task context (order details, etc.)
    /// </summary>
    [Input(Description = "JSON payload with task context (optional)")]
    public Input<string?> ContextData { get; set; }

    /// <summary>
    /// Created task ID
    /// </summary>
    [Output(Description = "Created task ID")]
    public Output<string> TaskId { get; set; } = default!;

    /// <summary>
    /// Whether the task was created successfully
    /// </summary>
    [Output(Description = "Whether the task was created successfully")]
    public Output<bool> Success { get; set; } = default!;

    /// <summary>
    /// Error message if failed
    /// </summary>
    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var userId = context.Get(UserId);
            var roleName = context.Get(RoleName);
            var taskTitle = context.Get(TaskTitle);
            var taskDescription = context.Get(TaskDescription);
            var dueDate = context.Get(DueDate);
            var priority = context.Get(Priority);
            var contextData = context.Get(ContextData);

            if (string.IsNullOrWhiteSpace(userId) && string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentException("Either 'UserId' or 'RoleName' is required");

            if (string.IsNullOrWhiteSpace(taskTitle))
                throw new ArgumentException("'TaskTitle' is required");

            if (string.IsNullOrWhiteSpace(priority))
                priority = "Normal";

            // Validate priority
            var validPriorities = new[] { "Low", "Normal", "High", "Critical" };
            if (!validPriorities.Contains(priority, StringComparer.OrdinalIgnoreCase))
                priority = "Normal";

            // Create task
            var taskId = CreateTask(userId, roleName, taskTitle, taskDescription, dueDate, priority, contextData, context);

            context.Set(TaskId, taskId);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(TaskId, string.Empty);
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }

    private string CreateTask(string? userId, string? roleName, string taskTitle, string? taskDescription, DateTime? dueDate, 
        string priority, string? contextData, ActivityExecutionContext context)
    {
        // TODO: Implement task creation in AppEnd's task/inbox system
        // This should:
        // 1. Create a task record in the database
        // 2. Associate it with the specified user or role
        // 3. Store the workflow instance ID for resumption
        // 4. Store context data (order details, etc.)
        // 5. Make it visible in WorkflowInbox.vue (kartabl component)
        //
        // Implementation approach:
        // - Use AppEnd's task/inbox service/repository
        // - Store workflow instance context for resumption when task is completed
        // - Generate unique task ID
        //
        // Placeholder implementation:
        var taskId = Guid.NewGuid().ToString();
        return taskId;
    }
}
