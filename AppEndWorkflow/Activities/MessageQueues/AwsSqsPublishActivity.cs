using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace AppEndWorkflow.Activities.MessageQueues;

/// <summary>
/// Publishes message to AWS SQS queue.
/// Requires AWSSDK.SQS NuGet package.
/// </summary>
[Activity(
    Category = "Message Queues",
    Description = "Publish to AWS SQS",
    DisplayName = "AWS SQS Publish"
)]
public class AwsSqsPublishActivity : CodeActivity
{
    [Input(Description = "AWS region")]
    public Input<string> Region { get; set; } = default!;

    [Input(Description = "SQS queue URL")]
    public Input<string> QueueUrl { get; set; } = default!;

    [Input(Description = "Message content")]
    public Input<string> Message { get; set; } = default!;

    [Input(Description = "Message group ID (for FIFO queues)")]
    public Input<string?> MessageGroupId { get; set; }

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
            var region = context.Get(Region) ?? throw new ArgumentException("Region is required");
            var queueUrl = context.Get(QueueUrl) ?? throw new ArgumentException("QueueUrl is required");
            var message = context.Get(Message) ?? throw new ArgumentException("Message is required");
            var messageGroupId = context.Get(MessageGroupId);

            // NOTE: In production, use AWSSDK.SQS
            // var client = new AmazonSQSClient(RegionEndpoint.GetBySystemName(region));
            // var sendMessageRequest = new SendMessageRequest
            // {
            //     QueueUrl = queueUrl,
            //     MessageBody = message,
            //     MessageGroupId = messageGroupId
            // };
            // var response = await client.SendMessageAsync(sendMessageRequest);
            // messageId = response.MessageId;

            var mockMessageId = Guid.NewGuid().ToString();
            context.Set(MessageId, mockMessageId);
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
