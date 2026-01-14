
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


        public static object? ZyncObjectExists(string ObjectName, string ObjectType)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZyncObjectExists] '{ObjectName}', '{ObjectType}'");
        }

        public static object? ZyncSmartTableUpdate(string TableName, string CreateTableScript)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZyncSmartTableUpdate] '{TableName}', '{CreateTableScript}'");
        }

        public static object? ZyncParseObject(string SqlScript)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZyncParseObject] '{SqlScript}'");
        }

        public static object? Zync(string Command, string Repo)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[Zync] '{Command}', '{Repo}'");
        }

        public static object? ZzAlterColumn(string TableName, string ColumnName, string ColumnTypeSize, string AllowNull, string Default)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzAlterColumn] '{TableName}', '{ColumnName}', '{ColumnTypeSize}', '{AllowNull}', '{Default}'");
        }

        public static object? ZzCreateColumn(string TableName, string ColumnName, string ColumnTypeSize, string AllowNull)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzCreateColumn] '{TableName}', '{ColumnName}', '{ColumnTypeSize}', '{AllowNull}'");
        }

        public static object? ZzCreateEmptyProcedure(string ProcedureName)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzCreateEmptyProcedure] '{ProcedureName}'");
        }

        public static object? ZzCreateEmptyScalarFunction(string ScalarValuedFunctionName)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzCreateEmptyScalarFunction] '{ScalarValuedFunctionName}'");
        }

        public static object? ZzCreateEmptyTableFunction(string TableValuedFunctionName)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzCreateEmptyTableFunction] '{TableValuedFunctionName}'");
        }

        public static object? ZzCreateEmptyView(string ViewName)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzCreateEmptyView] '{ViewName}'");
        }

        public static object? ZzCreateOrAlterFk(string FkName, string BaseTableName, string BaseColumnName, string TargetTableName, string TargetColumnName, string EnforceRelation)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzCreateOrAlterFk] '{FkName}', '{BaseTableName}', '{BaseColumnName}', '{TargetTableName}', '{TargetColumnName}', '{EnforceRelation}'");
        }

        public static object? ZzCreateTableGuid(string TableName, string PkFieldName, string IgnoreIfExist)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzCreateTableGuid] '{TableName}', '{PkFieldName}', '{IgnoreIfExist}'");
        }

        public static object? ZzCreateTableIdentity(string TableName, string PkFieldName, string PkFieldType, string PkIdentityStart, string PkIdentityStep, string IgnoreIfExist)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzCreateTableIdentity] '{TableName}', '{PkFieldName}', '{PkFieldType}', '{PkIdentityStart}', '{PkIdentityStep}', '{IgnoreIfExist}'");
        }

        public static object? ZzDropColumn(string TableName, string ColumnName)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzDropColumn] '{TableName}', '{ColumnName}'");
        }

        public static object? ZzDropFk(string FkName, string BaseTableName)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzDropFk] '{FkName}', '{BaseTableName}'");
        }

        public static object? ZzDropFunction(string FunctionName)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzDropFunction] '{FunctionName}'");
        }

        public static object? ZzDropProcedure(string ProcedureName)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzDropProcedure] '{ProcedureName}'");
        }

        public static object? ZzDropTable(string TableName)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzDropTable] '{TableName}'");
        }

        public static object? ZzDropView(string ViewName)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzDropView] '{ViewName}'");
        }

        public static object? ZzGetCreateOrAlter(string ObjectName)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzGetCreateOrAlter] '{ObjectName}'");
        }

        public static object? ZzObjectExist(string ObjectName)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzObjectExist] '{ObjectName}'");
        }

        public static object? ZzRenameColumn(string TableName, string InitialName, string NewName)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzRenameColumn] '{TableName}', '{InitialName}', '{NewName}'");
        }

        public static object? ZzRenameTable(string TableName, string NewName)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzRenameTable] '{TableName}', '{NewName}'");
        }

        public static object? ZzTruncateTable(string TableName)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzTruncateTable] '{TableName}'");
        }

        public static object? ZzEnsureSchema(string schema)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzEnsureSchema] '{schema}'");
        }

        public static object? ZzEnsureIndex(string schema, string table, string indexName, string columns, string include, string where)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzEnsureIndex] '{schema}', '{table}', '{indexName}', '{columns}', '{include}', '{where}'");
        }

        public static object? ZzCalculateHID(string TableName, string ParentId, string ParentDigits, string ChildDigits, string Delimiter)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzCalculateHID] '{TableName}', '{ParentId}', '{ParentDigits}', '{ChildDigits}', '{Delimiter}'");
        }

        public static object? ZzRPad(string DbConfName,string s, string length, string pad)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"SELECT [DBO].[ZzRPad]('{s}', '{length}', '{pad}')");
        }

    }
}
