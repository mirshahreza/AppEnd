using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Text.Json;

namespace AppEndWorkflow.Activities.Collections;

/// <summary>
/// Sorts a JSON array by property with optional direction and type.
/// </summary>
[Activity(
    Category = "Collections",
    Description = "Sorts JSON array",
    DisplayName = "Sort Array"
)]
public class SortArrayActivity : CodeActivity
{
    /// <summary>
    /// JSON array to sort
    /// </summary>
    [Input(Description = "JSON array to sort")]
    public Input<string> InputArray { get; set; } = default!;

    /// <summary>
    /// Property name to sort by (e.g., "CreatedDate")
    /// </summary>
    [Input(Description = "Property name to sort by")]
    public Input<string> SortBy { get; set; } = default!;

    /// <summary>
    /// Direction: "Asc" or "Desc" (default: "Asc")
    /// </summary>
    [Input(Description = "Sort direction")]
    public Input<string> Direction { get; set; } = new("Asc");

    /// <summary>
    /// Sort type: "String", "Number", "Date" (default: "String")
    /// </summary>
    [Input(Description = "Sort type")]
    public Input<string> SortType { get; set; } = new("String");

    /// <summary>
    /// Sorted JSON array
    /// </summary>
    [Output(Description = "Sorted JSON array")]
    public Output<string> Result { get; set; } = default!;

    /// <summary>
    /// Number of items
    /// </summary>
    [Output(Description = "Number of items")]
    public Output<int> Count { get; set; } = default!;

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var inputArrayJson = context.Get(InputArray);
            var sortBy = context.Get(SortBy);
            var direction = context.Get(Direction) ?? "Asc";
            var sortType = context.Get(SortType) ?? "String";

            if (string.IsNullOrWhiteSpace(inputArrayJson))
                throw new ArgumentException("'InputArray' is required");

            if (string.IsNullOrWhiteSpace(sortBy))
                throw new ArgumentException("'SortBy' is required");

            // Parse input array
            using var doc = JsonDocument.Parse(inputArrayJson);
            if (doc.RootElement.ValueKind != JsonValueKind.Array)
                throw new ArgumentException("InputArray must be a JSON array");

            var items = doc.RootElement.EnumerateArray().ToList();

            // Sort items
            var sortedItems = SortItems(items, sortBy, direction, sortType);

            var result = JsonSerializer.Serialize(sortedItems);

            context.Set(Result, result);
            context.Set(Count, sortedItems.Count);
        }
        catch (Exception ex)
        {
            context.Set(Result, "[]");
            context.Set(Count, 0);
        }
    }

    private List<JsonElement> SortItems(List<JsonElement> items, string property, string direction, string sortType)
    {
        var sorted = items.OrderBy((Func<JsonElement, IComparable?>)(item =>
        {
            if (!item.TryGetProperty(property, out var value))
                return null;

            return (IComparable?)sortType.ToLower() switch
            {
                "number" => value.ValueKind == JsonValueKind.Number ? value.GetDecimal() : (decimal?)null,
                "date" => DateTime.TryParse(value.GetString(), out var dt) ? dt : (DateTime?)null,
                _ => value.GetString()
            };
        })).ToList();

        if (direction.ToLower() == "desc")
            sorted.Reverse();

        return sorted;
    }
}
