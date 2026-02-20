namespace Zzz
{
    public static partial class AppEndProxy
    {
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
    }
}
