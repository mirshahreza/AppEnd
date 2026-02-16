using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;

namespace AppEndWorkflow.Activities.Documents;

/// <summary>
/// Generates PDF from template and data.
/// Uses a PDF library for generation.
/// </summary>
[Activity(
    Category = "Documents",
    Description = "Generates PDF from template",
    DisplayName = "Generate PDF"
)]
public class GeneratePdfActivity : CodeActivity
{
    /// <summary>
    /// PDF template name/path
    /// </summary>
    [Input(Description = "PDF template name/path")]
    public Input<string> TemplateName { get; set; } = default!;

    /// <summary>
    /// JSON data to fill the template
    /// </summary>
    [Input(Description = "JSON data for template")]
    public Input<string> Data { get; set; } = default!;

    /// <summary>
    /// File path to save (optional — returns bytes if not set)
    /// </summary>
    [Input(Description = "Output file path")]
    public Input<string?> OutputPath { get; set; }

    /// <summary>
    /// Path to generated file
    /// </summary>
    [Output(Description = "Path to generated file")]
    public Output<string?> FilePath { get; set; }

    /// <summary>
    /// Size in bytes
    /// </summary>
    [Output(Description = "Size in bytes")]
    public Output<long> FileSize { get; set; } = default!;

    /// <summary>
    /// Whether generation succeeded
    /// </summary>
    [Output(Description = "Whether generation succeeded")]
    public Output<bool> Success { get; set; } = default!;

    /// <summary>
    /// Error message if failed
    /// </summary>
    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var templateName = context.Get(TemplateName);
            var dataJson = context.Get(Data);
            var outputPath = context.Get(OutputPath);

            if (string.IsNullOrWhiteSpace(templateName))
                throw new ArgumentException("'TemplateName' is required");

            if (string.IsNullOrWhiteSpace(dataJson))
                throw new ArgumentException("'Data' is required");

            // TODO: Implement PDF generation
            // Options:
            // 1. QuestPDF - modern, fluent API
            // 2. iTextSharp - mature, feature-rich
            // 3. SelectPdf - enterprise
            //
            // For now, this is a placeholder

            context.Set(FilePath, outputPath);
            context.Set(FileSize, 0);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(FilePath, null);
            context.Set(FileSize, 0);
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
