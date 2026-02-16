using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;

namespace AppEndWorkflow.Activities.FlowControl;

/// <summary>
/// Multi-branch switch activity based on expression value.
/// More readable than nested If/Else chains.
/// </summary>
[Activity(
    Category = "Flow Control",
    Description = "Multi-branch switch",
    DisplayName = "Switch"
)]
public class SwitchActivity : CodeActivity
{
    /// <summary>
    /// Value to evaluate
    /// </summary>
    [Input(Description = "Value to evaluate")]
    public Input<string> Expression { get; set; } = default!;

    /// <summary>
    /// JSON object of {"value": "branchName"} mappings
    /// </summary>
    [Input(Description = "Cases mapping")]
    public Input<string> Cases { get; set; } = default!;

    /// <summary>
    /// Branch name for unmatched values
    /// </summary>
    [Input(Description = "Default branch")]
    public Input<string?> DefaultBranch { get; set; }

    /// <summary>
    /// The case value that matched
    /// </summary>
    [Output(Description = "The case value that matched")]
    public Output<string> MatchedCase { get; set; } = default!;

    /// <summary>
    /// The branch that was selected
    /// </summary>
    [Output(Description = "The branch that was selected")]
    public Output<string> SelectedBranch { get; set; } = default!;

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var expression = context.Get(Expression);
            var casesJson = context.Get(Cases);
            var defaultBranch = context.Get(DefaultBranch);

            if (string.IsNullOrWhiteSpace(expression))
                throw new ArgumentException("'Expression' is required");

            if (string.IsNullOrWhiteSpace(casesJson))
                throw new ArgumentException("'Cases' is required");

            // Parse cases
            using var doc = System.Text.Json.JsonDocument.Parse(casesJson);
            var cases = new Dictionary<string, string>();
            foreach (var property in doc.RootElement.EnumerateObject())
            {
                cases[property.Name] = property.Value.GetString() ?? "";
            }

            // Find matching case
            if (cases.TryGetValue(expression, out var branch))
            {
                context.Set(MatchedCase, expression);
                context.Set(SelectedBranch, branch);
            }
            else if (!string.IsNullOrWhiteSpace(defaultBranch))
            {
                context.Set(MatchedCase, "default");
                context.Set(SelectedBranch, defaultBranch);
            }
            else
            {
                context.Set(MatchedCase, "");
                context.Set(SelectedBranch, "");
            }
        }
        catch (Exception ex)
        {
            context.Set(MatchedCase, "");
            context.Set(SelectedBranch, "");
        }
    }
}
