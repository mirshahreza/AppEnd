using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;

namespace AppEndWorkflow.Activities.FileSystem;

/// <summary>
/// Copies or moves a file.
/// </summary>
[Activity(
    Category = "FileSystem",
    Description = "Copies or moves a file",
    DisplayName = "Copy/Move File"
)]
public class CopyMoveFileActivity : CodeActivity
{
    /// <summary>
    /// Source file path
    /// </summary>
    [Input(Description = "Source file path")]
    public Input<string> SourcePath { get; set; } = default!;

    /// <summary>
    /// Destination file path
    /// </summary>
    [Input(Description = "Destination file path")]
    public Input<string> DestinationPath { get; set; } = default!;

    /// <summary>
    /// Operation: "Copy" or "Move"
    /// </summary>
    [Input(Description = "Operation: 'Copy' or 'Move'")]
    public Input<string> Operation { get; set; } = new("Copy");

    /// <summary>
    /// Overwrite if destination exists (default: false)
    /// </summary>
    [Input(Description = "Overwrite if destination exists")]
    public Input<bool> Overwrite { get; set; } = new(false);

    /// <summary>
    /// Final destination path
    /// </summary>
    [Output(Description = "Final destination path")]
    public Output<string> DestinationPath_Out { get; set; } = default!;

    /// <summary>
    /// Whether operation succeeded
    /// </summary>
    [Output(Description = "Whether operation succeeded")]
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
            var sourcePath = context.Get(SourcePath);
            var destinationPath = context.Get(DestinationPath);
            var operation = context.Get(Operation);
            var overwrite = context.Get(Overwrite);

            if (string.IsNullOrWhiteSpace(sourcePath))
                throw new ArgumentException("'SourcePath' is required");

            if (string.IsNullOrWhiteSpace(destinationPath))
                throw new ArgumentException("'DestinationPath' is required");

            if (!File.Exists(sourcePath))
                throw new FileNotFoundException($"Source file not found: {sourcePath}");

            // Create destination directory if it doesn't exist
            var destDirectory = Path.GetDirectoryName(destinationPath);
            if (!string.IsNullOrWhiteSpace(destDirectory) && !Directory.Exists(destDirectory))
                Directory.CreateDirectory(destDirectory);

            // Perform operation
            switch (operation?.ToLower())
            {
                case "move":
                    File.Move(sourcePath, destinationPath, overwrite);
                    break;

                case "copy":
                default:
                    File.Copy(sourcePath, destinationPath, overwrite);
                    break;
            }

            context.Set(DestinationPath_Out, destinationPath);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(DestinationPath_Out, "");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
