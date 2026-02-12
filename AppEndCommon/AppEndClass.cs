using System.Text;

namespace AppEndCommon
{
	public class AppEndClass(string className, string namespaceName)
	{
		private readonly string _tempBody = CSharpImpBodies.ClassImp.Replace("$Namespace$", namespaceName).Replace("$ClassName$", className);

		public List<string> DbDialogMethods { get; set; } = [];
		public List<string> NotMappedMethods { get; set; } = [];
		public Dictionary<string, List<string>> DbProducerMethods { get; set; } = [];
		public Dictionary<string, List<string>> DbScalarFunctionMethods { get; set; } = [];
		public Dictionary<string, List<string>> DbTableFunctionMethods { get; set; } = [];

		public string ToCode()
		{
			StringBuilder methodsSB = new();
			foreach (var method in DbDialogMethods) methodsSB.Append(CSharpImpBodies.DbDialogImp(method));
			foreach (var method in NotMappedMethods) methodsSB.Append(CSharpImpBodies.NotMappedImp(method));
			foreach (var method in DbProducerMethods) methodsSB.Append(CSharpImpBodies.DbProducerImp(method.Key, method.Value));
			foreach (var method in DbScalarFunctionMethods) methodsSB.Append(CSharpImpBodies.DbScalarFunctionImp(method.Key, method.Value));
			foreach (var method in DbTableFunctionMethods) methodsSB.Append(CSharpImpBodies.DbTableFunctionImp(method.Key, method.Value));
			return _tempBody.Replace("$Methods$", methodsSB.ToString());
		}
	}

	public class AppEndMethod(string methodName, MethodTemplate methodTemplate, List<string>? InputArgs = null)
	{
		public string MethodImplementation
		{
			get
			{
				if (methodTemplate == MethodTemplate.DbDialog) return CSharpImpBodies.DbDialogImp(methodName);
				if (methodTemplate == MethodTemplate.DbProducer) return CSharpImpBodies.DbProducerImp(methodName, InputArgs);
				if (methodTemplate == MethodTemplate.DbScalarFunction) return CSharpImpBodies.DbScalarFunctionImp(methodName, InputArgs);
				if (methodTemplate == MethodTemplate.DbTableFunction) return CSharpImpBodies.DbTableFunctionImp(methodName, InputArgs);
				if (methodTemplate == MethodTemplate.DbDialog) return CSharpImpBodies.NotMappedImp(methodName);
				return "";
			}
		}
	}


	internal static class CSharpImpBodies
	{
		internal static string ClassImp => @"
using System;
using System.Text.Json;
using AppEndCommon;
using AppEndDynaCode;
using AppEndDbIO;
using AppEndServer;

namespace $Namespace$
{    
	public static class $ClassName$
    {
		private static string DbConfName = ""$Namespace$"";

$Methods$
    }
}
";




		internal static string DbDialogImp(string MethodName)
		{
			return @"
        public static object? $MethodName$(JsonElement ClientQueryJE, AppEndUser? Actor)
        {
            return AppEndDbIO.ClientQuery.GetInstanceByQueryJson(ClientQueryJE, Actor?.ContextInfo).Exec();
        }
".Replace("$MethodName$", MethodName);
		}

		internal static string NotMappedImp(string MethodName)
		{
			return @"
        public static object? $MethodName$(AppEndUser? Actor)
        {
            return true;
        }
".Replace("$MethodName$", MethodName);
		}


