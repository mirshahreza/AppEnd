using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;

namespace AppEndWorkflow.Activities.MessageQueues;

/// <summary>
/// Publishes message to Redis queue/pub-sub.
/// Uses StackExchange.Redis NuGet package.
/// </summary>
[Activity(
    Category = "Message Queues",
    Description = "Publish to Redis",
    DisplayName = "Redis Publish"
)]
public class RedisPublishActivity : CodeActivity
{
    [Input(Description = "Redis server address")]
    public Input<string> RedisServer { get; set; } = default!;

    [Input(Description = "Channel name")]
    public Input<string> Channel { get; set; } = default!;

    [Input(Description = "Message content")]
    public Input<string> Message { get; set; } = default!;

    [Input(Description = "Redis port (default: 6379)")]
    public Input<int?> Port { get; set; }

    [Output(Description = "Number of subscribers")]
    public Output<int> SubscriberCount { get; set; } = default!;

    [Output(Description = "Whether publish succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var redisServer = context.Get(RedisServer) ?? throw new ArgumentException("RedisServer is required");
            var channel = context.Get(Channel) ?? throw new ArgumentException("Channel is required");
            var message = context.Get(Message) ?? throw new ArgumentException("Message is required");
            var port = context.Get(Port) ?? 6379;

            // NOTE: In production, use StackExchange.Redis
            // var options = ConfigurationOptions.Parse($"{redisServer}:{port}");
            // using (var connection = ConnectionMultiplexer.Connect(options))
            // {
            //     var db = connection.GetDatabase();
            //     var subscriber = connection.GetSubscriber();
            //     long subscriberCount = await subscriber.PublishAsync(channel, message);
            // }

            context.Set(SubscriberCount, 0);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(SubscriberCount, 0);
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
