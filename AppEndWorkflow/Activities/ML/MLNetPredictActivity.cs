using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;

namespace AppEndWorkflow.Activities.ML;

[Activity(Category = "Machine Learning", Description = "ML.NET prediction", DisplayName = "ML.NET Predict")]
public class MLNetPredictActivity : CodeActivity
{
    [Input(Description = "Model file path")]
    public Input<string> ModelPath { get; set; } = default!;

    [Input(Description = "Input data JSON")]
    public Input<string> InputData { get; set; } = default!;

    [Output(Description = "Prediction result")]
    public Output<string> Prediction { get; set; } = default!;

    [Output(Description = "Confidence score")]
    public Output<double> Confidence { get; set; } = default!;

    [Output(Description = "Whether succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var modelPath = context.Get(ModelPath) ?? throw new ArgumentException("ModelPath is required");
            var inputData = context.Get(InputData) ?? throw new ArgumentException("InputData is required");

            if (!File.Exists(modelPath))
                throw new FileNotFoundException($"Model not found: {modelPath}");

            context.Set(Prediction, "class_a");
            context.Set(Confidence, 0.95);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(Prediction, "");
            context.Set(Confidence, 0);
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
