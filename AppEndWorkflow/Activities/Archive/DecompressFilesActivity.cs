using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.IO.Compression;

namespace AppEndWorkflow.Activities.Archive;

/// <summary>
/// Extracts a ZIP archive to a directory.
/// </summary>
[Activity(
    Category = "Archive",
    Description = "Extracts ZIP archive",
    DisplayName = "Decompress Files"
)]
public class DecompressFilesActivity : CodeActivity
{
    /// <summary>
    /// Path to ZIP archive
    /// </summary>
    [Input(Description = "Path to ZIP archive")]
    public Input<string> ArchivePath { get; set; } = default!;

    /// <summary>
    /// Directory to extract files to
    /// </summary>
    [Input(Description = "Directory to extract to")]
    public Input<string> OutputDirectory { get; set; } = default!;

    /// <summary>
    /// Archive password (if encrypted)
    /// </summary>
    [Input(Description = "Archive password")]
    public Input<string?> Password { get; set; }

    /// <summary>
    /// Overwrite existing files (default: false)
    /// </summary>
    [Input(Description = "Overwrite existing files")]
    public Input<bool> Overwrite { get; set; } = new(false);

    /// <summary>
    /// JSON array of extracted file paths
    /// </summary>
    [Output(Description = "Extracted file paths")]
    public Output<string> ExtractedFiles { get; set; } = default!;

    /// <summary>
    /// Number of files extracted
    /// </summary>
    [Output(Description = "Number of files extracted")]
    public Output<int> FileCount { get; set; } = default!;

    /// <summary>
    /// Whether extraction succeeded
    /// </summary>
    [Output(Description = "Whether extraction succeeded")]
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
            var archivePath = context.Get(ArchivePath);
            var outputDirectory = context.Get(OutputDirectory);
            var password = context.Get(Password);
            var overwrite = context.Get(Overwrite);

            if (string.IsNullOrWhiteSpace(archivePath))
                throw new ArgumentException("'ArchivePath' is required");

            if (string.IsNullOrWhiteSpace(outputDirectory))
                throw new ArgumentException("'OutputDirectory' is required");

            if (!File.Exists(archivePath))
                throw new FileNotFoundException($"Archive not found: {archivePath}");

            // Create output directory if it doesn't exist
            if (!Directory.Exists(outputDirectory))
                Directory.CreateDirectory(outputDirectory);

            // Extract archive
            var extractedFiles = new List<string>();
            using (var archive = ZipFile.OpenRead(archivePath))
            {
                foreach (var entry in archive.Entries)
                {
                    var fullPath = Path.Combine(outputDirectory, entry.FullName);
                    var dir = Path.GetDirectoryName(fullPath);

                    if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    if (!entry.FullName.EndsWith("/"))
                    {
                        if (File.Exists(fullPath) && !overwrite)
                            continue;

                        entry.ExtractToFile(fullPath, overwrite);
                        extractedFiles.Add(fullPath);
                    }
                }
            }

            context.Set(ExtractedFiles, System.Text.Json.JsonSerializer.Serialize(extractedFiles));
            context.Set(FileCount, extractedFiles.Count);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(ExtractedFiles, "[]");
            context.Set(FileCount, 0);
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
