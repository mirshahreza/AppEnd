using AppEndCommon;
using System.Reflection;

namespace AppEndDynaCode
{
	public static class Utils
    {
        public static string SerializeObjectsAsJson(this object[]? inputParams, MethodInfo methodInfo)
        {
            if (inputParams is null) return "{}";
            return ExtractInputItems(inputParams, methodInfo).ToJsonStringByBuiltIn();
        }

        public static Dictionary<string, object> ExtractInputItems(this object[] inputParams, MethodInfo methodInfo)
        {
			ParameterInfo[] parameterInfos = methodInfo.GetParameters();
			Dictionary<string, object> keyValuePairs = [];
			int i = 0;
			foreach (object o in inputParams)
			{
				keyValuePairs[parameterInfos[i].Name] = o;
				i++;
			}
			return keyValuePairs;
		}

        public static string GetFullName(this MethodInfo methodInfo)
        {
            if (methodInfo.DeclaringType is not null)
                return methodInfo.DeclaringType.Namespace + "." + methodInfo.DeclaringType.Name + "." + methodInfo.Name;
            else
                return methodInfo.Name;
        }

        public static void AddExampleCode(CodeInvokeOptions codeInvokeOptions)
        {
            File.WriteAllText(codeInvokeOptions.StartPath + "/Example.cs", @"
namespace Example
{
    public static class ExampleT
    {
        public static int ExampleM(int a, int b)
        {
            return a + b;
        }
    }
}

");
        }
        public static void RemoveExampleCode(CodeInvokeOptions codeInvokeOptions)
        {
            if (File.Exists(codeInvokeOptions.StartPath + "/Example.cs"))
                File.Delete(codeInvokeOptions.StartPath + "/Example.cs");
        }

        public static void AddBuiltinLogMethods()
        {

        }

        public static bool IsRealType(string typeName)
        {
            CodeMap? codeMap = DynaCode.CodeMaps.FirstOrDefault(i => DynaCode.MethodPartsNames(i.MethodFullName).Item2 == typeName);
            return codeMap != null;
        }

        public static bool IsRealMethod(string methodName)
        {
            CodeMap? codeMap = DynaCode.CodeMaps.FirstOrDefault(i => DynaCode.MethodPartsNames(i.MethodFullName).Item3 == methodName);
            return codeMap != null;
        }

    }
}
