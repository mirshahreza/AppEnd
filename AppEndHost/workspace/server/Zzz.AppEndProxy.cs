using AngleSharp.Common;
using AngleSharp.Text;
using AppEndCommon;
using AppEndDbIO;
using AppEndDynaCode;
using AppEndServer;
using Microsoft.AspNetCore.Routing.Matching;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Reflection;
using System.Text;
using System.Text.Encodings;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using static System.Net.WebRequestMethods;
using Microsoft.Identity.Client;


namespace Zzz
{
    public static partial class AppEndProxy
    {
        #region Ai
        public static object? Generate(AppEndUser? Actor, string prompt, string model)
        {
			return AiServices.GenerateFromAppSettingsAsync(prompt, model).GetAwaiter().GetResult();
        }
        public static object? GetAiProvidersWithModels(AppEndUser? actor)
        {
            return AiServices.GetAiProvidersWithModels();
        }
        #endregion

        #region Scheduler API

        public static object? SchedulerGetAllTasks(AppEndUser? Actor, object? Enabled = null, string? SearchTerm = null)
        {
            try
            {
                // Parse the Enabled parameter - it might come as JsonElement, bool, or string
                bool? enabled = null;
                
                if (Enabled != null)
                {
                    if (Enabled is JsonElement je)
                    {
                        if (je.ValueKind == System.Text.Json.JsonValueKind.True)
                            enabled = true;
                        else if (je.ValueKind == System.Text.Json.JsonValueKind.False)
                            enabled = false;
                        else if (je.ValueKind == System.Text.Json.JsonValueKind.String)
                        {
                            var str = je.GetString();
                            if (bool.TryParse(str, out var boolValue))
                                enabled = boolValue;
                        }
                    }
                    else if (Enabled is bool b)
                    {
                        enabled = b;
                    }
                    else if (Enabled is string s)
                    {
                        if (bool.TryParse(s, out var boolValue))
                            enabled = boolValue;
                    }
                }

                var manager = GetSchedulerManager();
                return manager.GetAllTasks(enabled, SearchTerm);
            }
            catch (Exception ex)
            {
                return new { error = ex.Message };
            }
        }

        public static object? SchedulerGetTask(AppEndUser? Actor, string TaskId)
        {
            try
            {
                var manager = GetSchedulerManager();
                var task = manager.GetTask(TaskId);
                if (task == null)
                    return new { error = "Task not found" };
                return task;
            }
            catch (Exception ex)
            {
                return new { error = ex.Message };
            }
        }

        public static object? SchedulerCreateTask(AppEndUser? Actor, JsonElement TaskData)
        {
            try
            {
                var task = JsonSerializer.Deserialize<AppEndServer.ScheduledTask>(TaskData.GetRawText());
                if (task == null)
                    return new { error = "Invalid task data" };

                task.CreatedBy = Actor?.UserName;
                var manager = GetSchedulerManager();
                return manager.CreateTask(task);
            }
            catch (Exception ex)
            {
                return new { error = ex.Message };
            }
        }

        public static object? SchedulerUpdateTask(AppEndUser? Actor, JsonElement TaskData)
        {
            try
            {
                var task = JsonSerializer.Deserialize<AppEndServer.ScheduledTask>(TaskData.GetRawText());
                if (task == null)
                    return new { error = "Invalid task data" };

                var manager = GetSchedulerManager();
                return manager.UpdateTask(task);
            }
            catch (Exception ex)
            {
                return new { error = ex.Message };
            }
        }

        public static object? SchedulerDeleteTask(AppEndUser? Actor, string TaskId)
        {
            try
            {
                var manager = GetSchedulerManager();
                return manager.DeleteTask(TaskId);
            }
            catch (Exception ex)
            {
                return new { error = ex.Message };
            }
        }

        public static object? SchedulerStartTask(AppEndUser? Actor, string TaskId)
        {
            try
            {
                var manager = GetSchedulerManager();
                return manager.StartTask(TaskId);
            }
            catch (Exception ex)
            {
                return new { error = ex.Message };
            }
        }

        public static object? SchedulerStopTask(AppEndUser? Actor, string TaskId)
        {
            try
            {
                var manager = GetSchedulerManager();
                return manager.StopTask(TaskId);
            }
            catch (Exception ex)
            {
                return new { error = ex.Message };
            }
        }

