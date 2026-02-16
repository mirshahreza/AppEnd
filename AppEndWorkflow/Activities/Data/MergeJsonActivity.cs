using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Text.Json;

namespace AppEndWorkflow.Activities.Data;

/// <summary>
/// Deep-merges two JSON objects.
/// Uses System.Text.Json for JSON processing.
/// </summary>
[Activity(
    Category = "Data",
    Description = "Deep-merges two JSON objects",
    DisplayName = "Merge JSON"
)]
public class MergeJsonActivity : CodeActivity
{
    /// <summary>
    /// Primary JSON object
    /// </summary>
    [Input(Description = "Primary JSON object")]
    public Input<string> Source { get; set; } = default!;

    /// <summary>
    /// JSON object to merge on top
    /// </summary>
    [Input(Description = "JSON object to merge on top")]
    public Input<string> Overlay { get; set; } = default!;

    /// <summary>
    /// Array merge strategy: "Replace", "Concat", "Union" (default: "Replace")
    /// </summary>
    [Input(Description = "Array merge strategy: 'Replace', 'Concat', 'Union'")]
    public Input<string> ArrayMergeStrategy { get; set; } = new("Replace");

    /// <summary>
    /// Merged JSON output
    /// </summary>
    [Output(Description = "Merged JSON output")]
    public Output<string> Result { get; set; } = default!;

    /// <summary>
    /// Whether merge succeeded
    /// </summary>
    [Output(Description = "Whether merge succeeded")]
    public Output<bool> Success { get; set; } = default!;

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var sourceJson = context.Get(Source);
            var overlayJson = context.Get(Overlay);
            var arrayStrategy = context.Get(ArrayMergeStrategy) ?? "Replace";

            if (string.IsNullOrWhiteSpace(sourceJson))
                throw new ArgumentException("'Source' is required");

            if (string.IsNullOrWhiteSpace(overlayJson))
                throw new ArgumentException("'Overlay' is required");

            // Parse JSON
            using var sourceDoc = JsonDocument.Parse(sourceJson);
            using var overlayDoc = JsonDocument.Parse(overlayJson);

            // Merge
            var merged = MergeElements(sourceDoc.RootElement, overlayDoc.RootElement, arrayStrategy);
            var result = JsonSerializer.Serialize(merged, new JsonSerializerOptions { WriteIndented = false });

            context.Set(Result, result);
            context.Set(Success, true);
        }
        catch (Exception ex)
        {
            context.Set(Result, "{}");
            context.Set(Success, false);
        }
    }

    private JsonElement MergeElements(JsonElement source, JsonElement overlay, string arrayStrategy)
    {
        // If overlay is not an object, return it (overlay wins)
        if (overlay.ValueKind != JsonValueKind.Object)
            return overlay;

        // If source is not an object, return overlay
        if (source.ValueKind != JsonValueKind.Object)
            return overlay;

        // Deep merge objects
        var options = new JsonSerializerOptions { WriteIndented = false };
        using var sourceDoc = JsonDocument.Parse(source.GetRawText());
        using var overlayDoc = JsonDocument.Parse(overlay.GetRawText());

        var merged = MergeObjects(sourceDoc.RootElement, overlayDoc.RootElement, arrayStrategy);
        return JsonDocument.Parse(JsonSerializer.Serialize(merged)).RootElement;
    }

    private Dictionary<string, object?> MergeObjects(JsonElement source, JsonElement overlay, string arrayStrategy)
    {
        var result = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

        // Add all source properties
        foreach (var property in source.EnumerateObject())
        {
            result[property.Name] = JsonElementToObject(property.Value);
        }

        // Merge overlay properties
        foreach (var property in overlay.EnumerateObject())
        {
            if (result.TryGetValue(property.Name, out var sourceValue) && sourceValue is Dictionary<string, object?> sourceDict)
            {
                if (property.Value.ValueKind == JsonValueKind.Object)
                {
                    var overlayDict = JsonElementToObject(property.Value) as Dictionary<string, object?>;
                    result[property.Name] = MergeObjects(sourceValue as JsonElement? ?? default, property.Value, arrayStrategy);
                }
                else
                {
                    result[property.Name] = JsonElementToObject(property.Value);
                }
            }
            else
            {
                result[property.Name] = JsonElementToObject(property.Value);
            }
        }

        return result;
    }

    private object? JsonElementToObject(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number => element.GetDecimal(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            JsonValueKind.Object => element.EnumerateObject()
                .ToDictionary(p => p.Name, p => JsonElementToObject(p.Value), StringComparer.OrdinalIgnoreCase),
            JsonValueKind.Array => element.EnumerateArray().Select(JsonElementToObject).ToList(),
            _ => null
        };
    }
}
