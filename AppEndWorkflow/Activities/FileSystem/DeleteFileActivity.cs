using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;

namespace AppEndWorkflow.Activities.FileSystem;

/// <summary>
/// Deletes a file.
/// </summary>
[Activity(
    Category = "FileSystem",
    Description = "Deletes a file",
    DisplayName = "Delete File"
)]
public class DeleteFileActivity : CodeActivity
{
    /// <summary>
    /// Path to file to delete
    /// </summary>
    [Input(Description = "Path to file to delete")]
    public Input<string> FilePath { get; set; } = default!;

    /// <summary>
    /// Don't fail if file doesn't exist (default: true)
    /// </summary>
    [Input(Description = "Don't fail if file doesn't exist")]
    public Input<bool> IgnoreIfNotExists { get; set; } = new(true);

    /// <summary>
    /// Whether the file existed before deletion
    /// </summary>
    [Output(Description = "Whether the file existed before deletion")]
    public Output<bool> Existed { get; set; } = default!;

    /// <summary>
    /// Whether deletion succeeded
    /// </summary>
    [Output(Description = "Whether deletion succeeded")]
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
            var ignoreIfNotExists = context.Get(IgnoreIfNotExists);

            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("'FilePath' is required");

            // Check if file exists
            var existed = File.Exists(filePath);
            context.Set(Existed, existed);

            if (!existed && !ignoreIfNotExists)
                throw new FileNotFoundException($"File not found: {filePath}");

            // Delete file
            if (existed)
                File.Delete(filePath);

            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(Existed, false);
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
