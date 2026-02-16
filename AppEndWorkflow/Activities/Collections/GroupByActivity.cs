using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Text.Json;

namespace AppEndWorkflow.Activities.Collections;

/// <summary>
/// Groups array items by field value with optional aggregation.
/// </summary>
[Activity(
    Category = "Collections",
    Description = "Groups array items by field",
    DisplayName = "Group By"
)]
public class GroupByActivity : CodeActivity
{
    /// <summary>
    /// JSON array to group
    /// </summary>
    [Input(Description = "JSON array to group")]
    public Input<string> InputArray { get; set; } = default!;

    /// <summary>
    /// Property name to group by
    /// </summary>
    [Input(Description = "Property name to group by")]
    public Input<string> GroupByField { get; set; } = default!;

    /// <summary>
    /// Property to aggregate per group (optional)
    /// </summary>
    [Input(Description = "Property to aggregate per group")]
    public Input<string?> AggregateField { get; set; }

    /// <summary>
    /// Aggregation operation: "Sum", "Count", "Average" (optional)
    /// </summary>
    [Input(Description = "Aggregation operation")]
    public Input<string?> AggregateOperation { get; set; }

    /// <summary>
    /// JSON array of {Key, Items, AggregateValue?}
    /// </summary>
    [Output(Description = "Grouped data")]
    public Output<string> Result { get; set; } = default!;

    /// <summary>
    /// Number of distinct groups
    /// </summary>
    [Output(Description = "Number of distinct groups")]
    public Output<int> GroupCount { get; set; } = default!;

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var inputArrayJson = context.Get(InputArray);
            var groupByField = context.Get(GroupByField);
            var aggregateField = context.Get(AggregateField);
            var aggregateOperation = context.Get(AggregateOperation);

            if (string.IsNullOrWhiteSpace(inputArrayJson))
                throw new ArgumentException("'InputArray' is required");

            if (string.IsNullOrWhiteSpace(groupByField))
                throw new ArgumentException("'GroupByField' is required");

            // Parse input array
            using var doc = JsonDocument.Parse(inputArrayJson);
            if (doc.RootElement.ValueKind != JsonValueKind.Array)
                throw new ArgumentException("InputArray must be a JSON array");

            var items = doc.RootElement.EnumerateArray().ToList();

            // Group items
            var groups = items.GroupBy(item =>
            {
                if (item.TryGetProperty(groupByField, out var value))
                    return value.GetString() ?? "null";
                return "null";
            }).ToList();

            // Build result
            var groupList = new List<Dictionary<string, object>>();
            foreach (var group in groups)
            {
                var groupData = new Dictionary<string, object>
                {
                    { "Key", group.Key },
                    { "Items", group.Select(x => JsonDocument.Parse(x.GetRawText()).RootElement).ToList() }
                };

                // Add aggregation if specified
                if (!string.IsNullOrWhiteSpace(aggregateField) && !string.IsNullOrWhiteSpace(aggregateOperation))
                {
                    var values = new List<decimal>();
                    foreach (var item in group)
                    {
                        if (item.TryGetProperty(aggregateField, out var value) && value.ValueKind == JsonValueKind.Number)
                            values.Add(value.GetDecimal());
                    }

                    var aggregateValue = aggregateOperation.ToLower() switch
                    {
                        "sum" => values.Sum(),
                        "average" => values.Count > 0 ? values.Average() : 0,
                        "count" => values.Count,
                        _ => 0
                    };

                    groupData["AggregateValue"] = aggregateValue;
                }

                groupList.Add(groupData);
            }

            var result = JsonSerializer.Serialize(groupList);
            context.Set(Result, result);
            context.Set(GroupCount, groups.Count);
        }
        catch (Exception ex)
        {
            context.Set(Result, "[]");
            context.Set(GroupCount, 0);
        }
    }
}
