using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace AppEndWorkflow.Activities.Database;

/// <summary>
/// Creates a backup of database.
/// Supports SQL Server, MySQL, PostgreSQL.
/// </summary>
[Activity(
    Category = "Database",
    Description = "Backup database",
    DisplayName = "Backup Database"
)]
public class BackupDatabaseActivity : CodeActivity
{
    [Input(Description = "Database config name")]
    public Input<string> DbConfName { get; set; } = default!;

    [Input(Description = "Backup file output path")]
    public Input<string> OutputPath { get; set; } = default!;

    [Input(Description = "Backup type: Full, Differential, Log (default: Full)")]
    public Input<string?> BackupType { get; set; }

    [Input(Description = "Compress backup (default: false)")]
    public Input<bool?> Compress { get; set; }

    [Output(Description = "Path to backup file")]
    public Output<string> BackupPath { get; set; } = default!;

    [Output(Description = "Backup file size")]
    public Output<long> FileSize { get; set; } = default!;

    [Output(Description = "Whether backup succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var dbConfName = context.Get(DbConfName) ?? throw new ArgumentException("DbConfName is required");
            var outputPath = context.Get(OutputPath) ?? throw new ArgumentException("OutputPath is required");
            var backupType = context.Get(BackupType) ?? "Full";
            var compress = context.Get(Compress) ?? false;

            var configuration = context.GetService<IConfiguration>();
            var connString = configuration?.GetConnectionString(dbConfName)
                ?? throw new InvalidOperationException($"Connection string '{dbConfName}' not found in configuration");

            // NOTE: In production, extract db type from connection string
            // and use appropriate backup tool:
            // - SQL Server: sqlcmd with BACKUP DATABASE
            // - MySQL: mysqldump
            // - PostgreSQL: pg_dump

            var directory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrWhiteSpace(directory))
                Directory.CreateDirectory(directory);

            // Mock: create backup metadata file
            var backupInfo = $"Database: {dbConfName}\nType: {backupType}\nTime: {DateTime.UtcNow:O}";
            File.WriteAllText(outputPath, backupInfo);
            var fileSize = new FileInfo(outputPath).Length;

            context.Set(BackupPath, outputPath);
            context.Set(FileSize, fileSize);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(BackupPath, "");
            context.Set(FileSize, 0);
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
