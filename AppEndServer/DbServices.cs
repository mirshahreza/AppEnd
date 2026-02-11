using AppEndCommon;
using AppEndDbIO;
using AppEndDynaCode;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Security.Cryptography;
using System.Security.AccessControl;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using static AppEndDbIO.DbSchemaUtils;
using DbColumn = AppEndDbIO.DbColumn;

namespace AppEndServer
{
	public static class DbServices
	{
		public static bool DropFk(string dbConfName, string objectName, string fkName)
		{
			DbSchemaUtils dbSchemaUtils = new(dbConfName);
			dbSchemaUtils.DropFk(objectName, fkName);
			return true;
		}
		public static object? DropObject(string dbConfName, string objectName, string objectType)
		{
			DbSchemaUtils dbSchemaUtils = new(dbConfName);
			DbDialogServices.RemoveServerObjects(dbConfName, objectName, objectType);
			if (objectType.EqualsIgnoreCase("table")) dbSchemaUtils.DropTable(objectName);
			if (objectType.EqualsIgnoreCase("view")) dbSchemaUtils.DropView(objectName);
			if (objectType.EqualsIgnoreCase("procedure")) dbSchemaUtils.DropProcedure(objectName);
			if (objectType.ContainsIgnoreCase("function")) dbSchemaUtils.DropFunction(objectName);
			DynaCode.Refresh();
			return true;
		}
		public static string GetCreateOrAlterObject(string dbConfName, string objectName)
		{
			DbSchemaUtils dbSchemaUtils = new(dbConfName);
			return dbSchemaUtils.GetCreateOrAlterObject(objectName);
		}
		public static object? CreateEmptyDbView(string dbConfName, string viewName)
		{
			DbSchemaUtils dbSchemaUtils = new(dbConfName);
			dbSchemaUtils.CreateEmptyView(viewName);
			return true;
		}
		public static object? CreateEmptyDbProcedure(string dbConfName, string procedureName)
		{
			DbSchemaUtils dbSchemaUtils = new(dbConfName);
			dbSchemaUtils.CreateEmptyProcedure(procedureName);
			return true;
		}
		public static object? CreateEmptyDbScalarFunction(string dbConfName, string scalarFunctionName)
		{
			DbSchemaUtils dbSchemaUtils = new(dbConfName);
			dbSchemaUtils.CreateEmptyScalarFunction(scalarFunctionName);
			return true;
		}
		public static object? CreateEmptyDbTableFunction(string dbConfName, string tableFunctionName)
		{
			DbSchemaUtils dbSchemaUtils = new(dbConfName);
			dbSchemaUtils.CreateEmptyTableFunction(tableFunctionName);
			return true;
		}
		public static bool AlterObjectScript(string dbConfName, string objectScript)
		{
			DbSchemaUtils dbSchemaUtils = new(dbConfName);
			dbSchemaUtils.AlterObjectScript(objectScript);
			return true;
		}
		public static object? RenameObject(string dbConfName, string objectNameOld, string objectNameNew, string objectType)
		{
			DbSchemaUtils dbSchemaUtils = new(dbConfName);
			DbDialogServices.RemoveServerObjects(dbConfName, objectNameOld, objectType);
			if (objectType.EqualsIgnoreCase("table"))
			{
				dbSchemaUtils.RenameTable(objectNameOld, objectNameNew);
				return true;
			}
			else
			{
				throw new AppEndException("NotImplementedYet", System.Reflection.MethodBase.GetCurrentMethod())
					.AddParam("ObjectType", objectType)
					.GetEx();
			}
		}
		public static object? TruncateTable(string dbConfName, string tableName)
		{
			DbSchemaUtils dbSchemaUtils = new(dbConfName);
			dbSchemaUtils.TruncateTable(tableName);
			return true;
		}

        public static List<DbTable> GetDbTables(string dbConfName)
        {
            DbSchemaUtils dbSchemaUtils = new(dbConfName);
            return dbSchemaUtils.GetTables();
        }

