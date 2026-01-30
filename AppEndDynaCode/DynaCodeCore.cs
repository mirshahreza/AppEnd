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
            var processedAssemblies = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // 1. Load all assemblies from current AppDomain
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            foreach (var asm in loadedAssemblies)
            {
                AddReferencesForRecursive(asm, references, processedAssemblies);
            }

            // 2. Ensure critical assemblies are loaded
            var criticalAssemblies = new List<Assembly?>
            {
                Assembly.GetExecutingAssembly(),
                Assembly.GetEntryAssembly(),
                Assembly.GetCallingAssembly(),
                typeof(object).Assembly,
                typeof(System.ComponentModel.TypeConverter).Assembly,
                typeof(System.Linq.Expressions.Expression).Assembly,
                typeof(System.Text.Encodings.Web.JavaScriptEncoder).Assembly,
                typeof(Exception).Assembly,
                typeof(ArgumentNullException).Assembly,
                typeof(System.Runtime.CompilerServices.DynamicAttribute).Assembly,
                typeof(Microsoft.CSharp.RuntimeBinder.RuntimeBinderException).Assembly
            };

            // Load AppEndCommon assembly explicitly
            try
            {
                var appEndCommonAsm = typeof(AppEndCommon.AppEndException).Assembly;
                criticalAssemblies.Add(appEndCommonAsm);
            }
            catch { }

            // Try to load from references path if AppEndCommon.dll exists there
            if (Directory.Exists(invokeOptions.ReferencesPath))
            {
                string appEndCommonPath = Path.Combine(invokeOptions.ReferencesPath, "AppEndCommon.dll");
                if (File.Exists(appEndCommonPath))
                {
                    try
                    {
                        var asm = Assembly.LoadFrom(appEndCommonPath);
                        criticalAssemblies.Add(asm);
                    }
                    catch { }
                }
            }

            foreach (var asm in criticalAssemblies)
            {
                AddReferencesForRecursive(asm, references, processedAssemblies);
            }

            // 3. Try to load netstandard
            try
            {
                var netstandardAsm = Assembly.Load("netstandard, Version=2.1.0.0");
                AddReferencesForRecursive(netstandardAsm, references, processedAssemblies);
            }
            catch { }

            // 4. Load from runtime directory
            string runtimeDir = Path.GetDirectoryName(typeof(object).Assembly.Location) ?? "";
            if (Directory.Exists(runtimeDir))
            {
                foreach (string dllPath in Directory.GetFiles(runtimeDir, "*.dll"))
                {
                    string fileName = Path.GetFileNameWithoutExtension(dllPath);
                    if (IsCoreAssembly(fileName))
                    {
                        TryAddReferenceByPath(dllPath, references);
                    }
                }
            }

            // 5. Load from custom references path
            if (Directory.Exists(invokeOptions.ReferencesPath))
            {
                foreach (string dllPath in Directory.GetFiles(invokeOptions.ReferencesPath, "*.dll", SearchOption.AllDirectories))
                {
                    if (File.Exists(dllPath))
                    {
                        TryAddReferenceByPath(dllPath, references);
                        TryLoadAssemblyAndAddReferences(dllPath, references, processedAssemblies);
                    }
                }
            }

            // 6. Deduplicate and return
            return references
                .GroupBy(r => (r as PortableExecutableReference)?.FilePath, StringComparer.OrdinalIgnoreCase)
                .Select(g => g.First())
                .ToList();
        }

        private static List<MetadataReference> GetCompilationReferences_Old()
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

        private static void AddReferencesForRecursive(Assembly? asm, List<MetadataReference> references, HashSet<string> processedAssemblies, int depth = 0)
        {
            if (asm is null || depth > 5) return;
            
            string loc;
            try { loc = asm.Location; } catch { return; }
            if (string.IsNullOrWhiteSpace(loc) || !File.Exists(loc)) return;
            
            if (!processedAssemblies.Add(loc)) return;
            
            TryAddReferenceByPath(loc, references);

            AssemblyName[] referencedAssemblies;
            try { referencedAssemblies = asm.GetReferencedAssemblies(); } catch { return; }
            
            foreach (var refName in referencedAssemblies)
            {
                Assembly? refAsm = null;
                try
                {
                    refAsm = Assembly.Load(refName);
                }
                catch
                {
                    try
                    {
                        string refPath = Path.Combine(Path.GetDirectoryName(loc) ?? "", refName.Name + ".dll");
                        if (File.Exists(refPath))
                        {
                            refAsm = Assembly.LoadFrom(refPath);
                        }
                    }
                    catch { }
                }
                
                if (refAsm != null)
                {
                    AddReferencesForRecursive(refAsm, references, processedAssemblies, depth + 1);
                }
            }
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
            
            // Skip Native DLLs
            string fileName = Path.GetFileNameWithoutExtension(path);
            if (IsNativeDll(fileName))
            {
                return;
            }
            
            if (!referenceCache.TryGetValue(path, out var mr))
            {
                try
                {
                    mr = MetadataReference.CreateFromFile(path);
                    referenceCache[path] = mr;
                }
                catch
                {
                    // Ignore files that cannot be loaded as managed assemblies
                    return;
                }
            }
            references.Add(mr);
        }

        private static void TryLoadAssemblyAndAddReferences(string dllPath, List<MetadataReference> references, HashSet<string> processedAssemblies)
        {
            try
            {
                var asmName = AssemblyName.GetAssemblyName(dllPath);
                Assembly? asm = null;
                
                try
                {
                    asm = Assembly.Load(asmName);
                }
                catch
                {
                    try
                    {
                        asm = Assembly.LoadFrom(dllPath);
                    }
                    catch { }
                }
                
                if (asm != null)
                {
                    AddReferencesForRecursive(asm, references, processedAssemblies);
                }
            }
            catch { }
        }

        private static bool IsCoreAssembly(string assemblyName)
        {
            var coreAssemblies = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "System.Runtime", "System.Collections", "System.Linq", "System.Core",
                "System.Private.CoreLib", "System.Console", "System.IO", "System.Threading",
                "System.Text.RegularExpressions", "System.Net.Http", "System.Xml",
                "System.Collections.Concurrent", "System.Linq.Expressions", "System.ComponentModel",
                "System.ObjectModel", "System.Runtime.Extensions", "System.Text.Json",
                "System.Private.Xml", "System.Diagnostics", "System.Memory", "netstandard"
            };
            
            return coreAssemblies.Contains(assemblyName) || assemblyName.StartsWith("System.", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsNativeDll(string fileName)
        {
            var nativeDlls = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "System.IO.Compression.Native",
                "System.Native",
                "System.Net.Security.Native",
                "System.Security.Cryptography.Native.OpenSsl",
                "clrcompression",
                "clretwrc",
                "clrjit",
                "coreclr",
                "dbgshim",
                "hostfxr",
                "hostpolicy",
                "mscordaccore",
                "mscordbi",
                "mscorrc",
                "ucrtbase",
                "api-ms-win-core",
                "sni"
            };
            
            // Check if it's in the native DLL list
            if (nativeDlls.Contains(fileName))
                return true;
            
            // Check if filename contains "Native" or "native"
            if (fileName.Contains("Native", StringComparison.OrdinalIgnoreCase))
                return true;
            
            // Check if filename starts with common native prefixes
            if (fileName.StartsWith("api-ms-win", StringComparison.OrdinalIgnoreCase))
                return true;
            
            return false;
        }
        #endregion

        #region Internal Properties Access
        internal static CodeInvokeOptions InvokeOptions => invokeOptions;
        #endregion
    }
}
