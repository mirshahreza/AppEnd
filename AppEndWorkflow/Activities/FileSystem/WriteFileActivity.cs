using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Text;

namespace AppEndWorkflow.Activities.FileSystem;

/// <summary>
/// Writes content to a file.
/// Supports Create, Append, and Overwrite modes.
/// </summary>
[Activity(
    Category = "FileSystem",
    Description = "Writes content to a file",
    DisplayName = "Write File"
)]
public class WriteFileActivity : CodeActivity
{
    /// <summary>
    /// Path to file
    /// </summary>
    [Input(Description = "Path to file")]
    public Input<string> FilePath { get; set; } = default!;

    /// <summary>
    /// Content to write
    /// </summary>
    [Input(Description = "Content to write")]
    public Input<string> Content { get; set; } = default!;

    /// <summary>
    /// Mode: "Create", "Append", "Overwrite" (default: "Create")
    /// </summary>
    [Input(Description = "Mode: 'Create', 'Append', 'Overwrite'")]
    public Input<string> Mode { get; set; } = new("Create");

    /// <summary>
    /// Text encoding (default: "UTF-8")
    /// </summary>
    [Input(Description = "Text encoding")]
    public Input<string> Encoding { get; set; } = new("UTF-8");

    /// <summary>
    /// Path to written file
    /// </summary>
    [Output(Description = "Path to written file")]
    public Output<string> FilePath_Out { get; set; } = default!;

    /// <summary>
    /// Resulting file size
    /// </summary>
    [Output(Description = "Resulting file size")]
    public Output<long> FileSize { get; set; } = default!;

    /// <summary>
    /// Whether write succeeded
    /// </summary>
    [Output(Description = "Whether write succeeded")]
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
            var filePath = context.Get(FilePath);
            var content = context.Get(Content);
            var mode = context.Get(Mode);
            var encoding = context.Get(Encoding);

            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("'FilePath' is required");

            if (content == null)
                throw new ArgumentException("'Content' is required");

            if (string.IsNullOrWhiteSpace(mode))
                mode = "Create";

            // Create directory if it doesn't exist
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            // Get encoding
            var textEncoding = GetEncoding(encoding);

            // Write file based on mode
            switch (mode.ToLower())
            {
                case "append":
                    File.AppendAllText(filePath, content, textEncoding);
                    break;

                case "overwrite":
                case "create":
                default:
                    File.WriteAllText(filePath, content, textEncoding);
                    break;
            }

            // Get file size
            var fileInfo = new FileInfo(filePath);

            context.Set(FilePath_Out, filePath);
            context.Set(FileSize, fileInfo.Length);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(FilePath_Out, "");
            context.Set(FileSize, 0);
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }

    private Encoding GetEncoding(string? encodingName)
    {
        if (string.IsNullOrWhiteSpace(encodingName))
            return System.Text.Encoding.UTF8;

        return encodingName.ToLower() switch
        {
            "utf-8" or "utf8" => System.Text.Encoding.UTF8,
            "ascii" => System.Text.Encoding.ASCII,
            "utf-16" or "utf16" => System.Text.Encoding.Unicode,
            "utf-32" or "utf32" => System.Text.Encoding.UTF32,
            _ => System.Text.Encoding.UTF8
        };
    }
}
