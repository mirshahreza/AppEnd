using System.Text;

namespace AppEndCommon
{
	public class AppEndClass(string className, string namespaceName)
	{
		private readonly string _tempBody = CSharpImpBodies.ClassImp.Replace("$Namespace$", namespaceName).Replace("$ClassName$", className);

		public List<string> DbMethods { get; set; } = [];
		public List<string> EmptyMethods { get; set; } = [];
		public Dictionary<string, List<string>> DbDirectMethods { get; set; } = [];

		public string ToCode()
		{
			StringBuilder methodsSB = new();
			foreach (var method in DbMethods) methodsSB.Append(CSharpImpBodies.DbMethodImp(method));
			foreach (var method in EmptyMethods) methodsSB.Append(CSharpImpBodies.EmptyMethodImp(method));
			foreach (var method in DbDirectMethods) methodsSB.Append(CSharpImpBodies.DirectMethodImp(method.Key, method.Value));
			return _tempBody.Replace("$Methods$", methodsSB.ToString());
		}
	}

	public class AppEndMethod(string methodName, MethodTemplate? methodTemplate = MethodTemplate.DbIoHandler)
	{
		public string MethodImplementation => methodTemplate == MethodTemplate.DbIoHandler ?
			CSharpImpBodies.DbMethodImp(methodName) :
			(methodTemplate == MethodTemplate.DbDirect ? CSharpImpBodies.DirectMethodImp(methodName,[]) : CSharpImpBodies.EmptyMethodImp( methodName));
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




		internal static string DbMethodImp(string MethodName)
		{
			return @"
        public static object? $MethodName$(JsonElement ClientQueryJE, AppEndUser? Actor)
        {
            return AppEndDbIO.ClientQuery.GetInstanceByQueryJson(ClientQueryJE, Actor?.ContextInfo).Exec();
        }
".Replace("$MethodName$", MethodName);
		}

		internal static string EmptyMethodImp(string MethodName)
		{
			return @"
        public static object? $MethodName$(AppEndUser? Actor)
        {
            return true;
        }
".Replace("$MethodName$", MethodName);
		}


		internal static string DirectMethodImp(string MethodName, List<string> args)
		{
			string inputArgs = String.Join(", ", args);

			return @"
        public static object? $MethodName$($InputArgs$)
        {
            return true;
        }
".Replace("$MethodName$", MethodName).Replace("$InputArgs$", inputArgs);
		}

	}

	public enum MethodTemplate
	{
		Empty,
		DbDirect,
		DbIoHandler
	}

}
