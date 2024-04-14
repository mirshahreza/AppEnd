using AppEndCommon;
using AppEndDbIO;
using AppEndDynaCode;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace AppEndServer
{
	public static class HostingDbServices
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
			HostingDbDialogServices.RemoveServerObjects(dbConfName, objectName, objectType);
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
			HostingDbDialogServices.RemoveServerObjects(dbConfName, objectNameOld, objectType);
			if (objectType.EqualsIgnoreCase("table"))
			{
				dbSchemaUtils.RenameTable(objectNameOld, objectNameNew);
				return true;
			}
			else
			{
				throw new AppEndException("NotImplementedYet")
					.AddParam("ObjectType", objectType)
					.AddParam("Site", $"{System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name}, {System.Reflection.MethodBase.GetCurrentMethod()?.Name}")
					;
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
			DbTable? dbTable = ExtensionsForJson.TryDeserializeTo<DbTable>(tableDef, new JsonSerializerOptions() { IncludeFields = true }) ?? throw new AppEndException("TableDefinationIsNotValid")
					.AddParam("TableDef", tableDef)
					.AddParam("Site", $"{System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name}, {System.Reflection.MethodBase.GetCurrentMethod()?.Name}");
			DbSchemaUtils dbSchemaUtils = new(dbConfName);
			dbSchemaUtils.CreateOrAlterTable(dbTable);

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
		public static bool AddOrUpdateDbServer(JsonElement serverInfo)
		{
			DbServer? dbServer = ExtensionsForJson.TryDeserializeTo<DbServer>(serverInfo, new JsonSerializerOptions() { IncludeFields = true }) ?? throw new AppEndException("DeserializeError")
					.AddParam("DbServerInfo", serverInfo)
					.AddParam("Site", $"{System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name}, {System.Reflection.MethodBase.GetCurrentMethod()?.Name}");
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

	}

	public class DbServer
	{
		public string Name { set; get; } = "";
		public string ServerType { set; get; } = "";
		public string ConnectionString { set; get; } = "";
	}
}
