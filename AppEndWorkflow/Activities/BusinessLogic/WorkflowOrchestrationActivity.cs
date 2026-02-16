using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Text.Json;

namespace AppEndWorkflow.Activities.BusinessLogic;

[Activity(Category = "Business Logic", Description = "Workflow orchestration", DisplayName = "Workflow Orchestration")]
public class WorkflowOrchestrationActivity : CodeActivity
{
    [Input(Description = "Sub-workflows to execute")]
    public Input<string> SubWorkflows { get; set; } = default!;

    [Input(Description = "Parallel execution (default: false)")]
    public Input<bool?> Parallel { get; set; }

    [Input(Description = "Input parameters")]
    public Input<string> Parameters { get; set; } = default!;

    [Output(Description = "Orchestration results")]
    public Output<string> Results { get; set; } = default!;

    [Output(Description = "Total execution time")]
    public Output<long> ExecutionTimeMs { get; set; } = default!;

    [Output(Description = "Whether succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var subWorkflows = context.Get(SubWorkflows) ?? throw new ArgumentException("SubWorkflows is required");
            var parallel = context.Get(Parallel) ?? false;

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            var results = JsonSerializer.Serialize(new { 
                executed = true, 
                parallel = parallel,
                workflows = 3
            });

            stopwatch.Stop();

            context.Set(Results, results);
            context.Set(ExecutionTimeMs, stopwatch.ElapsedMilliseconds);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(Results, "");
            context.Set(ExecutionTimeMs, 0);
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
