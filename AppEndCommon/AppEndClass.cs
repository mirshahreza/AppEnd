using System.Text;

namespace AppEndCommon
{
	public class AppEndClass(string className, string namespaceName)
	{
		private readonly string _tempBody = CSharpImpBodies.ClassImp.Replace("$Namespace$", namespaceName).Replace("$ClassName$", className);

		public List<string> Methods { get; set; } = [];

		public string ToCode()
		{
			StringBuilder sb = new();
			foreach (var method in Methods)
				sb.Append(CSharpImpBodies.MethodImp.Replace("$MethodName$", method));
			return _tempBody.Replace("$Methods$", sb.ToString());
		}
	}

	public class AppEndMethod(string methodName)
	{
		public string MethodImplementation => CSharpImpBodies.MethodImp.Replace("$MethodName$", methodName);
	}


	internal static class CSharpImpBodies
	{
		internal static string ClassImp => @"
using System.Text.Json;
using AppEnd;

namespace $Namespace$
{
    public static class $ClassName$
    {
$Methods$
    }
}
";

		internal static string MethodImp => @"
        public static object? $MethodName$(JsonElement ClientQueryJE, AppEndUser? Actor)
        {
            return AppEndDbIO.ClientQuery.GetInstanceByQueryJson(ClientQueryJE, Actor?.ContextInfo).Exec();
        }
";

	}

}
