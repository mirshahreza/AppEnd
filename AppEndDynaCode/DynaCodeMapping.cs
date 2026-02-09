using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AppEndCommon;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AppEndDynaCode
{
    internal static class DynaCodeMapping
    {
        #region Fields
        private static List<CodeMap>? codeMaps;
        #endregion

        #region Source Code Mapping
        public static List<CodeMap> CodeMaps
        {
            get
            {
                codeMaps ??= GenerateSourceCodeMap();
                return codeMaps;
            }
        }

        private static List<CodeMap> GenerateSourceCodeMap()
        {
            List<CodeMap> codeMaps = [];
            foreach (var st in DynaCodeCore.EntierCodeSyntaxes)
            {
                var members = st.GetRoot().DescendantNodes().OfType<MemberDeclarationSyntax>();
                foreach (var member in members)
                {
                    if (member is MethodDeclarationSyntax method)
                    {
                        string nsn = "";
                        SyntaxNode? parentClass = method.Parent as ClassDeclarationSyntax;
                        SyntaxNode? parentNameSpace = parentClass?.Parent;
                        if (parentNameSpace is not null) nsn = ((NamespaceDeclarationSyntax)parentNameSpace).Name.ToString() + ".";
                        string tn = parentClass is null ? "" : ((ClassDeclarationSyntax)parentClass).Identifier.ValueText + ".";
                        string mn = method.Identifier.ValueText;
                        codeMaps.Add(new(nsn + tn + mn, st.FilePath));
                    }
                }
            }
            return codeMaps;
        }

        internal static void ClearCodeMaps()
        {
            codeMaps = null;
        }
        #endregion

        #region File Path Retrieval
        public static string GetMethodFilePath(string methodFullName)
        {
            CodeMap? codeMap = CodeMaps.FirstOrDefault(cm => cm.MethodFullName.EqualsIgnoreCase(methodFullName));
            return codeMap is null
                ? throw new AppEndException($"MethodDoesNotExist : [ {methodFullName} ]", System.Reflection.MethodBase.GetCurrentMethod())
                    .AddParam("MethodFullName", methodFullName)
                    .GetEx() : codeMap.FilePath;
        }

        public static string TryGetMethodFilePath(string methodFullName)
        {
            CodeMap? codeMap = CodeMaps.FirstOrDefault(cm => cm.MethodFullName.EqualsIgnoreCase(methodFullName));
            if (codeMap is null) return "";
            return codeMap.FilePath;
        }

        public static string GetClassFilePath(string typeFullName)
        {
            CodeMap? codeMap = CodeMaps.FirstOrDefault(cm => cm.FilePath.EndsWithIgnoreCase(typeFullName + ".cs"));
            return codeMap is null
                ? throw new AppEndException("ClassFileDoesNotExist", System.Reflection.MethodBase.GetCurrentMethod())
                    .AddParam("TypeFullName", typeFullName)
                    .GetEx() : codeMap.FilePath;
        }

        public static string TryGetClassFilePath(string typeFullName)
        {
            CodeMap? codeMap = CodeMaps.FirstOrDefault(cm => cm.FilePath.EndsWithIgnoreCase(typeFullName + ".cs"));
            if (codeMap is null) return "";
            return codeMap.FilePath;
        }
        #endregion

        #region Type & Method Resolution
        internal static MethodInfo GetMethodInfo(string methodFullName)
        {
            var parts = StaticMethods.MethodPartsNames(methodFullName);
            return GetMethodInfo(parts.Item1, parts.Item2, parts.Item3);
        }

        internal static MethodInfo GetMethodInfo(string? namespaceName, string className, string methodName)
        {
            if (className.Trim() == "") throw new AppEndException($"ClassNameCanNotBeEmpty", System.Reflection.MethodBase.GetCurrentMethod())
                    .AddParam("MethodName", methodName)
                    .GetEx();
            string tn = namespaceName is null || namespaceName == "" ? className : namespaceName + "." + className;
            MethodInfo? methodInfo = GetType(tn).GetMethod(methodName);
            return methodInfo ?? throw new AppEndException($"MethodDoesNotExist", System.Reflection.MethodBase.GetCurrentMethod())
                    .AddParam("NamespaceName", namespaceName.ToStringEmpty())
                    .AddParam("ClassName", className)
                    .AddParam("MethodName", methodName)
                    .GetEx();
        }

        internal static Type GetType(string classFullName)
        {
            string tName = classFullName;
            string nsName = "";
            Type? dynamicType;
            if (classFullName.Contains('.'))
            {
                nsName = classFullName.Split('.')[0];
                tName = classFullName.Split(".")[1];
            }

            // Always try DynaAsm first to get the latest recompiled version
            dynamicType = DynaCodeCore.DynaAsm?.GetTypes().FirstOrDefault(i => i.Name == tName && (nsName == "" || i.Namespace == nsName));

            // Only fall back to EntryAssembly if not found in DynaAsm
            if (dynamicType == null)
            {
                dynamicType = Assembly.GetEntryAssembly()?.GetTypes().FirstOrDefault(i => i.Name == tName && (nsName == "" || i.Namespace == nsName));
            }

            if (dynamicType == null) throw new AppEndException("TypeDoesNotExist", System.Reflection.MethodBase.GetCurrentMethod())
                    .AddParam("ClassFullName", classFullName)
                    .GetEx();
            return dynamicType;
        }
        #endregion
    }
}
