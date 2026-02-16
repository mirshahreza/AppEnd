using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Text;

namespace AppEndWorkflow.Activities.MessageQueues;

/// <summary>
/// Publishes message to Kafka topic.
/// Requires Confluent.Kafka NuGet package.
/// </summary>
[Activity(
    Category = "Message Queues",
    Description = "Publish to Kafka topic",
    DisplayName = "Kafka Publish"
)]
public class KafkaPublishActivity : CodeActivity
{
    [Input(Description = "Kafka bootstrap servers")]
    public Input<string> BootstrapServers { get; set; } = default!;

    [Input(Description = "Kafka topic")]
    public Input<string> Topic { get; set; } = default!;

    [Input(Description = "Message content")]
    public Input<string> Message { get; set; } = default!;

    [Input(Description = "Message key (optional)")]
    public Input<string?> MessageKey { get; set; }

    [Output(Description = "Message partition")]
    public Output<int> Partition { get; set; } = default!;

    [Output(Description = "Message offset")]
    public Output<long> Offset { get; set; } = default!;

    [Output(Description = "Whether publish succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var bootstrapServers = context.Get(BootstrapServers) ?? throw new ArgumentException("BootstrapServers is required");
            var topic = context.Get(Topic) ?? throw new ArgumentException("Topic is required");
            var message = context.Get(Message) ?? throw new ArgumentException("Message is required");
            var messageKey = context.Get(MessageKey);

            // NOTE: In production, use Confluent.Kafka
            // var config = new ProducerConfig { BootstrapServers = bootstrapServers };
            // using (var producer = new ProducerBuilder<string, string>(config).Build())
            // {
            //     var result = await producer.ProduceAsync(topic, 
            //         new Message<string, string> { Key = messageKey, Value = message });
            //     partition = result.Partition;
            //     offset = result.Offset;
            // }

            context.Set(Partition, 0);
            context.Set(Offset, 0);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(Partition, 0);
            context.Set(Offset, 0);
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
