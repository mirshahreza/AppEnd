using AppEndCommon;
using AppEndDbIO;
using AppEndDynaCode;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace AppEndServer
{
	public static class HostingDbDialogServices
	{
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
			List<string> clientComponents = Directory.GetFiles($"{AppEndSettings.ClientObjectsPath}/a.DbComponents", "*.vue").Select(i => i.Replace("\\", "/").Replace($"{AppEndSettings.ClientObjectsPath}/a.DbComponents/", "")).ToList();

			foreach (DbObject dbObject in dbObjects)
			{
				string dbDialogFilePath = DbDialog.GetFullFilePath(AppEndSettings.ServerObjectsPath, dbCn, dbObject.Name);
				FileInfo fileInfo = new(dbDialogFilePath);
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

			foreach (DbQuery dbQuery in dbDialog.DbQueries)
			{
				if (methods.ContainsIgnoreCase(dbQuery.Name)) methods.Remove(dbQuery.Name);
			}

			return methods;
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
			if (dbDialog is not null)
			{
				if (dbDialog.PreventUpdateServerObjects == true) return false;
				if (dbDialog.ClientUIs is not null)
				{
					foreach (ClientUI clientUi in dbDialog.ClientUIs)
					{
						string fullFileName = $"{AppEndSettings.ClientObjectsPath}/a.DbComponents/{clientUi.FileName}.vue";
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
		

		public static Dictionary<string, Exception> BuildUiForDbObject(string dbConfName, string objectName)
		{
			DbDialog? dbDialog = DbDialog.Load(AppEndSettings.ServerObjectsPath, dbConfName, objectName);
			if (dbDialog.ClientUIs is null || dbDialog.ClientUIs.Count == 0) return [];

			Dictionary<string, Exception> errors = [];

			if (dbDialog.PreventBuildUI == true)
			{
				errors.Add("PreventBuildUI", new Exception("This DbDialog prevented from building UIs by template engine"));
			}
			else
			{
				Dictionary<string, string> outputs = [];
				foreach (ClientUI clientUi in dbDialog.ClientUIs)
				{
					try
					{
						string s = HostingTemplateServices.RunTemplate(dbConfName, objectName, clientUi);
						string outputVueFile = $"{AppEndSettings.ClientObjectsPath}/a.DbComponents/{clientUi.FileName}.vue";
						outputs[outputVueFile] = s;

						if (File.Exists(outputVueFile)) File.Delete(outputVueFile);
						File.WriteAllText(outputVueFile, s);
					}
					catch (Exception ex)
					{
						errors.Add(clientUi.FileName, ex);
					}
				}
			}


			return errors;
		}

	}

	public record DbObjectStack(string ObjectName, string ObjectType)
	{
		public string ObjectName { get; set; } = ObjectName;
		public string ObjectType { get; set; } = ObjectType;
		public bool HasServerObjects { get; set; }
		public List<string> ClientComponents { get; set; } = [];

		public DateTime LastWriteTime { get; set; }

	}
}
