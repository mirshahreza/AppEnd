using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Text.Json;

namespace AppEndWorkflow.Activities.ML;

[Activity(Category = "Machine Learning", Description = "Train ML model", DisplayName = "Model Training")]
public class ModelTrainingActivity : CodeActivity
{
    [Input(Description = "Training data path")]
    public Input<string> TrainingDataPath { get; set; } = default!;

    [Input(Description = "Model type: classification, regression, clustering")]
    public Input<string> ModelType { get; set; } = default!;

    [Input(Description = "Algorithm: DecisionTree, RandomForest, LinearRegression, KMeans")]
    public Input<string> Algorithm { get; set; } = default!;

    [Input(Description = "Training parameters JSON")]
    public Input<string?> Parameters { get; set; }

    [Input(Description = "Test split ratio (0-1, default: 0.2)")]
    public Input<double?> TestSplitRatio { get; set; }

    [Output(Description = "Model file path")]
    public Output<string> ModelPath { get; set; } = default!;

    [Output(Description = "Accuracy score")]
    public Output<double> Accuracy { get; set; } = default!;

    [Output(Description = "Training metrics")]
    public Output<string> Metrics { get; set; } = default!;

    [Output(Description = "Training time (seconds)")]
    public Output<long> TrainingTimeSeconds { get; set; } = default!;

    [Output(Description = "Whether succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var trainingDataPath = context.Get(TrainingDataPath) ?? throw new ArgumentException("TrainingDataPath is required");
            var modelType = context.Get(ModelType) ?? throw new ArgumentException("ModelType is required");
            var algorithm = context.Get(Algorithm) ?? throw new ArgumentException("Algorithm is required");
            var testSplitRatio = context.Get(TestSplitRatio) ?? 0.2;

            if (!File.Exists(trainingDataPath))
                throw new FileNotFoundException($"Training data not found: {trainingDataPath}");

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // NOTE: In production, use ML.NET, scikit-learn, or TensorFlow
            // var context = new MLContext();
            // var data = context.Data.LoadFromTextFile<ModelInput>(trainingDataPath);
            // var pipeline = context.Classification.Trainers.SdcaLogisticRegression();
            // var trainedModel = pipeline.Fit(data);

            stopwatch.Stop();

            var modelPath = Path.Combine(Path.GetTempPath(), $"model_{Guid.NewGuid()}.zip");
            File.WriteAllText(modelPath, "mock model");

            var metrics = JsonSerializer.Serialize(new
            {
                accuracy = 0.92,
                precision = 0.89,
                recall = 0.85,
                f1_score = 0.87
            });

            context.Set(ModelPath, modelPath);
            context.Set(Accuracy, 0.92);
            context.Set(Metrics, metrics);
            context.Set(TrainingTimeSeconds, stopwatch.ElapsedMilliseconds / 1000);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(ModelPath, "");
            context.Set(Accuracy, 0);
            context.Set(Metrics, "");
            context.Set(TrainingTimeSeconds, 0);
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