        public static object? SchedulerPauseTask(AppEndUser? Actor, string TaskId)
        {
            try
            {
                var manager = GetSchedulerManager();
                return manager.PauseTask(TaskId);
            }
            catch (Exception ex)
            {
                return new { error = ex.Message };
            }
        }

        public static object? SchedulerResumeTask(AppEndUser? Actor, string TaskId)
        {
            try
            {
                var manager = GetSchedulerManager();
                return manager.ResumeTask(TaskId);
            }
            catch (Exception ex)
            {
                return new { error = ex.Message };
            }
        }

        public static object? SchedulerExecuteNow(AppEndUser? Actor, string TaskId)
        {
            try
            {
                var manager = GetSchedulerManager();
                return manager.ExecuteNow(TaskId);
            }
            catch (Exception ex)
            {
                return new { error = ex.Message };
            }
        }

        public static object? SchedulerToggleTask(AppEndUser? Actor, string TaskId, bool Enable)
        {
            try
            {
                var manager = GetSchedulerManager();
                return manager.ToggleTask(TaskId, Enable);
            }
            catch (Exception ex)
            {
                return new { error = ex.Message };
            }
        }

        public static object? SchedulerGetStatistics(AppEndUser? Actor)
        {
            try
            {
                var manager = GetSchedulerManager();
                return manager.GetStatistics();
            }
            catch (Exception ex)
            {
                return new { error = ex.Message };
            }
        }

        public static object? SchedulerGetHistory(AppEndUser? Actor, string? TaskId = null, int MaxEntries = 100)
        {
            try
            {
                var manager = GetSchedulerManager();
                return manager.GetHistory(TaskId, MaxEntries);
            }
            catch (Exception ex)
            {
                return new { error = ex.Message };
            }
        }

        public static object? SchedulerGetAvailableMethods(AppEndUser? Actor)
        {
            try
            {
                return AppEndServer.MethodDiscovery.GetAllMethods();
            }
            catch (Exception ex)
            {
                return new { error = ex.Message };
            }
        }

        public static object? SchedulerReloadTasks(AppEndUser? Actor)
        {
            try
            {
                var manager = GetSchedulerManager();
                return manager.ReloadTasksFromSettings();
            }
            catch (Exception ex)
            {
                return new { error = ex.Message };
            }
        }

        private static AppEndServer.SchedulerManager? _schedulerManagerInstance;
        
        public static void InitializeScheduler(AppEndServer.SchedulerManager manager)
        {
            _schedulerManagerInstance = manager;
            AppEndServer.SchedulerService.SetManager(manager);
        }

        private static AppEndServer.SchedulerManager GetSchedulerManager()
        {
            return _schedulerManagerInstance ?? throw new InvalidOperationException("Scheduler not initialized");
        }

        #endregion

