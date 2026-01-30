using System;
using System.Collections.Concurrent;
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
        private static readonly ConcurrentDictionary<string, MetadataReference> referenceCache = new(StringComparer.OrdinalIgnoreCase);
        private static readonly ConcurrentDictionary<string, byte> visitedReferencePaths = new(StringComparer.OrdinalIgnoreCase);
        private static readonly ConcurrentDictionary<string, Assembly> loadedAssemblies = new(StringComparer.OrdinalIgnoreCase);
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
            loadedAssemblies.Clear();
            
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

            // Build assembly cache from compilation references for dependency resolution
            BuildAssemblyCache(compileRefs);

            // Create a new load context for each build to ensure fresh assembly loading
            loadContext = new AssemblyLoadContext($"DynaContext_{asmName}", isCollectible: true);
            
            // Add resolving handler for dependencies
            loadContext.Resolving += LoadContext_Resolving;
            
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

        private static void BuildAssemblyCache(List<MetadataReference> compileRefs)
        {
            loadedAssemblies.Clear();
            
            foreach (var reference in compileRefs)
            {
                if (reference is PortableExecutableReference peRef && !string.IsNullOrWhiteSpace(peRef.FilePath))
                {
                    try
                    {
                        var asmName = AssemblyName.GetAssemblyName(peRef.FilePath);
                        var fullName = asmName.FullName;
                        var simpleName = asmName.Name ?? "";
                        
                        // Try to load the assembly
                        Assembly? asm = null;
                        try
                        {
                            asm = Assembly.Load(asmName);
                        }
                        catch
                        {
                            try
                            {
                                asm = Assembly.LoadFrom(peRef.FilePath);
                            }
                            catch { }
                        }
                        
                        if (asm != null)
                        {
                            if (!string.IsNullOrWhiteSpace(fullName))
                                loadedAssemblies.TryAdd(fullName, asm);
                            
                            if (!string.IsNullOrWhiteSpace(simpleName))
                                loadedAssemblies.TryAdd(simpleName, asm);
                            
                            loadedAssemblies.TryAdd(peRef.FilePath, asm);
                        }
                    }
                    catch { }
                }
            }
        }

        private static Assembly? LoadContext_Resolving(AssemblyLoadContext context, AssemblyName assemblyName)
        {
            try
            {
                var fullName = assemblyName.FullName;
                var simpleName = assemblyName.Name ?? "";
                
                // First, try to find in cache by full name
                if (!string.IsNullOrWhiteSpace(fullName) && loadedAssemblies.TryGetValue(fullName, out var cachedAsm))
                    return cachedAsm;
                
                // Try by simple name
                if (!string.IsNullOrWhiteSpace(simpleName) && loadedAssemblies.TryGetValue(simpleName, out cachedAsm))
                    return cachedAsm;
                
                // Try to load from default context
                try
                {
                    return AssemblyLoadContext.Default.LoadFromAssemblyName(assemblyName);
                }
                catch { }
                
                // Try to load from current AppDomain
                try
                {
                    return Assembly.Load(assemblyName);
                }
                catch { }
                
                // Search in various directories
                var searchPaths = new List<string>();
                
                // Add references path
                if (Directory.Exists(invokeOptions.ReferencesPath))
                    searchPaths.Add(invokeOptions.ReferencesPath);
                
                // Add executing assembly directory
                var executingDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                if (!string.IsNullOrWhiteSpace(executingDir) && Directory.Exists(executingDir))
                    searchPaths.Add(executingDir);
                
                // Add entry assembly directory
                var entryAsm = Assembly.GetEntryAssembly();
                if (entryAsm != null)
                {
                    var entryDir = Path.GetDirectoryName(entryAsm.Location);
                    if (!string.IsNullOrWhiteSpace(entryDir) && Directory.Exists(entryDir))
                        searchPaths.Add(entryDir);
                }
                
                // Add runtime directory
                var runtimeDir = Path.GetDirectoryName(typeof(object).Assembly.Location);
                if (!string.IsNullOrWhiteSpace(runtimeDir) && Directory.Exists(runtimeDir))
                    searchPaths.Add(runtimeDir);
                
                // Search for the assembly file
                foreach (var searchPath in searchPaths.Distinct())
                {
                    var dllPath = Path.Combine(searchPath, simpleName + ".dll");
                    if (File.Exists(dllPath))
                    {
                        try
                        {
                            var asm = context.LoadFromAssemblyPath(dllPath);
                            if (asm != null)
                            {
                                // Cache it for future use
                                if (!string.IsNullOrWhiteSpace(fullName))
                                    loadedAssemblies.TryAdd(fullName, asm);
                                if (!string.IsNullOrWhiteSpace(simpleName))
                                    loadedAssemblies.TryAdd(simpleName, asm);
                                
                                return asm;
                            }
                        }
                        catch { }
                    }
                }
                
                // If still not found, search recursively in references path
                if (Directory.Exists(invokeOptions.ReferencesPath))
                {
                    foreach (var dllPath in Directory.GetFiles(invokeOptions.ReferencesPath, simpleName + ".dll", SearchOption.AllDirectories))
                    {
                        if (File.Exists(dllPath))
                        {
                            try
                            {
                                var asm = context.LoadFromAssemblyPath(dllPath);
                                if (asm != null)
                                {
                                    if (!string.IsNullOrWhiteSpace(fullName))
                                        loadedAssemblies.TryAdd(fullName, asm);
                                    if (!string.IsNullOrWhiteSpace(simpleName))
                                        loadedAssemblies.TryAdd(simpleName, asm);
                                    
                                    return asm;
                                }
                            }
                            catch { }
                        }
                    }
                }
            }
            catch { }
            

            return null;
        }
        #endregion


        #region Reference Management
        private static List<MetadataReference> GetCompilationReferences()
        {
            var references = new List<MetadataReference>();
            var processedPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // Step 1: Get all assemblies currently loaded in AppDomain (most reliable source)
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic) // Skip dynamic assemblies
                .ToList();

            // Step 2: Add core assemblies (essential for compilation)
            AddCoreAssemblies(references, processedPaths);

            // Step 3: Process all loaded assemblies and their dependencies
            foreach (var assembly in loadedAssemblies)
            {
                ProcessAssemblyRecursive(assembly, references, processedPaths, depth: 0, maxDepth: 3);
            }

            // Step 4: Scan file system for additional assemblies
            ScanFileSystemForAssemblies(references, processedPaths);

            // Step 5: Ensure all AppEnd assemblies are included
            EnsureAppEndAssembliesIncluded(references, processedPaths);

            // Step 6: Validate and deduplicate
            var finalReferences = references
                .Where(r => r != null)
                .GroupBy(r => (r as PortableExecutableReference)?.FilePath, StringComparer.OrdinalIgnoreCase)
                .Select(g => g.First())
                .ToList();

            return finalReferences;
        }

        private static void AddCoreAssemblies(List<MetadataReference> references, HashSet<string> processedPaths)
        {
            // Core .NET assemblies
            var coreTypes = new[]
            {
                typeof(object),                                                          // System.Private.CoreLib
                typeof(System.ComponentModel.TypeConverter),                            // System.ComponentModel.TypeConverter
                typeof(System.Linq.Enumerable),                                         // System.Linq
                typeof(System.Linq.Expressions.Expression),                             // System.Linq.Expressions
                typeof(System.Collections.Generic.List<>),                              // System.Collections
                typeof(System.Text.StringBuilder),                                      // System.Runtime
                typeof(System.Text.RegularExpressions.Regex),                          // System.Text.RegularExpressions
                typeof(System.Text.Encodings.Web.JavaScriptEncoder),                   // System.Text.Encodings.Web
                typeof(System.Text.Json.JsonSerializer),                               // System.Text.Json
                typeof(System.Net.Http.HttpClient),                                     // System.Net.Http
                typeof(System.IO.File),                                                 // System.IO.FileSystem
                typeof(System.Runtime.CompilerServices.RuntimeHelpers),                // System.Runtime
                typeof(System.Diagnostics.Debug),                                       // System.Diagnostics
                typeof(Microsoft.CSharp.RuntimeBinder.RuntimeBinderException),         // Microsoft.CSharp
            };

            foreach (var type in coreTypes)
            {
                try
                {
                    var assembly = type.Assembly;
                    AddAssemblyReference(assembly, references, processedPaths);
                }
                catch { }
            }

            // Try to load netstandard
            try
            {
                var netstandardAsm = Assembly.Load("netstandard, Version=2.1.0.0");
                AddAssemblyReference(netstandardAsm, references, processedPaths);
            }
            catch { }

            // Add current executing, entry, and calling assemblies
            var contextAssemblies = new[]
            {
                Assembly.GetExecutingAssembly(),
                Assembly.GetEntryAssembly(),
                Assembly.GetCallingAssembly()
            };

            foreach (var assembly in contextAssemblies)
            {
                if (assembly != null)
                {
                    AddAssemblyReference(assembly, references, processedPaths);
                }
            }
        }

        private static void ProcessAssemblyRecursive(Assembly assembly, List<MetadataReference> references, 
            HashSet<string> processedPaths, int depth, int maxDepth)
        {
            if (assembly == null || depth > maxDepth) return;
            if (assembly.IsDynamic) return;

            // Add the assembly itself
            if (!AddAssemblyReference(assembly, references, processedPaths))
                return; // Already processed

            // Process referenced assemblies
            try
            {
                var referencedAssemblies = assembly.GetReferencedAssemblies();
                foreach (var refAsmName in referencedAssemblies)
                {
                    try
                    {
                        Assembly? refAsm = null;
                        
                        // Try to load from name
                        try
                        {
                            refAsm = Assembly.Load(refAsmName);
                        }
                        catch
                        {
                            // Try to find it next to the parent assembly
                            var parentDir = Path.GetDirectoryName(assembly.Location);
                            if (!string.IsNullOrWhiteSpace(parentDir))
                            {
                                var refPath = Path.Combine(parentDir, refAsmName.Name + ".dll");
                                if (File.Exists(refPath))
                                {
                                    try
                                    {
                                        refAsm = Assembly.LoadFrom(refPath);
                                    }
                                    catch { }
                                }
                            }
                        }

                        if (refAsm != null && !refAsm.IsDynamic)
                        {
                            ProcessAssemblyRecursive(refAsm, references, processedPaths, depth + 1, maxDepth);
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }

        private static bool AddAssemblyReference(Assembly assembly, List<MetadataReference> references, 
            HashSet<string> processedPaths)
        {
            if (assembly == null || assembly.IsDynamic) return false;

            try
            {
                var location = assembly.Location;
                if (string.IsNullOrWhiteSpace(location) || !File.Exists(location))
                    return false;

                if (processedPaths.Contains(location))
                    return false;

                // Skip native DLLs
                var fileName = Path.GetFileNameWithoutExtension(location);
                if (IsNativeDll(fileName))
                    return false;

                processedPaths.Add(location);

                var metadataRef = referenceCache.GetOrAdd(location, path =>
                {
                    try
                    {
                        return MetadataReference.CreateFromFile(path);
                    }
                    catch
                    {
                        return null!;
                    }
                });

                if (metadataRef != null)
                {
                    references.Add(metadataRef);
                    return true;
                }
            }
            catch { }

            return false;
        }

        private static void ScanFileSystemForAssemblies(List<MetadataReference> references, 
            HashSet<string> processedPaths)
        {
            var searchDirectories = new List<string>();

            // 1. Directory of currently executing assembly
            var executingDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (!string.IsNullOrWhiteSpace(executingDir) && Directory.Exists(executingDir))
                searchDirectories.Add(executingDir);

            // 2. Directory of entry assembly
            var entryAsm = Assembly.GetEntryAssembly();
            if (entryAsm != null)
            {
                var entryDir = Path.GetDirectoryName(entryAsm.Location);
                if (!string.IsNullOrWhiteSpace(entryDir) && Directory.Exists(entryDir))
                    searchDirectories.Add(entryDir);
            }

            // 3. Custom references path
            if (Directory.Exists(invokeOptions.ReferencesPath))
                searchDirectories.Add(invokeOptions.ReferencesPath);

            // 4. Runtime directory
            var runtimeDir = Path.GetDirectoryName(typeof(object).Assembly.Location);
            if (!string.IsNullOrWhiteSpace(runtimeDir) && Directory.Exists(runtimeDir))
                searchDirectories.Add(runtimeDir);

            // Scan each directory
            foreach (var dir in searchDirectories.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                if (!Directory.Exists(dir)) continue;

                try
                {
                    var searchOption = (dir == invokeOptions.ReferencesPath) 
                        ? SearchOption.AllDirectories 
                        : SearchOption.TopDirectoryOnly;

                    foreach (var dllPath in Directory.GetFiles(dir, "*.dll", searchOption))
                    {
                        if (processedPaths.Contains(dllPath)) continue;
                        
                        var fileName = Path.GetFileNameWithoutExtension(dllPath);
                        if (IsNativeDll(fileName)) continue;

                        try
                        {
                            // Verify it's a valid managed assembly
                            var asmName = AssemblyName.GetAssemblyName(dllPath);
                            
                            processedPaths.Add(dllPath);
                            
                            var metadataRef = referenceCache.GetOrAdd(dllPath, path =>
                            {
                                try
                                {
                                    return MetadataReference.CreateFromFile(path);
                                }
                                catch
                                {
                                    return null!;
                                }
                            });

                            if (metadataRef != null)
                            {
                                references.Add(metadataRef);
                            }
                        }
                        catch { }
                    }
                }
                catch { }
            }
        }

        private static void EnsureAppEndAssembliesIncluded(List<MetadataReference> references, 
            HashSet<string> processedPaths)
        {
            var searchDirectories = new List<string>();

            // Directory where AppEndDynaCode is located
            var currentDir = Path.GetDirectoryName(typeof(DynaCodeCore).Assembly.Location);
            if (!string.IsNullOrWhiteSpace(currentDir) && Directory.Exists(currentDir))
                searchDirectories.Add(currentDir);

            // Entry assembly directory
            var entryAsm = Assembly.GetEntryAssembly();
            if (entryAsm != null)
            {
                var entryDir = Path.GetDirectoryName(entryAsm.Location);
                if (!string.IsNullOrWhiteSpace(entryDir) && Directory.Exists(entryDir))
                    searchDirectories.Add(entryDir);
            }

            // References path
            if (Directory.Exists(invokeOptions.ReferencesPath))
                searchDirectories.Add(invokeOptions.ReferencesPath);

            // Find all AppEnd assemblies
            foreach (var dir in searchDirectories.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                if (!Directory.Exists(dir)) continue;

                try
                {
                    var searchOption = (dir == invokeOptions.ReferencesPath) 
                        ? SearchOption.AllDirectories 
                        : SearchOption.TopDirectoryOnly;

                    foreach (var appEndDll in Directory.GetFiles(dir, "AppEnd*.dll", searchOption))
                    {
                        if (processedPaths.Contains(appEndDll)) continue;

                        try
                        {
                            var asmName = AssemblyName.GetAssemblyName(appEndDll);
                            processedPaths.Add(appEndDll);

                            var metadataRef = referenceCache.GetOrAdd(appEndDll, path =>
                            {
                                try
                                {
                                    return MetadataReference.CreateFromFile(path);
                                }
                                catch
                                {
                                    return null!;
                                }
                            });

                            if (metadataRef != null)
                            {
                                references.Add(metadataRef);
                            }
                        }
                        catch { }
                    }
                }
                catch { }
            }
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
            
            if (nativeDlls.Contains(fileName))
                return true;
            
            if (fileName.Contains("Native", StringComparison.OrdinalIgnoreCase))
                return true;
            
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
