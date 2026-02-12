
using System;
using System.Text.Json;
using AppEndCommon;
using AppEndDynaCode;
using AppEndDbIO;
using AppEndServer;

namespace DefaultRepo
{    
	public static class DbDirect
    {
		private static string DbConfName = "DefaultRepo";


        public static object? Zync(string Command, string Repo)
        {
			var dbParams = new System.Collections.Generic.List<System.Data.Common.DbParameter>();
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@Command", System.Data.SqlDbType.NVarChar) { Value = Command });
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@Repo", System.Data.SqlDbType.NVarChar) { Value = Repo });
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar(
				"EXEC [DBO].[Zync] @Command, @Repo",
				dbParams);
        }

        public static object? ZyncObjectExists(string ObjectName, string ObjectType)
        {
			var dbParams = new System.Collections.Generic.List<System.Data.Common.DbParameter>();
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@ObjectName", System.Data.SqlDbType.NVarChar) { Value = ObjectName });
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@ObjectType", System.Data.SqlDbType.NVarChar) { Value = ObjectType });
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar(
				"EXEC [DBO].[ZyncObjectExists] @ObjectName, @ObjectType",
				dbParams);
        }

        public static object? ZyncParseObject(string SqlScript)
        {
			var dbParams = new System.Collections.Generic.List<System.Data.Common.DbParameter>();
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@SqlScript", System.Data.SqlDbType.NVarChar) { Value = SqlScript });
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar(
				"EXEC [DBO].[ZyncParseObject] @SqlScript",
				dbParams);
        }

        public static object? ZyncSmartTableUpdate(string TableName, string CreateTableScript)
        {
			var dbParams = new System.Collections.Generic.List<System.Data.Common.DbParameter>();
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@TableName", System.Data.SqlDbType.NVarChar) { Value = TableName });
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@CreateTableScript", System.Data.SqlDbType.NVarChar) { Value = CreateTableScript });
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar(
				"EXEC [DBO].[ZyncSmartTableUpdate] @TableName, @CreateTableScript",
				dbParams);
        }

        public static object? ZzAlterColumn(string TableName, string ColumnName, string ColumnTypeSize, Boolean AllowNull, string Default)
        {
			var dbParams = new System.Collections.Generic.List<System.Data.Common.DbParameter>();
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@TableName", System.Data.SqlDbType.NVarChar) { Value = TableName });
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@ColumnName", System.Data.SqlDbType.NVarChar) { Value = ColumnName });
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@ColumnTypeSize", System.Data.SqlDbType.NVarChar) { Value = ColumnTypeSize });
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@AllowNull", System.Data.SqlDbType.Bit) { Value = AllowNull });
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@Default", System.Data.SqlDbType.NVarChar) { Value = Default });
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar(
				"EXEC [DBO].[ZzAlterColumn] @TableName, @ColumnName, @ColumnTypeSize, @AllowNull, @Default",
				dbParams);
        }

        public static object? ZzCalculateHID(string TableName, string ParentId, int ChildDigits, string Delimiter)
        {
			var dbParams = new System.Collections.Generic.List<System.Data.Common.DbParameter>();
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@TableName", System.Data.SqlDbType.NVarChar) { Value = TableName });
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@ParentId", System.Data.SqlDbType.NVarChar) { Value = ParentId });
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@ChildDigits", System.Data.SqlDbType.Int) { Value = ChildDigits });
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@Delimiter", System.Data.SqlDbType.NVarChar) { Value = Delimiter });
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar(
				"EXEC [DBO].[ZzCalculateHID] @TableName, @ParentId, @ChildDigits, @Delimiter",
				dbParams);
        }

