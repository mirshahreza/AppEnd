using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AppEndCommon;

namespace AppEndDynaCode
{
    // Utility helpers kept in a single place; used by multiple partials
	public static class Utils
    {

        public static string GetFullName(this MethodInfo methodInfo)
        {
            if (methodInfo.DeclaringType is not null)
                return methodInfo.DeclaringType.Namespace + "." + methodInfo.DeclaringType.Name + "." + methodInfo.Name;
            else
                return methodInfo.Name;
        }

        // Introspection helpers against generated map
        public static bool IsRealType(string typeName)
        {
            CodeMap? codeMap = DynaCode.CodeMaps.FirstOrDefault(i => AppEndCommon.StaticMethods.MethodPartsNames(i.MethodFullName).Item2 == typeName);
            return codeMap != null;
        }

        public static bool IsRealMethod(string methodName)
        {
            CodeMap? codeMap = DynaCode.CodeMaps.FirstOrDefault(i => AppEndCommon.StaticMethods.MethodPartsNames(i.MethodFullName).Item3 == methodName);
            return codeMap != null;
        }
    }
}
