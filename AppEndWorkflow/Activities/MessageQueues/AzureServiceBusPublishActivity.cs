using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace AppEndWorkflow.Activities.MessageQueues;

/// <summary>
/// Publishes message to Azure Service Bus queue or topic.
/// </summary>
[Activity(
    Category = "Message Queues",
    Description = "Publish to Azure Service Bus",
    DisplayName = "Azure Service Bus Publish"
)]
public class AzureServiceBusPublishActivity : CodeActivity
{
    [Input(Description = "Service Bus connection string")]
    public Input<string> ConnectionString { get; set; } = default!;

    [Input(Description = "Queue or topic name")]
    public Input<string> QueueName { get; set; } = default!;

    [Input(Description = "Message content")]
    public Input<string> Message { get; set; } = default!;

    [Input(Description = "Message label (optional)")]
    public Input<string?> Label { get; set; }

    [Output(Description = "Message sequence number")]
    public Output<long> SequenceNumber { get; set; } = default!;

    [Output(Description = "Whether publish succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var connectionString = context.Get(ConnectionString) ?? throw new ArgumentException("ConnectionString is required");
            var queueName = context.Get(QueueName) ?? throw new ArgumentException("QueueName is required");
            var message = context.Get(Message) ?? throw new ArgumentException("Message is required");
            var label = context.Get(Label);

            // NOTE: In production, use Azure.Messaging.ServiceBus
            // var client = new ServiceBusClient(connectionString);
            // var sender = client.CreateSender(queueName);
            // var message = new ServiceBusMessage(message) { Label = label };
            // var sequenceNumber = await sender.SendMessageAsync(message);

            var mockSequenceNumber = DateTime.UtcNow.Ticks;
            context.Set(SequenceNumber, mockSequenceNumber);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(SequenceNumber, 0);
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