        /// <summary>
        /// Returns flat list of schema entities (tables + columns) for Enrich DB grid.
        /// Uses INFORMATION_SCHEMA so it works on any SQL Server database (no ZzSelectObjectsDetails required).
        /// StructureId = MD5(connectionName:schemaName:tableName:objectName) — 32 hex chars.
        /// Enriched state is read from BaseZetadata in DefaultRepo (ConnectionName = dbConfName).
        /// </summary>
        public static List<SchemaRowForEnrich> GetSchemaForEnrich(string dbConfName)
        {
            var list = new List<SchemaRowForEnrich>();
            var dbConf = DbConf.FromSettings(dbConfName);
            if (dbConf?.ConnectionString == null) return list;
            try
            {
                using var dbIO = DbIO.Instance(dbConf);
                const string tablesSql = @"
SELECT TABLE_SCHEMA AS SchemaName, TABLE_NAME AS TableName
FROM INFORMATION_SCHEMA.TABLES WITH (NOLOCK)
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_SCHEMA, TABLE_NAME";
                var tablesDt = dbIO.ToDataTable(tablesSql, null, "Master")["Master"];
                foreach (DataRow t in tablesDt.Rows)
                {
                    var schemaName = t["SchemaName"]?.ToString() ?? "dbo";
                    var tableName = t["TableName"]?.ToString() ?? "";
                    if (string.IsNullOrEmpty(tableName)) continue;
                    string tableId = ComputeStructureId(dbConfName, schemaName, tableName, tableName);
                    list.Add(new SchemaRowForEnrich
                    {
                        ObjectType = "Table",
                        SchemaName = schemaName,
                        TableName = tableName,
                        ObjectName = tableName,
                        StructureId = tableId
                    });
                }
                const string columnsSql = @"
SELECT TABLE_SCHEMA AS SchemaName, TABLE_NAME AS TableName, COLUMN_NAME AS ObjectName
FROM INFORMATION_SCHEMA.COLUMNS WITH (NOLOCK)
ORDER BY TABLE_SCHEMA, TABLE_NAME, ORDINAL_POSITION";
                var colsDt = dbIO.ToDataTable(columnsSql, null, "Master")["Master"];
                foreach (DataRow c in colsDt.Rows)
                {
                    var schemaName = c["SchemaName"]?.ToString() ?? "dbo";
                    var tableName = c["TableName"]?.ToString() ?? "";
                    var objectName = c["ObjectName"]?.ToString() ?? "";
                    if (string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(objectName)) continue;
                    list.Add(new SchemaRowForEnrich
                    {
                        ObjectType = "Column",
                        SchemaName = schemaName,
                        TableName = tableName,
                        ObjectName = objectName,
                        StructureId = ComputeStructureId(dbConfName, schemaName, tableName, objectName)
                    });
                }
            }
            catch
            {
                // Return empty list on error (e.g. connection failure)
            }
            return list;
        }

        private static string BaseZetadataDbConfName => string.IsNullOrEmpty(AppEndSettings.DefaultDbConfName) ? "DefaultRepo" : AppEndSettings.DefaultDbConfName;

        /// <summary>
        /// Returns full BaseZetadata rows for a connection (for detail panel: HumanTitleEn, NoteEn, etc.).
        /// </summary>
        public static List<BaseZetadataDetailRow> GetBaseZetadataByConnection(string connectionName)
        {
            try
            {
                using var dbIO = DbIO.Instance(DbConf.FromSettings(BaseZetadataDbConfName));
                var prm = dbIO.CreateParameter("ConnectionName", "NVARCHAR", 200, connectionName);
                var dt = dbIO.ToDataTable(
                    "SELECT StructureId, ObjectName, ObjectType, DataType, HumanTitleEn, HumanTitleNative, NoteEn, NoteNative, KeywordsEn, KeywordsNative, CreatedOn, UpdatedOn FROM BaseZetadata WITH (NOLOCK) WHERE ConnectionName = @ConnectionName",
                    new List<DbParameter> { prm },
                    "Master");
                var table = dt["Master"];
                var list = new List<BaseZetadataDetailRow>();
                foreach (DataRow row in table.Rows)
                {
                    list.Add(new BaseZetadataDetailRow
                    {
                        StructureId = row["StructureId"]?.ToString() ?? "",
                        ObjectName = row["ObjectName"]?.ToString(),
                        ObjectType = row["ObjectType"]?.ToString(),
                        DataType = row["DataType"]?.ToString(),
                        HumanTitleEn = row["HumanTitleEn"]?.ToString(),
                        HumanTitleNative = row["HumanTitleNative"]?.ToString(),
                        NoteEn = row["NoteEn"]?.ToString(),
                        NoteNative = row["NoteNative"]?.ToString(),
                        KeywordsEn = row["KeywordsEn"]?.ToString(),
                        KeywordsNative = row["KeywordsNative"]?.ToString(),
                        CreatedOn = row["CreatedOn"] == DBNull.Value ? null : (DateTime?)row["CreatedOn"],
                        UpdatedOn = row["UpdatedOn"] == DBNull.Value ? null : (DateTime?)row["UpdatedOn"]
                    });
                }
                return list;
            }
            catch { return []; }
        }

