
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

        public static object? Zz_CreateTableIdentity(string TableName, string PkFieldName, string PkFieldType, string PkIdentityStart, string PkIdentityStep, string IgnoreIfExist)
        {
            return true;
        }

        public static object? Zz_CreateTableGuid(string TableName, string PkFieldName, string IgnoreIfExist)
        {
            return true;
        }

        public static object? Zz_CreateColumn(string TableName, string ColumnName, string ColumnTypeSize, string AllowNull)
        {
            return true;
        }

        public static object? Zz_DropColumn(string TableName, string ColumnName)
        {
            return true;
        }

        public static object? Zz_DropTable(string TableName)
        {
            return true;
        }

        public static object? Zz_DropView(string ViewName)
        {
            return true;
        }

        public static object? Zz_DropProcedure(string Procedure)
        {
            return true;
        }

        public static object? Zz_DropFunction(string Function)
        {
            return true;
        }

        public static object? Zz_DropAllTable()
        {
            return true;
        }

        public static object? Zz_TruncateTable(string TableName)
        {
            return true;
        }

        public static object? Zz_RenameColumn(string TableName, string InitialName, string NewName)
        {
            return true;
        }

        public static object? Zz_RenameTable(string TableName, string NewName)
        {
            return true;
        }

        public static object? Zz_CreateEmptyView(string ViewName)
        {
            return true;
        }

        public static object? Zz_CreateEmptyProcedure(string ProcedureName)
        {
            return true;
        }

        public static object? Zz_CreateEmptyTableFunction(string TableFunctionName)
        {
            return true;
        }

        public static object? Zz_CreateEmptyScalarFunction(string ScalarFunctionName)
        {
            return true;
        }

        public static object? Zz_CreateOrAlterFk(string FkName, string BaseTableName, string BaseColumnName, string TargetTableName, string TargetColumnName, string EnforceRelation)
        {
            return true;
        }

        public static object? Zz_DropFk(string FkName, string BaseTableName)
        {
            return true;
        }

        public static object? Zz_GetCreateOrAlter(string ObjectName)
        {
            return true;
        }

        public static object? Zzz_Deploy(string PackageName)
        {
            return true;
        }

        public static object? Zz_AlterColumn(string TableName, string ColumnName, string ColumnTypeSize, string AllowNull, string Default)
        {
            return true;
        }

        public static object? Zz_NthItem(string String, string Splitter, string N)
        {
            return true;
        }

        public static object? Zz_Trim(string String)
        {
            return true;
        }

        public static object? Zz_Fix2Char(string S)
        {
            return true;
        }

        public static object? Zz_CountChar(string pInput, string pSearchChar)
        {
            return true;
        }

        public static object? Zz_CountWord(string InputPhrase, string SearchWord)
        {
            return true;
        }

        public static object? Zz_FormatBytes(string InputNumber, string InputUOM)
        {
            return true;
        }

        public static object? Zz_HumanizeNumber(string InputNumber)
        {
            return true;
        }

        public static object? Zz_ObjectExist(string ObjectName)
        {
            return true;
        }

        public static object? Zz_SplitString(string String, string Splitter)
        {
            return true;
        }

    }
}
