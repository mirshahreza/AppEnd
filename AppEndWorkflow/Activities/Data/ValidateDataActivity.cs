using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace AppEndWorkflow.Activities.Data;

/// <summary>
/// Validates data against a set of rules.
/// Supports nested field paths (e.g., "Order.Customer.Email").
/// </summary>
[Activity(
    Category = "Data",
    Description = "Validates data against rules",
    DisplayName = "Validate Data"
)]
public class ValidateDataActivity : CodeActivity
{
    /// <summary>
    /// JSON data to validate
    /// </summary>
    [Input(Description = "JSON data to validate")]
    public Input<string> Data { get; set; } = default!;

    /// <summary>
    /// JSON validation rules [{"Field", "Rule", "Message"}]
    /// Rules: required, minLength, maxLength, regex, range, email, numeric
    /// </summary>
    [Input(Description = "JSON validation rules")]
    public Input<string> Rules { get; set; } = default!;

    /// <summary>
    /// If true, stops at first validation error
    /// </summary>
    [Input(Description = "Stop on first error")]
    public Input<bool> StopOnFirstError { get; set; } = new(false);

    /// <summary>
    /// Whether all validations passed
    /// </summary>
    [Output(Description = "Whether all validations passed")]
    public Output<bool> IsValid { get; set; } = default!;

    /// <summary>
    /// JSON array of {Field, Rule, Message} for failed validations
    /// </summary>
    [Output(Description = "JSON array of validation errors")]
    public Output<string> Errors { get; set; } = default!;

    /// <summary>
    /// Number of validation errors
    /// </summary>
    [Output(Description = "Number of validation errors")]
    public Output<int> ErrorCount { get; set; } = default!;

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var dataJson = context.Get(Data);
            var rulesJson = context.Get(Rules);
            var stopOnFirstError = context.Get(StopOnFirstError);

            if (string.IsNullOrWhiteSpace(dataJson))
                throw new ArgumentException("'Data' is required");

            if (string.IsNullOrWhiteSpace(rulesJson))
                throw new ArgumentException("'Rules' is required");

            // Validate JSON format
            using var dataDoc = JsonDocument.Parse(dataJson);
            using var rulesDoc = JsonDocument.Parse(rulesJson);

            var (isValid, errors) = ValidateData(dataDoc.RootElement, rulesDoc.RootElement, stopOnFirstError);

            context.Set(IsValid, isValid);
            context.Set(Errors, JsonSerializer.Serialize(errors));
            context.Set(ErrorCount, errors.Count);
        }
        catch (Exception ex)
        {
            context.Set(IsValid, false);
            context.Set(Errors, JsonSerializer.Serialize(new[] { new { error = ex.Message } }));
            context.Set(ErrorCount, 1);
        }
    }

    private (bool isValid, List<Dictionary<string, string>>) ValidateData(JsonElement data, JsonElement rules, bool stopOnFirstError)
    {
        var errors = new List<Dictionary<string, string>>();

        if (rules.ValueKind != JsonValueKind.Array)
            throw new InvalidOperationException("Rules must be a JSON array");

        foreach (var rule in rules.EnumerateArray())
        {
            var field = rule.GetProperty("Field").GetString();
            var ruleType = rule.GetProperty("Rule").GetString();
            var message = rule.GetProperty("Message").GetString() ?? $"Validation failed for {field}";

            if (string.IsNullOrWhiteSpace(field) || string.IsNullOrWhiteSpace(ruleType))
                continue;

            var value = GetNestedValue(data, field);

            if (!ValidateField(value, ruleType, rule))
            {
                errors.Add(new Dictionary<string, string>
                {
                    { "Field", field },
                    { "Rule", ruleType },
                    { "Message", message }
                });

                if (stopOnFirstError)
                    break;
            }
        }

        return (errors.Count == 0, errors);
    }

    private JsonElement? GetNestedValue(JsonElement data, string path)
    {
        var parts = path.Split('.');
        JsonElement current = data;

        foreach (var part in parts)
        {
            if (current.ValueKind == JsonValueKind.Object && current.TryGetProperty(part, out var next))
            {
                current = next;
            }
            else
            {
                return null;
            }
        }

        return current;
    }

    private bool ValidateField(JsonElement? value, string rule, JsonElement ruleObj)
    {
        var ruleTypeLower = rule.ToLower();

        if (value == null)
            return ruleTypeLower != "required";

        switch (ruleTypeLower)
        {
            case "required":
                return value.Value.ValueKind != JsonValueKind.Null;

            case "minlength":
                if (value.Value.ValueKind == JsonValueKind.String)
                {
                    var minLength = ruleObj.TryGetProperty("MinLength", out var ml) ? ml.GetInt32() : 0;
                    return value.Value.GetString()?.Length >= minLength;
                }
                return true;

            case "maxlength":
                if (value.Value.ValueKind == JsonValueKind.String)
                {
                    var maxLength = ruleObj.TryGetProperty("MaxLength", out var ml) ? ml.GetInt32() : int.MaxValue;
                    return value.Value.GetString()?.Length <= maxLength;
                }
                return true;

            case "regex":
                if (value.Value.ValueKind == JsonValueKind.String)
                {
                    var pattern = ruleObj.TryGetProperty("Pattern", out var p) ? p.GetString() : null;
                    if (string.IsNullOrWhiteSpace(pattern))
                        return true;
                    return Regex.IsMatch(value.Value.GetString() ?? "", pattern);
                }
                return true;

            case "email":
                if (value.Value.ValueKind == JsonValueKind.String)
                {
                    var email = value.Value.GetString() ?? "";
                    return Regex.IsMatch(email, @"^[^\s@]+@[^\s@]+\.[^\s@]+$");
                }
                return true;

            case "numeric":
                if (value.Value.ValueKind == JsonValueKind.String)
                {
                    return double.TryParse(value.Value.GetString(), out _);
                }
                return value.Value.ValueKind == JsonValueKind.Number;

            default:
                return true;
        }
    }
}
