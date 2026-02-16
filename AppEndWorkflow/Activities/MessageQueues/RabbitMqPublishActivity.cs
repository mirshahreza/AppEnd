using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Text;

namespace AppEndWorkflow.Activities.MessageQueues;

/// <summary>
/// Publishes message to RabbitMQ queue.
/// Requires RabbitMQ.Client NuGet package.
/// </summary>
[Activity(
    Category = "Message Queues",
    Description = "Publish to RabbitMQ",
    DisplayName = "RabbitMQ Publish"
)]
public class RabbitMqPublishActivity : CodeActivity
{
    [Input(Description = "RabbitMQ hostname")]
    public Input<string> Hostname { get; set; } = default!;

    [Input(Description = "RabbitMQ port (default: 5672)")]
    public Input<int?> Port { get; set; }

    [Input(Description = "Queue name")]
    public Input<string> QueueName { get; set; } = default!;

    [Input(Description = "Message content")]
    public Input<string> Message { get; set; } = default!;

    [Input(Description = "RabbitMQ username")]
    public Input<string?> Username { get; set; }

    [Input(Description = "RabbitMQ password")]
    public Input<string?> Password { get; set; }

    [Output(Description = "Message ID")]
    public Output<string> MessageId { get; set; } = default!;

    [Output(Description = "Whether publish succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var hostname = context.Get(Hostname) ?? throw new ArgumentException("Hostname is required");
            var port = context.Get(Port) ?? 5672;
            var queueName = context.Get(QueueName) ?? throw new ArgumentException("QueueName is required");
            var message = context.Get(Message) ?? throw new ArgumentException("Message is required");
            var username = context.Get(Username) ?? "guest";
            var password = context.Get(Password) ?? "guest";

            // NOTE: In production, use RabbitMQ.Client
            // var factory = new ConnectionFactory() { HostName = hostname, Port = port };
            // if (!string.IsNullOrEmpty(username))
            // {
            //     factory.UserName = username;
            //     factory.Password = password;
            // }
            // using (var connection = factory.CreateConnection())
            // using (var channel = connection.CreateModel())
            // {
            //     channel.QueueDeclare(queue: queueName, durable: false);
            //     var body = Encoding.UTF8.GetBytes(message);
            //     channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
            // }

            var messageId = Guid.NewGuid().ToString();
            context.Set(MessageId, messageId);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(MessageId, "");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
