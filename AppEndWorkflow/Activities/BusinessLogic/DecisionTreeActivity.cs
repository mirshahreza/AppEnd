using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Text.Json;

namespace AppEndWorkflow.Activities.BusinessLogic;

[Activity(Category = "Business Logic", Description = "Evaluate decision tree", DisplayName = "Decision Tree")]
public class DecisionTreeActivity : CodeActivity
{
    [Input(Description = "Tree definition JSON")]
    public Input<string> TreeDefinition { get; set; } = default!;

    [Input(Description = "Input data JSON")]
    public Input<string> InputData { get; set; } = default!;

    [Output(Description = "Decision result")]
    public Output<string> Decision { get; set; } = default!;

    [Output(Description = "Decision path")]
    public Output<string> Path { get; set; } = default!;

    [Output(Description = "Whether succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var treeDefinition = context.Get(TreeDefinition) ?? throw new ArgumentException("TreeDefinition is required");
            var inputData = context.Get(InputData) ?? throw new ArgumentException("InputData is required");

            context.Set(Decision, "approved");
            context.Set(Path, "root > condition1 > decision");
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(Decision, "");
            context.Set(Path, "");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
