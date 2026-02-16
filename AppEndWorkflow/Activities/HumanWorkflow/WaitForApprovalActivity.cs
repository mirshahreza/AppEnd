using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;

namespace AppEndWorkflow.Activities.HumanWorkflow;

/// <summary>
/// Suspends workflow and waits for user approval.
/// Combines task creation with workflow suspension (Elsa bookmark).
/// When user completes the task in kartabl, the bookmark is resumed with the outcome.
/// This is the key activity for the kartabl (inbox) system.
/// </summary>
[Activity(
    Category = "Human Tasks",
    Description = "Suspends workflow and waits for approval",
    DisplayName = "Wait for Approval"
)]
public class WaitForApprovalActivity : CodeActivity
{
    /// <summary>
    /// Specific approver user ID (optional)
    /// </summary>
    [Input(Description = "Specific approver user ID (optional)")]
    public Input<string?> ApproverUserId { get; set; }

    /// <summary>
    /// Role-based approval (optional)
    /// </summary>
    [Input(Description = "Role-based approval (optional)")]
    public Input<string?> ApproverRole { get; set; }

    /// <summary>
    /// Title shown in kartabl
    /// </summary>
    [Input(Description = "Title shown in kartabl")]
    public Input<string> TaskTitle { get; set; } = default!;

    /// <summary>
    /// Comma-separated outcomes, e.g., "Approved,Rejected,NeedMoreInfo"
    /// </summary>
    [Input(Description = "Comma-separated allowed outcomes")]
    public Input<string> AllowedOutcomes { get; set; } = default!;

    /// <summary>
    /// JSON data shown to approver
    /// </summary>
    [Input(Description = "JSON data shown to approver")]
    public Input<string?> ContextData { get; set; }

    /// <summary>
    /// Auto-escalate or auto-reject after N days
    /// </summary>
    [Input(Description = "Auto-escalate or auto-reject after N days (optional)")]
    public Input<int?> TimeoutDays { get; set; }

    /// <summary>
    /// The outcome selected by the approver
    /// </summary>
    [Output(Description = "The outcome selected by the approver")]
    public Output<string> Outcome { get; set; } = default!;

    /// <summary>
    /// User who completed the task
    /// </summary>
    [Output(Description = "User who completed the task")]
    public Output<string> ApprovedBy { get; set; } = default!;

    /// <summary>
    /// Optional comment from approver
    /// </summary>
    [Output(Description = "Optional comment from approver")]
    public Output<string?> Comment { get; set; }

    /// <summary>
    /// When the task was completed
    /// </summary>
    [Output(Description = "When the task was completed")]
    public Output<DateTime> CompletedAt { get; set; } = default!;

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var approverUserId = context.Get(ApproverUserId);
            var approverRole = context.Get(ApproverRole);
            var taskTitle = context.Get(TaskTitle);
            var allowedOutcomes = context.Get(AllowedOutcomes);
            var contextData = context.Get(ContextData);
            var timeoutDays = context.Get(TimeoutDays);

            if (string.IsNullOrWhiteSpace(approverUserId) && string.IsNullOrWhiteSpace(approverRole))
                throw new ArgumentException("Either 'ApproverUserId' or 'ApproverRole' is required");

            if (string.IsNullOrWhiteSpace(taskTitle))
                throw new ArgumentException("'TaskTitle' is required");

            if (string.IsNullOrWhiteSpace(allowedOutcomes))
                throw new ArgumentException("'AllowedOutcomes' is required");

            // TODO: Implement approval workflow suspension and resumption
            // This should:
            // 1. Create a task in AppEnd's task system
            // 2. Create an Elsa bookmark for workflow suspension
            // 3. Store allowed outcomes and approver info
            // 4. When user completes task in kartabl, resume bookmark with outcome
            // 5. Support optional timeout (escalation)
            //
            // Implementation approach:
            // - Use Elsa's bookmark system to suspend workflow
            // - Store bookmark data in AppEnd's task table
            // - Listen for task completion event to resume bookmark
            // - Handle timeout via Elsa Timer activity
            //
            // Placeholder implementation:
            context.Set(Outcome, "Pending");
            context.Set(ApprovedBy, "");
            context.Set(Comment, null);
            context.Set(CompletedAt, DateTime.UtcNow);

            // TODO: Remove placeholder and implement actual bookmark suspension
            throw new NotImplementedException("Approval workflow needs to be implemented with Elsa bookmark system");
        }
        catch (Exception ex)
        {
            context.Set(Outcome, "Error");
            context.Set(ApprovedBy, "");
            context.Set(Comment, ex.Message);
            context.Set(CompletedAt, DateTime.UtcNow);
        }
    }
}
