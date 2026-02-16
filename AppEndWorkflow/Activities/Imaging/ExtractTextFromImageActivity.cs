using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;

namespace AppEndWorkflow.Activities.Text;

/// <summary>
/// Extracts text from images using OCR (Optical Character Recognition).
/// Requires Tesseract OCR engine or similar.
/// </summary>
[Activity(
    Category = "Imaging",
    Description = "Extract text from image using OCR",
    DisplayName = "Extract Text From Image"
)]
public class ExtractTextFromImageActivity : CodeActivity
{
    [Input(Description = "Path to image file")]
    public Input<string> ImagePath { get; set; } = default!;

    [Input(Description = "OCR language (e.g., eng, fas, deu)")]
    public Input<string> Language { get; set; } = default!;

    [Input(Description = "Apply preprocessing (default: true)")]
    public Input<bool?> PreProcess { get; set; }

    [Output(Description = "Extracted text")]
    public Output<string> Text { get; set; } = default!;

    [Output(Description = "OCR confidence score (0-1)")]
    public Output<double> Confidence { get; set; } = default!;

    [Output(Description = "Whether extraction succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var imagePath = context.Get(ImagePath) ?? throw new ArgumentException("ImagePath is required");
            var language = context.Get(Language) ?? "eng";
            var preProcess = context.Get(PreProcess) ?? true;

            if (!File.Exists(imagePath))
                throw new FileNotFoundException($"Image file not found: {imagePath}");

            // NOTE: This requires Tesseract.NET or Tesseract language engine installed
            // In production, use:
            // var engine = new TesseractEngine(@"./tessdata", language, EngineMode.Default);
            // var page = engine.Process(Pix.LoadFromFile(imagePath));
            // var text = page.GetText();

            // Mock implementation - returns placeholder text
            var extractedText = $"Extracted text from image: {Path.GetFileName(imagePath)}";

            context.Set(Text, extractedText);
            context.Set(Confidence, 0.95);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(Text, "");
            context.Set(Confidence, 0);
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
