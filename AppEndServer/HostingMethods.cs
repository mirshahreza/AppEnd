using AppEndCommon;
using AppEndDynaCode;
using AppEndDbIO;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;
using System.Net;
using System.Xml.Linq;

namespace AppEndServer
{
	public static class HostingMethods
    {
		public static bool CreateLogicalFk(string dbConfName, string fkName, string baseTable, string baseColumn, string targetTable, string targetColumn)
		{
            DbDialogFactory dbDialogFactory = new(dbConfName);
            dbDialogFactory.CreateLogicalFk(fkName, baseTable, baseColumn, targetTable, targetColumn);
			return true;
		}
		public static bool RemoveLogicalFk(string dbConfName, string baseTable, string baseColumn)
		{
			DbDialogFactory dbDialogFactory = new(dbConfName);
			dbDialogFactory.RemoveLogicalFk(baseTable, baseColumn);
			return true;
		}
		public static List<DynaClass> GetDynaClasses()
        {
            return DynaCode.GetDynaClasses();
        }

		public static string[] GetStoredApiCalls()
		{
			List<string> res = new();
			string[] files = Directory.GetFiles($"{AppEndSettings.ApiCallsPath}");
			foreach (string f in files)
			{
				res.Add(f.Replace(AppEndSettings.ApiCallsPath + "\\", "").Replace(".json", ""));
			}
			return res.ToArray();
		}
		public static Dictionary<string, Exception> BuildUiForDbObject(string dbConfName, string objectName)
        {
            DbDialog? dbDialog = DbDialog.Load(AppEndSettings.ServerObjectsPath, dbConfName, objectName);
            if (dbDialog.PreventBuildUI == true) return [];
            if (dbDialog.ClientUIs is null || dbDialog.ClientUIs.Count == 0) return [];

			Dictionary<string, Exception> errors = [];
			Dictionary<string,string> outputs = [];
            foreach(ClientUI clientUi in dbDialog.ClientUIs)
            {
                try
                {
                    string s = TemplateEngine.RunTemplate(dbConfName, objectName, clientUi);
					string outputVueFile = $"{AppEndSettings.ClientObjectsPath}/.DbComponents/{clientUi.FileName}.vue";
                    outputs[outputVueFile] = s;
                    
                    if (File.Exists(outputVueFile)) File.Delete(outputVueFile);
					File.WriteAllText(outputVueFile, s);
				}
				catch (Exception ex)
                {
                    errors.Add(clientUi.FileName, ex);
                }
            }

            return errors;
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
                if(n is not null)
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
        public static object? GetMethodSettings(string namespaceName, string className, string methodName)
        {
            return DynaCode.ReadMethodSettings($"{namespaceName}.{className}.{methodName}");
        }
        public static object? WriteMethodSettings(string namespaceName, string className, string methodName, JsonElement newMethodSettings)
        {
            MethodSettings? methodSettings = ExtensionsForJson.TryDeserializeTo<MethodSettings>(newMethodSettings, new JsonSerializerOptions() { IncludeFields = true }) ?? throw new AppEndException("MethodSettingsIsNotValid")
                    .AddParam("MethodSettings", newMethodSettings)
                    .AddParam("Site", $"{System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name}, {System.Reflection.MethodBase.GetCurrentMethod()?.Name}");
			DynaCode.WriteMethodSettings($"{namespaceName}.{className}.{methodName}", methodSettings);
            return true;
        }
        public static object? RemoveServerObjects(string dbConfName, string objectName, string objectType)
        {
            DbSchemaUtils dbSchemaUtils = new(dbConfName);
            List<DbObject> dbObjects = dbSchemaUtils.GetObjects(Enum.Parse<DbObjectType>(objectType), objectName, true);
            if (dbObjects.Count == 0) throw new AppEndException("ObjectDoesNotExist")
                    .AddParam("DbConfName", dbConfName)
                    .AddParam("ObjectType", objectType)
                    .AddParam("ObjectName", objectName)
                    .AddParam("Site", $"{System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name}, {System.Reflection.MethodBase.GetCurrentMethod()?.Name}")
                    ;
            DbDialogFactory dbDialogFactory = new(dbConfName);

			DbDialog? dbDialog = DbDialog.TryLoad(AppEndSettings.ServerObjectsPath, dbConfName, objectName);
			if(dbDialog is not null)
            {
				if (dbDialog.PreventUpdateServerObjects == true) return false;
				if (dbDialog.ClientUIs is not null)
				{
					foreach (ClientUI clientUi in dbDialog.ClientUIs)
					{
						string fullFileName = $"{AppEndSettings.ClientObjectsPath}/.DbComponents/{clientUi.FileName}.vue";
						if (File.Exists(fullFileName)) File.Delete(fullFileName);
					}
				}
			}

			dbDialogFactory.RemoveServerObjectsFor(dbObjects.First().Name);

			return true;
        }
        public static object? CreateServerObjects(string dbConfName, string objectName, string objectType)
        {
            DbSchemaUtils dbSchemaUtils = new(dbConfName);
            List<DbObject> dbObjects = dbSchemaUtils.GetObjects(Enum.Parse<DbObjectType>(objectType), objectName, true);
            if (dbObjects.Count == 0) throw new AppEndException("ObjectDoesNotExist")
                    .AddParam("DbConfName", dbConfName)
                    .AddParam("ObjectType", objectType)
                    .AddParam("ObjectName", objectName)
                    .AddParam("Site", $"{System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name}, {System.Reflection.MethodBase.GetCurrentMethod()?.Name}")
                    ;
            DbDialogFactory dbDialogFactory = new(dbConfName);
            dbDialogFactory.CreateServerObjectsFor(dbObjects.First());

            DbDialog dbDialog = DbDialog.Load(AppEndSettings.ServerObjectsPath, dbConfName, objectName);

			DynaCode.Refresh();

			foreach (var dbq in dbDialog.DbQueries)
            {
                DynaCode.WriteMethodSettings($"{dbConfName}.{objectName}.{dbq.Name}", new MethodSettings() { LogPolicy = new LogPolicy() { OnErrorLogMethod = AppEndSettings.DefaultErrorLoggerMethod, OnSuccessLogMethod = AppEndSettings.DefaultSuccessLoggerMethod } });
			}

            return true;
        }
        public static bool DropFk(string dbConfName, string objectName, string fkName)
        {
            DbSchemaUtils dbSchemaUtils = new(dbConfName);
            dbSchemaUtils.DropFk(objectName, fkName);
            return true;
        }
        public static object? DropObject(string dbConfName, string objectName, string objectType)
        {
            DbSchemaUtils dbSchemaUtils = new(dbConfName);
            RemoveServerObjects(dbConfName, objectName, objectType);
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
            RemoveServerObjects(dbConfName, objectNameOld, objectType);
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
        public static object? ReCreateMethodJson(string dbConfName, string objectName, string objectType, string methodName)
        {
            DbSchemaUtils dbSchemaUtils = new(dbConfName);
            List<DbObject> dbObjects = dbSchemaUtils.GetObjects(Enum.Parse<DbObjectType>(objectType), objectName, true);
            if (dbObjects.Count == 0) throw new AppEndException("ObjectDoesNotExist")
                    .AddParam("DbConfName", dbConfName)
                    .AddParam("ObjectType", objectType)
                    .AddParam("ObjectName", objectName)
                    .AddParam("Site", $"{System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name}, {System.Reflection.MethodBase.GetCurrentMethod()?.Name}")
                    ;
            DbDialogFactory dbDialogFactory = new(dbConfName);
            dbDialogFactory.ReCreateMethodJson(dbObjects.First(), methodName);
            DynaCode.Refresh();
            return true;
        }
        public static object? RemoveMethodQuery(string dbConfName, string objectName, string methodName)
        {
            DbDialogFactory dbDialogFactory = new(dbConfName);
            dbDialogFactory.RemoveQuery(objectName, methodName);
            return true;
        }
        public static object? RemoveNotMappedMethod(string dbConfName, string objectName, string methodName)
        {
            DynaCode.RemoveMethod($"{dbConfName}.{objectName}.{methodName}");
            return true;
        }
        public static object? DuplicateMethodQuery(string dbConfName, string objectName, string methodName, string methodCopyName)
        {
            DbDialogFactory dbDialogFactory = new(dbConfName);
            dbDialogFactory.DuplicateQuery(objectName, methodName, methodCopyName);
            DynaCode.DuplicateMethod($"{dbConfName}.{objectName}.{methodName}", methodCopyName);
            return true;
        }
        public static object? CreateNewNotMappedMethod(string dbConfName, string objectName, string methodName)
        {
            DynaCode.CreateMethod($"{dbConfName}.{objectName}", methodName);
            return true;
        }
        public static bool CreateNewMethodQuery(string dbConfName, string objectName, string methodType, string methodName)
        {
            DbDialogFactory dbDialogFactory = new(dbConfName);
            dbDialogFactory.CreateQuery(objectName, methodType, methodName);
            DynaCode.CreateMethod($"{dbConfName}.{objectName}", methodName);
            return true;
        }
        public static bool CreateNewUpdateByKey(string dbConfName, string objectName, List<string> columnsToUpdate, string methodName, string? byColumnName, string? onColumnName, string? logTableName)
        {
            DbDialogFactory dbDialogFactory = new(dbConfName);
            dbDialogFactory.CreateNewUpdateByKey(objectName, columnsToUpdate, methodName, byColumnName, onColumnName, logTableName);
            return true;
        }
        public static object? SyncDbDialog(string dbConfName, string objectName)
        {
            DbDialogFactory dbDialogFactory = new(dbConfName);
            dbDialogFactory.SyncDbDialog(objectName);
            return true;
        }
        public static bool RemoveRemovedRelationsFromDbQueries(string dbConfName, string objectName)
        {
            DbDialogFactory dbDialogFactory = new(dbConfName);
            dbDialogFactory.RemoveRemovedRelationsFromDbQueries(objectName);
            return true;
        }
        public static List<DbObjectStack> GetDbObjectsStack(string dbConfName, string objectType, string filter)
        {
            string dbCn = dbConfName.Split(':')[0];
            List<DbObjectStack> dbObjectStacks = [];
            DbSchemaUtils dbSchemaUtils = new(dbCn);
            DbObjectType dbObjectType = Enum.Parse<DbObjectType>(objectType);
            List<DbObject> dbObjects = dbSchemaUtils.GetObjects(dbObjectType, filter);
            List<string> clientComponents = Directory.GetFiles($"{AppEndSettings.ClientObjectsPath}/.DbComponents", "*.vue").Select(i => i.Replace("\\", "/").Replace($"{AppEndSettings.ClientObjectsPath}/.DbComponents/", "")).ToList();

			foreach (DbObject dbObject in dbObjects)
            {
                string dbDialogFilePath = DbDialog.GetFullFilePath(AppEndSettings.ServerObjectsPath, dbCn, dbObject.Name);
                FileInfo fileInfo = new FileInfo(dbDialogFilePath);
                DbObjectStack dbObjectStack = new(dbObject.Name, dbObject.DbObjectType.ToString()) { HasServerObjects = File.Exists(dbDialogFilePath), LastWriteTime = fileInfo.LastWriteTime };
                if (dbObjectStack.HasServerObjects)
                {
                    dbObjectStack.ClientComponents.AddRange(clientComponents.Where(i => i.StartsWith($"{dbCn}_{dbObject.Name}_ReadList") || i.StartsWith($"{dbCn}_{dbObject.Name}_ReadTreeList")).ToList());
				}
                dbObjectStacks.Add(dbObjectStack);
            }
            return [.. dbObjectStacks.OrderBy(i => i.LastWriteTime).Reverse()];
        }
		public static List<string>? GetDbObjectNotMappedMethods(string dbConfName, string objectName)
        {
            string dbCn = dbConfName.Split(':')[0];

            Type? t = DynaCode.DynaAsm.GetType($"{dbCn}.{objectName}");
            string dbDialogFilePath = DbDialog.GetFullFilePath(AppEndSettings.ServerObjectsPath, dbCn, objectName);
            string? controllerFilePath = File.Exists(dbDialogFilePath) ? dbDialogFilePath.Replace(".dbdialog.json", ".cs") : null;
            DbDialog? dbDialog = DbDialog.Load(AppEndSettings.ServerObjectsPath, dbConfName, objectName);

            if (controllerFilePath is null) throw new AppEndException("ControllerFileNotExist")
                    .AddParam("DbConfName", dbConfName)
                    .AddParam("ObjectName", objectName)
                    .AddParam("Site", $"{System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name}, {System.Reflection.MethodBase.GetCurrentMethod()?.Name}")
                    ;

            if (dbDialog is null) throw new AppEndException("DbDialogFileNotExist")
                    .AddParam("DbConfName", dbConfName)
                    .AddParam("ObjectName", objectName)
                    .AddParam("Site", $"{System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name}, {System.Reflection.MethodBase.GetCurrentMethod()?.Name}")
                    ;

            List<string>? methods = t?.GetMethods().Where(i => i.IsStatic == true && i.IsPublic).Select(i => i.Name).ToList();

            if (methods is null) return null;

            foreach(DbQuery dbQuery in dbDialog.DbQueries)
            {
                if(methods.ContainsIgnoreCase(dbQuery.Name)) methods.Remove(dbQuery.Name);
            }

            return methods;
        }
        public static List<DbTable> GetDbTables(string dbConfName)
        {
            DbSchemaUtils dbSchemaUtils = new(dbConfName);
            return dbSchemaUtils.GetTables();
        }
        public static void CreateController(string creationPath, string controllerName)
        {
            string[] parts = controllerName.Replace(".cs", string.Empty).Split('.');
            string ns;
            string cn;
            if (parts.Length == 1)
            {
                ns = "AppEnd";
                cn = controllerName;
            }
            else if (parts.Length == 2)
            {
                ns = parts[0];
                cn = parts[1];
            }
            else
            {
                throw new AppEndException("ControllerNameCanNotHaveMoreThan1DotInTheName")
                    .AddParam("ControllerName", controllerName)
                    .AddParam("Site", $"{System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name}, {System.Reflection.MethodBase.GetCurrentMethod()?.Name}")
                    ;
            }

            string fileName = $"{ns}.{cn}.cs";
            string filePath = $"{creationPath}/{fileName}";
            string csharpBody = new AppEndClass(cn, ns).ToCode();
            File.WriteAllText(filePath, csharpBody);
            DynaCode.Refresh();
        }
        public static bool RebuildProject()
        {
            DynaCode.Refresh();
            return true;
        }

		public static List<string> GetUiComponents(string folderName)
		{
			string dir = $"{AppEndSettings.ClientObjectsPath}/{folderName}";
			string[] ts = Directory.GetFiles(dir);
			List<string> result = [];
			foreach (string t in ts)
			{
				result.Add(new FileInfo(t).Name);
			}
			return result;
		}
		public static bool CreateEmptyComponent(string componentFullPath)
		{
			File.Copy($"{AppEndSettings.ClientObjectsPath}/..templates/VueEmptyComponent.vue", componentFullPath);
			return true;
		}
		public static bool RenameFileItem(string filePath, string newFilePath)
		{
			File.Move($"{filePath}", $"{newFilePath}");
			return true;
		}
		public static bool FolderHasContent(string pathToCheck)
		{
			string[] directories = Directory.GetDirectories($"{pathToCheck}");
			string[] files = Directory.GetFiles($"{pathToCheck}");
			if (directories.Length > 0 || files.Length > 0) return true;
			return false;
		}
		public static bool CreateNewFolder(string pathToCreate, string newFolderName)
		{
			string p = $"{pathToCreate}/{newFolderName}";
			if (Directory.Exists(p)) throw new AppEndException("NameInUsed")
					.AddParam("PathToCreate", pathToCreate)
					.AddParam("NewFolderName", newFolderName)
					.AddParam("Site", $"{System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name}, {System.Reflection.MethodBase.GetCurrentMethod()?.Name}")
					;
			Directory.CreateDirectory(p);
			return true;
		}
		public static bool DuplicateFileItem(string filePath)
		{
            FileInfo fi = new(filePath);
            File.Copy(filePath, filePath.Replace(fi.Extension, StaticMethods.GetRandomName("_Copy") + fi.Extension));
			return true;
		}
		public static bool DeleteFileItem(string filePath)
		{
			File.Delete($"{filePath}");
			return true;
		}
		public static bool DeleteFolderItem(string pathToDelete, bool recursive)
		{
			Directory.Delete(pathToDelete, recursive);
			return true;
		}
		public static Dictionary<string, List<NameValue>> GetFolderContent(string pathToRead)
		{
			Dictionary<string, List<NameValue>> keyValuePairs = new()
			{
				{ "folders", new() { } },
				{ "files", new() { } }
			};
			var directories = Directory.GetDirectories(pathToRead);
			foreach (var d in directories)
			{
				keyValuePairs["folders"].Add(new()
				{
					Name = (new DirectoryInfo(d)).Name,
					Value = d.Replace("\\", "/")
				});
			}
			var files = Directory.GetFiles(pathToRead);
			foreach (string s in files)
			{
				keyValuePairs["files"].Add(new()
				{
					Name = (new FileInfo(s)).Name,
					Value = s.Replace("\\", "/")
				});
			}

			return keyValuePairs;

		}
		public static List<NameValue> GetThemes()
		{
            List<NameValue> apps = [];
			var directories = Directory.GetDirectories(AppEndSettings.ClientObjectsPath);
			foreach (var d in directories)
			{
				DirectoryInfo directoryInfo = new(d);
                if (!AppEndSettings.ReservedFolders.ContainsIgnoreCase(directoryInfo.Name))
                {
                    JObject appConf = File.ReadAllText(AppEndSettings.ClientObjectsPath + "/" + directoryInfo.Name + "/app.json").ToJObjectByNewtonsoft();
					appConf["navigation"] = new JObject();
					appConf["translation"] = new JObject();
                    apps.Add(new() { Name = directoryInfo.Name, Value = appConf.ToJsonStringByNewtonsoft() });
                }
			}
			return apps;
		}
		public static bool SetThemeProps(string origFolderName, string folderName, string title, string subTitle)
		{
			string appPath = AppEndSettings.ClientObjectsPath + "/" + origFolderName ;
			string appConfPath = appPath + "/app.json";
			JObject joAppConf = File.ReadAllText(appConfPath).ToJObjectByNewtonsoft();
			joAppConf["title"] = title;
			joAppConf["sub-title"] = subTitle;
			File.WriteAllText(appConfPath, joAppConf.ToJsonStringByNewtonsoft());

			if (!AppEndSettings.ReservedFolders.ContainsIgnoreCase(folderName) && !origFolderName.EqualsIgnoreCase(folderName))
			{
				Directory.Move(appPath, AppEndSettings.ClientObjectsPath + "/" + folderName);
			}

			return true;
		}
		public static bool DuplicateTheme(string origFolderName, string copyFolderName)
		{
			if (!AppEndSettings.ReservedFolders.ContainsIgnoreCase(copyFolderName) && !origFolderName.EqualsIgnoreCase(copyFolderName))
			{
				DirectoryInfo dOrig = new(AppEndSettings.ClientObjectsPath + "/" + origFolderName);
				DirectoryInfo dCopy = new(AppEndSettings.ClientObjectsPath + "/" + copyFolderName);
				dOrig.CopyFilesRecursively(dCopy);
			}
			return true;
		}
		public static bool RemoveTheme(string folderName)
		{
			if (!AppEndSettings.ReservedFolders.ContainsIgnoreCase(folderName))
			{
				Directory.Delete(AppEndSettings.ClientObjectsPath + "/" + folderName, true);
			}
			return true;
		}
		public static bool ExtractTranslationKeys(string folderName)
		{
            string appConfigAddr = AppEndSettings.ClientObjectsPath + "/" + folderName + "/app.json";
			JObject appConfig = File.ReadAllText(appConfigAddr).ToJObjectByNewtonsoft();

			List<string> Keys = [];
			Keys.AddRange(GetTranslationKeys(folderName));
			Keys.AddRange(GetTranslationKeys(".DbComponents"));
			Keys.AddRange(GetTranslationKeys(".PublicComponents"));

			if (appConfig["translation"]==null) appConfig["translation"] = new JObject();

            foreach(string k in Keys)
            {
                if (appConfig["translation"][k] == null) appConfig["translation"][k] = k;
			}

            File.WriteAllText(appConfigAddr, appConfig.ToJsonStringByNewtonsoft(true));

			return true;
		}

        private static List<string> GetTranslationKeys(string folderName)
        {
			List<string> Keys = [];
			DirectoryInfo diApp = new(AppEndSettings.ClientObjectsPath + "/" + folderName);
			foreach (string f in diApp.GetFilesRecursive())
			{
				if (f.EndsWith(".vue"))
				{
					FileInfo fi = new(f);
					string fileContent = File.ReadAllText(fi.FullName);
					MatchCollection translations = ExtensionsForString.JsTranslationRegex().Matches(fileContent);
					foreach (Match match in translations)
					{
                        string v = match.Value.Replace("shared.translate(", "").Replace(")", "").Replace(@"""", "").Replace(@"'", "");
                        if(v!= "i" && v != "m" && !v.ContainsIgnoreCase("i.") && !v.ContainsIgnoreCase("inputs.")) Keys.Add(v);
					}
				}
			}
            return Keys;
		}

		public static string GetFileContent(string pathToRead)
		{
			return File.ReadAllText($"{pathToRead}");
		}
		public static bool SaveFileContent(string pathToWrite, string fileContent)
		{
			File.WriteAllText(pathToWrite, fileContent);
			SV.SharedMemoryCache.TryRemove(new FileInfo(pathToWrite).GetCacheKeyForFiles());
			return true;
		}

		public static void RemoveAllCacheItems()
		{
            SV.SharedMemoryCache = new MemoryCache(new MemoryCacheOptions() { TrackStatistics = true });
		}
		public static MemoryCacheState? GetCacheItems(string likeStr)
		{
			MemoryCacheStatistics? ms = SV.SharedMemoryCache.GetCurrentStatistics();
			List<string> keys = [];
			foreach (var key in SV.SharedMemoryCache.GetKeys())
            {
                if (likeStr == "" || key.ToStringEmpty().ContainsIgnoreCase(likeStr)) keys.Add(key.ToStringEmpty());
			}
			if (ms == null) return new MemoryCacheState(0, 0, 0, 0, keys);
			return new(ms.CurrentEntryCount, ms.CurrentEstimatedSize == null ? 0 : (long)ms.CurrentEstimatedSize, ms.TotalMisses, ms.TotalHits, keys);
		}
		public static object? GetCacheItem(string key)
		{
			SV.SharedMemoryCache.TryGetValue(key, out var value);
			return value;
		}
		public static bool RemoveCacheItem(string key)
		{
			SV.SharedMemoryCache.TryRemove(key);
			return true;
		}

        public static JObject CreateStandardLogContent(MethodInfo methodInfo, string actor, string methodFullPath, string clientInfo, CodeInvokeResult codeInvokeResult, object[]? inputParams)
        {
            JObject methodInputs = inputParams is null ? "{}".ToJObjectByNewtonsoft() : inputParams.ExtractInputItems(methodInfo).ToJsonStringByBuiltIn().ToJObjectByNewtonsoft();
            string? recordId = null;
            if (methodInputs["ClientQueryJE"] != null && methodInputs["ClientQueryJE"]?["Params"] != null)
            {
				if (methodInputs["ClientQueryJE"]?["Params"] is JArray paramsArr)
				{
					foreach (JObject jo in paramsArr.Cast<JObject>())
					{
						if (jo["Name"]?.ToString() == "Id")
                        {
							recordId = jo["Value"]?.ToString();
                            break;
						}
					}
				}
			}

            return new()
            {
				["Method"] = methodFullPath,
				["IsSucceeded"] = codeInvokeResult.IsSucceeded,
				["FromCache"] = codeInvokeResult.FromCache,
				["RecordId"] = recordId,
				["EventBy"] = actor,
                ["EventOn"] = DateTime.Now,
				["Duration"] = codeInvokeResult.Duration,
				["ClientInfo"] = clientInfo
            };
		}

		public static object? GetAppEndSummary()
		{
            JObject appendSummary = [];

			appendSummary["ServerPhisicalAddress"] = (new DirectoryInfo(".")).FullName;
			appendSummary["ServerDateTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:MM");
            appendSummary["ServerTimeZone"] = TimeZoneInfo.Local.ToString();

			appendSummary["HostName"] = Dns.GetHostName();
			appendSummary["IpAddress"] = StaticMethods.GetServerIP();

            return appendSummary;
		}

        public static object? GetNodeToDoItems(bool considerLastTime, bool considerIgnoreRules, int ind)
        {
            string logFile = $"deploy_{ind}_{considerLastTime.ToString().ToLower()}.json";
            if (!File.Exists(logFile))
            {
                JsonNode? jn = AppEndSettings.Nodes[ind.ToIntSafe()];
                FileInfo fileInfo = new FileInfo("appsettings.json");
                if (fileInfo.Directory is null) return null;
                JArray arr = fileInfo.Directory.GetFilesRecursiveWithInfo();
                if (considerLastTime)
                {
					JArray list = [];
					JObject n = (JObject)((JArray)GetNodes())[ind];
					string dtStr = n["LastDeploy"].ToStringEmpty();
					foreach (var item in arr)
					{
						if (!dtStr.IsNullOrEmpty() && Convert.ToDateTime(item["LastWrite"].ToStringEmpty()) > Convert.ToDateTime(dtStr))
						{
							item["FilePath"] = item["FilePath"].ToStringEmpty().Replace(fileInfo.Directory.FullName, "");
							list.Add(item);
						}
					}
                    arr = list;
				}

				JArray sorted = new JArray(arr.OrderBy(obj =>  (DateTime)obj["LastWrite"]).Reverse());
				File.WriteAllText(logFile, sorted.ToJsonStringByNewtonsoft());
            }

			return File.ReadAllText(logFile).ToJArrayByNewtonsoft();
		}


		public static object? GetNodes()
        {
			JArray res = new();
			foreach (var n in AppEndSettings.Nodes)
			{
				JObject nn = new();
				nn["Ip"] = n["Ip"].ToStringEmpty();
				nn["Port"] = n["Port"].ToStringEmpty();
				nn["Name"] = n["Name"].ToStringEmpty();
				nn["UserName"] = n["UserName"].ToStringEmpty();
				nn["Password"] = n["Password"].ToStringEmpty();
				nn["LastDeploy"] = n["LastDeploy"].ToStringEmpty();
				nn["FilesToDo"] = new JArray();
				res.Add(nn);
			}
			return res;
		}
		public static void RemoveNode(string ind)
		{
			var nodes = AppEndSettings.Nodes;
			JsonNode? jn = nodes[ind.ToIntSafe()];
			if (jn != null) nodes.Remove(jn);
			AppEndSettings.Nodes = nodes;
			AppEndSettings.Save();
		}
		public static void CreateUpdateNode(int ind, string ip, string port, string name, string userName, string password)
		{
			var nodes = AppEndSettings.Nodes;
            if (ind == -1)
            {
                JObject jn = new();
                jn["Name"] = name;
                jn["Ip"] = ip;
                jn["Port"] = port;
                jn["UserName"] = userName;
                jn["Password"] = password;
                nodes.Add(JsonNode.Parse(jn.ToJsonStringByNewtonsoft()));
            }
            else
            {
                //JsonNode? jn = nodes.FirstOrDefault(i => i["Ip"].ToStringEmpty() == ip);
                JsonNode? jn = nodes[ind];

                if (jn != null)
                {
                    jn["Name"] = name;
                    jn["Port"] = port;
                    jn["UserName"] = userName;
                    jn["Password"] = password;
                }

                nodes = [];
                foreach (var item in nodes)
                {
                    JObject tempJN = new();
                    tempJN["Name"] = item["Name"].ToStringEmpty();
                    tempJN["Ip"] = item["Ip"].ToStringEmpty();
                    tempJN["Port"] = item["Port"].ToStringEmpty();
                    tempJN["UserName"] = item["UserName"].ToStringEmpty();
                    tempJN["Password"] = item["Password"].ToStringEmpty();
                    tempJN["LastDeploy"] = item["LastDeploy"].ToStringEmpty();
                    nodes.Add(JsonNode.Parse(tempJN.ToJsonStringByNewtonsoft()));
                }
            }


			AppEndSettings.Nodes = nodes;
			AppEndSettings.Save();
		}

	}
}
