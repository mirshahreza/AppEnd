using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Globalization;

namespace AppEndWorkflow.Activities.Text;

/// <summary>
/// Formats a string using composite format string.
/// Supports culture-specific formatting.
/// </summary>
[Activity(
    Category = "Text",
    Description = "Formats string with arguments",
    DisplayName = "Format String"
)]
public class FormatStringActivity : CodeActivity
{
    /// <summary>
    /// Format string (e.g., "Order {0} created on {1}")
    /// </summary>
    [Input(Description = "Format string")]
    public Input<string> Format { get; set; } = default!;

    /// <summary>
    /// JSON array of arguments
    /// </summary>
    [Input(Description = "JSON array of arguments")]
    public Input<string> Arguments { get; set; } = default!;

    /// <summary>
    /// Culture name for formatting (e.g., "fa-IR", "en-US")
    /// </summary>
    [Input(Description = "Culture name for formatting")]
    public Input<string?> Culture { get; set; }

    /// <summary>
    /// Formatted string
    /// </summary>
    [Output(Description = "Formatted string")]
    public Output<string> Result { get; set; } = default!;

    /// <summary>
    /// Whether formatting succeeded
    /// </summary>
    [Output(Description = "Whether formatting succeeded")]
    public Output<bool> Success { get; set; } = default!;

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var format = context.Get(Format);
            var argumentsJson = context.Get(Arguments);
            var cultureStr = context.Get(Culture);

            if (string.IsNullOrWhiteSpace(format))
                throw new ArgumentException("'Format' is required");

            if (string.IsNullOrWhiteSpace(argumentsJson))
                throw new ArgumentException("'Arguments' is required");

            // Parse arguments
            using var doc = System.Text.Json.JsonDocument.Parse(argumentsJson);
            if (doc.RootElement.ValueKind != System.Text.Json.JsonValueKind.Array)
                throw new ArgumentException("Arguments must be a JSON array");

            var args = new List<object?>();
            foreach (var element in doc.RootElement.EnumerateArray())
            {
                var value = element.ValueKind switch
                {
                    System.Text.Json.JsonValueKind.String => (object?)(element.GetString()),
                    System.Text.Json.JsonValueKind.Number => element.GetDecimal(),
                    System.Text.Json.JsonValueKind.True => true,
                    System.Text.Json.JsonValueKind.False => false,
                    System.Text.Json.JsonValueKind.Null => null,
                    _ => element.GetRawText()
                };
                args.Add(value);
            }

            // Get culture
            CultureInfo culture = CultureInfo.InvariantCulture;
            if (!string.IsNullOrWhiteSpace(cultureStr))
            {
                try
                {
                    culture = CultureInfo.GetCultureInfo(cultureStr);
                }
                catch
                {
                    culture = CultureInfo.InvariantCulture;
                }
            }

            // Format string
            var result = string.Format(culture, format, args.ToArray());

            context.Set(Result, result);
            context.Set(Success, true);
        }
        catch (Exception ex)
        {
            context.Set(Result, "");
            context.Set(Success, false);
        }
    }
}
