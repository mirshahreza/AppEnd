using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Text.Json;

namespace AppEndWorkflow.Activities.Collections;

/// <summary>
/// Picks a single item from array (First, Last, Random, ByIndex).
/// </summary>
[Activity(
    Category = "Collections",
    Description = "Picks an item from array",
    DisplayName = "Pick from Array"
)]
public class PickFromArrayActivity : CodeActivity
{
    /// <summary>
    /// JSON array
    /// </summary>
    [Input(Description = "JSON array")]
    public Input<string> InputArray { get; set; } = default!;

    /// <summary>
    /// Operation: "First", "Last", "Random", "ByIndex"
    /// </summary>
    [Input(Description = "Operation")]
    public Input<string> Operation { get; set; } = default!;

    /// <summary>
    /// Index for ByIndex operation
    /// </summary>
    [Input(Description = "Index for ByIndex operation")]
    public Input<int?> Index { get; set; }

    /// <summary>
    /// Selected item as JSON
    /// </summary>
    [Output(Description = "Selected item as JSON")]
    public Output<string> Result { get; set; } = default!;

    /// <summary>
    /// Whether an item was found
    /// </summary>
    [Output(Description = "Whether an item was found")]
    public Output<bool> Found { get; set; } = default!;

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var inputArrayJson = context.Get(InputArray);
            var operation = context.Get(Operation);
            var index = context.Get(Index);

            if (string.IsNullOrWhiteSpace(inputArrayJson))
                throw new ArgumentException("'InputArray' is required");

            if (string.IsNullOrWhiteSpace(operation))
                throw new ArgumentException("'Operation' is required");

            // Parse input array
            using var doc = JsonDocument.Parse(inputArrayJson);
            if (doc.RootElement.ValueKind != JsonValueKind.Array)
                throw new ArgumentException("InputArray must be a JSON array");

            var items = doc.RootElement.EnumerateArray().ToList();

            if (items.Count == 0)
            {
                context.Set(Result, "");
                context.Set(Found, false);
                return;
            }

            // Pick item
            JsonElement? pickedItem = operation.ToLower() switch
            {
                "first" => items[0],
                "last" => items[items.Count - 1],
                "random" => items[new Random().Next(items.Count)],
                "byindex" => index >= 0 && index < items.Count ? items[index.Value] : null,
                _ => null
            };

            if (pickedItem == null)
            {
                context.Set(Result, "");
                context.Set(Found, false);
            }
            else
            {
                context.Set(Result, pickedItem.Value.GetRawText());
                context.Set(Found, true);
            }
        }
        catch (Exception ex)
        {
            context.Set(Result, "");
            context.Set(Found, false);
        }
    }
}
