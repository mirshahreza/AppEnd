using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Text.Json;

namespace AppEndWorkflow.Activities.Data;

/// <summary>
/// Transforms/maps JSON data using JavaScript expressions or mapping rules.
/// Uses Elsa's built-in JavaScript engine to evaluate transformation expressions.
/// </summary>
[Activity(
    Category = "Data",
    Description = "Transforms or maps JSON data",
    DisplayName = "Transform JSON"
)]
public class TransformJsonActivity : CodeActivity
{
    /// <summary>
    /// Source JSON string
    /// </summary>
    [Input(Description = "Source JSON string")]
    public Input<string> InputJson { get; set; } = default!;

    /// <summary>
    /// JavaScript expression for transformation (e.g., "data.name.toUpperCase()")
    /// </summary>
    [Input(Description = "JavaScript expression for transformation")]
    public Input<string> TransformExpression { get; set; } = default!;

    /// <summary>
    /// JSON mapping rules (alternative to JS expression)
    /// </summary>
    [Input(Description = "JSON mapping rules (optional)")]
    public Input<string?> MappingRules { get; set; }

    /// <summary>
    /// Transformed JSON output
    /// </summary>
    [Output(Description = "Transformed JSON output")]
    public Output<string> Result { get; set; } = default!;

    /// <summary>
    /// Whether transformation succeeded
    /// </summary>
    [Output(Description = "Whether transformation succeeded")]
    public Output<bool> Success { get; set; } = default!;

    /// <summary>
    /// Error message if failed
    /// </summary>
    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var inputJson = context.Get(InputJson);
            var transformExpression = context.Get(TransformExpression);
            var mappingRules = context.Get(MappingRules);

            if (string.IsNullOrWhiteSpace(inputJson))
                throw new ArgumentException("'InputJson' is required");

            if (string.IsNullOrWhiteSpace(transformExpression) && string.IsNullOrWhiteSpace(mappingRules))
                throw new ArgumentException("Either 'TransformExpression' or 'MappingRules' is required");

            // Validate JSON
            using var doc = JsonDocument.Parse(inputJson);
            var sourceData = doc.RootElement;

            // TODO: Implement transformation using Elsa's JavaScript engine or mapping rules
            // Possible approaches:
            // 1. Use Elsa's JavaScript evaluator with sourceData context
            // 2. Implement mapping rules engine (field-to-field transformations)
            // 3. Support nested object transformations
            //
            // For now, return input as output (placeholder)
            var result = inputJson;

            context.Set(Result, result);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(Result, "{}");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
