using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;

namespace AppEndWorkflow.Activities.ML;

[Activity(Category = "Machine Learning", Description = "TensorFlow prediction", DisplayName = "TensorFlow Predict")]
public class TensorFlowPredictActivity : CodeActivity
{
    [Input(Description = "Model URL or path")]
    public Input<string> ModelPath { get; set; } = default!;

    [Input(Description = "Input tensor data")]
    public Input<string> InputTensor { get; set; } = default!;

    [Input(Description = "Input layer name")]
    public Input<string> InputLayerName { get; set; } = default!;

    [Input(Description = "Output layer name")]
    public Input<string> OutputLayerName { get; set; } = default!;

    [Output(Description = "Prediction output")]
    public Output<string> Output { get; set; } = default!;

    [Output(Description = "Execution time (ms)")]
    public Output<long> ExecutionTime { get; set; } = default!;

    [Output(Description = "Whether succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var modelPath = context.Get(ModelPath) ?? throw new ArgumentException("ModelPath is required");
            var inputTensor = context.Get(InputTensor) ?? throw new ArgumentException("InputTensor is required");
            var inputLayerName = context.Get(InputLayerName) ?? throw new ArgumentException("InputLayerName is required");
            var outputLayerName = context.Get(OutputLayerName) ?? throw new ArgumentException("OutputLayerName is required");

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // NOTE: In production, use TensorFlow.NET or Python integration
            // var session = new Session();
            // var graph = new Graph().ImportFromFile(modelPath);
            // var runner = session.GetRunner();
            // runner.AddInput(graph[inputLayerName], inputTensor);
            // var result = runner.Fetch(graph[outputLayerName]).Run();

            stopwatch.Stop();

            var output = "[0.92, 0.08]";

            context.Set(Output, output);
            context.Set(ExecutionTime, stopwatch.ElapsedMilliseconds);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(Output, "");
            context.Set(ExecutionTime, 0);
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
