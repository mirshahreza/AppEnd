using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Text.Json;
using System.Xml.Linq;

namespace AppEndWorkflow.Activities.DataConversion;

/// <summary>
/// Converts XML to JSON structure.
/// </summary>
[Activity(
    Category = "Data Conversion",
    Description = "Convert XML to JSON",
    DisplayName = "Convert XML to JSON"
)]
public class ConvertXmlToJsonActivity : CodeActivity
{
    [Input(Description = "XML input")]
    public Input<string> InputXml { get; set; } = default!;

    [Input(Description = "Keep XML attributes (default: true)")]
    public Input<bool?> PreserveAttributes { get; set; }

    [Output(Description = "Converted JSON string")]
    public Output<string> OutputJson { get; set; } = default!;

    [Output(Description = "Whether conversion succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var inputXml = context.Get(InputXml) ?? throw new ArgumentException("InputXml is required");
            var preserveAttrs = context.Get(PreserveAttributes) ?? true;

            var xDoc = XDocument.Parse(inputXml);
            var json = XmlToJson(xDoc.Root, preserveAttrs);

            context.Set(OutputJson, json);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(OutputJson, "");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }

    private string XmlToJson(XElement? element, bool preserveAttributes)
    {
        if (element == null)
            return "{}";

        using var ms = new MemoryStream();
        using var writer = new Utf8JsonWriter(ms, new JsonWriterOptions { Indented = true });

        ConvertXmlToJsonElement(element, writer, preserveAttributes);
        writer.Flush();

        return System.Text.Encoding.UTF8.GetString(ms.ToArray());
    }

    private void ConvertXmlToJsonElement(XElement element, Utf8JsonWriter writer, bool preserveAttributes)
    {
        writer.WriteStartObject();

        // Add attributes if enabled
        if (preserveAttributes)
        {
            foreach (var attr in element.Attributes())
            {
                writer.WriteString($"@{attr.Name.LocalName}", attr.Value);
            }
        }

        // Group child elements by name
        var childElements = element.Elements().GroupBy(e => e.Name.LocalName).ToList();
        var hasText = !string.IsNullOrWhiteSpace(element.Value);

        if (childElements.Count > 0)
        {
            foreach (var group in childElements)
            {
                var elements = group.ToList();

                if (elements.Count == 1)
                {
                    writer.WritePropertyName(group.Key);
                    ConvertXmlToJsonElement(elements[0], writer, preserveAttributes);
                }
                else
                {
                    writer.WritePropertyName(group.Key);
                    writer.WriteStartArray();

                    foreach (var elem in elements)
                    {
                        ConvertXmlToJsonElement(elem, writer, preserveAttributes);
                    }

                    writer.WriteEndArray();
                }
            }
        }
        else if (hasText)
        {
            writer.WritePropertyName("#text");
            writer.WriteStringValue(element.Value);
        }

        writer.WriteEndObject();
    }
}
