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
			inputArgs = inputArgs.Trim().Length == 0 ? "string DbConfName" : "string DbConfName," + inputArgs;
			return @"
        public static object? $MethodName$($InputArgs$)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($""EXEC [DBO].[$MethodName$] $Args$"");
        }
".Replace("$MethodName$", MethodName).Replace("$InputArgs$", inputArgs).Replace("$Args$", ArgsToSqlArgs(args));
		}
		internal static string DbScalarFunctionImp(string MethodName, List<string>? args)
		{
			string inputArgs = args == null ? "" : String.Join(", ", args);
			inputArgs = inputArgs.Trim().Length == 0 ? "string DbConfName" : "string DbConfName," + inputArgs;
			return @"
        public static object? $MethodName$($InputArgs$)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($""SELECT [DBO].[$MethodName$]($Args$)"");
        }
".Replace("$MethodName$", MethodName).Replace("$InputArgs$", inputArgs).Replace("$Args$", ArgsToSqlArgs(args));
		}
		internal static string DbTableFunctionImp(string MethodName, List<string>? args)
		{
			string inputArgs = args == null ? "" : String.Join(", ", args);
			inputArgs = inputArgs.Trim().Length == 0 ? "string DbConfName" : "string DbConfName," + inputArgs;
			return @"
        public static object? $MethodName$($InputArgs$)
        {
            return DbIO.Instance(DbConf.FromSettings(DbConfName)).ToScalar($""SELECT * FROM [DBO].[$MethodName$]($Args$)"");
        }
".Replace("$MethodName$", MethodName).Replace("$InputArgs$", inputArgs).Replace("$Args$", ArgsToSqlArgs(args));
		}

		internal static string ArgsToSqlArgs(List<string>? args)
		{
			if (args is null || args.Count == 0) return "";
			List<string> sb = new();
			foreach(string s in args)
			{
				string[] argParts = s.Split(" ");
				if (NeedSingleCoute(argParts[0])) sb.Add("'{"+ argParts[1] + "}'");
				else sb.Add("{" + argParts[1] + "}");
			}

			return string.Join(", ", sb);
		}

		internal static bool NeedSingleCoute(string typePart)
		{
			string tp = typePart.Trim().ToLower();
			if (tp.StartsWithIgnoreCase("int")) return false;
			if (tp.StartsWithIgnoreCase("float")) return false;
			if (tp.StartsWithIgnoreCase("bool")) return false;
			if (tp.StartsWithIgnoreCase("decimal")) return false;
			
			return true;
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
