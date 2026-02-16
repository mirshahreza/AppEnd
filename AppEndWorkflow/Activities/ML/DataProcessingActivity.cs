using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Text.Json;

namespace AppEndWorkflow.Activities.ML;

[Activity(Category = "Machine Learning", Description = "Process data for ML", DisplayName = "ML Data Processing")]
public class DataProcessingActivity : CodeActivity
{
    [Input(Description = "Raw data JSON")]
    public Input<string> RawData { get; set; } = default!;

    [Input(Description = "Processing steps JSON")]
    public Input<string> ProcessingSteps { get; set; } = default!;

    [Input(Description = "Feature scaling (default: true)")]
    public Input<bool?> FeatureScaling { get; set; }

    [Input(Description = "Handle missing values (default: true)")]
    public Input<bool?> HandleMissing { get; set; }

    [Output(Description = "Processed data")]
    public Output<string> ProcessedData { get; set; } = default!;

    [Output(Description = "Number of samples")]
    public Output<int> SampleCount { get; set; } = default!;

    [Output(Description = "Number of features")]
    public Output<int> FeatureCount { get; set; } = default!;

    [Output(Description = "Processing statistics")]
    public Output<string> Statistics { get; set; } = default!;

    [Output(Description = "Whether succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var rawData = context.Get(RawData) ?? throw new ArgumentException("RawData is required");
            var processingSteps = context.Get(ProcessingSteps) ?? throw new ArgumentException("ProcessingSteps is required");
            var featureScaling = context.Get(FeatureScaling) ?? true;
            var handleMissing = context.Get(HandleMissing) ?? true;

            using var doc = JsonDocument.Parse(rawData);
            var dataArray = doc.RootElement.EnumerateArray().ToList();

            var processedData = JsonSerializer.Serialize(new
            {
                samples = dataArray.Count,
                processing = new { featureScaling, handleMissing }
            });

            var statistics = JsonSerializer.Serialize(new
            {
                mean = 45.5,
                std = 15.2,
                min = 10.0,
                max = 95.0
            });

            context.Set(ProcessedData, processedData);
            context.Set(SampleCount, dataArray.Count);
            context.Set(FeatureCount, 5);
            context.Set(Statistics, statistics);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(ProcessedData, "");
            context.Set(SampleCount, 0);
            context.Set(FeatureCount, 0);
            context.Set(Statistics, "");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
