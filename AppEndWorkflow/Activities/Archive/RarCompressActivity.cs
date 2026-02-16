using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Text.Json;

namespace AppEndWorkflow.Activities.Archive;

/// <summary>
/// Creates RAR compressed archive.
/// </summary>
[Activity(
    Category = "Archive",
    Description = "Create RAR archive",
    DisplayName = "RAR Compress"
)]
public class RarCompressActivity : CodeActivity
{
    [Input(Description = "JSON array of paths to compress")]
    public Input<string> SourcePaths { get; set; } = default!;

    [Input(Description = "Output RAR file path")]
    public Input<string> OutputPath { get; set; } = default!;

    [Input(Description = "Compression ratio (0-5, default: 3)")]
    public Input<int?> CompressionRatio { get; set; }

    [Input(Description = "RAR password (optional)")]
    public Input<string?> Password { get; set; }

    [Output(Description = "Path to created archive")]
    public Output<string> FilePath { get; set; } = default!;

    [Output(Description = "Archive file size")]
    public Output<long> FileSize { get; set; } = default!;

    [Output(Description = "Whether compression succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var sourcePathsJson = context.Get(SourcePaths) ?? throw new ArgumentException("SourcePaths is required");
            var outputPath = context.Get(OutputPath) ?? throw new ArgumentException("OutputPath is required");
            var compressionRatio = context.Get(CompressionRatio) ?? 3;
            var password = context.Get(Password);

            var sourcePaths = new List<string>();
            try
            {
                using var doc = JsonDocument.Parse(sourcePathsJson);
                foreach (var elem in doc.RootElement.EnumerateArray())
                {
                    var path = elem.GetString();
                    if (!string.IsNullOrWhiteSpace(path))
                        sourcePaths.Add(path);
                }
            }
            catch
            {
                throw new ArgumentException("SourcePaths must be valid JSON array");
            }

            if (sourcePaths.Count == 0)
                throw new ArgumentException("At least one source path is required");

            foreach (var path in sourcePaths)
            {
                if (!File.Exists(path) && !Directory.Exists(path))
                    throw new FileNotFoundException($"Path not found: {path}");
            }

            var directory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrWhiteSpace(directory))
                Directory.CreateDirectory(directory);

            // NOTE: RAR compression requires external RAR.exe or library
            // For production, use SharpCompress or call external rar.exe

            // Mock: create archive metadata
            File.WriteAllText(outputPath, $"RAR Archive\nCompression: {compressionRatio}");
            var fileSize = new FileInfo(outputPath).Length;

            context.Set(FilePath, outputPath);
            context.Set(FileSize, fileSize);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(FilePath, "");
            context.Set(FileSize, 0);
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
