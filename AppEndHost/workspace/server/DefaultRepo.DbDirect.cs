
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

		public static object? ZzCreateTableIdentity(string DbConfName, string TableName, string PkFieldName, string PkFieldType, string PkIdentityStart, string PkIdentityStep, string IgnoreIfExist)
		{
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzCreateTableIdentity] '{TableName}', '{PkFieldName}', '{PkFieldType}', '{PkIdentityStart}', '{PkIdentityStep}', '{IgnoreIfExist}'");
		}

		public static object? ZzCreateTableGuid(string DbConfName, string TableName, string PkFieldName, string IgnoreIfExist)
		{
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzCreateTableGuid] '{TableName}', '{PkFieldName}', '{IgnoreIfExist}'");
		}

		public static object? ZzCreateColumn(string DbConfName, string TableName, string ColumnName, string ColumnTypeSize, string AllowNull)
		{
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzCreateColumn] '{TableName}', '{ColumnName}', '{ColumnTypeSize}', '{AllowNull}'");
		}

		public static object? ZzDropColumn(string DbConfName, string TableName, string ColumnName)
		{
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzDropColumn] '{TableName}', '{ColumnName}'");
		}

		public static object? ZzDropTable(string DbConfName, string TableName)
		{
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzDropTable] '{TableName}'");
		}

		public static object? ZzDropView(string DbConfName, string ViewName)
		{
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzDropView] '{ViewName}'");
		}

		public static object? ZzDropProcedure(string DbConfName, string ProcedureName)
		{
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzDropProcedure] '{ProcedureName}'");
		}

		public static object? ZzDropFunction(string DbConfName, string FunctionName)
		{
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzDropFunction] '{FunctionName}'");
		}

		public static object? ZzDropAllTable(string DbConfName)
		{
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzDropAllTable] ");
		}

		public static object? ZzTruncateTable(string DbConfName, string TableName)
		{
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzTruncateTable] '{TableName}'");
		}

		public static object? ZzRenameColumn(string DbConfName, string TableName, string InitialName, string NewName)
		{
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzRenameColumn] '{TableName}', '{InitialName}', '{NewName}'");
		}

		public static object? ZzRenameTable(string DbConfName, string TableName, string NewName)
		{
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzRenameTable] '{TableName}', '{NewName}'");
		}

		public static object? ZzCreateEmptyView(string DbConfName, string ViewName)
		{
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzCreateEmptyView] '{ViewName}'");
		}

		public static object? ZzCreateEmptyProcedure(string DbConfName, string ProcedureName)
		{
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzCreateEmptyProcedure] '{ProcedureName}'");
		}

		public static object? ZzCreateEmptyTableFunction(string DbConfName, string TableFunctionName)
		{
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzCreateEmptyTableFunction] '{TableFunctionName}'");
		}

		public static object? ZzCreateEmptyScalarFunction(string DbConfName, string ScalarFunctionName)
		{
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzCreateEmptyScalarFunction] '{ScalarFunctionName}'");
		}

		public static object? ZzCreateOrAlterFk(string DbConfName, string FkName, string BaseTableName, string BaseColumnName, string TargetTableName, string TargetColumnName, string EnforceRelation)
		{
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzCreateOrAlterFk] '{FkName}', '{BaseTableName}', '{BaseColumnName}', '{TargetTableName}', '{TargetColumnName}', '{EnforceRelation}'");
		}

		public static object? ZzDropFk(string DbConfName, string FkName, string BaseTableName)
		{
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzDropFk] '{FkName}', '{BaseTableName}'");
		}

		public static object? ZzGetCreateOrAlter(string DbConfName, string ObjectName)
		{
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzGetCreateOrAlter] '{ObjectName}'");
		}

		public static object? Zzz_Deploy(string DbConfName, string PackageName)
		{
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[Zzz_Deploy] '{PackageName}'");
		}

		public static object? ZzAlterColumn(string DbConfName, string TableName, string ColumnName, string ColumnTypeSize, string AllowNull, string Default)
		{
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"EXEC [DBO].[ZzAlterColumn] '{TableName}', '{ColumnName}', '{ColumnTypeSize}', '{AllowNull}', '{Default}'");
		}

		public static object? ZzNthItem(string DbConfName, string String, string Splitter, string N)
		{
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"SELECT [DBO].[ZzNthItem]('{String}', '{Splitter}', '{N}')");
		}

		public static object? ZzTrim(string DbConfName, string String)
		{
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"SELECT [DBO].[ZzTrim]('{String}')");
		}

		public static object? ZzFix2Char(string DbConfName, string S)
		{
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"SELECT [DBO].[ZzFix2Char]('{S}')");
		}

		public static object? ZzCountChar(string DbConfName, string pInput, string pSearchChar)
		{
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"SELECT [DBO].[ZzCountChar]('{pInput}', '{pSearchChar}')");
		}

		public static object? ZzCountWord(string DbConfName, string InputPhrase, string SearchWord)
		{
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"SELECT [DBO].[ZzCountWord]('{InputPhrase}', '{SearchWord}')");
		}

		public static object? ZzFormatBytes(string DbConfName, string InputNumber, string InputUOM)
		{
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"SELECT [DBO].[ZzFormatBytes]('{InputNumber}', '{InputUOM}')");
		}

		public static object? ZzHumanizeNumber(string DbConfName, string InputNumber)
		{
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"SELECT [DBO].[ZzHumanizeNumber]('{InputNumber}')");
		}

		public static object? ZzObjectExist(string DbConfName, string ObjectName)
		{
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"SELECT [DBO].[ZzObjectExist]('{ObjectName}')");
		}

		public static object? ZzSplitString(string DbConfName, string String, string Splitter)
		{
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($"SELECT * FROM [DBO].[ZzSplitString]('{String}', '{Splitter}')");
		}

	}
}
