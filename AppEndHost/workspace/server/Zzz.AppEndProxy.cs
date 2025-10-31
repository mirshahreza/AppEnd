using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Encodings;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using AppEndCommon;
using AppEndDynaCode;
using AppEndDbIO;
using AppEndServer;
using Microsoft.AspNetCore.Routing.Matching;
using System.Data;
using Newtonsoft.Json.Linq;
using System.Collections;
using AngleSharp.Text;
using Microsoft.Extensions.Caching.Memory;
using AngleSharp.Common;
using System.Reflection;
using static System.Net.WebRequestMethods;



namespace Zzz
{
    public static partial class AppEndProxy
    {
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
		public static object? GetThemes()
		{
			return FileServices.GetThemes();
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
		#endregion

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

		#region BuildServices
		public static object? RebuildProject()
		{
			return BuildServices.RebuildProject();
		}
		#endregion

		#region HostingUtils
		public static object? GetAppEndSummary()
		{
			return HostingUtils.GetAppEndSummary();
		}
		public static object? PingMe()
		{
			return "I am at your service :)";
		}
		#endregion
	}
}

