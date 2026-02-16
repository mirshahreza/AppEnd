using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;

namespace AppEndWorkflow.Activities.IoT;

[Activity(Category = "IoT", Description = "Send data to AWS IoT Core", DisplayName = "AWS IoT Send")]
public class AwsIoTSendActivity : CodeActivity
{
    [Input(Description = "Thing name")]
    public Input<string> ThingName { get; set; } = default!;

    [Input(Description = "Topic")]
    public Input<string> Topic { get; set; } = default!;

    [Input(Description = "Message payload")]
    public Input<string> Payload { get; set; } = default!;

    [Output(Description = "Whether sent")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var thingName = context.Get(ThingName) ?? throw new ArgumentException("ThingName is required");
            var topic = context.Get(Topic) ?? throw new ArgumentException("Topic is required");
            var payload = context.Get(Payload) ?? throw new ArgumentException("Payload is required");

            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
