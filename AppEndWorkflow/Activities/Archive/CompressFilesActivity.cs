using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.IO.Compression;

namespace AppEndWorkflow.Activities.Archive;

/// <summary>
/// Creates a ZIP archive from files/directories.
/// </summary>
[Activity(
    Category = "Archive",
    Description = "Creates ZIP archive",
    DisplayName = "Compress Files"
)]
public class CompressFilesActivity : CodeActivity
{
    /// <summary>
    /// JSON array of file/directory paths to compress
    /// </summary>
    [Input(Description = "JSON array of paths to compress")]
    public Input<string> SourcePaths { get; set; } = default!;

    /// <summary>
    /// Path for the output ZIP file
    /// </summary>
    [Input(Description = "Output ZIP file path")]
    public Input<string> OutputPath { get; set; } = default!;

    /// <summary>
    /// Compression level: "Fastest", "Optimal", "NoCompression"
    /// </summary>
    [Input(Description = "Compression level")]
    public Input<string> CompressionLevel { get; set; } = new("Optimal");

    /// <summary>
    /// Optional ZIP password
    /// </summary>
    [Input(Description = "Optional ZIP password")]
    public Input<string?> Password { get; set; }

    /// <summary>
    /// Path to created archive
    /// </summary>
    [Output(Description = "Path to created archive")]
    public Output<string> FilePath { get; set; } = default!;

    /// <summary>
    /// Archive size in bytes
    /// </summary>
    [Output(Description = "Archive size in bytes")]
    public Output<long> FileSize { get; set; } = default!;

    /// <summary>
    /// Number of files included
    /// </summary>
    [Output(Description = "Number of files included")]
    public Output<int> FileCount { get; set; } = default!;

    /// <summary>
    /// Whether compression succeeded
    /// </summary>
    [Output(Description = "Whether compression succeeded")]
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
            var sourcePathsJson = context.Get(SourcePaths);
            var outputPath = context.Get(OutputPath);
            var compressionLevelStr = context.Get(CompressionLevel);
            var password = context.Get(Password);

            if (string.IsNullOrWhiteSpace(sourcePathsJson))
                throw new ArgumentException("'SourcePaths' is required");

            if (string.IsNullOrWhiteSpace(outputPath))
                throw new ArgumentException("'OutputPath' is required");

            // Parse source paths
            using var doc = System.Text.Json.JsonDocument.Parse(sourcePathsJson);
            if (doc.RootElement.ValueKind != System.Text.Json.JsonValueKind.Array)
                throw new ArgumentException("SourcePaths must be a JSON array");

            var sourcePaths = doc.RootElement.EnumerateArray()
                .Select(e => e.GetString())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();

            if (sourcePaths.Count == 0)
                throw new ArgumentException("No valid paths in SourcePaths");

            // Parse compression level
            var level = compressionLevelStr?.ToLower() switch
            {
                "fastest" => System.IO.Compression.CompressionLevel.Fastest,
                "optimal" => System.IO.Compression.CompressionLevel.Optimal,
                "nocompression" => System.IO.Compression.CompressionLevel.NoCompression,
                _ => System.IO.Compression.CompressionLevel.Optimal
            };

            // Create output directory
            var outputDir = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrWhiteSpace(outputDir) && !Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            // Create ZIP archive
            var fileCount = 0;
            using (var archive = ZipFile.Open(outputPath, ZipArchiveMode.Create))
            {
                foreach (var sourcePath in sourcePaths)
                {
                    if (File.Exists(sourcePath))
                    {
                        var fileName = Path.GetFileName(sourcePath);
                        archive.CreateEntryFromFile(sourcePath, fileName, level);
                        fileCount++;
                    }
                    else if (Directory.Exists(sourcePath))
                    {
                        var files = Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories);
                        foreach (var file in files)
                        {
                            var relative = Path.GetRelativePath(sourcePath, file);
                            archive.CreateEntryFromFile(file, relative, level);
                            fileCount++;
                        }
                    }
                }
            }

            // Get file info
            var fileInfo = new FileInfo(outputPath);

            context.Set(FilePath, outputPath);
            context.Set(FileSize, fileInfo.Length);
            context.Set(FileCount, fileCount);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(FilePath, "");
            context.Set(FileSize, 0);
            context.Set(FileCount, 0);
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
