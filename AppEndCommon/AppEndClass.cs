using System.Text;

namespace AppEndCommon
{
	public class AppEndClass(string className, string namespaceName)
	{
		private readonly string _tempBody = CSharpImpBodies.ClassImp.Replace("$Namespace$", namespaceName).Replace("$ClassName$", className);

		public List<string> DbMethods { get; set; } = [];
		public List<string> EmptyMethods { get; set; } = [];
		public List<string> Injections { get; set; } = [];

		public string ToCode()
		{
			StringBuilder methodsSB = new();
			foreach (var method in DbMethods) methodsSB.Append(CSharpImpBodies.DbMethodImp.Replace("$MethodName$", method));
			foreach (var method in EmptyMethods) methodsSB.Append(CSharpImpBodies.EmptyMethodImp.Replace("$MethodName$", method));
			return _tempBody.Replace("$Methods$", methodsSB.ToString());
		}
	}

	public class AppEndMethod(string methodName, MethodTemplate? methodTemplate = MethodTemplate.DbIoHandler)
	{
		public string MethodImplementation => methodTemplate== MethodTemplate.DbIoHandler ?
			CSharpImpBodies.DbMethodImp.Replace("$MethodName$", methodName) : 
			CSharpImpBodies.EmptyMethodImp.Replace("$MethodName$", methodName);
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

		internal static string DbMethodImp => @"
        public static object? $MethodName$(JsonElement ClientQueryJE, AppEndUser? Actor)
        {
            return AppEndDbIO.ClientQuery.GetInstanceByQueryJson(ClientQueryJE, Actor?.ContextInfo).Exec();
        }
";

		internal static string EmptyMethodImp => @"
        public static object? $MethodName$(AppEndUser? Actor)
        {
            return true;
        }
";

	}

	public enum MethodTemplate
	{
		Empty,
		DbIoHandler
	}

}