        public static object? ZzCalculateHIDDigitsCount(string TableName, string ParentId, string Delimiter)
        {
			var dbParams = new System.Collections.Generic.List<System.Data.Common.DbParameter>();
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@TableName", System.Data.SqlDbType.NVarChar) { Value = TableName });
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@ParentId", System.Data.SqlDbType.NVarChar) { Value = ParentId });
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@Delimiter", System.Data.SqlDbType.NVarChar) { Value = Delimiter });
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar(
				"EXEC [DBO].[ZzCalculateHIDDigitsCount] @TableName, @ParentId, @Delimiter",
				dbParams);
        }

        public static object? ZzCreateColumn(string TableName, string ColumnName, string ColumnTypeSize, Boolean AllowNull)
        {
			var dbParams = new System.Collections.Generic.List<System.Data.Common.DbParameter>();
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@TableName", System.Data.SqlDbType.NVarChar) { Value = TableName });
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@ColumnName", System.Data.SqlDbType.NVarChar) { Value = ColumnName });
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@ColumnTypeSize", System.Data.SqlDbType.NVarChar) { Value = ColumnTypeSize });
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@AllowNull", System.Data.SqlDbType.Bit) { Value = AllowNull });
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar(
				"EXEC [DBO].[ZzCreateColumn] @TableName, @ColumnName, @ColumnTypeSize, @AllowNull",
				dbParams);
        }

        public static object? ZzCreateEmptyProcedure(string ProcedureName)
        {
			var dbParams = new System.Collections.Generic.List<System.Data.Common.DbParameter>();
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@ProcedureName", System.Data.SqlDbType.NVarChar) { Value = ProcedureName });
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar(
				"EXEC [DBO].[ZzCreateEmptyProcedure] @ProcedureName",
				dbParams);
        }

        public static object? ZzCreateEmptyScalarFunction(string ScalarValuedFunctionName)
        {
			var dbParams = new System.Collections.Generic.List<System.Data.Common.DbParameter>();
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@ScalarValuedFunctionName", System.Data.SqlDbType.NVarChar) { Value = ScalarValuedFunctionName });
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar(
				"EXEC [DBO].[ZzCreateEmptyScalarFunction] @ScalarValuedFunctionName",
				dbParams);
        }

        public static object? ZzCreateEmptyTableFunction(string TableValuedFunctionName)
        {
			var dbParams = new System.Collections.Generic.List<System.Data.Common.DbParameter>();
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@TableValuedFunctionName", System.Data.SqlDbType.NVarChar) { Value = TableValuedFunctionName });
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar(
				"EXEC [DBO].[ZzCreateEmptyTableFunction] @TableValuedFunctionName",
				dbParams);
        }

        public static object? ZzCreateEmptyView(string ViewName)
        {
			var dbParams = new System.Collections.Generic.List<System.Data.Common.DbParameter>();
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@ViewName", System.Data.SqlDbType.NVarChar) { Value = ViewName });
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar(
				"EXEC [DBO].[ZzCreateEmptyView] @ViewName",
				dbParams);
        }

        public static object? ZzCreateOrAlterFk(string FkName, string BaseTableName, string BaseColumnName, string TargetTableName, string TargetColumnName, Boolean EnforceRelation)
        {
			var dbParams = new System.Collections.Generic.List<System.Data.Common.DbParameter>();
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@FkName", System.Data.SqlDbType.NVarChar) { Value = FkName });
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@BaseTableName", System.Data.SqlDbType.NVarChar) { Value = BaseTableName });
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@BaseColumnName", System.Data.SqlDbType.NVarChar) { Value = BaseColumnName });
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@TargetTableName", System.Data.SqlDbType.NVarChar) { Value = TargetTableName });
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@TargetColumnName", System.Data.SqlDbType.NVarChar) { Value = TargetColumnName });
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@EnforceRelation", System.Data.SqlDbType.Bit) { Value = EnforceRelation });
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar(
				"EXEC [DBO].[ZzCreateOrAlterFk] @FkName, @BaseTableName, @BaseColumnName, @TargetTableName, @TargetColumnName, @EnforceRelation",
				dbParams);
        }

