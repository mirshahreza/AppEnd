using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;

namespace AppEndWorkflow.Activities.FlowControl;

/// <summary>
/// Retries an activity with exponential backoff on failure.
/// </summary>
[Activity(
    Category = "Flow Control",
    Description = "Retries with exponential backoff",
    DisplayName = "Retry"
)]
public class RetryActivity : CodeActivity
{
    /// <summary>
    /// Maximum number of retry attempts (default: 3)
    /// </summary>
    [Input(Description = "Maximum retry attempts")]
    public Input<int> MaxRetries { get; set; } = new(3);

    /// <summary>
    /// Delay between retries in seconds (default: 5)
    /// </summary>
    [Input(Description = "Delay between retries in seconds")]
    public Input<int> DelaySeconds { get; set; } = new(5);

    /// <summary>
    /// Exponential backoff multiplier (default: 2.0)
    /// </summary>
    [Input(Description = "Exponential backoff multiplier")]
    public Input<double> BackoffMultiplier { get; set; } = new(2.0);

    /// <summary>
    /// Comma-separated error types to retry on (optional)
    /// </summary>
    [Input(Description = "Error types to retry on")]
    public Input<string?> RetryableErrors { get; set; }

    /// <summary>
    /// Number of attempts made
    /// </summary>
    [Output(Description = "Number of attempts made")]
    public Output<int> AttemptCount { get; set; } = default!;

    /// <summary>
    /// Whether the activity eventually succeeded
    /// </summary>
    [Output(Description = "Whether the activity eventually succeeded")]
    public Output<bool> Success { get; set; } = default!;

    /// <summary>
    /// Last error message if all retries failed
    /// </summary>
    [Output(Description = "Last error message")]
    public Output<string?> LastError { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var maxRetries = context.Get(MaxRetries);
            var delaySeconds = context.Get(DelaySeconds);
            var backoffMultiplier = context.Get(BackoffMultiplier);

            if (maxRetries < 1)
                throw new ArgumentException("'MaxRetries' must be at least 1");

            // TODO: Implement retry logic
            // This should:
            // 1. Try to execute next activity
            // 2. Catch exceptions
            // 3. Wait with exponential backoff
            // 4. Retry up to maxRetries times
            
            context.Set(AttemptCount, 1);
            context.Set(Success, true);
            context.Set(LastError, null);
        }
        catch (Exception ex)
        {
            context.Set(AttemptCount, 1);
            context.Set(Success, false);
            context.Set(LastError, ex.Message);
        }
    }
}