		internal static string DbProducerImp(string MethodName, List<string>? args)
		{
			string inputArgs = args == null ? "" : String.Join(", ", args);
			string listBody = ArgsToSqlDbParamsListBody(args);
			string dbParamsExpr = ArgsToSqlDbParamsExpr();
			return @"
        public static object? $MethodName$($InputArgs$)
        {
			$DbParamsInit$
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar(
				""EXEC [DBO].[$MethodName$] $Args$"",
				$DbParams$);
        }
"
				.Replace("$MethodName$", MethodName)
				.Replace("$InputArgs$", inputArgs)
				.Replace("$Args$", ArgsToSqlArgs(args))
				.Replace("$DbParams$", dbParamsExpr)
				.Replace("$DbParamsInit$", listBody);
		}
		internal static string DbScalarFunctionImp(string MethodName, List<string>? args)
		{
			string inputArgs = args == null ? "" : String.Join(", ", args);
			string listBody = ArgsToSqlDbParamsListBody(args);
			string dbParamsExpr = ArgsToSqlDbParamsExpr();
			return @"
        public static object? $MethodName$($InputArgs$)
        {
			$DbParamsInit$
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar(
				""SELECT [DBO].[$MethodName$]($Args$)"",
				$DbParams$);
        }
"
				.Replace("$MethodName$", MethodName)
				.Replace("$InputArgs$", inputArgs)
				.Replace("$Args$", ArgsToSqlArgs(args))
				.Replace("$DbParams$", dbParamsExpr)
				.Replace("$DbParamsInit$", listBody);
		}
		internal static string DbTableFunctionImp(string MethodName, List<string>? args)
		{
			string inputArgs = args == null ? "" : String.Join(", ", args);
			string listBody = ArgsToSqlDbParamsListBody(args);
			string dbParamsExpr = ArgsToSqlDbParamsExpr();
			return @"
        public static object? $MethodName$($InputArgs$)
        {
			$DbParamsInit$
			return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar(
				""SELECT * FROM [DBO].[$MethodName$]($Args$)"",
				$DbParams$);
        }
"
				.Replace("$MethodName$", MethodName)
				.Replace("$InputArgs$", inputArgs)
				.Replace("$Args$", ArgsToSqlArgs(args))
				.Replace("$DbParams$", dbParamsExpr)
				.Replace("$DbParamsInit$", listBody);
		}

		internal static string ArgsToSqlArgs(List<string>? args)
		{
			if (args is null || args.Count == 0) return "";
			List<string> sb = [];
			foreach(string s in args)
			{
				string[] argParts = s.Split(" ");
				sb.Add("@" + argParts[1]);
			}

			return string.Join(", ", sb);
		}

		internal static string ArgsToSqlDbParamsListBody(List<string>? args)
		{
			if (args is null || args.Count == 0) return string.Empty;
			List<string> lines =
			[
				"var dbParams = new System.Collections.Generic.List<System.Data.Common.DbParameter>();"
			];
			foreach (string s in args)
			{
				string[] argParts = s.Split(" ");
				string typePart = argParts[0];
				string namePart = argParts[1];
				lines.Add($"dbParams.Add(new Microsoft.Data.SqlClient.SqlParameter(\"@{namePart}\", {CSharpTypeToSqlDbType(typePart)}) {{ Value = {namePart} }});");
			}
			return string.Join("\n\t\t\t", lines);
		}

		internal static string ArgsToSqlDbParamsExpr()
		{
			return "dbParams";
		}

		internal static string CSharpTypeToSqlDbType(string typePart)
		{
			string tp = typePart.Trim().ToLower();
			if (tp.StartsWith("int64") || tp.StartsWith("long")) return "System.Data.SqlDbType.BigInt";
			if (tp.StartsWith("int")) return "System.Data.SqlDbType.Int";
			if (tp.StartsWith("single") || tp.StartsWith("float")) return "System.Data.SqlDbType.Float";
			if (tp.StartsWith("decimal")) return "System.Data.SqlDbType.Decimal";
			if (tp.StartsWith("datetime")) return "System.Data.SqlDbType.DateTime";
			if (tp.StartsWith("boolean") || tp.StartsWith("bool")) return "System.Data.SqlDbType.Bit";
			if (tp.StartsWith("byte[]")) return "System.Data.SqlDbType.VarBinary";
			return "System.Data.SqlDbType.NVarChar";
		}
	}

	public enum MethodTemplate
	{
		NotMapped,
		DbProducer,
		DbScalarFunction,
		DbTableFunction,
		DbDialog
	}

}
