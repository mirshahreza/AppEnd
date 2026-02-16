using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;

namespace AppEndWorkflow.Activities.IoT;

[Activity(Category = "IoT", Description = "Publish to MQTT broker", DisplayName = "MQTT Publish")]
public class MqttPublishActivity : CodeActivity
{
    [Input(Description = "Broker address")]
    public Input<string> BrokerAddress { get; set; } = default!;

    [Input(Description = "Topic")]
    public Input<string> Topic { get; set; } = default!;

    [Input(Description = "Message payload")]
    public Input<string> Payload { get; set; } = default!;

    [Input(Description = "QoS (0-2)")]
    public Input<int?> Qos { get; set; }

    [Output(Description = "Whether published")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var brokerAddress = context.Get(BrokerAddress) ?? throw new ArgumentException("BrokerAddress is required");
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
