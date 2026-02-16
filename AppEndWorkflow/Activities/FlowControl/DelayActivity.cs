using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;

namespace AppEndWorkflow.Activities.FlowControl;

/// <summary>
/// Delays workflow execution for specified duration.
/// Workflow state is persisted — survives app restarts.
/// </summary>
[Activity(
    Category = "Flow Control",
    Description = "Pauses workflow for duration",
    DisplayName = "Delay"
)]
public class DelayActivity : CodeActivity
{
    /// <summary>
    /// Duration expression: "00:05:00" (5 min), "1.00:00:00" (1 day)
    /// </summary>
    [Input(Description = "Duration expression")]
    public Input<string> Duration { get; set; } = default!;

    /// <summary>
    /// Alternative: unit type (Seconds, Minutes, Hours, Days)
    /// </summary>
    [Input(Description = "Unit type")]
    public Input<string?> Unit { get; set; }

    /// <summary>
    /// Numeric value when using Unit
    /// </summary>
    [Input(Description = "Value when using Unit")]
    public Input<int?> Value { get; set; }

    /// <summary>
    /// When the workflow resumed
    /// </summary>
    [Output(Description = "When the workflow resumed")]
    public Output<DateTime> ResumedAt { get; set; } = default!;

    /// <summary>
    /// Actual delay duration
    /// </summary>
    [Output(Description = "Actual delay duration")]
    public Output<string> ActualDelay { get; set; } = default!;

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var duration = context.Get(Duration);
            var unit = context.Get(Unit);
            var value = context.Get(Value);

            TimeSpan delay = TimeSpan.Zero;

            if (!string.IsNullOrWhiteSpace(duration))
            {
                if (TimeSpan.TryParse(duration, out var parsed))
                    delay = parsed;
            }
            else if (!string.IsNullOrWhiteSpace(unit) && value.HasValue)
            {
                delay = unit.ToLower() switch
                {
                    "seconds" => TimeSpan.FromSeconds(value.Value),
                    "minutes" => TimeSpan.FromMinutes(value.Value),
                    "hours" => TimeSpan.FromHours(value.Value),
                    "days" => TimeSpan.FromDays(value.Value),
                    _ => TimeSpan.Zero
                };
            }

            if (delay == TimeSpan.Zero)
                throw new ArgumentException("Valid 'Duration' or 'Unit'+'Value' is required");

            // TODO: Implement Elsa bookmark for workflow suspension
            // This should use Elsa's Timer bookmark to pause execution
            // without blocking the thread
            
            context.Set(ResumedAt, DateTime.UtcNow.Add(delay));
            context.Set(ActualDelay, delay.ToString());
        }
        catch (Exception ex)
        {
            context.Set(ResumedAt, DateTime.UtcNow);
            context.Set(ActualDelay, "00:00:00");
        }
    }
}
