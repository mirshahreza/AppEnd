namespace Zzz
{
    public static partial class AppEndProxy
    {
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

		public static object? Exec(string DbConfName, string Query)
		{
			return DbServices.Exec(DbConfName, Query);
		}

		#endregion
	}
}