        #region FileServices
        public static object? DownloadFile(string FileName)
        {
            return FileServices.DownloadFile(FileName);
        }
        public static object? UploadFile(string FileName, string FileBody)
        {
            return FileServices.UploadFile(FileName, FileBody);
        }
        public static object? GetFolderContent(string PathToRead)
		{
			return FileServices.GetFolderContent(PathToRead);
		}
        public static object? GetFileContent(string PathToRead)
        {
            return FileServices.GetFileContent(PathToRead);
        }
        public static object? GetZipFileContent(string PathToRead)
        {
            return FileServices.GetZipFileContent(PathToRead);
        }
        public static bool PackItemToZipFile(string ItemToPack, string ZipFile)
        {
			return FileServices.PackItemToZipFile(ItemToPack, ZipFile);
        }
        public static object? SaveFileContent(string PathToWrite, string FileContent)
		{
			return FileServices.SaveFileContent(PathToWrite, FileContent);
		}
		public static object? RemoveTheme(string FolderName)
		{
			return FileServices.RemoveTheme(FolderName);
		}
		public static object? DuplicateTheme(string OrigFolderName, string CopyFolderName)
		{
			return FileServices.DuplicateTheme(OrigFolderName, CopyFolderName);
		}
		public static object? SetThemeProps(string OrigFolderName, string FolderName, string Title, string SubTitle)
		{
			return FileServices.SetThemeProps(OrigFolderName, FolderName, Title, SubTitle);
		}
		public static object? GetSubApps()
		{
			return FileServices.GetSubApps();
		}
		public static object? GetUiComponents(string FolderName)
		{
			return FileServices.GetUiComponents(FolderName);
		}
		public static object? FolderHasContent(string PathToCheck)
		{
			return FileServices.FolderHasContent(PathToCheck);
		}
		public static object? CreateEmptyComponent(string ComponentFullPath)
		{
			return FileServices.CreateEmptyComponent(ComponentFullPath);
		}
		public static object? RenameItem(string ItemPath, string NewItemPath)
		{
			return FileServices.RenameItem(ItemPath, NewItemPath);
		}
		public static object? DuplicateItem(string PathToDuplicate, string PathType)
		{
			return FileServices.DuplicateItem(PathToDuplicate, PathType);
		}
        public static object? DeleteItem(string ItemPath,string PathType)
        {
            return FileServices.DeleteItem(ItemPath, PathType);
        }
        public static object? DeleteFileItem(string FilePath)
        {
            return FileServices.DeleteFileItem(FilePath);
        }
        public static object? DeleteFolderItem(string PathToDelete, bool Recursive)
		{
			return FileServices.DeleteFolderItem(PathToDelete, Recursive);
		}
        public static object? CreateNewFolder(string PathToCreate)
        {
            return FileServices.CreateNewFolder(PathToCreate);
        }
        public static object? CreateNewFile(string PathToCreate)
        {
            return FileServices.CreateNewFile(PathToCreate);
        }
        public static object? GetStoredApiCalls()
        {
            return FileServices.GetStoredApiCalls();
        }
        public static object? GetStoredSqlQueries()
        {
            return FileServices.GetStoredSqlQueries();
        }
        public static object? ExtractTranslationKeys(string FolderName)
        {
            return FileServices.ExtractTranslationKeys(FolderName);
        }
        public static object? ReadPackages()
        {
            return FileServices.ReadPackages();
        }
        public static object? SavePackageInfo(string PackageName, string PackageNewName, JsonElement PackageInfo)
        {
            return FileServices.SavePackageInfo(PackageName, PackageNewName, PackageInfo);
        }
        public static object? DownloadPackage(string PackageName)
        {
            return FileServices.DownloadPackage(PackageName);
        }
        public static object? UploadPackage(string PackageName, string PackageBody)
        {
            return FileServices.UploadPackage(PackageName, PackageBody);
        }
        public static object? RemovePackage(string PackageName)
        {
            return FileServices.RemovePackage(PackageName);
        }
        public static object? InstallPackage(AppEndUser? Actor, string PackageName, string DbConfName)
        {
            return FileServices.InstallPackage(Actor, PackageName, DbConfName);
        }
        public static object? UnInstallPackage(string PackageName)
        {
            return FileServices.UnInstallPackage(PackageName);
        }
        public static object? RepackPackage(string PackageName)
        {
            return FileServices.RepackPackage(PackageName);
        }

        #endregion