        public static object? ZzCreateTableGuid(string TableName, string PkFieldName, Boolean IgnoreIfExist)
        {
			var dbParams = new System.Collections.Generic.List<System.Data.Common.DbParameter>();
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@TableName", System.Data.SqlDbType.NVarChar) { Value = TableName });
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@PkFieldName", System.Data.SqlDbType.NVarChar) { Value = PkFieldName });
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@IgnoreIfExist", System.Data.SqlDbType.Bit) { Value = IgnoreIfExist });
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar(
				"EXEC [DBO].[ZzCreateTableGuid] @TableName, @PkFieldName, @IgnoreIfExist",
				dbParams);
        }

        public static object? ZzCreateTableIdentity(string TableName, string PkFieldName, string PkFieldType, int PkIdentityStart, int PkIdentityStep, Boolean IgnoreIfExist)
        {
			var dbParams = new System.Collections.Generic.List<System.Data.Common.DbParameter>();
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@TableName", System.Data.SqlDbType.NVarChar) { Value = TableName });
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@PkFieldName", System.Data.SqlDbType.NVarChar) { Value = PkFieldName });
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@PkFieldType", System.Data.SqlDbType.NVarChar) { Value = PkFieldType });
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@PkIdentityStart", System.Data.SqlDbType.Int) { Value = PkIdentityStart });
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@PkIdentityStep", System.Data.SqlDbType.Int) { Value = PkIdentityStep });
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@IgnoreIfExist", System.Data.SqlDbType.Bit) { Value = IgnoreIfExist });
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar(
				"EXEC [DBO].[ZzCreateTableIdentity] @TableName, @PkFieldName, @PkFieldType, @PkIdentityStart, @PkIdentityStep, @IgnoreIfExist",
				dbParams);
        }

        public static object? ZzDropColumn(string TableName, string ColumnName)
        {
			var dbParams = new System.Collections.Generic.List<System.Data.Common.DbParameter>();
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@TableName", System.Data.SqlDbType.NVarChar) { Value = TableName });
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@ColumnName", System.Data.SqlDbType.NVarChar) { Value = ColumnName });
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar(
				"EXEC [DBO].[ZzDropColumn] @TableName, @ColumnName",
				dbParams);
        }

        public static object? ZzDropFk(string FkName, string BaseTableName)
        {
			var dbParams = new System.Collections.Generic.List<System.Data.Common.DbParameter>();
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@FkName", System.Data.SqlDbType.NVarChar) { Value = FkName });
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@BaseTableName", System.Data.SqlDbType.NVarChar) { Value = BaseTableName });
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar(
				"EXEC [DBO].[ZzDropFk] @FkName, @BaseTableName",
				dbParams);
        }

        public static object? ZzDropFunction(string FunctionName)
        {
			var dbParams = new System.Collections.Generic.List<System.Data.Common.DbParameter>();
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@FunctionName", System.Data.SqlDbType.NVarChar) { Value = FunctionName });
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar(
				"EXEC [DBO].[ZzDropFunction] @FunctionName",
				dbParams);
        }

        public static object? ZzDropProcedure(string ProcedureName)
        {
			var dbParams = new System.Collections.Generic.List<System.Data.Common.DbParameter>();
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@ProcedureName", System.Data.SqlDbType.NVarChar) { Value = ProcedureName });
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar(
				"EXEC [DBO].[ZzDropProcedure] @ProcedureName",
				dbParams);
        }

        public static object? ZzDropTable(string TableName)
        {
			var dbParams = new System.Collections.Generic.List<System.Data.Common.DbParameter>();
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@TableName", System.Data.SqlDbType.NVarChar) { Value = TableName });
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar(
				"EXEC [DBO].[ZzDropTable] @TableName",
				dbParams);
        }

        public static object? ZzDropView(string ViewName)
        {
			var dbParams = new System.Collections.Generic.List<System.Data.Common.DbParameter>();
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@ViewName", System.Data.SqlDbType.NVarChar) { Value = ViewName });
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar(
				"EXEC [DBO].[ZzDropView] @ViewName",
				dbParams);
        }

