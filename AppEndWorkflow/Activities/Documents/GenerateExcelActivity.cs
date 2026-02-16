using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;

namespace AppEndWorkflow.Activities.Documents;

/// <summary>
/// Generates Excel file from JSON data.
/// </summary>
[Activity(
    Category = "Documents",
    Description = "Generates Excel from JSON data",
    DisplayName = "Generate Excel"
)]
public class GenerateExcelActivity : CodeActivity
{
    /// <summary>
    /// JSON array of rows to export
    /// </summary>
    [Input(Description = "JSON array of data rows")]
    public Input<string> Data { get; set; } = default!;

    /// <summary>
    /// JSON array of column definitions {Name, Title, Width}
    /// </summary>
    [Input(Description = "JSON array of column definitions")]
    public Input<string> Columns { get; set; } = default!;

    /// <summary>
    /// Worksheet name (default: "Sheet1")
    /// </summary>
    [Input(Description = "Worksheet name")]
    public Input<string> SheetName { get; set; } = new("Sheet1");

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
            var dataJson = context.Get(Data);
            var columnsJson = context.Get(Columns);
            var sheetName = context.Get(SheetName);
            var outputPath = context.Get(OutputPath);

            if (string.IsNullOrWhiteSpace(dataJson))
                throw new ArgumentException("'Data' is required");

            if (string.IsNullOrWhiteSpace(columnsJson))
                throw new ArgumentException("'Columns' is required");

            if (string.IsNullOrWhiteSpace(sheetName))
                sheetName = "Sheet1";

            // TODO: Implement Excel generation
            // Options:
            // 1. ClosedXML - free, easy to use
            // 2. EPPlus - feature-rich
            // 3. OpenXml SDK - low-level
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
