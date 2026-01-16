
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
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[Zync] N'{Command}', N'{Repo}'");
        }

        public static object? ZyncObjectExists(string ObjectName, string ObjectType)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZyncObjectExists] N'{ObjectName}', N'{ObjectType}'");
        }

        public static object? ZyncParseObject(string SqlScript)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZyncParseObject] N'{SqlScript}'");
        }

        public static object? ZyncSmartTableUpdate(string TableName, string CreateTableScript)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZyncSmartTableUpdate] N'{TableName}', N'{CreateTableScript}'");
        }

        public static object? ZzAlterColumn(string TableName, string ColumnName, string ColumnTypeSize, Boolean AllowNull, string Default)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzAlterColumn] N'{TableName}', N'{ColumnName}', N'{ColumnTypeSize}', {AllowNull}, N'{Default}'");
        }

        public static object? ZzCalculateHID(string TableName, string ParentId, int ChildDigits, string Delimiter)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzCalculateHID] N'{TableName}', N'{ParentId}', {ChildDigits}, N'{Delimiter}'");
        }

        public static object? ZzCalculateHIDDigitsCount(string TableName, string ParentId, string Delimiter)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzCalculateHIDDigitsCount] N'{TableName}', N'{ParentId}', N'{Delimiter}'");
        }

        public static object? ZzCreateColumn(string TableName, string ColumnName, string ColumnTypeSize, Boolean AllowNull)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzCreateColumn] N'{TableName}', N'{ColumnName}', N'{ColumnTypeSize}', {AllowNull}");
        }

        public static object? ZzCreateEmptyProcedure(string ProcedureName)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzCreateEmptyProcedure] N'{ProcedureName}'");
        }

        public static object? ZzCreateEmptyScalarFunction(string ScalarValuedFunctionName)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzCreateEmptyScalarFunction] N'{ScalarValuedFunctionName}'");
        }

        public static object? ZzCreateEmptyTableFunction(string TableValuedFunctionName)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzCreateEmptyTableFunction] N'{TableValuedFunctionName}'");
        }

        public static object? ZzCreateEmptyView(string ViewName)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzCreateEmptyView] N'{ViewName}'");
        }

        public static object? ZzCreateOrAlterFk(string FkName, string BaseTableName, string BaseColumnName, string TargetTableName, string TargetColumnName, Boolean EnforceRelation)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzCreateOrAlterFk] N'{FkName}', N'{BaseTableName}', N'{BaseColumnName}', N'{TargetTableName}', N'{TargetColumnName}', {EnforceRelation}");
        }

        public static object? ZzCreateTableGuid(string TableName, string PkFieldName, Boolean IgnoreIfExist)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzCreateTableGuid] N'{TableName}', N'{PkFieldName}', {IgnoreIfExist}");
        }

        public static object? ZzCreateTableIdentity(string TableName, string PkFieldName, string PkFieldType, int PkIdentityStart, int PkIdentityStep, Boolean IgnoreIfExist)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzCreateTableIdentity] N'{TableName}', N'{PkFieldName}', N'{PkFieldType}', {PkIdentityStart}, {PkIdentityStep}, {IgnoreIfExist}");
        }

        public static object? ZzDropColumn(string TableName, string ColumnName)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzDropColumn] N'{TableName}', N'{ColumnName}'");
        }

        public static object? ZzDropFk(string FkName, string BaseTableName)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzDropFk] N'{FkName}', N'{BaseTableName}'");
        }

        public static object? ZzDropFunction(string FunctionName)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzDropFunction] N'{FunctionName}'");
        }

        public static object? ZzDropProcedure(string ProcedureName)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzDropProcedure] N'{ProcedureName}'");
        }

        public static object? ZzDropTable(string TableName)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzDropTable] N'{TableName}'");
        }

        public static object? ZzDropView(string ViewName)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzDropView] N'{ViewName}'");
        }

        public static object? ZzEnsureIndex(string schema, string table, string indexName, string columns, string include, string where)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzEnsureIndex] N'{schema}', N'{table}', N'{indexName}', N'{columns}', N'{include}', N'{where}'");
        }

        public static object? ZzEnsureSchema(string schema)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzEnsureSchema] N'{schema}'");
        }

        public static object? ZzGetCreateOrAlter(string ObjectName)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzGetCreateOrAlter] N'{ObjectName}'");
        }

        public static object? ZzObjectExist(string ObjectName)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzObjectExist] N'{ObjectName}'");
        }

        public static object? ZzRenameColumn(string TableName, string InitialName, string NewName)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzRenameColumn] N'{TableName}', N'{InitialName}', N'{NewName}'");
        }

        public static object? ZzRenameTable(string TableName, string NewName)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzRenameTable] N'{TableName}', N'{NewName}'");
        }

        public static object? ZzTruncateTable(string TableName)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzTruncateTable] N'{TableName}'");
        }

        public static object? ZzRPad(string s, int length, string pad)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"SELECT [DBO].[ZzRPad](N'{s}', {length}, N'{pad}')");
        }

    }
}