        public static object? ZzEnsureIndex(string schema, string table, string indexName, string columns, string include, string where)
        {
			var dbParams = new System.Collections.Generic.List<System.Data.Common.DbParameter>();
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@schema", System.Data.SqlDbType.NVarChar) { Value = schema });
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@table", System.Data.SqlDbType.NVarChar) { Value = table });
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@indexName", System.Data.SqlDbType.NVarChar) { Value = indexName });
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@columns", System.Data.SqlDbType.NVarChar) { Value = columns });
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@include", System.Data.SqlDbType.NVarChar) { Value = include });
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@where", System.Data.SqlDbType.NVarChar) { Value = where });
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar(
				"EXEC [DBO].[ZzEnsureIndex] @schema, @table, @indexName, @columns, @include, @where",
				dbParams);
        }

        public static object? ZzEnsureSchema(string schema)
        {
			var dbParams = new System.Collections.Generic.List<System.Data.Common.DbParameter>();
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@schema", System.Data.SqlDbType.NVarChar) { Value = schema });
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar(
				"EXEC [DBO].[ZzEnsureSchema] @schema",
				dbParams);
        }

        public static object? ZzGetCreateOrAlter(string ObjectName)
        {
			var dbParams = new System.Collections.Generic.List<System.Data.Common.DbParameter>();
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@ObjectName", System.Data.SqlDbType.NVarChar) { Value = ObjectName });
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar(
				"EXEC [DBO].[ZzGetCreateOrAlter] @ObjectName",
				dbParams);
        }

        public static object? ZzObjectExist(string ObjectName)
        {
			var dbParams = new System.Collections.Generic.List<System.Data.Common.DbParameter>();
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@ObjectName", System.Data.SqlDbType.NVarChar) { Value = ObjectName });
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar(
				"EXEC [DBO].[ZzObjectExist] @ObjectName",
				dbParams);
        }

        public static object? ZzRenameColumn(string TableName, string InitialName, string NewName)
        {
			var dbParams = new System.Collections.Generic.List<System.Data.Common.DbParameter>();
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@TableName", System.Data.SqlDbType.NVarChar) { Value = TableName });
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@InitialName", System.Data.SqlDbType.NVarChar) { Value = InitialName });
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@NewName", System.Data.SqlDbType.NVarChar) { Value = NewName });
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar(
				"EXEC [DBO].[ZzRenameColumn] @TableName, @InitialName, @NewName",
				dbParams);
        }

        public static object? ZzRenameTable(string TableName, string NewName)
        {
			var dbParams = new System.Collections.Generic.List<System.Data.Common.DbParameter>();
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@TableName", System.Data.SqlDbType.NVarChar) { Value = TableName });
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@NewName", System.Data.SqlDbType.NVarChar) { Value = NewName });
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar(
				"EXEC [DBO].[ZzRenameTable] @TableName, @NewName",
				dbParams);
        }

        public static object? ZzTruncateTable(string TableName)
        {
			var dbParams = new System.Collections.Generic.List<System.Data.Common.DbParameter>();
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@TableName", System.Data.SqlDbType.NVarChar) { Value = TableName });
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar(
				"EXEC [DBO].[ZzTruncateTable] @TableName",
				dbParams);
        }

        public static object? ZzRPad(string s, int length, string pad)
        {
			var dbParams = new System.Collections.Generic.List<System.Data.Common.DbParameter>();
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@s", System.Data.SqlDbType.NVarChar) { Value = s });
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@length", System.Data.SqlDbType.Int) { Value = length });
			dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@pad", System.Data.SqlDbType.NVarChar) { Value = pad });
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar(
				"SELECT [DBO].[ZzRPad](@s, @length, @pad)",
				dbParams);
        }

    }
}
