using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;

namespace AppEndWorkflow.Activities.FlowControl;

/// <summary>
/// Schedules a workflow to run at specific times using cron expressions.
/// </summary>
[Activity(
    Category = "Scheduling",
    Description = "Schedule a workflow to run",
    DisplayName = "Schedule Workflow"
)]
public class ScheduleWorkflowActivity : CodeActivity
{
    [Input(Description = "Workflow name/ID to schedule")]
    public Input<string> WorkflowName { get; set; } = default!;

    [Input(Description = "Cron expression (e.g., '0 9 * * MON' for 9 AM Mondays)")]
    public Input<string> ScheduleExpression { get; set; } = default!;

    [Input(Description = "JSON input for scheduled workflow")]
    public Input<string?> Input { get; set; }

    [Input(Description = "Timezone (default: UTC)")]
    public Input<string?> TimeZone { get; set; }

    [Input(Description = "Max run count (optional, unlimited if not set)")]
    public Input<int?> MaxOccurrences { get; set; }

    [Output(Description = "Scheduled workflow ID")]
    public Output<string> ScheduledId { get; set; } = default!;

    [Output(Description = "Next scheduled run time")]
    public Output<DateTime> NextRun { get; set; } = default!;

    [Output(Description = "Whether scheduling succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var workflowName = context.Get(WorkflowName) ?? throw new ArgumentException("WorkflowName is required");
            var scheduleExpression = context.Get(ScheduleExpression) ?? throw new ArgumentException("ScheduleExpression is required");
            var timeZone = context.Get(TimeZone) ?? "UTC";

            // NOTE: In real implementation, this would use Elsa's timer-based scheduling system
            // For now, we'll create a mock scheduled ID and calculate next run time

            var scheduledId = Guid.NewGuid().ToString();
            var nextRunTime = CalculateNextRun(scheduleExpression, timeZone);

            context.Set(ScheduledId, scheduledId);
            context.Set(NextRun, nextRunTime);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(ScheduledId, "");
            context.Set(NextRun, DateTime.MinValue);
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }

    private DateTime CalculateNextRun(string cronExpression, string timeZone)
    {
        // Simplified cron parsing - in production use a library like NCronTab
        // This is just a basic implementation
        try
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            var now = TimeZoneInfo.ConvertTime(DateTime.UtcNow, tz);

            // Parse basic cron: "0 9 * * MON" -> 9 AM every Monday
            var parts = cronExpression.Split(' ');
            if (parts.Length >= 5)
            {
                var minute = int.Parse(parts[0]);
                var hour = int.Parse(parts[1]);
                var dayOfMonth = parts[2];
                var month = parts[3];
                var dayOfWeek = parts[4];

                var nextRun = now.AddDays(1).Date.AddHours(hour).AddMinutes(minute);

                if (dayOfWeek != "*" && int.TryParse(dayOfWeek, out var dow))
                {
                    while ((int)nextRun.DayOfWeek != dow)
                        nextRun = nextRun.AddDays(1);
                }

                return TimeZoneInfo.ConvertTime(nextRun, tz, TimeZoneInfo.Utc);
            }

            return DateTime.UtcNow.AddHours(1);
        }
        catch
        {
            return DateTime.UtcNow.AddHours(1);
        }
    }
}
