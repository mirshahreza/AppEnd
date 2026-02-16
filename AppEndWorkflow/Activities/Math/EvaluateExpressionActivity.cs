using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Data;

namespace AppEndWorkflow.Activities.Math;

/// <summary>
/// Evaluates mathematical expressions with variable substitution.
/// Uses DataTable.Compute for safe expression evaluation.
/// </summary>
[Activity(
    Category = "Math",
    Description = "Evaluates math expression",
    DisplayName = "Evaluate Expression"
)]
public class EvaluateExpressionActivity : CodeActivity
{
    /// <summary>
    /// Math expression (e.g., "(Price * Quantity) - (Price * Quantity * DiscountRate)")
    /// </summary>
    [Input(Description = "Math expression")]
    public Input<string> Expression { get; set; } = default!;

    /// <summary>
    /// JSON object of variable values
    /// </summary>
    [Input(Description = "JSON object of variable values")]
    public Input<string> Variables { get; set; } = default!;

    /// <summary>
    /// Calculation result
    /// </summary>
    [Output(Description = "Calculation result")]
    public Output<double> Result { get; set; } = default!;

    /// <summary>
    /// Formatted result string
    /// </summary>
    [Output(Description = "Formatted result string")]
    public Output<string> FormattedResult { get; set; } = default!;

    /// <summary>
    /// Whether evaluation succeeded
    /// </summary>
    [Output(Description = "Whether evaluation succeeded")]
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
            var expression = context.Get(Expression);
            var variablesJson = context.Get(Variables);

            if (string.IsNullOrWhiteSpace(expression))
                throw new ArgumentException("'Expression' is required");

            if (string.IsNullOrWhiteSpace(variablesJson))
                throw new ArgumentException("'Variables' is required");

            // Parse variables
            using var doc = System.Text.Json.JsonDocument.Parse(variablesJson);
            var variables = new Dictionary<string, object?>();
            foreach (var property in doc.RootElement.EnumerateObject())
            {
                var value = property.Value.ValueKind switch
                {
                    System.Text.Json.JsonValueKind.Number => (object?)property.Value.GetDecimal(),
                    System.Text.Json.JsonValueKind.String => property.Value.GetString(),
                    _ => null
                };
                variables[property.Name] = value;
            }

            // Replace variables in expression
            var evaluableExpression = expression;
            foreach (var (name, value) in variables)
            {
                if (value is decimal dec)
                    evaluableExpression = evaluableExpression.Replace($"{{{{{name}}}}}", dec.ToString());
                else if (value is string str)
                    evaluableExpression = evaluableExpression.Replace($"{{{{{name}}}}}", str);
            }

            // Evaluate using DataTable
            var result = EvaluateMathExpression(evaluableExpression);

            context.Set(Result, result);
            context.Set(FormattedResult, result.ToString("F2"));
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(Result, 0);
            context.Set(FormattedResult, "0.00");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }

    private double EvaluateMathExpression(string expression)
    {
        var dt = new DataTable();
        var result = dt.Compute(expression, null);
        return Convert.ToDouble(result);
    }
}
