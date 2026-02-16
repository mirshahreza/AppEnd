using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;

namespace AppEndWorkflow.Activities.Text;

/// <summary>
/// Renders a template string with placeholders using data values.
/// Supports nested paths like {{Order.Customer.Email}}.
/// </summary>
[Activity(
    Category = "Text",
    Description = "Renders template with data values",
    DisplayName = "Render Template"
)]
public class RenderTemplateActivity : CodeActivity
{
    /// <summary>
    /// Template string with placeholders {{FieldName}}
    /// </summary>
    [Input(Description = "Template string with {{FieldName}} placeholders")]
    public Input<string> Template { get; set; } = default!;

    /// <summary>
    /// JSON object of replacement values
    /// </summary>
    [Input(Description = "JSON object of replacement values")]
    public Input<string> Data { get; set; } = default!;

    /// <summary>
    /// Behavior for missing keys: "LeaveBlank", "KeepPlaceholder", "Error"
    /// </summary>
    [Input(Description = "Missing key behavior: 'LeaveBlank', 'KeepPlaceholder', 'Error'")]
    public Input<string> MissingKeyBehavior { get; set; } = new("LeaveBlank");

    /// <summary>
    /// Rendered text with values substituted
    /// </summary>
    [Output(Description = "Rendered text with values substituted")]
    public Output<string> Result { get; set; } = default!;

    /// <summary>
    /// JSON array of placeholder keys not found in data
    /// </summary>
    [Output(Description = "JSON array of missing keys")]
    public Output<string> MissingKeys { get; set; } = default!;

    /// <summary>
    /// Whether rendering succeeded
    /// </summary>
    [Output(Description = "Whether rendering succeeded")]
    public Output<bool> Success { get; set; } = default!;

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var template = context.Get(Template);
            var dataJson = context.Get(Data);
            var missingKeyBehavior = context.Get(MissingKeyBehavior) ?? "LeaveBlank";

            if (string.IsNullOrWhiteSpace(template))
                throw new ArgumentException("'Template' is required");

            if (string.IsNullOrWhiteSpace(dataJson))
                throw new ArgumentException("'Data' is required");

            // Parse data
            using var doc = System.Text.Json.JsonDocument.Parse(dataJson);
            var data = doc.RootElement;

            // Render template
            var (result, missingKeys) = RenderTemplateInternal(template, data, missingKeyBehavior);

            context.Set(Result, result);
            context.Set(MissingKeys, System.Text.Json.JsonSerializer.Serialize(missingKeys));
            context.Set(Success, true);
        }
        catch (Exception ex)
        {
            context.Set(Result, "");
            context.Set(MissingKeys, "[]");
            context.Set(Success, false);
        }
    }

    private (string result, List<string> missingKeys) RenderTemplateInternal(
        string template, System.Text.Json.JsonElement data, string behavior)
    {
        var result = template;
        var missingKeys = new List<string>();

        // Find all placeholders {{...}}
        var regex = new System.Text.RegularExpressions.Regex(@"\{\{([^}]+)\}\}");
        var matches = regex.Matches(template);

        foreach (System.Text.RegularExpressions.Match match in matches)
        {
            var key = match.Groups[1].Value;
            var value = GetNestedValue(data, key);

            if (value == null)
            {
                missingKeys.Add(key);

                switch (behavior.ToLower())
                {
                    case "error":
                        throw new KeyNotFoundException($"Key not found: {key}");

                    case "keepplaceholder":
                        // Keep original placeholder
                        break;

                    case "leaveblank":
                    default:
                        result = result.Replace($"{{{{{key}}}}}", "");
                        break;
                }
            }
            else
            {
                result = result.Replace($"{{{{{key}}}}}", value);
            }
        }

        return (result, missingKeys);
    }

    private string? GetNestedValue(System.Text.Json.JsonElement element, string path)
    {
        var parts = path.Split('.');
        var current = element;

        foreach (var part in parts)
        {
            if (current.ValueKind == System.Text.Json.JsonValueKind.Object && 
                current.TryGetProperty(part, out var next))
            {
                current = next;
            }
            else
            {
                return null;
            }
        }

        return current.ValueKind switch
        {
            System.Text.Json.JsonValueKind.String => current.GetString(),
            System.Text.Json.JsonValueKind.Number => current.GetDecimal().ToString(),
            System.Text.Json.JsonValueKind.True => "true",
            System.Text.Json.JsonValueKind.False => "false",
            System.Text.Json.JsonValueKind.Null => null,
            _ => current.GetRawText()
        };
    }
}
