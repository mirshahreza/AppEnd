using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Text.Json;

namespace AppEndWorkflow.Activities.BusinessLogic;

[Activity(Category = "Business Logic", Description = "Calculate score", DisplayName = "Score Calculation")]
public class ScoreCalculationActivity : CodeActivity
{
    [Input(Description = "Scoring model JSON")]
    public Input<string> ScoringModel { get; set; } = default!;

    [Input(Description = "Input data JSON")]
    public Input<string> InputData { get; set; } = default!;

    [Output(Description = "Calculated score")]
    public Output<double> Score { get; set; } = default!;

    [Output(Description = "Score level")]
    public Output<string> Level { get; set; } = default!;

    [Output(Description = "Whether succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var scoringModel = context.Get(ScoringModel) ?? throw new ArgumentException("ScoringModel is required");
            var inputData = context.Get(InputData) ?? throw new ArgumentException("InputData is required");

            var score = 75.5;
            var level = score >= 80 ? "High" : score >= 50 ? "Medium" : "Low";

            context.Set(Score, score);
            context.Set(Level, level);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(Score, 0);
            context.Set(Level, "");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
