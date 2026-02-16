using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Text.Json;

namespace AppEndWorkflow.Activities.FlowControl;

/// <summary>
/// Executes an activity for each item in array with concurrency limit.
/// </summary>
[Activity(
    Category = "Flow Control",
    Description = "Parallel iteration over array",
    DisplayName = "Parallel ForEach"
)]
public class ParallelForEachActivity : CodeActivity
{
    /// <summary>
    /// JSON array of items to process
    /// </summary>
    [Input(Description = "JSON array of items")]
    public Input<string> InputArray { get; set; } = default!;

    /// <summary>
    /// Maximum parallel executions (default: 5)
    /// </summary>
    [Input(Description = "Maximum parallel executions")]
    public Input<int> MaxConcurrency { get; set; } = new(5);

    /// <summary>
    /// Name/ID of the activity to run for each item
    /// </summary>
    [Input(Description = "Activity name to execute")]
    public Input<string> ActivityToExecute { get; set; } = default!;

    /// <summary>
    /// JSON array of results from each execution
    /// </summary>
    [Output(Description = "Results from each execution")]
    public Output<string> Results { get; set; } = default!;

    /// <summary>
    /// Total items processed
    /// </summary>
    [Output(Description = "Total items processed")]
    public Output<int> TotalCount { get; set; } = default!;

    /// <summary>
    /// Items processed successfully
    /// </summary>
    [Output(Description = "Items processed successfully")]
    public Output<int> SuccessCount { get; set; } = default!;

    /// <summary>
    /// Items that failed
    /// </summary>
    [Output(Description = "Items that failed")]
    public Output<int> FailedCount { get; set; } = default!;

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var inputArrayJson = context.Get(InputArray);
            var maxConcurrency = context.Get(MaxConcurrency);
            var activityName = context.Get(ActivityToExecute);

            if (string.IsNullOrWhiteSpace(inputArrayJson))
                throw new ArgumentException("'InputArray' is required");

            if (string.IsNullOrWhiteSpace(activityName))
                throw new ArgumentException("'ActivityToExecute' is required");

            // Parse input array
            using var doc = JsonDocument.Parse(inputArrayJson);
            if (doc.RootElement.ValueKind != JsonValueKind.Array)
                throw new ArgumentException("InputArray must be a JSON array");

            var items = doc.RootElement.EnumerateArray().ToList();
            var totalCount = items.Count;

            // TODO: Implement parallel execution
            // This should use SemaphoreSlim for concurrency control
            // and execute activity for each item
            
            var results = new List<object>();
            var successCount = 0;
            var failedCount = 0;

            context.Set(Results, JsonSerializer.Serialize(results));
            context.Set(TotalCount, totalCount);
            context.Set(SuccessCount, successCount);
            context.Set(FailedCount, failedCount);
        }
        catch (Exception ex)
        {
            context.Set(Results, "[]");
            context.Set(TotalCount, 0);
            context.Set(SuccessCount, 0);
            context.Set(FailedCount, 0);
        }
    }
}
