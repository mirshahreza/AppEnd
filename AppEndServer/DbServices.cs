using AppEndCommon;
using AppEndDbIO;
using AppEndDynaCode;
using System.Data.Common;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
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

	public class DbServer
	{
		public string Name { set; get; } = "";
		public string ServerType { set; get; } = "";
		public string ConnectionString { set; get; } = "";
	}
}