        #region DbServices
        public static object? GetCreateOrAlterTable(string DbConfName, string ObjectName)
		{
			return DbServices.GetCreateOrAlterObject(DbConfName, ObjectName);
		}
		public static object? CreateEmptyDbView(string DbConfName, string ViewName)
		{
			return DbServices.CreateEmptyDbView(DbConfName, ViewName);
		}
		public static object? CreateEmptyDbProcedure(string DbConfName, string ProcedureName)
		{
			return DbServices.CreateEmptyDbProcedure(DbConfName, ProcedureName);
		}
		public static object? CreateEmptyDbScalarFunction(string DbConfName, string ScalarFunctionName)
		{
			return DbServices.CreateEmptyDbScalarFunction(DbConfName, ScalarFunctionName);
		}
		public static object? CreateEmptyDbTableFunction(string DbConfName, string TableFunctionName)
		{
			return DbServices.CreateEmptyDbTableFunction(DbConfName, TableFunctionName);
		}
		public static object? AlterObjectScript(string DbConfName, string ObjectScript)
		{
			return DbServices.AlterObjectScript(DbConfName, ObjectScript);
		}
		public static object? RenameObject(string DbConfName, string ObjectName_Old, string ObjectName_New, string ObjectType)
		{
			return DbServices.RenameObject(DbConfName, ObjectName_Old, ObjectName_New, ObjectType);
		}
		public static object? DeleteObject(string DbConfName, string ObjectName, string ObjectType)
		{
			return DbServices.DropObject(DbConfName, ObjectName, ObjectType);
		}
		public static object? DropFk(string DbConfName, string ObjectName, string FkName)
		{
			return DbServices.DropFk(DbConfName, ObjectName, FkName);
		}
		public static object? TruncateTable(string DbConfName, string TableName)
		{
			return DbServices.TruncateTable(DbConfName, TableName);
		}
		public static object? SaveTableSchema(string DbConfName, JsonElement TableDef)
		{
			return DbServices.SaveTableSchema(DbConfName, TableDef);
		}
		public static object? ReadObjectSchema(string DbConfName, string ObjectName)
		{
			return DbServices.ReadObjectSchema(DbConfName, ObjectName);
		}
        public static object? GetDbTables(string DbConfName)
        {
            return DbServices.GetDbTables(DbConfName);
        }
        public static object? GetAllDbObjectsWithDependencies(string DbConfName)
        {
            return DbServices.GetAllDbObjectsWithDependencies(DbConfName);
        }
        public static object? GetDbObjectsForDiagram(string DbConfName, string ObjectTypes)
        {
            return DbServices.GetDbObjectsForDiagram(DbConfName, ObjectTypes);
        }
        public static object? GetObjectDependencies(string DbConfName, string ObjectName)
        {
            return DbServices.GetObjectDependencies(DbConfName, ObjectName);
        }
        public static object? GetSchemaForEnrich(string DbConfName)
        {
            return DbServices.GetSchemaForEnrich(DbConfName);
        }
        public static object? GetEnrichedStructureIds(string ConnectionName)
        {
            return DbServices.GetEnrichedStructureIds(ConnectionName);
        }
        /// <summary>Fetches schema details (with table + column context) for the given StructureIds.</summary>
        public static object? GetSchemaDetailsForStructureIds(string ConnectionName, System.Text.Json.JsonElement StructureIds)
        {
            var list = new List<string>();
            if (StructureIds.ValueKind == System.Text.Json.JsonValueKind.Array)
                foreach (var e in StructureIds.EnumerateArray())
                {
                    var s = e.GetString();
                    if (!string.IsNullOrEmpty(s)) list.Add(s);
                }
            return DbServices.GetSchemaDetailsForStructureIds(ConnectionName, list);
        }
        /// <summary>Default LLM model for enrichment (LocalOllama or Gemini Vertex AI).</summary>
        public static object? GetDefaultEnrichmentModel(AppEndUser? Actor)
        {
            return AiEnrichmentServices.GetDefaultEnrichmentModel();
        }
        /// <summary>First model for the given provider name (e.g. LocalOllama, Gemini Vertex AI).</summary>
        public static object? GetModelForEnrichmentProvider(AppEndUser? Actor, string ProviderName)
        {
            return AiEnrichmentServices.GetModelForProvider(ProviderName);
        }
        /// <summary>Runs AI enrichment for the given StructureIds: LLM generates bilingual metadata and upserts into BaseZetadata.</summary>
        public static object? RunAiEnrichment(AppEndUser? Actor, string ConnectionName, System.Text.Json.JsonElement StructureIds, string? Model = null)
        {
            var list = new List<string>();
            if (StructureIds.ValueKind == System.Text.Json.JsonValueKind.Array)
                foreach (var e in StructureIds.EnumerateArray())
                {
                    var s = e.GetString();
                    if (!string.IsNullOrEmpty(s)) list.Add(s);
                }
            return AiEnrichmentServices.EnrichStructuresAsync(ConnectionName, list, Model).GetAwaiter().GetResult();
        }
        public static object? GetBaseZetadataByConnection(string ConnectionName)
        {
            return DbServices.GetBaseZetadataByConnection(ConnectionName);
        }
        public static object? CreateBaseZetadata(string StructureId, string ConnectionName, string ObjectName, string? ObjectType, string? HumanTitleEn, string? HumanTitleNative, string? NoteEn, string? NoteNative, string? KeywordsEn, string? KeywordsNative)
        {
            return DbServices.CreateBaseZetadata(StructureId, ConnectionName, ObjectName, ObjectType, HumanTitleEn, HumanTitleNative, NoteEn, NoteNative, KeywordsEn, KeywordsNative);
        }
        public static object? UpdateBaseZetadata(string StructureId, string? HumanTitleEn, string? HumanTitleNative, string? NoteEn, string? NoteNative, string? KeywordsEn, string? KeywordsNative)
        {
            return DbServices.UpdateBaseZetadata(StructureId, HumanTitleEn, HumanTitleNative, NoteEn, NoteNative, KeywordsEn, KeywordsNative);
        }
        public static object? DeleteBaseZetadata(string StructureId)
        {
            return DbServices.DeleteBaseZetadata(StructureId);
        }
        #endregion

