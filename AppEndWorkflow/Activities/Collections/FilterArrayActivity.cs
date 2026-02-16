using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Text.Json;

namespace AppEndWorkflow.Activities.Collections;

/// <summary>
/// Filters a JSON array using JavaScript filter expression.
/// </summary>
[Activity(
    Category = "Collections",
    Description = "Filters JSON array",
    DisplayName = "Filter Array"
)]
public class FilterArrayActivity : CodeActivity
{
    /// <summary>
    /// JSON array to filter
    /// </summary>
    [Input(Description = "JSON array to filter")]
    public Input<string> InputArray { get; set; } = default!;

    /// <summary>
    /// JavaScript filter expression (e.g., "item.Amount > 1000")
    /// </summary>
    [Input(Description = "JavaScript filter expression")]
    public Input<string> FilterExpression { get; set; } = default!;

    /// <summary>
    /// Filtered JSON array
    /// </summary>
    [Output(Description = "Filtered JSON array")]
    public Output<string> Result { get; set; } = default!;

    /// <summary>
    /// Number of items after filtering
    /// </summary>
    [Output(Description = "Number of items after filtering")]
    public Output<int> Count { get; set; } = default!;

    /// <summary>
    /// Number of items before filtering
    /// </summary>
    [Output(Description = "Number of items before filtering")]
    public Output<int> OriginalCount { get; set; } = default!;

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var inputArrayJson = context.Get(InputArray);
            var filterExpression = context.Get(FilterExpression);

            if (string.IsNullOrWhiteSpace(inputArrayJson))
                throw new ArgumentException("'InputArray' is required");

            if (string.IsNullOrWhiteSpace(filterExpression))
                throw new ArgumentException("'FilterExpression' is required");

            // Parse input array
            using var doc = JsonDocument.Parse(inputArrayJson);
            if (doc.RootElement.ValueKind != JsonValueKind.Array)
                throw new ArgumentException("InputArray must be a JSON array");

            var originalItems = doc.RootElement.EnumerateArray().ToList();
            var originalCount = originalItems.Count;

            // TODO: Implement filtering with JavaScript engine
            // For now, return all items as filtered result
            var filteredItems = originalItems;

            var result = JsonSerializer.Serialize(filteredItems.Select(x => JsonDocument.Parse(x.GetRawText()).RootElement).ToList());

            context.Set(Result, result);
            context.Set(Count, filteredItems.Count);
            context.Set(OriginalCount, originalCount);
        }
        catch (Exception ex)
        {
            context.Set(Result, "[]");
            context.Set(Count, 0);
            context.Set(OriginalCount, 0);
        }
    }
}
