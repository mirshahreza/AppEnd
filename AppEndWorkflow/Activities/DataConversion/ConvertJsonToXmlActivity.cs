using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;

namespace AppEndWorkflow.Activities.DataConversion;

/// <summary>
/// Converts JSON to XML structure.
/// </summary>
[Activity(
    Category = "Data Conversion",
    Description = "Convert JSON to XML",
    DisplayName = "Convert JSON to XML"
)]
public class ConvertJsonToXmlActivity : CodeActivity
{
    [Input(Description = "JSON input")]
    public Input<string> InputJson { get; set; } = default!;

    [Input(Description = "XML root element name (default: root)")]
    public Input<string?> RootElementName { get; set; }

    [Input(Description = "Prefix for attributes (default: @)")]
    public Input<string?> AttributePrefix { get; set; }

    [Output(Description = "Converted XML string")]
    public Output<string> OutputXml { get; set; } = default!;

    [Output(Description = "Whether conversion succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var inputJson = context.Get(InputJson) ?? throw new ArgumentException("InputJson is required");
            var rootName = context.Get(RootElementName) ?? "root";
            var attrPrefix = context.Get(AttributePrefix) ?? "@";

            var xmlOutput = JsonToXml(inputJson, rootName, attrPrefix);

            context.Set(OutputXml, xmlOutput);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(OutputXml, "");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }

    private string JsonToXml(string json, string rootName, string attrPrefix)
    {
        using var doc = JsonDocument.Parse(json);
        var root = new XElement(rootName);
        ConvertJsonElementToXml(doc.RootElement, root, attrPrefix);

        using var stringWriter = new StringWriter();
        using var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = true });
        root.WriteTo(xmlWriter);
        return stringWriter.ToString();
    }

    private void ConvertJsonElementToXml(JsonElement element, XElement parent, string attrPrefix)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var property in element.EnumerateObject())
                {
                    var propName = property.Name;
                    if (propName.StartsWith(attrPrefix))
                    {
                        parent.Add(new XAttribute(propName.Substring(attrPrefix.Length), property.Value.ToString()));
                    }
                    else
                    {
                        var childElement = new XElement(propName);
                        ConvertJsonElementToXml(property.Value, childElement, attrPrefix);
                        parent.Add(childElement);
                    }
                }
                break;

            case JsonValueKind.Array:
                foreach (var item in element.EnumerateArray())
                {
                    var itemElement = new XElement("item");
                    ConvertJsonElementToXml(item, itemElement, attrPrefix);
                    parent.Add(itemElement);
                }
                break;

            case JsonValueKind.String:
                parent.Value = element.GetString() ?? "";
                break;

            case JsonValueKind.Number:
                parent.Value = element.GetRawText();
                break;

            case JsonValueKind.True:
            case JsonValueKind.False:
                parent.Value = element.GetBoolean().ToString().ToLower();
                break;

            case JsonValueKind.Null:
                parent.Value = "";
                break;
        }
    }
}