        #region DbDialogServices
        public static object? ReadDbObjectBody(string DbConfName, string ObjectName)
        {
            return DbDialogServices.ReadDbObjectBody(DbConfName, ObjectName);
        }
        public static object? SaveDbObjectBody(string DbConfName, string ObjectName, string ObjectBody)
		{
            return DbDialogServices.SaveDbObjectBody(DbConfName, ObjectName, ObjectBody);
        }
        public static object? GetDbObjects(string DbConfName, string ObjectType, string Filter)
		{
			return DbDialogServices.GetDbObjects(DbConfName, ObjectType, Filter);
		}
		public static object? GetDbObjectsStack(string DbConfName, string ObjectType, string Filter)
		{
			return DbDialogServices.GetDbObjectsStack(DbConfName, ObjectType, Filter);
		}
		public static object? CreateLogicalFk(string DbConfName, string FkName, string BaseTable, string BaseColumn, string TargetTable, string TargetColumn)
		{
			return DbDialogServices.CreateLogicalFk(DbConfName, FkName, BaseTable, BaseColumn, TargetTable, TargetColumn);
		}
		public static object? RemoveLogicalFk(string DbConfName, string BaseTable, string BaseColumn)
		{
			return DbDialogServices.RemoveLogicalFk(DbConfName, BaseTable, BaseColumn);
		}
		public static object? CreateNewNotMappedMethod(string DbConfName, string ObjectName, string MethodName)
		{
			return DbDialogServices.CreateNewNotMappedMethod(DbConfName, ObjectName, MethodName);
		}
		public static object? CreateNewMethodQuery(string DbConfName, string ObjectName, string MethodType, string MethodName)
		{
			return DbDialogServices.CreateNewMethodQuery(DbConfName, ObjectName, MethodType, MethodName);
		}
		public static object? CreateNewUpdateByKey(string DbConfName, string ObjectName, string ReadByKeyApiName, List<string> ColumnsToUpdate, string PartialUpdateApiName, string ByColumnName, string OnColumnName, string HistoryTableName)
		{
			return DbDialogServices.CreateNewUpdateByKey(DbConfName, ObjectName, ReadByKeyApiName, ColumnsToUpdate, PartialUpdateApiName, ByColumnName, OnColumnName, HistoryTableName);
		}
		public static object? DuplicateMethodQuery(string DbConfName, string ObjectName, string MethodName, string MethodCopyName)
		{
			return DbDialogServices.DuplicateMethodQuery(DbConfName, ObjectName, MethodName, MethodCopyName);
		}
		public static object? RemoveMethodQuery(string DbConfName, string ObjectName, string MethodName)
		{
			return DbDialogServices.RemoveMethodQuery(DbConfName, ObjectName, MethodName);
		}
		public static object? RemoveNotMappedMethod(string DbConfName, string ObjectName, string MethodName)
		{
			return DbDialogServices.RemoveNotMappedMethod(DbConfName, ObjectName, MethodName);
		}
		public static object? GetDbObjectNotMappedMethods(string DbConfName, string ObjectName)
		{
			return DbDialogServices.GetDbObjectNotMappedMethods(DbConfName, ObjectName);
		}
		public static object? ReCreateMethodJson(string DbConfName, string ObjectName, string ObjectType, string MethodName)
		{
			return DbDialogServices.ReCreateMethodJson(DbConfName, ObjectName, ObjectType, MethodName);
		}
		public static object? RemoveServerObjects(string DbConfName, string ObjectName, string ObjectType)
		{
			return DbDialogServices.RemoveServerObjects(DbConfName, ObjectName, ObjectType);
		}
		public static object? CreateServerObjects(string DbConfName, string ObjectName, string ObjectType)
		{
			return DbDialogServices.CreateServerObjects(DbConfName, ObjectName, ObjectType);
		}
		public static object? SyncDbDialog(string DbConfName, string ObjectName)
		{
			return DbDialogServices.SyncDbDialog(DbConfName, ObjectName);
		}
		public static object? BuildUiForDbObject(string DbConfName, string ObjectName)
		{
			return DbDialogServices.BuildUiForDbObject(DbConfName, ObjectName);
		}
		public static object? SynchDbDirectMethods(string DbConfName)
		{
			return DbDialogServices.SynchDbDirectMethods(DbConfName);
		}
		public static object? BuildUiOne(string DbConfName, string ObjectName, string ComponentName)
		{
			return DbDialogServices.BuildUiOne(DbConfName, ObjectName, ComponentName);
		}
		public static object? GenerateHintsForDbObject(string DbConfName, string ObjectName)
		{
			return DbDialogServices.GenerateHintsForDbObject(DbConfName, ObjectName);
		}
		#endregion

