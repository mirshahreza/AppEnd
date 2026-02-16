using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Text.Json;

namespace AppEndWorkflow.Activities.FileSystem;

/// <summary>
/// Lists files in a directory.
/// Supports search patterns and recursive search.
/// </summary>
[Activity(
    Category = "FileSystem",
    Description = "Lists files in a directory",
    DisplayName = "List Files"
)]
public class ListFilesActivity : CodeActivity
{
    /// <summary>
    /// Path to directory
    /// </summary>
    [Input(Description = "Path to directory")]
    public Input<string> DirectoryPath { get; set; } = default!;

    /// <summary>
    /// Search pattern (default: "*.*")
    /// </summary>
    [Input(Description = "Search pattern")]
    public Input<string> Pattern { get; set; } = new("*.*");

    /// <summary>
    /// Search subdirectories (default: false)
    /// </summary>
    [Input(Description = "Search subdirectories")]
    public Input<bool> Recursive { get; set; } = new(false);

    /// <summary>
    /// JSON array of {Name, Path, Size, LastModified}
    /// </summary>
    [Output(Description = "JSON array of files")]
    public Output<string> Files { get; set; } = default!;

    /// <summary>
    /// Number of files found
    /// </summary>
    [Output(Description = "Number of files found")]
    public Output<int> Count { get; set; } = default!;

    /// <summary>
    /// Whether listing succeeded
    /// </summary>
    [Output(Description = "Whether listing succeeded")]
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
            var directoryPath = context.Get(DirectoryPath);
            var pattern = context.Get(Pattern);
            var recursive = context.Get(Recursive);

            if (string.IsNullOrWhiteSpace(directoryPath))
                throw new ArgumentException("'DirectoryPath' is required");

            if (!Directory.Exists(directoryPath))
                throw new DirectoryNotFoundException($"Directory not found: {directoryPath}");

            if (string.IsNullOrWhiteSpace(pattern))
                pattern = "*.*";

            // List files
            var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var fileInfos = Directory.GetFiles(directoryPath, pattern, searchOption);

            // Convert to JSON
            var files = fileInfos.Select(f => new FileInfo(f))
                .Select(fi => new
                {
                    Name = fi.Name,
                    Path = fi.FullName,
                    Size = fi.Length,
                    LastModified = fi.LastWriteTimeUtc
                })
                .ToList();

            var filesJson = JsonSerializer.Serialize(files);

            context.Set(Files, filesJson);
            context.Set(Count, files.Count);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(Files, "[]");
            context.Set(Count, 0);
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
