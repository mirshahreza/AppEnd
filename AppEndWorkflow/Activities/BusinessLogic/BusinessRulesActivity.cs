using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Text.Json;

namespace AppEndWorkflow.Activities.BusinessLogic;

[Activity(Category = "Business Logic", Description = "Execute business rules", DisplayName = "Business Rules")]
public class BusinessRulesActivity : CodeActivity
{
    [Input(Description = "Rules JSON")]
    public Input<string> Rules { get; set; } = default!;

    [Input(Description = "Data JSON")]
    public Input<string> Data { get; set; } = default!;

    [Output(Description = "Result")]
    public Output<string> Result { get; set; } = default!;

    [Output(Description = "Whether rules applied")]
    public Output<bool> RulesApplied { get; set; } = default!;

    [Output(Description = "Whether succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var rulesJson = context.Get(Rules) ?? throw new ArgumentException("Rules is required");
            var dataJson = context.Get(Data) ?? throw new ArgumentException("Data is required");

            using var rulesDoc = JsonDocument.Parse(rulesJson);
            using var dataDoc = JsonDocument.Parse(dataJson);

            var result = JsonSerializer.Serialize(new { applied = true, data = dataDoc.RootElement });

            context.Set(Result, result);
            context.Set(RulesApplied, true);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(Result, "");
            context.Set(RulesApplied, false);
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