		#region DbServices
		public static object? GetDataSources()
		{
			return DbServices.GetDataSources();
		}
		public static object? GetDataSourcesWithCnn()
		{
			return DbServices.GetDataSourcesWithCnn();
		}
		public static object? AddOrAlterDbServer(JsonElement DataSourceInfo)
		{
			return DbServices.AddOrAlterDbServer(DataSourceInfo);
		}
		public static object? RemoveDbServer(string DbServerName)
		{
			return DbServices.RemoveDbServer(DbServerName);
		}
		public static object? TestDbConnection(JsonElement ServerInfo)
		{
			return DbServices.TestDbConnection(ServerInfo);
		}
		#endregion

        public static object? Exec(string DbConfName,string Query)
        {
            return DbServices.Exec(DbConfName, Query);
        }



        #region CacheServices
        public static object? RemoveAllCacheItems()
		{
			CacheServices.RemoveAllCacheItems();
			return true;
		}
		public static object? GetCacheItems(string LikeStr)
		{
			return CacheServices.GetCacheItems(LikeStr);
		}
		public static object? GetCacheItem(string Key)
		{
			return CacheServices.GetCacheItem(Key);
		}
		public static object? RemoveCacheItem(string Key)
		{
			return CacheServices.RemoveCacheItem(Key);
		}
		#endregion

		#region DynaCodeServices
		public static object? GetMethodSettings(string NamespaceName, string ClassName, string MethodName)
		{
			return DynaCodeServices.GetMethodSettings(NamespaceName, ClassName, MethodName);
		}
		public static object? WriteMethodSettings(string NamespaceName, string ClassName, string MethodName, JsonElement NewMethodSettings)
		{
			return DynaCodeServices.WriteMethodSettings(NamespaceName, ClassName, MethodName, NewMethodSettings);
		}
		public static object? GetDynaClasses()
		{
			return DynaCodeServices.GetDynaClasses();
		}
		public static object? CreateController(string NamespaceName, string ClassName, bool AddSampleMthod)
		{
			DynaCodeServices.CreateController(NamespaceName, ClassName, AddSampleMthod);
			return true;
		}
		public static object? CreateMethod(string NamespaceName, string ClassName, string MethodName)
		{
			DynaCodeServices.CreateMethod(NamespaceName, ClassName, MethodName);
			return true;
		}
		public static object? RemoveMethod(string NamespaceName, string ClassName, string MethodName)
		{
			DynaCodeServices.RemoveMethod(NamespaceName, ClassName, MethodName);
			return true;
		}
		public static object? RemoveClass(string NamespaceName, string ClassName)
		{
			DynaCodeServices.RemoveClass(NamespaceName, ClassName);
			return true;
		}
        #endregion

        #region ServerActions
        public static object? RebuildProject()
        {
            return BuildServices.RebuildProject();
        }
        public static object? RestartApp()
        {
            
