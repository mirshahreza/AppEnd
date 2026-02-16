using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Text.Json;

namespace AppEndWorkflow.Activities.Http;

/// <summary>
/// Transforms webhook payload to standard format using mapping rules.
/// </summary>
[Activity(
    Category = "Webhooks",
    Description = "Transform webhook payload",
    DisplayName = "Transform Webhook Payload"
)]
public class TransformWebhookPayloadActivity : CodeActivity
{
    [Input(Description = "Original webhook payload")]
    public Input<string> PayloadJson { get; set; } = default!;

    [Input(Description = "JSON transformation rules")]
    public Input<string> MappingRules { get; set; } = default!;

    [Output(Description = "Normalized JSON output")]
    public Output<string> TransformedPayload { get; set; } = default!;

    [Output(Description = "Whether transformation succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var payload = context.Get(PayloadJson) ?? throw new ArgumentException("PayloadJson is required");
            var rules = context.Get(MappingRules) ?? throw new ArgumentException("MappingRules is required");

            // Parse payload
            using var payloadDoc = JsonDocument.Parse(payload);
            var payloadRoot = payloadDoc.RootElement;

            // Parse mapping rules
            using var rulesDoc = JsonDocument.Parse(rules);
            var rulesRoot = rulesDoc.RootElement;

            // Build transformed object
            var transformedObject = new Dictionary<string, object?>();

            foreach (var rule in rulesRoot.EnumerateObject())
            {
                var targetField = rule.Name;
                var sourcePath = rule.Value.GetString();

                if (!string.IsNullOrWhiteSpace(sourcePath))
                {
                    var sourceValue = GetValueByPath(payloadRoot, sourcePath);
                    transformedObject[targetField] = sourceValue;
                }
            }

            var transformedJson = JsonSerializer.Serialize(transformedObject);

            context.Set(TransformedPayload, transformedJson);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(TransformedPayload, "{}");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }

    private object? GetValueByPath(JsonElement root, string path)
    {
        var parts = path.Split('.');
        var current = root;

        foreach (var part in parts)
        {
            if (current.ValueKind == JsonValueKind.Object && current.TryGetProperty(part, out var next))
            {
                current = next;
            }
            else
            {
                return null;
            }
        }

        return current.GetRawText();
    }
}
