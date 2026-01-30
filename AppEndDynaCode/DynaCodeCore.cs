using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using AppEndCommon;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace AppEndDynaCode
{
    internal static class DynaCodeCore
    {
        #region Fields & Options
        private static CodeInvokeOptions invokeOptions = new("");
        private static Assembly? dynaAsm;
        private static AssemblyLoadContext? loadContext;
        private static IEnumerable<SyntaxTree>? entierCodeSyntaxes;
        private static string[]? scriptFiles;
        private static readonly Dictionary<string, MetadataReference> referenceCache = new(StringComparer.OrdinalIgnoreCase);
        private static readonly HashSet<string> visitedReferencePaths = new(StringComparer.OrdinalIgnoreCase);
        #endregion

        #region Lifecycle Management
        public static void Init(CodeInvokeOptions? codeInvokeOptions = null)
        {
            if (codeInvokeOptions is not null) invokeOptions = codeInvokeOptions;
            Refresh();
        }

        public static void Refresh()
        {
            if (loadContext != null)
            {
                var contextToUnload = loadContext;
                loadContext = null;
                dynaAsm = null;

                try
                {
                    contextToUnload.Unload();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                }
                catch { }
            }
            else
            {
                dynaAsm = null;
            }

            entierCodeSyntaxes = null;
            scriptFiles = null;
            referenceCache.Clear();
            visitedReferencePaths.Clear();
            
            // Clear code maps as well
            DynaCodeMapping.ClearCodeMaps();
        }

        public static string ReBuild()
        {
            Refresh();
            Assembly asm = DynaAsm;
            return asm.FullName.ToStringEmpty();
        }
        #endregion

        #region Assembly Management
        public static Assembly DynaAsm
        {
            get
            {
                if (dynaAsm == null) Build();
                return dynaAsm;
            }
        }

        private static void Build()
        {
            using var peStream = new MemoryStream();

            var compileRefs = GetCompilationReferences();
            var compilerOptions = new CSharpCompilationOptions(outputKind: OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release, assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default);
            string asmName = $"DynaAsm{Guid.NewGuid().ToString().Replace("-", "")}";
            CSharpCompilation cSharpCompilation = CSharpCompilation.Create(asmName, EntierCodeSyntaxes, compileRefs, compilerOptions);

            var result = cSharpCompilation.Emit(peStream);

            if (!result.Success)
            {
                var failures = result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);
                var error = failures.FirstOrDefault();
                throw new AppEndException($"{error?.Id}: {error?.GetMessage()}", System.Reflection.MethodBase.GetCurrentMethod()).GetEx();
            }

            peStream.Seek(0, SeekOrigin.Begin);
            byte[] dllBytes = peStream.ToArray();

            // Create a new load context for each build to ensure fresh assembly loading
            loadContext = new AssemblyLoadContext($"DynaContext_{asmName}", isCollectible: true);
            peStream.Seek(0, SeekOrigin.Begin);
            dynaAsm = loadContext.LoadFromStream(peStream);
        }

        public static string[] ScriptFiles
        {
            get
            {
                scriptFiles ??= [.. new DirectoryInfo(invokeOptions.StartPath).GetFilesRecursive("*.cs")];
                return scriptFiles;
            }
        }

        internal static IEnumerable<SyntaxTree> EntierCodeSyntaxes
        {
            get
            {
                if (entierCodeSyntaxes is null)
                {
                    List<SourceCode> sourceCodes = GetAllSourceCodes();
                    var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp14);
                    entierCodeSyntaxes = sourceCodes.Select(sourceCode => SyntaxFactory.ParseSyntaxTree(sourceCode.RawCode, options, sourceCode.FilePath));
                }
                return entierCodeSyntaxes;
            }
        }

        private static List<SourceCode> GetAllSourceCodes()
        {
            List<SourceCode> sourceCodes = [];
            foreach (string f in ScriptFiles) sourceCodes.Add(new(f, File.ReadAllText(f)));
            return sourceCodes;
        }
        #endregion

        #region Reference Management
        private static List<MetadataReference> GetCompilationReferences()
        {
            var references = new List<MetadataReference>();

            foreach (var a in AppDomain.CurrentDomain.GetAssemblies()) AddReferencesFor(a, references);
            AddReferencesFor(Assembly.GetExecutingAssembly(), references);
            AddReferencesFor(Assembly.GetEntryAssembly(), references);
            AddReferencesFor(Assembly.GetCallingAssembly(), references);

            AddReferencesFor(typeof(object).Assembly, references);
            AddReferencesFor(typeof(System.ComponentModel.TypeConverter).Assembly, references);
            try { AddReferencesFor(Assembly.Load("netstandard, Version=2.1.0.0"), references); } catch { }
            AddReferencesFor(typeof(System.Linq.Expressions.Expression).Assembly, references);
            AddReferencesFor(typeof(System.Text.Encodings.Web.JavaScriptEncoder).Assembly, references);
            AddReferencesFor(typeof(Exception).Assembly, references);
            AddReferencesFor(typeof(AppEndCommon.AppEndException).Assembly, references);
            AddReferencesFor(typeof(ArgumentNullException).Assembly, references);

            if (Directory.Exists(invokeOptions.ReferencesPath))
            {
                foreach (string f in Directory.GetFiles(invokeOptions.ReferencesPath, "*.dll"))
                {
                    if (File.Exists(f))
                    {
                        TryAddReferenceByPath(f, references);
                    }
                }
            }

            return references
                .GroupBy(r => (r as PortableExecutableReference)?.FilePath, StringComparer.OrdinalIgnoreCase)
                .Select(g => g.First())
                .ToList();
        }

        private static void AddReferencesFor(Assembly? asm, List<MetadataReference> references)
        {
            if (asm is null) return;
            string loc;
            try { loc = asm.Location; } catch { return; }
            if (string.IsNullOrWhiteSpace(loc) || !File.Exists(loc)) return;

            TryAddReferenceByPath(loc, references);

            AssemblyName[] rfs;
            try { rfs = asm.GetReferencedAssemblies(); } catch { rfs = Array.Empty<AssemblyName>(); }
            foreach (var a in rfs)
            {
                Assembly? asmF = null;
                try { asmF = Assembly.Load(a); } catch { }
                if (asmF is null) continue;
                string rloc;
                try { rloc = asmF.Location; } catch { continue; }
                if (string.IsNullOrWhiteSpace(rloc) || !File.Exists(rloc)) continue;
                TryAddReferenceByPath(rloc, references);
            }
        }

        private static void TryAddReferenceByPath(string path, List<MetadataReference> references)
        {
            if (!visitedReferencePaths.Add(path)) return;
            if (!referenceCache.TryGetValue(path, out var mr))
            {
                mr = MetadataReference.CreateFromFile(path);
                referenceCache[path] = mr;
            }
            references.Add(mr);
        }
        #endregion

        #region Internal Properties Access
        internal static CodeInvokeOptions InvokeOptions => invokeOptions;
        #endregion
    }
}
