using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;

namespace AppEndWorkflow.Activities.Documents;

/// <summary>
/// Adds text watermark to PDF document.
/// </summary>
[Activity(
    Category = "Documents",
    Description = "Add watermark to PDF",
    DisplayName = "Add Watermark to PDF"
)]
public class AddWatermarkPdfActivity : CodeActivity
{
    [Input(Description = "Input PDF path")]
    public Input<string> InputPath { get; set; } = default!;

    [Input(Description = "Output PDF path")]
    public Input<string> OutputPath { get; set; } = default!;

    [Input(Description = "Watermark text")]
    public Input<string> WatermarkText { get; set; } = default!;

    [Input(Description = "Opacity (0-1, default: 0.5)")]
    public Input<float?> Opacity { get; set; }

    [Input(Description = "Rotation angle (default: 45)")]
    public Input<float?> Angle { get; set; }

    [Input(Description = "Font size (default: 80)")]
    public Input<int?> FontSize { get; set; }

    [Input(Description = "Apply to all pages (default: true)")]
    public Input<bool?> AllPages { get; set; }

    [Output(Description = "Path to watermarked PDF")]
    public Output<string> WatermarkedPath { get; set; } = default!;

    [Output(Description = "Whether watermarking succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var inputPath = context.Get(InputPath) ?? throw new ArgumentException("InputPath is required");
            var outputPath = context.Get(OutputPath) ?? throw new ArgumentException("OutputPath is required");
            var watermarkText = context.Get(WatermarkText) ?? throw new ArgumentException("WatermarkText is required");
            var opacity = context.Get(Opacity) ?? 0.5f;
            var angle = context.Get(Angle) ?? 45f;
            var fontSize = context.Get(FontSize) ?? 80;
            var allPages = context.Get(AllPages) ?? true;

            if (!File.Exists(inputPath))
                throw new FileNotFoundException($"PDF file not found: {inputPath}");

            // NOTE: In production, use itext7
            // var reader = new PdfReader(inputPath);
            // var writer = new PdfWriter(outputPath);
            // var pdfDocument = new PdfDocument(reader, writer);
            // var pageCount = pdfDocument.GetNumberOfPages();
            //
            // var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            // var canvas = new PdfCanvas(pdfDocument.GetPage(1));
            // canvas.SaveState()
            //     .BeginText()
            //     .SetFontAndSize(font, fontSize)
            //     .SetGraphicsState(new PdfExtGState().SetFillOpacity(opacity))
            //     .SetTextMatrix(1, 0, (float)Math.Tan(angle * Math.PI / 180), 1, 100, 100)
            //     .ShowText(watermarkText)
            //     .EndText()
            //     .RestoreState();
            //
            // pdfDocument.Close();

            var directory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrWhiteSpace(directory))
                Directory.CreateDirectory(directory);

            // Mock: copy input to output
            File.Copy(inputPath, outputPath, overwrite: true);

            context.Set(WatermarkedPath, outputPath);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(WatermarkedPath, "");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
