
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

        public static object? Zz_CreateTableIdentity(string DbConfName,string TableName, string PkFieldName, string PkFieldType, string PkIdentityStart, string PkIdentityStep, string IgnoreIfExist)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[Zz_CreateTableIdentity] '{TableName}', '{PkFieldName}', '{PkFieldType}', '{PkIdentityStart}', '{PkIdentityStep}', '{IgnoreIfExist}'");
        }

        public static object? Zz_CreateTableGuid(string DbConfName,string TableName, string PkFieldName, string IgnoreIfExist)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[Zz_CreateTableGuid] '{TableName}', '{PkFieldName}', '{IgnoreIfExist}'");
        }

        public static object? Zz_CreateColumn(string DbConfName,string TableName, string ColumnName, string ColumnTypeSize, string AllowNull)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[Zz_CreateColumn] '{TableName}', '{ColumnName}', '{ColumnTypeSize}', '{AllowNull}'");
        }

        public static object? Zz_DropColumn(string DbConfName,string TableName, string ColumnName)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[Zz_DropColumn] '{TableName}', '{ColumnName}'");
        }

        public static object? Zz_DropTable(string DbConfName,string TableName)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[Zz_DropTable] '{TableName}'");
        }

        public static object? Zz_DropView(string DbConfName,string ViewName)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[Zz_DropView] '{ViewName}'");
        }

        public static object? Zz_DropProcedure(string DbConfName,string ProcedureName)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[Zz_DropProcedure] '{ProcedureName}'");
        }

        public static object? Zz_DropFunction(string DbConfName,string FunctionName)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[Zz_DropFunction] '{FunctionName}'");
        }

        public static object? Zz_DropAllTable(string DbConfName)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[Zz_DropAllTable] ");
        }

        public static object? Zz_TruncateTable(string DbConfName,string TableName)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[Zz_TruncateTable] '{TableName}'");
        }

        public static object? Zz_RenameColumn(string DbConfName,string TableName, string InitialName, string NewName)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[Zz_RenameColumn] '{TableName}', '{InitialName}', '{NewName}'");
        }

        public static object? Zz_RenameTable(string DbConfName,string TableName, string NewName)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[Zz_RenameTable] '{TableName}', '{NewName}'");
        }

        public static object? Zz_CreateEmptyView(string DbConfName,string ViewName)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[Zz_CreateEmptyView] '{ViewName}'");
        }

        public static object? Zz_CreateEmptyProcedure(string DbConfName,string ProcedureName)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[Zz_CreateEmptyProcedure] '{ProcedureName}'");
        }

        public static object? Zz_CreateEmptyTableFunction(string DbConfName,string TableFunctionName)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[Zz_CreateEmptyTableFunction] '{TableFunctionName}'");
        }

        public static object? Zz_CreateEmptyScalarFunction(string DbConfName,string ScalarFunctionName)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[Zz_CreateEmptyScalarFunction] '{ScalarFunctionName}'");
        }

        public static object? Zz_CreateOrAlterFk(string DbConfName,string FkName, string BaseTableName, string BaseColumnName, string TargetTableName, string TargetColumnName, string EnforceRelation)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[Zz_CreateOrAlterFk] '{FkName}', '{BaseTableName}', '{BaseColumnName}', '{TargetTableName}', '{TargetColumnName}', '{EnforceRelation}'");
        }

        public static object? Zz_DropFk(string DbConfName,string FkName, string BaseTableName)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[Zz_DropFk] '{FkName}', '{BaseTableName}'");
        }

        public static object? Zz_GetCreateOrAlter(string DbConfName,string ObjectName)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[Zz_GetCreateOrAlter] '{ObjectName}'");
        }

        public static object? Zzz_Deploy(string DbConfName,string PackageName)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[Zzz_Deploy] '{PackageName}'");
        }

        public static object? Zz_AlterColumn(string DbConfName,string TableName, string ColumnName, string ColumnTypeSize, string AllowNull, string Default)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[Zz_AlterColumn] '{TableName}', '{ColumnName}', '{ColumnTypeSize}', '{AllowNull}', '{Default}'");
        }

        public static object? Zz_NthItem(string DbConfName,string String, string Splitter, string N)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"SELECT [DBO].[Zz_NthItem]('{String}', '{Splitter}', '{N}')");
        }

        public static object? Zz_Trim(string DbConfName,string String)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"SELECT [DBO].[Zz_Trim]('{String}')");
        }

        public static object? Zz_Fix2Char(string DbConfName,string S)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"SELECT [DBO].[Zz_Fix2Char]('{S}')");
        }

        public static object? Zz_CountChar(string DbConfName,string pInput, string pSearchChar)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"SELECT [DBO].[Zz_CountChar]('{pInput}', '{pSearchChar}')");
        }

        public static object? Zz_CountWord(string DbConfName,string InputPhrase, string SearchWord)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"SELECT [DBO].[Zz_CountWord]('{InputPhrase}', '{SearchWord}')");
        }

        public static object? Zz_FormatBytes(string DbConfName,string InputNumber, string InputUOM)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"SELECT [DBO].[Zz_FormatBytes]('{InputNumber}', '{InputUOM}')");
        }

        public static object? Zz_HumanizeNumber(string DbConfName,string InputNumber)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"SELECT [DBO].[Zz_HumanizeNumber]('{InputNumber}')");
        }

        public static object? Zz_ObjectExist(string DbConfName,string ObjectName)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"SELECT [DBO].[Zz_ObjectExist]('{ObjectName}')");
        }

        public static object? Zz_SplitString(string DbConfName,string String, string Splitter)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"SELECT * FROM [DBO].[Zz_SplitString]('{String}', '{Splitter}')");
        }

    }
}