        /// <summary>
        /// Inserts a new BaseZetadata row (manual enrich).
        /// </summary>
        public static bool CreateBaseZetadata(string structureId, string connectionName, string objectName, string? objectType, string? humanTitleEn, string? humanTitleNative, string? noteEn, string? noteNative, string? keywordsEn, string? keywordsNative)
        {
            if (string.IsNullOrEmpty(structureId) || string.IsNullOrEmpty(connectionName))
                throw new InvalidOperationException("StructureId and ConnectionName are required.");
            using var dbIO = DbIO.Instance(DbConf.FromSettings(BaseZetadataDbConfName));
            const string emptyVector = "[]";
            var prms = new List<DbParameter>
            {
                dbIO.CreateParameter("StructureId", "CHAR", 32, structureId),
                dbIO.CreateParameter("ConnectionName", "NVARCHAR", 200, connectionName),
                dbIO.CreateParameter("ObjectName", "NVARCHAR", 256, (object?)objectName ?? DBNull.Value),
                dbIO.CreateParameter("ObjectType", "VARCHAR", 50, (object?)objectType ?? DBNull.Value),
                dbIO.CreateParameter("HumanTitleEn", "NVARCHAR", 500, (object?)humanTitleEn ?? DBNull.Value),
                dbIO.CreateParameter("HumanTitleNative", "NVARCHAR", 500, (object?)humanTitleNative ?? DBNull.Value),
                dbIO.CreateParameter("NoteEn", "NVARCHAR", -1, (object?)noteEn ?? DBNull.Value),
                dbIO.CreateParameter("NoteNative", "NVARCHAR", -1, (object?)noteNative ?? DBNull.Value),
                dbIO.CreateParameter("KeywordsEn", "NVARCHAR", -1, (object?)keywordsEn ?? DBNull.Value),
                dbIO.CreateParameter("KeywordsNative", "NVARCHAR", -1, (object?)keywordsNative ?? DBNull.Value),
                dbIO.CreateParameter("HumanTitleEnVector", "NVARCHAR", -1, emptyVector),
                dbIO.CreateParameter("HumanTitleNativeVector", "NVARCHAR", -1, emptyVector),
                dbIO.CreateParameter("NoteEnVector", "NVARCHAR", -1, emptyVector),
                dbIO.CreateParameter("NoteNativeVector", "NVARCHAR", -1, emptyVector),
                dbIO.CreateParameter("KeywordsEnVector", "NVARCHAR", -1, emptyVector),
                dbIO.CreateParameter("KeywordsNativeVector", "NVARCHAR", -1, emptyVector)
            };
            dbIO.ToNoneQuery(@"
INSERT INTO BaseZetadata (StructureId, ConnectionName, ObjectName, ObjectType, HumanTitleEn, HumanTitleNative, NoteEn, NoteNative, KeywordsEn, KeywordsNative, HumanTitleEnVector, HumanTitleNativeVector, NoteEnVector, NoteNativeVector, KeywordsEnVector, KeywordsNativeVector, CreatedOn, UpdatedOn)
VALUES (@StructureId, @ConnectionName, @ObjectName, @ObjectType, @HumanTitleEn, @HumanTitleNative, @NoteEn, @NoteNative, @KeywordsEn, @KeywordsNative, @HumanTitleEnVector, @HumanTitleNativeVector, @NoteEnVector, @NoteNativeVector, @KeywordsEnVector, @KeywordsNativeVector, GETDATE(), GETDATE())", prms);
            return true;
        }

        /// <summary>
        /// Updates a BaseZetadata row by StructureId (HumanTitleEn, HumanTitleNative, NoteEn, NoteNative, KeywordsEn, KeywordsNative).
        /// </summary>
        public static bool UpdateBaseZetadata(string structureId, string? humanTitleEn, string? humanTitleNative, string? noteEn, string? noteNative, string? keywordsEn, string? keywordsNative)
        {
            if (string.IsNullOrEmpty(structureId)) return false;
            try
            {
                using var dbIO = DbIO.Instance(DbConf.FromSettings(BaseZetadataDbConfName));
                var prms = new List<DbParameter>
                {
                    dbIO.CreateParameter("StructureId", "CHAR", 32, structureId),
                    dbIO.CreateParameter("HumanTitleEn", "NVARCHAR", 500, (object?)humanTitleEn ?? DBNull.Value),
                    dbIO.CreateParameter("HumanTitleNative", "NVARCHAR", 500, (object?)humanTitleNative ?? DBNull.Value),
                    dbIO.CreateParameter("NoteEn", "NVARCHAR", -1, (object?)noteEn ?? DBNull.Value),
                    dbIO.CreateParameter("NoteNative", "NVARCHAR", -1, (object?)noteNative ?? DBNull.Value),
                    dbIO.CreateParameter("KeywordsEn", "NVARCHAR", -1, (object?)keywordsEn ?? DBNull.Value),
                    dbIO.CreateParameter("KeywordsNative", "NVARCHAR", -1, (object?)keywordsNative ?? DBNull.Value)
                };
                dbIO.ToNoneQuery(@"
UPDATE BaseZetadata SET
    HumanTitleEn = @HumanTitleEn,
    HumanTitleNative = @HumanTitleNative,
    NoteEn = @NoteEn,
    NoteNative = @NoteNative,
    KeywordsEn = @KeywordsEn,
    KeywordsNative = @KeywordsNative,
    UpdatedOn = GETDATE()
WHERE StructureId = @StructureId", prms);
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// Deletes a BaseZetadata row by StructureId.
        /// </summary>
        public static bool DeleteBaseZetadata(string structureId)
        {
            if (string.IsNullOrEmpty(structureId)) return false;
            try
            {
                using var dbIO = DbIO.Instance(DbConf.FromSettings(BaseZetadataDbConfName));
                var prm = dbIO.CreateParameter("StructureId", "CHAR", 32, structureId);
                dbIO.ToNoneQuery("DELETE FROM BaseZetadata WHERE StructureId = @StructureId", new List<DbParameter> { prm });
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// Returns set of StructureIds already stored in BaseZetadata (DefaultRepo) for the given connection.
        /// </summary>
        public static List<string> GetEnrichedStructureIds(string connectionName)
        {
            try
            {
                using var dbIO = DbIO.Instance(DbConf.FromSettings(BaseZetadataDbConfName));
                var prm = dbIO.CreateParameter("ConnectionName", "NVARCHAR", 200, connectionName);
                var dt = dbIO.ToDataTable(
                    "SELECT StructureId FROM BaseZetadata WITH (NOLOCK) WHERE ConnectionName = @ConnectionName",
                    new List<DbParameter> { prm },
                    "Master");
                var table = dt["Master"];
                var list = new List<string>();
                foreach (DataRow row in table.Rows)
                {
                    var id = row["StructureId"]?.ToString();
                    if (!string.IsNullOrEmpty(id)) list.Add(id);
                }
                return list;
            }
            catch
            {
                return [];
            }
        }

        private static string ComputeStructureId(string connectionName, string schemaName, string tableName, string objectName)
        {
            var input = $"{connectionName}:{schemaName}:{tableName}:{objectName}";
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = MD5.HashData(bytes);
            return Convert.ToHexString(hash).ToLowerInvariant();
        }

        public static List<string> GetObjectDependencies(string dbConfName, string objectName)
        {
            DbSchemaUtils dbSchemaUtils = new(dbConfName);
            return dbSchemaUtils.GetObjectDependencies(objectName);
        }

        public static List<object> GetAllDbObjectsWithDependencies(string dbConfName)
        {
            DbSchemaUtils dbSchemaUtils = new(dbConfName);
            return dbSchemaUtils.GetAllObjectsForDiagram(dbConfName);
        }

        public static List<object> GetDbObjectsForDiagram(string dbConfName, string objectTypes = "Table")
		{
			DbSchemaUtils dbSchemaUtils = new(dbConfName);
			List<object> allObjects = [];
			
			// Default to tables only if no objectTypes specified
			if (string.IsNullOrEmpty(objectTypes))
			{
				objectTypes = "Table";
			}
			
			var types = objectTypes.Split(',').Select(t => t.Trim()).ToList();
			
			if (types.Contains("Table"))
			{
				var tables = dbSchemaUtils.GetTables();
				foreach (var table in tables)
				{
					allObjects.Add(new 
					{ 
						ObjectType = "Table",
						Name = table.Name,
						Columns = table.Columns,
						Dependencies = (List<string>?)null
					});
				}
			}
			
			if (types.Contains("View"))
			{
				var views = dbSchemaUtils.GetViews();
				foreach (var view in views)
				{
					var columns = dbSchemaUtils.GetTableViewColumns(view.Name);
					var dependencies = dbSchemaUtils.GetObjectDependencies(view.Name);
					allObjects.Add(new 
					{ 
						ObjectType = "View",
						Name = view.Name,
						Columns = columns,
						Dependencies = dependencies
					});
				}
			}
			
			if (types.Contains("StoredProcedure"))
			{
				var procedures = dbSchemaUtils.GetProcedures();
				foreach (var proc in procedures)
				{
					var parameters = dbSchemaUtils.GetProceduresFunctionsParameters(proc.Name);
					var dependencies = dbSchemaUtils.GetObjectDependencies(proc.Name);
					allObjects.Add(new 
					{ 
						ObjectType = "StoredProcedure",
						Name = proc.Name,
						Columns = (List<DbColumn>?)null,
						Parameters = parameters,
						Dependencies = dependencies
					});
				}
			}
			
			if (types.Contains("Function"))
			{
				var scalarFunctions = dbSchemaUtils.GetScalarFunctions();
				foreach (var func in scalarFunctions)
				{
					var parameters = dbSchemaUtils.GetProceduresFunctionsParameters(func.Name);
					var dependencies = dbSchemaUtils.GetObjectDependencies(func.Name);
					allObjects.Add(new 
					{ 
						ObjectType = "Function",
						Name = func.Name,
						Columns = (List<DbColumn>?)null,
						Parameters = parameters,
						Dependencies = dependencies
					});
				}
				
				var tableFunctions = dbSchemaUtils.GetTableFunctions();
				foreach (var func in tableFunctions)
				{
					var parameters = dbSchemaUtils.GetProceduresFunctionsParameters(func.Name);
					var dependencies = dbSchemaUtils.GetObjectDependencies(func.Name);
					allObjects.Add(new 
					{ 
						ObjectType = "Function",
						Name = func.Name,
						Columns = (List<DbColumn>?)null,
						Parameters = parameters,
						Dependencies = dependencies
					});
				}
			}
			
			return allObjects;
		}
		public static bool SaveTableSchema(string dbConfName, JsonElement tableDef)
		{
			DbTable? dbTable = ExtensionsForJson.TryDeserializeTo<DbTable>(tableDef, new JsonSerializerOptions() { IncludeFields = true }) ?? throw new AppEndException("TableDefinationIsNotValid", System.Reflection.MethodBase.GetCurrentMethod())
					.AddParam("TableDef", tableDef)
					.GetEx();
			DbSchemaUtils dbSchemaUtils = new(dbConfName);
			dbSchemaUtils.CreateOrAlterTable(dbTable);

			DbDialogServices.SyncDbDialog(dbConfName, dbTable.Name);

			return true;
		}
		public static List<DbColumnChangeTrackable> ReadObjectSchema(string dbConfName, string objectName)
		{
			DbSchemaUtils dbSchemaUtils = new(dbConfName);
			List<DbColumn> columns = dbSchemaUtils.GetTableViewColumns(objectName);
			List<DbColumnChangeTrackable> tblColumns = [];
			DbDialog? dbDialog = DbDialog.TryLoad(AppEndSettings.ServerObjectsPath, dbConfName, objectName);
			foreach (DbColumn c in columns)
			{
				if (c.Fk is null && dbDialog is not null)
				{
					DbColumn? dbc = dbDialog.TryGetColumn(c.Name);
					if (dbc != null) c.Fk = dbDialog.GetColumn(c.Name).Fk;
				}

				tblColumns.Add
				(
					new DbColumnChangeTrackable(c.Name)
					{
						InitialName = c.Name,
						AllowNull = c.AllowNull,
						DbDefault = c.DbDefault,
						DbType = c.DbType,
						IsIdentity = c.IsIdentity,
						IdentityStart = c.IdentityStart,
						IdentityStep = c.IdentityStep,
						IsPrimaryKey = c.IsPrimaryKey,
						Size = c.Size,
						State = "",
						Fk = c.Fk
					}
				);
			}
			return tblColumns;
		}



		public static List<NameValue> GetDataSources()
		{
			List<NameValue> list = [];
			foreach (var n in AppEndSettings.DbServers)
				if (n is not null)
					list.Add(new() { Name = n.AsObject()["Name"].ToStringEmpty(), Value = n.AsObject()["ServerType"].ToStringEmpty() });

			return list;
		}

		public static List<DbServer> GetDataSourcesWithCnn()
		{
			List<DbServer> list = [];
			foreach (var n in AppEndSettings.DbServers)
			{
				if (n is not null)
				{
					JsonObject jo = n.AsObject();
					list.Add
					(
						new()
						{
							Name = jo["Name"].ToStringEmpty(),
							ServerType = jo["ServerType"].ToStringEmpty(),
							ConnectionString = jo["ConnectionString"].ToStringEmpty()
						}
					);
				}
			}
			return list;
		}
		public static bool AddOrAlterDbServer(JsonElement serverInfo)
		{
			DbServer? dbServer = ExtensionsForJson.TryDeserializeTo<DbServer>(serverInfo, new JsonSerializerOptions() { IncludeFields = true }) ?? throw new AppEndException("DeserializeError", System.Reflection.MethodBase.GetCurrentMethod())
					.AddParam("DbServerInfo", serverInfo)
					.GetEx();
			DbConf dbInfo = new()
			{
				Name = dbServer.Name,
				ServerType = Enum.Parse<ServerType>(dbServer.ServerType),
				ConnectionString = dbServer.ConnectionString
			};
			dbInfo.Save();

			return true;
		}
		public static bool RemoveDbServer(string dbServerName)
		{
			DbConf.Remove(dbServerName);
			return true;
		}
		public static object Exec(string dbConfName, string query)
		{
			Dictionary<string, DataTable> results = DbIO.Instance(DbConf.FromSettings(dbConfName)).ToDataSet(query);
			Dictionary<string, string> finalResults = new Dictionary<string, string>();
			foreach (var item in results)
			{
				finalResults.Add(item.Key, item.Value.ToCSV());
			}
			return finalResults;
		}

		public static bool TestDbConnection(JsonElement serverInfo)
		{
			DbServer? dbServer = null;
			try
			{
				dbServer = ExtensionsForJson.TryDeserializeTo<DbServer>(serverInfo, new JsonSerializerOptions() { IncludeFields = true });
				if (dbServer == null)
				{
					throw new AppEndException("DeserializeError", System.Reflection.MethodBase.GetCurrentMethod())
						.AddParam("DbServerInfo", serverInfo)
						.GetEx();
				}

				// Validate required fields
				if (string.IsNullOrWhiteSpace(dbServer.ConnectionString))
				{
					throw new AppEndException("ConnectionTestFailed", System.Reflection.MethodBase.GetCurrentMethod())
						.AddParam("ErrorMessage", "ConnectionString cannot be empty")
						.GetEx();
				}

				if (string.IsNullOrWhiteSpace(dbServer.ServerType))
				{
					throw new AppEndException("ConnectionTestFailed", System.Reflection.MethodBase.GetCurrentMethod())
						.AddParam("ErrorMessage", "ServerType cannot be empty")
						.GetEx();
				}

				// Validate and parse ServerType
				if (!Enum.TryParse<ServerType>(dbServer.ServerType, ignoreCase: true, out ServerType serverType))
				{
					throw new AppEndException("ConnectionTestFailed", System.Reflection.MethodBase.GetCurrentMethod())
						.AddParam("ErrorMessage", $"Invalid ServerType: '{dbServer.ServerType}'. Valid values are: MsSql, MySql, Oracle, Postgres")
						.AddParam("ServerType", dbServer.ServerType)
						.GetEx();
				}

				DbConf dbInfo = new()
				{
					Name = dbServer.Name ?? "Test",
					ServerType = serverType,
					ConnectionString = dbServer.ConnectionString
				};

				try
				{
					using DbIO dbIO = DbIO.Instance(dbInfo);
					using DbConnection connection = dbIO.CreateConnection();
					// If we reach here, connection is successful
					return true;
				}
				catch (Exception ex)
				{
					// Build detailed error message
					string errorMessage = ex.Message;
					if (ex.InnerException != null)
					{
						errorMessage += $" | Inner Exception: {ex.InnerException.Message}";
					}

					// Log the error for debugging
					LogMan.LogError($"TestDbConnection failed for server '{dbServer.Name}': {errorMessage}");

					// Include the actual error message in the exception message so it's visible to the client
					string fullErrorMessage = $"ConnectionTestFailed: {errorMessage}";
					throw new AppEndException(fullErrorMessage, System.Reflection.MethodBase.GetCurrentMethod())
						.AddParam("ServerName", dbServer.Name ?? "Unknown")
						.AddParam("ServerType", dbServer.ServerType)
						.AddParam("ErrorMessage", errorMessage)
						.GetEx();
				}
			}
			catch (AppEndException appEx)
			{
				// Re-throw AppEndException as-is, but ensure error message is visible
				// If it has ErrorMessage in Data, include it in the message
				if (appEx.GetParams().Any(p => p.Key == "ErrorMessage"))
				{
					var errorMsg = appEx.GetParams().First(p => p.Key == "ErrorMessage").Value?.ToString();
					if (!string.IsNullOrEmpty(errorMsg) && !appEx.Message.Contains(errorMsg))
					{
						throw new AppEndException($"{appEx.Message}: {errorMsg}", System.Reflection.MethodBase.GetCurrentMethod())
							.AddParam("ErrorMessage", errorMsg)
							.GetEx();
					}
				}
				throw;
			}
			catch (Exception ex)
			{
				// Catch any other unexpected exceptions
				string serverName = dbServer?.Name ?? "Unknown";
				string errorMessage = ex.Message;

				// Add inner exception details if available
				if (ex.InnerException != null)
				{
					errorMessage += $" | Inner Exception: {ex.InnerException.Message}";
				}

				// Add stack trace for debugging (first line only)
				if (!string.IsNullOrEmpty(ex.StackTrace))
				{
					var firstLine = ex.StackTrace.Split('\n')[0].Trim();
					errorMessage += $" | Location: {firstLine}";
				}

				LogMan.LogError($"TestDbConnection unexpected error for server '{serverName}': {errorMessage}");
				LogMan.LogError($"Full exception: {ex}");

				// Include the actual error message in the exception message so it's visible to the client
				string fullErrorMessage = $"ConnectionTestFailed: {errorMessage}";
				throw new AppEndException(fullErrorMessage, System.Reflection.MethodBase.GetCurrentMethod())
					.AddParam("ErrorMessage", errorMessage)
					.AddParam("ExceptionType", ex.GetType().Name)
					.GetEx();
			}
		}

	}
}

public class DbServer
{
	public string Name { set; get; } = "";
	public string ServerType { set; get; } = "";
	public string ConnectionString { set; get; } = "";
}

public class SchemaRowForEnrich
{
	public string ObjectType { set; get; } = ""; // Table | Column
	public string SchemaName { set; get; } = "";
	public string TableName { set; get; } = "";
	public string ObjectName { set; get; } = "";
	public string StructureId { set; get; } = "";
}

public class BaseZetadataDetailRow
{
	public string StructureId { set; get; } = "";
	public string? ObjectName { set; get; }
	public string? ObjectType { set; get; }
	public string? DataType { set; get; }
	public string? HumanTitleEn { set; get; }
	public string? HumanTitleNative { set; get; }
	public string? NoteEn { set; get; }
	public string? NoteNative { set; get; }
	public string? KeywordsEn { set; get; }
	public string? KeywordsNative { set; get; }
	public DateTime? CreatedOn { set; get; }
	public DateTime? UpdatedOn { set; get; }
}
