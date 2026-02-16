using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Text.Json;

namespace AppEndWorkflow.Activities.Collections;

/// <summary>
/// Aggregates array values (Sum, Average, Min, Max, Count, Distinct).
/// </summary>
[Activity(
    Category = "Collections",
    Description = "Aggregates array values",
    DisplayName = "Aggregate Array"
)]
public class AggregateArrayActivity : CodeActivity
{
    /// <summary>
    /// JSON array to aggregate
    /// </summary>
    [Input(Description = "JSON array to aggregate")]
    public Input<string> InputArray { get; set; } = default!;

    /// <summary>
    /// Property name to aggregate
    /// </summary>
    [Input(Description = "Property name to aggregate")]
    public Input<string> Field { get; set; } = default!;

    /// <summary>
    /// Operation: "Sum", "Average", "Min", "Max", "Count", "Distinct"
    /// </summary>
    [Input(Description = "Aggregation operation")]
    public Input<string> Operation { get; set; } = default!;

    /// <summary>
    /// Aggregation result (as string)
    /// </summary>
    [Output(Description = "Aggregation result")]
    public Output<string> Result { get; set; } = default!;

    /// <summary>
    /// Numeric result (for Sum/Average/Min/Max)
    /// </summary>
    [Output(Description = "Numeric result")]
    public Output<double?> NumericResult { get; set; }

    /// <summary>
    /// Count of items
    /// </summary>
    [Output(Description = "Count of items")]
    public Output<int> Count { get; set; } = default!;

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var inputArrayJson = context.Get(InputArray);
            var field = context.Get(Field);
            var operation = context.Get(Operation);

            if (string.IsNullOrWhiteSpace(inputArrayJson))
                throw new ArgumentException("'InputArray' is required");

            if (string.IsNullOrWhiteSpace(field))
                throw new ArgumentException("'Field' is required");

            if (string.IsNullOrWhiteSpace(operation))
                throw new ArgumentException("'Operation' is required");

            // Parse input array
            using var doc = JsonDocument.Parse(inputArrayJson);
            if (doc.RootElement.ValueKind != JsonValueKind.Array)
                throw new ArgumentException("InputArray must be a JSON array");

            var items = doc.RootElement.EnumerateArray().ToList();

            // Perform aggregation
            var (result, numericResult) = AggregateItems(items, field, operation);

            context.Set(Result, result);
            context.Set(NumericResult, numericResult);
            context.Set(Count, items.Count);
        }
        catch (Exception ex)
        {
            context.Set(Result, "");
            context.Set(NumericResult, null);
            context.Set(Count, 0);
        }
    }

    private (string result, double? numericResult) AggregateItems(List<JsonElement> items, string field, string operation)
    {
        var values = new List<decimal>();

        foreach (var item in items)
        {
            if (item.TryGetProperty(field, out var value) && value.ValueKind == JsonValueKind.Number)
            {
                values.Add(value.GetDecimal());
            }
        }

        double? numericResult = null;
        string result;

        switch (operation.ToLower())
        {
            case "sum":
                numericResult = (double)values.Sum();
                result = numericResult.ToString();
                break;

            case "average":
                numericResult = values.Count > 0 ? (double)values.Average() : 0;
                result = numericResult.ToString();
                break;

            case "min":
                numericResult = values.Count > 0 ? (double)values.Min() : 0;
                result = numericResult.ToString();
                break;

            case "max":
                numericResult = values.Count > 0 ? (double)values.Max() : 0;
                result = numericResult.ToString();
                break;

            case "count":
                numericResult = values.Count;
                result = numericResult.ToString();
                break;

            case "distinct":
                numericResult = values.Distinct().Count();
                result = numericResult.ToString();
                break;

            default:
                result = "";
                break;
        }

        return (result, numericResult);
    }
}
