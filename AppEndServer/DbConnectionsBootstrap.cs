using AppEndCommon;
using AppEndDbIO;
using System.Data;
using System.Data.Common;

namespace AppEndServer
{
    /// <summary>
    /// Ensures every DbServer from appsettings.json has a row in BaseDbConnections table
    /// so that Enrich DB page can read list from appsettings and enrichment params from DB.
    /// </summary>
    public static class DbConnectionsBootstrap
    {
        /// <summary>
        /// Ensures all connections from AppEnd.DbServers (appsettings) exist in BaseDbConnections.
        /// Called once at application startup. Safe to call multiple times (idempotent by Name).
        /// </summary>
        public static void EnsureFromAppSettings()
        {
            try
            {
                var dbServers = AppEndSettings.DbServers;
                if (dbServers == null || dbServers.Count == 0) return;

                using var dbIO = DbIO.Instance();
                var existingNames = GetExistingConnectionNames(dbIO);

                foreach (var node in dbServers)
                {
                    var name = node?["Name"]?.ToString()?.Trim();
                    if (string.IsNullOrEmpty(name)) continue;
                    if (existingNames.Contains(name, StringComparer.OrdinalIgnoreCase)) continue;

                    var serverType = node?["ServerType"]?.ToString()?.Trim() ?? "MsSql";
                    var connectionString = node?["ConnectionString"]?.ToString()?.Trim() ?? "";

                    var parameters = new List<DbParameter>
                    {
                        dbIO.CreateParameter("Name", "NVARCHAR", 200, name),
                        dbIO.CreateParameter("ServerType", "VarChar", 50, serverType),
                        dbIO.CreateParameter("ConnectionString", "NVARCHAR", 2000, connectionString),
                        dbIO.CreateParameter("Status", "VarChar", 50, "not_enriched"),
                        dbIO.CreateParameter("EnrichmentProgress", "Int", null, 0),
                        dbIO.CreateParameter("CreatedBy", "Int", null, 0),
                        dbIO.CreateParameter("CreatedOn", "DateTime", null, DateTime.UtcNow),
                        dbIO.CreateParameter("IsActive", "Bit", null, true)
                    };

                    dbIO.ToNoneQuery(@"
                        INSERT INTO [dbo].[BaseDbConnections] 
                        ( [Name], [ServerType], [ConnectionString], [Status], [EnrichmentProgress], [CreatedBy], [CreatedOn], [IsActive] )
                        VALUES ( @Name, @ServerType, @ConnectionString, @Status, @EnrichmentProgress, @CreatedBy, @CreatedOn, @IsActive )
                    ", parameters);
                    existingNames.Add(name);
                }
            }
            catch (Exception ex)
            {
                LogMan.LogWarning($"DbConnectionsBootstrap: Could not ensure DbServers in BaseDbConnections. {ex.Message}");
            }
        }

        private static HashSet<string> GetExistingConnectionNames(DbIO dbIO)
        {
            var existingNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                var dt = dbIO.ToDataTable("SELECT [Name] FROM [dbo].[BaseDbConnections] WITH (NOLOCK)", null, "Master");
                if (dt.TryGetValue("Master", out var table) && table.Rows.Count > 0)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        if (row["Name"] != DBNull.Value && row["Name"] != null && row["Name"] is string s && !string.IsNullOrEmpty(s))
                            existingNames.Add(s);
                    }
                }
            }
            catch
            {
                // Table may not exist yet
            }
            return existingNames;
        }
    }
}
