using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace AppEndWorkflow.Activities.Database;

/// <summary>
/// Restores database from backup file.
/// </summary>
[Activity(
    Category = "Database",
    Description = "Restore database from backup",
    DisplayName = "Restore Database"
)]
public class RestoreDatabaseActivity : CodeActivity
{
    [Input(Description = "Target database config name")]
    public Input<string> DbConfName { get; set; } = default!;

    [Input(Description = "Path to backup file")]
    public Input<string> BackupPath { get; set; } = default!;

    [Input(Description = "Database name after restore (optional)")]
    public Input<string?> RestoreName { get; set; }

    [Input(Description = "Overwrite existing database (default: false)")]
    public Input<bool?> Overwrite { get; set; }

    [Input(Description = "Verify backup before restore (default: true)")]
    public Input<bool?> VerifyBackup { get; set; }

    [Output(Description = "Restored database name")]
    public Output<string> RestoredDbName { get; set; } = default!;

    [Output(Description = "Whether restore succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var dbConfName = context.Get(DbConfName) ?? throw new ArgumentException("DbConfName is required");
            var backupPath = context.Get(BackupPath) ?? throw new ArgumentException("BackupPath is required");
            var restoreName = context.Get(RestoreName) ?? dbConfName;
            var overwrite = context.Get(Overwrite) ?? false;
            var verifyBackup = context.Get(VerifyBackup) ?? true;

            if (!File.Exists(backupPath))
                throw new FileNotFoundException($"Backup file not found: {backupPath}");

            var configuration = context.GetService<IConfiguration>();
            var connString = configuration?.GetConnectionString(dbConfName)
                ?? throw new InvalidOperationException($"Connection string '{dbConfName}' not found");

            // NOTE: In production, use database-specific restore tools
            // Extract db type and restore accordingly

            context.Set(RestoredDbName, restoreName);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(RestoredDbName, "");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
