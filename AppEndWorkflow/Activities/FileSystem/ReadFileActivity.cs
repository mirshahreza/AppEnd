using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Text;

namespace AppEndWorkflow.Activities.FileSystem;

/// <summary>
/// Reads text file content.
/// </summary>
[Activity(
    Category = "FileSystem",
    Description = "Reads text file content",
    DisplayName = "Read File"
)]
public class ReadFileActivity : CodeActivity
{
    /// <summary>
    /// Path to file
    /// </summary>
    [Input(Description = "Path to file")]
    public Input<string> FilePath { get; set; } = default!;

    /// <summary>
    /// Text encoding (default: "UTF-8")
    /// </summary>
    [Input(Description = "Text encoding")]
    public Input<string> Encoding { get; set; } = new("UTF-8");

    /// <summary>
    /// File content as string
    /// </summary>
    [Output(Description = "File content as string")]
    public Output<string> Content { get; set; } = default!;

    /// <summary>
    /// File size in bytes
    /// </summary>
    [Output(Description = "File size in bytes")]
    public Output<long> FileSize { get; set; } = default!;

    /// <summary>
    /// Whether the file exists
    /// </summary>
    [Output(Description = "Whether the file exists")]
    public Output<bool> Exists { get; set; } = default!;

    /// <summary>
    /// Whether read succeeded
    /// </summary>
    [Output(Description = "Whether read succeeded")]
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
            var encoding = context.Get(Encoding);

            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("'FilePath' is required");

            // Check if file exists
            var fileExists = File.Exists(filePath);
            context.Set(Exists, fileExists);

            if (!fileExists)
                throw new FileNotFoundException($"File not found: {filePath}");

            // Get encoding
            var textEncoding = GetEncoding(encoding);

            // Read file
            var content = File.ReadAllText(filePath, textEncoding);
            var fileInfo = new FileInfo(filePath);

            context.Set(Content, content);
            context.Set(FileSize, fileInfo.Length);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(Content, "");
            context.Set(FileSize, 0);
            context.Set(Exists, false);
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