            return true;
        }
        #endregion

        #region HostingUtils
        public static object? GetAppEndSummary()
		{
			return HostingUtils.GetAppEndSummary();
		}
        public static object? TestMe(string s)
        {
            LogMan.LogConsole($"TestMe called with : {s}");
            return "TestMe was ok :)";
        }
        public static object? PingMe()
        {
            LogMan.LogConsole("PingMe called");
            return "I am at your service :)";
        }

		public static object? LongRunningDemo(int Seconds, CancellationToken ct)
		{
			int total = Seconds * 10;
			for (int i = 0; i < total; i++)
			{
				ct.ThrowIfCancellationRequested();
				Thread.Sleep(100);
			}
			return new { Message = "LongRunningDemo completed successfully", Duration = Seconds, CompletedAt = DateTime.UtcNow };
		}

		#endregion

		#region Settings
		/// <summary>
		/// Ensures all DbServers from appsettings have a row in BaseDbConnections (for Enrich DB page).
		/// Call before loading Enrich DB list so new connections get an Id without app restart.
		/// </summary>
		public static object? EnsureDbConnectionsFromAppSettings(AppEndUser? Actor)
		{
			try
			{
				AppEndServer.DbConnectionsBootstrap.EnsureFromAppSettings();
				return true;
			}
			catch (Exception ex)
			{
				LogMan.LogWarning($"EnsureDbConnectionsFromAppSettings: {ex.Message}");
				return false;
			}
		}

		public static object? GetAppEndSettings(AppEndUser? Actor)
        {
            try
            {
                // Read directly from AppSettings.AppSettings which is the root JsonNode
                var appEndNode = AppEndSettings.AppSettings[AppEndSettings.ConfigSectionName];
                if (appEndNode == null)
                {
                    return new Dictionary<string, object>();
                }

                // Convert JsonNode to a clean Dictionary structure with proper types
                return ParseJsonNode(appEndNode);
            }
            catch (Exception ex)
            {
                return new Dictionary<string, object> { { "error", ex.Message }, { "stack", ex.StackTrace ?? "" } };
            }
        }

        private static object? ParseJsonNode(JsonNode? node)
        {
            if (node == null) return null;

            if (node is JsonValue jval)
            {
                // Extract primitive types properly
                if (jval.TryGetValue<string>(out var s)) return s;
                if (jval.TryGetValue<int>(out var i)) return i;
                if (jval.TryGetValue<long>(out var l)) return l;
                if (jval.TryGetValue<bool>(out var b)) return b;
                if (jval.TryGetValue<double>(out var d)) return d;
                return jval.ToString();
            }

            if (node is JsonArray jarr)
            {
                var list = new List<object?>();
                foreach (var item in jarr)
                {
                    list.Add(ParseJsonNode(item));
                }
                return list;
            }

            if (node is JsonObject jobj)
            {
                var dict = new Dictionary<string, object?>();
                foreach (var prop in jobj)
                {
                    dict[prop.Key] = ParseJsonNode(prop.Value);
                }
                return dict;
            }

            return node.ToString();
        }

        public static object? SaveAppEndSettings(AppEndUser? Actor, JsonElement AppEnd)
        {
            try
            {
                // Overwrite only the AppEnd section and persist
                var node = JsonNode.Parse(AppEnd.GetRawText());
                if (node is null) return false;
                AppEndSettings.AppSettings[AppEndSettings.ConfigSectionName] = node;
                AppEndSettings.Save();
                // refresh in-memory cache
                AppEndSettings.RefereshSettings();
                
                // Reload scheduler tasks if settings were saved successfully
                try
                {
                    var manager = GetSchedulerManager();
                    manager.ReloadTasksFromSettings();
                }
                catch (Exception ex)
                {
                    // Log but don't fail the save operation
                    LogMan.LogWarning($"Settings saved but failed to reload scheduler tasks: {ex.Message}");
                }
                
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region ActivityLog
        public static object? FlushActivityLogs(AppEndUser? Actor)
        {
            try
            {
                LogMan.Flush();
                return new { Success = true, Message = "Activity logs flushed successfully" };
            }
            catch (Exception ex)
            {
                return new { Success = false, Message = ex.Message };
            }
        }
        #endregion
    }
}

