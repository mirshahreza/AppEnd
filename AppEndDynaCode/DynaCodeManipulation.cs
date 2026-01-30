using System;
using System.IO;
using System.Linq;
using AppEndCommon;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TextChange = Microsoft.CodeAnalysis.Text.TextChange;

namespace AppEndDynaCode
{
    internal static class DynaCodeManipulation
    {
        #region Method CRUD Operations
        public static void DuplicateMethod(string methodFullName, string methodCopyName)
        {
            var parts = StaticMethods.MethodPartsNames(methodFullName);
            string methodName = parts.Item3;
            string? filePath = DynaCodeMapping.GetMethodFilePath(methodFullName) ?? throw new AppEndException("MethodFullNameDoesNotExist", System.Reflection.MethodBase.GetCurrentMethod())
                    .AddParam("MethodFullName", methodFullName)
                    .GetEx();
            string controllerBody = File.ReadAllText(filePath);
            SyntaxTree tree = CSharpSyntaxTree.ParseText(controllerBody);

            MethodDeclarationSyntax method = tree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().First(m => m.Identifier.ToString() == methodName);
            string m = method.GetText().ToString();
            string mCopy = m.Replace($"{methodName}(", $"{methodCopyName}(");
            TextChange tc = new(method.Span, $"{m.Trim()}{Environment.NewLine}{Environment.NewLine}{mCopy}");
            controllerBody = tree.GetText().WithChanges(tc).ToString().RemoveWhitelines();

            File.WriteAllText(filePath, controllerBody);
            DynaCodeCore.Refresh();
        }

        public static void CreateMethod(string methodFullName, string methodName, MethodTemplate methodTemplate = MethodTemplate.DbDialog)
        {
            string? filePath = DynaCodeMapping.GetClassFilePath(methodFullName);
            string controllerBody = File.ReadAllText(filePath);
            SyntaxTree tree = CSharpSyntaxTree.ParseText(controllerBody);
            string mBody = new AppEndMethod(methodName, methodTemplate).MethodImplementation;
            MethodDeclarationSyntax method = tree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().Last();
            string m = method.GetText().ToString();
            TextChange tc = new(method.Span, $"{m.Trim()}{Environment.NewLine}{Environment.NewLine}{mBody}");
            controllerBody = tree.GetText().WithChanges(tc).ToString().RemoveWhitelines();
            File.WriteAllText(filePath, controllerBody);
            DynaCodeCore.Refresh();
        }

        public static void RemoveMethod(string methodFullName)
        {
            var parts = StaticMethods.MethodPartsNames(methodFullName);
            string methodName = parts.Item3;
            string? filePath = DynaCodeMapping.GetMethodFilePath(methodFullName) ?? throw new AppEndException("MethodFullNameDoesNotExist", System.Reflection.MethodBase.GetCurrentMethod())
                    .AddParam("MethodFullName", methodFullName)
                    .GetEx();
            string controllerBody = File.ReadAllText(filePath);

            SyntaxTree tree = CSharpSyntaxTree.ParseText(controllerBody);
            MethodDeclarationSyntax method = tree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().First(m => m.Identifier.ToString() == methodName);

            TextChange tc = new(method.Span, string.Empty);
            controllerBody = tree.GetText().WithChanges(tc).ToString().RemoveWhitelines();
            File.WriteAllText(filePath, controllerBody);
            DynaCodeCore.Refresh();
        }

        public static bool MethodExist(string methodFullName)
        {
            var parts = StaticMethods.MethodPartsNames(methodFullName);
            string filePath = DynaCodeMapping.TryGetClassFilePath(methodFullName);
            if (string.IsNullOrEmpty(filePath)) return false;
            string fileBody = File.ReadAllText(filePath);
            fileBody = fileBody.Replace(" ", "");
            return fileBody.Contains($"publicstaticobject?{parts.Item3}(");
        }
        #endregion
    }
}
