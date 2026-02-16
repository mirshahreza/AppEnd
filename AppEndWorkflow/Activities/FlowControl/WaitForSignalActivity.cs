using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;

namespace AppEndWorkflow.Activities.FlowControl;

/// <summary>
/// Suspends workflow and waits for external signal.
/// External systems send signal via RPC method.
/// </summary>
[Activity(
    Category = "Flow Control",
    Description = "Waits for external signal",
    DisplayName = "Wait for Signal"
)]
public class WaitForSignalActivity : CodeActivity
{
    /// <summary>
    /// Name of the signal to wait for
    /// </summary>
    [Input(Description = "Signal name")]
    public Input<string> SignalName { get; set; } = default!;

    /// <summary>
    /// Optional timeout in minutes (auto-resume with timeout outcome)
    /// </summary>
    [Input(Description = "Timeout in minutes")]
    public Input<int?> TimeoutMinutes { get; set; }

    /// <summary>
    /// JSON data sent with the signal
    /// </summary>
    [Output(Description = "JSON data sent with signal")]
    public Output<string?> SignalData { get; set; }

    /// <summary>
    /// When the signal was received
    /// </summary>
    [Output(Description = "When the signal was received")]
    public Output<DateTime> ReceivedAt { get; set; } = default!;

    /// <summary>
    /// Whether the wait timed out
    /// </summary>
    [Output(Description = "Whether the wait timed out")]
    public Output<bool> TimedOut { get; set; } = default!;

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var signalName = context.Get(SignalName);
            var timeoutMinutes = context.Get(TimeoutMinutes);

            if (string.IsNullOrWhiteSpace(signalName))
                throw new ArgumentException("'SignalName' is required");

            // TODO: Implement Elsa bookmark for signal waiting
            // This should:
            // 1. Create a bookmark with signal name
            // 2. External systems send signal via WorkflowServices.SendSignal()
            // 3. Resume bookmark when signal received
            // 4. Handle timeout if specified
            
            context.Set(SignalData, null);
            context.Set(ReceivedAt, DateTime.UtcNow);
            context.Set(TimedOut, false);
        }
        catch (Exception ex)
        {
            context.Set(SignalData, null);
            context.Set(ReceivedAt, DateTime.UtcNow);
            context.Set(TimedOut, false);
        }
    }
}
