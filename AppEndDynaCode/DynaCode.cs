using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.ComponentModel;
using System.Reflection;
using static System.Text.Json.JsonElement;
using System.Text.Json;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using AppEndCommon;
using AppEndDynaCode;
using System.Runtime.CompilerServices;

namespace AppEndDynaCode
{
	public static class DynaCode
    {
        private static CodeInvokeOptions invokeOptions = new("");

		private static IEnumerable<SyntaxTree>? entierCodeSyntaxes;
        private static IEnumerable<SyntaxTree> EntierCodeSyntaxes
        {
            get
            {
                if (entierCodeSyntaxes is null)
                {
                    List<SourceCode> sourceCodes = GetAllSourceCodes();
                    var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp12);
                    entierCodeSyntaxes = sourceCodes.Select(sourceCode => SyntaxFactory.ParseSyntaxTree(sourceCode.RawCode, options, sourceCode.FilePath));
                }
                return entierCodeSyntaxes;
            }
        }

        private static string[]? scriptFiles;
        public static string[] ScriptFiles
        {
            get
            {
                scriptFiles ??= new DirectoryInfo(invokeOptions.StartPath).GetFilesRecursive("*.cs").ToArray();
                return scriptFiles;
            }
        }

        private static string? asmPath;
        private static string AsmPath
        {
            get
            {
				asmPath ??= $"DynaAsm{Guid.NewGuid().ToString().Replace("-", "")}.dll";
                return asmPath;
            }
        }

        private static Assembly? dynaAsm;
        public static Assembly DynaAsm
        {
            get
            {
                if (dynaAsm == null)
                {
                    if (!File.Exists(AsmPath)) Build();
                    dynaAsm = Assembly.LoadFrom(AsmPath);
                }
                return dynaAsm;
            }
        }

        private static List<CodeMap>? codeMaps;
        public static List<CodeMap> CodeMaps
        {
            get
            {
                codeMaps ??= GenerateSourceCodeMap();
                return codeMaps;
            }
        }

        public static void Init(CodeInvokeOptions? codeInvokeOptions = null)
        {
            if (codeInvokeOptions is not null) invokeOptions = codeInvokeOptions;
            Refresh();
        }
        public static void Refresh()
        {
            string[] oldAsmFiles = Directory.GetFiles(".", "DynaAsm*");
            foreach (string oldAsmFile in oldAsmFiles)
            {
                try
                {
                    File.Delete(oldAsmFile);
                }
                catch { }
            }
            entierCodeSyntaxes = null;
            scriptFiles = null;
            asmPath = null;
            dynaAsm = null;
            codeMaps = null;
            //Assembly asm = DynaAsm;
        }

        public static CodeInvokeResult InvokeByJsonInputs(string methodFullPath, JsonElement? inputParams = null, AppEndUser? dynaUser = null, AppEndBackgroundWorkerQueue? appEndBackgroundWorkerQueue=null, string clientInfo = "", bool ignoreCaching = false)
        {
            MethodInfo methodInfo = GetMethodInfo(methodFullPath);
            return Invoke(methodInfo, ExtractParams(methodInfo, inputParams, dynaUser, appEndBackgroundWorkerQueue), dynaUser, appEndBackgroundWorkerQueue, clientInfo, ignoreCaching);
        }
        public static CodeInvokeResult InvokeByParamsInputs(string methodFullPath, object[]? inputParams = null, AppEndUser? dynaUser = null, AppEndBackgroundWorkerQueue? appEndBackgroundWorkerQueue=null, string clientInfo = "", bool ignoreCaching = false)
        {
            MethodInfo methodInfo = GetMethodInfo(methodFullPath);
            return Invoke(methodInfo, inputParams, dynaUser, appEndBackgroundWorkerQueue, clientInfo, ignoreCaching);
        }

        private static CodeInvokeResult Invoke(MethodInfo methodInfo, object[]? inputParams = null, AppEndUser? dynaUser = null, AppEndBackgroundWorkerQueue? appEndBackgroundWorkerQueue = null, string clientInfo = "", bool ignoreCaching = false)
        {
            string methodFullName = methodInfo.GetFullName();
            string methodFilePath = GetMethodFilePath(methodFullName);
            MethodSettings methodSettings = ReadMethodSettings(methodFullName, methodFilePath);

            if (methodSettings.CachePolicy != null && methodSettings.CachePolicy.CacheLevel == CacheLevel.PerUser && (dynaUser is null || dynaUser.UserName.Trim() == ""))
                throw new AppEndException($"CachePolicy.CacheLevelIsSetToPerUserButTheCurrentUserIsNull", System.Reflection.MethodBase.GetCurrentMethod())
                    .AddParam("MethodFullName", methodFullName)
                    .GetEx();

            CodeInvokeResult codeInvokeResult;
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                CheckAccess(methodInfo, methodSettings, dynaUser);
                string cacheKey = CalculateCacheKey(methodInfo, methodSettings, inputParams, dynaUser);
				if (methodSettings.CachePolicy?.CacheLevel != CacheLevel.None && SV.SharedMemoryCache.TryGetValue(cacheKey, out object? result) && ignoreCaching == false)
				{
					stopwatch.Stop();
					codeInvokeResult = new() { Result = result, FromCache = true, IsSucceeded = true, Duration = stopwatch.ElapsedMilliseconds };
				}
				else
				{
					try
					{
						result = methodInfo.Invoke(null, inputParams);
                        if (methodSettings.CachePolicy?.CacheLevel != CacheLevel.None && methodSettings.CachePolicy is not null)
                        {
                            MemoryCacheEntryOptions cacheEntryOptions = new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(methodSettings.CachePolicy.AbsoluteExpirationSeconds) };
                            SV.SharedMemoryCache.Set(cacheKey, result, cacheEntryOptions);
                        }
						stopwatch.Stop();
						codeInvokeResult = new() { Result = result, IsSucceeded = true, Duration = stopwatch.ElapsedMilliseconds };
					}
					catch (Exception ex)
					{
						stopwatch.Stop();
                        object? oEx = ex.InnerException is null ? ex : ex.InnerException;
                        codeInvokeResult = new() { Result = oEx, IsSucceeded = false, Duration = stopwatch.ElapsedMilliseconds };
					}
				}
			}
            catch (Exception ex)
            {
                stopwatch.Stop();
                Exception exx = ex.InnerException is null ? ex : ex.InnerException;
                codeInvokeResult = new() { Result = exx, IsSucceeded = false, Duration = stopwatch.ElapsedMilliseconds };
            }

            LogMethodInvoke(methodInfo, methodSettings, codeInvokeResult, inputParams, dynaUser, clientInfo);
            return codeInvokeResult;
        }

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
            if(codeMap is null) return "";
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

        private static void CheckAccess(MethodInfo methodInfo, MethodSettings methodSettings, AppEndUser? actor)
        {
            if (actor is null) return;
            if (actor.UserName.EqualsIgnoreCase(invokeOptions.PublicKeyUser)) return;
            if (actor.Roles.ContainsIgnoreCase(invokeOptions.PublicKeyRole)) return;
            if (actor.Roles.ToList().HasIntersect(methodSettings.AccessRules.AllowedRoles)) return;
            if (methodSettings.AccessRules.AllowedRoles.Contains("*")) return;
            if (methodSettings.AccessRules.AllowedUsers.ContainsIgnoreCase(actor.UserName)) return;
            if (methodSettings.AccessRules.AllowedUsers.Contains("*")) return;
            if (invokeOptions.PublicMethods is not null && invokeOptions.PublicMethods.ContainsIgnoreCase(methodInfo.GetFullName())) return;
            throw new AppEndException("AccessDenied", System.Reflection.MethodBase.GetCurrentMethod())
                .AddParam("Method", methodInfo.GetFullName())
                .AddParam("Actor", actor.UserName)
                .GetEx();
        }
        private static void LogMethodInvoke(MethodInfo methodInfo, MethodSettings methodSettings, CodeInvokeResult codeInvokeResult, object[]? inputParams, AppEndUser? dynaUser, string clientInfo = "")
        {
            string logMethod = "";
			if (codeInvokeResult.IsSucceeded == true && !methodSettings.LogPolicy.OnSuccessLogMethod.IsNullOrEmpty())
				logMethod = methodSettings.LogPolicy.OnSuccessLogMethod;
			if (codeInvokeResult.IsSucceeded == false && !methodSettings.LogPolicy.OnErrorLogMethod.IsNullOrEmpty())
				logMethod = methodSettings.LogPolicy.OnErrorLogMethod;

            if (logMethod.IsNullOrEmpty() || methodInfo.GetFullName().EqualsIgnoreCase(logMethod)) return;

            List<object> list = [methodInfo, dynaUser is null ? "" : dynaUser.Id, methodInfo.GetFullName(), clientInfo, codeInvokeResult, inputParams];
            GetMethodInfo(logMethod).Invoke(null, [.. list]);
        }

        public static void DuplicateMethod(string methodFullName, string methodCopyName)
        {
            var parts = MethodPartsNames(methodFullName);
            string classFullName = methodFullName.Replace($".{parts.Item3}", "");
            string methodName = parts.Item3;
            string? filePath = GetMethodFilePath(methodFullName) ?? throw new AppEndException("MethodFullNameDoesNotExist", System.Reflection.MethodBase.GetCurrentMethod())
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
            Refresh();
        }

        public static bool MethodExist(string methodFullName)
        {
            var parts = MethodPartsNames(methodFullName);
            string filePath = TryGetClassFilePath(methodFullName);
            if (filePath.IsNullOrEmpty()) return false;
            string fileBody = File.ReadAllText(filePath);
            fileBody = fileBody.Replace(" ", "");
            return fileBody.Contains($"publicstaticobject?{parts.Item3}(");
        }

        public static void CreateMethod(string methodFullName, string methodName)
        {
            string? filePath = GetClassFilePath(methodFullName);
			string controllerBody = File.ReadAllText(filePath);
            SyntaxTree tree = CSharpSyntaxTree.ParseText(controllerBody);
            string mBody = new AppEndMethod(methodName).MethodImplementation;
            MethodDeclarationSyntax method = tree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().Last();
            string m = method.GetText().ToString();
            TextChange tc = new(method.Span, $"{m.Trim()}{Environment.NewLine}{Environment.NewLine}{mBody}");
            controllerBody = tree.GetText().WithChanges(tc).ToString().RemoveWhitelines();
            File.WriteAllText(filePath, controllerBody);
            Refresh();
        }

        public static void RemoveMethod(string methodFullName)
        {
            var parts = MethodPartsNames(methodFullName);
            string classFullName = methodFullName.Replace($".{parts.Item3}", "");
            string methodName = parts.Item3;
            string? filePath = GetMethodFilePath(methodFullName) ?? throw new AppEndException("MethodFullNameDoesNotExist", System.Reflection.MethodBase.GetCurrentMethod())
                    .AddParam("MethodFullName", methodFullName)
                    .GetEx();
			string controllerBody = File.ReadAllText(filePath);

            SyntaxTree tree = CSharpSyntaxTree.ParseText(controllerBody);
            MethodDeclarationSyntax method = tree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().First(m => m.Identifier.ToString() == methodName);

            TextChange tc = new(method.Span, string.Empty);
            controllerBody = tree.GetText().WithChanges(tc).ToString().RemoveWhitelines();
            File.WriteAllText(filePath, controllerBody);
            Refresh();
        }

        public static void WriteMethodSettings(string methodFullName, MethodSettings methodSettings)
        {
            string methodFilePath = GetMethodFilePath(methodFullName);
            WriteMethodSettings(methodFullName, methodFilePath, methodSettings);
        }
        public static void WriteMethodSettings(string methodFullName, string methodFilePath, MethodSettings methodSettings)
        {
            string settingsFileName = GetSettingsFile(methodFilePath);
            string settingsRaw = File.Exists(settingsFileName) ? File.ReadAllText(settingsFileName) : "{}";
            JsonNode? jsonNode = JsonNode.Parse(settingsRaw) ?? throw new AppEndException("DeserializeError", System.Reflection.MethodBase.GetCurrentMethod())
                    .AddParam("MethodFullName", methodFullName)
                    .AddParam("MethodSettings", methodSettings)
                    .GetEx();
			jsonNode[methodFullName] = JsonNode.Parse(methodSettings.Serialize());
            File.WriteAllText(settingsFileName, jsonNode.ToString());
        }

        public static MethodSettings ReadMethodSettings(string methodFullName)
        {
            string methodFilePath = GetMethodFilePath(methodFullName);
            return ReadMethodSettings(methodFullName, methodFilePath);
        }
        public static MethodSettings ReadMethodSettings(string methodFullName, string methodFilePath)
        {
            string settingsFileName = GetSettingsFile(methodFilePath);
            string settingsRaw = File.Exists(settingsFileName) ? File.ReadAllText(settingsFileName) : "{}";
            try
            {
                var jsonNode = JsonNode.Parse(settingsRaw) ?? throw new AppEndException("DeserializeError", System.Reflection.MethodBase.GetCurrentMethod())
                        .AddParam("MethodFullName", methodFullName)
                        .AddParam("SettingsRaw", settingsRaw)
                        .GetEx();
				if (jsonNode[methodFullName] == null) return new();
                MethodSettings? methodSettings = jsonNode[methodFullName].Deserialize<MethodSettings>(options: new() { IncludeFields = true });
                if (methodSettings is null) return new();
                return methodSettings;
            }
            catch
            {
                throw new AppEndException($"SettingsAreNotValid", System.Reflection.MethodBase.GetCurrentMethod())
                    .AddParam("MethodFullName", methodFullName)
                    .AddParam("MethodFilePath", methodFilePath)
                    .AddParam("Settings", settingsRaw)
                    .GetEx();
            }
        }

        public static List<DynaClass> GetDynaClasses()
        {
			List<DynaClass> dynaClasses = [];
            foreach (var i in DynaAsm.GetTypes())
            {
                if (Utils.IsRealType(i.Name))
                {
                    List<DynaMethod> dynaMethods = [];
                    
                    foreach (var method in i.GetMethods())
						if (Utils.IsRealMethod(method.Name))
							dynaMethods.Add(new(method.Name, ReadMethodSettings($"{i.Namespace}.{i.Name}.{method.Name}")));
					
                    DynaClass dynamicController = new(i.Name, dynaMethods) { Namespace = i.Namespace };
                    dynaClasses.Add(dynamicController);
                }
            }
            return [.. dynaClasses.OrderBy(i => i.Namespace + i.Name)];
        }


        private static string GetSettingsFile(string methodFilePath)
        {
            return methodFilePath.Replace(".cs", "") + ".settings.json";
        }

        public static void RemoveMethodSettings(string methodFullName)
        {
            CodeMap? codeMap = CodeMaps.FirstOrDefault(cm => cm.MethodFullName == methodFullName) ?? throw new AppEndException($"MethodDoesNotExist : [ {methodFullName} ]", System.Reflection.MethodBase.GetCurrentMethod())
                    .AddParam("MethodFullName", methodFullName)
                    .GetEx();
			string settingsFileName = codeMap.FilePath + ".settings.json";
            if (!File.Exists(settingsFileName)) return;
            File.Delete(settingsFileName);
        }

        private static void Build()
        {
            using var peStream = new MemoryStream();

            var compileRefs = GetCompilationReferences();
            var compilerOptions = new CSharpCompilationOptions(outputKind: OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release, assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default);
			CSharpCompilation cSharpCompilation = CSharpCompilation.Create(AsmPath, EntierCodeSyntaxes, compileRefs, compilerOptions);

			var result = cSharpCompilation.Emit(peStream);

            if (!result.Success)
            {
                var failures = result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);
                var error = failures.FirstOrDefault();
                throw new AppEndException($"{error?.Id}: {error?.GetMessage()}", System.Reflection.MethodBase.GetCurrentMethod())
                            .GetEx();
            }

            peStream.Seek(0, SeekOrigin.Begin);
            byte[] dllBytes = peStream.ToArray();

            File.WriteAllBytes(AsmPath, dllBytes);
        }

        public static void ReBuild()
        {
            Refresh();
			Assembly asm = DynaAsm;
		}

        private static List<SourceCode> GetAllSourceCodes()
        {
            List<SourceCode> sourceCodes = [];
            foreach (string f in ScriptFiles) sourceCodes.Add(new(f, File.ReadAllText(f)));
			return sourceCodes;
        }
        private static List<MetadataReference> GetCompilationReferences()
        {
            var references = new List<MetadataReference>();

            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                AddReferencesFor(a, references);
            }

            AddReferencesFor(Assembly.GetExecutingAssembly(), references);
            AddReferencesFor(Assembly.GetEntryAssembly(), references);
            AddReferencesFor(Assembly.GetCallingAssembly(), references);

            AddReferencesFor(typeof(object).Assembly, references);
            AddReferencesFor(typeof(TypeConverter).Assembly, references);
            AddReferencesFor(Assembly.Load("netstandard, Version=2.1.0.0"), references);
            AddReferencesFor(typeof(System.Linq.Expressions.Expression).Assembly, references);
            AddReferencesFor(typeof(System.Text.Encodings.Web.JavaScriptEncoder).Assembly, references);
            AddReferencesFor(typeof(Exception).Assembly, references);
            AddReferencesFor(typeof(AppEndException).Assembly, references);
            AddReferencesFor(typeof(ArgumentNullException).Assembly, references);

            if (Directory.Exists(invokeOptions.ReferencesPath))
            {
                foreach (string f in Directory.GetFiles(invokeOptions.ReferencesPath, "*.dll"))
                {
                    AddReferencesFor(Assembly.LoadFrom(f), references);
                }
            }

            return references;
        }
        private static void AddReferencesFor(Assembly? asm, List<MetadataReference> references)
        {
            if (asm is null || !File.Exists(asm.Location)) return;
            references.Add(MetadataReference.CreateFromFile(asm.Location));
            var rfs = asm.GetReferencedAssemblies();
            foreach(var a in rfs)
            {
                var asmF = Assembly.Load(a);
                if(asmF is null) continue;
                if(File.Exists( asmF.Location))
                {
                    references.Add(MetadataReference.CreateFromFile(asmF.Location));
                }
            }
            //references.AddRange(rfs.Select(a => MetadataReference.CreateFromFile(Assembly.Load(a).Location)));
        }


        private static List<CodeMap> GenerateSourceCodeMap()
        {
            List<CodeMap> codeMaps = [];
            foreach (var st in EntierCodeSyntaxes)
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

        private static object[]? ExtractParams(MethodInfo methodInfo, JsonElement? jsonElement, AppEndUser? actor, AppEndBackgroundWorkerQueue? appEndBackgroundWorkerQueue)
        {
            List<object> methodInputs = [];
            ParameterInfo[] methodParams = methodInfo.GetParameters();
            ObjectEnumerator? objects = jsonElement is null ? null : ((JsonElement)jsonElement).EnumerateObject();

            foreach (var paramInfo in methodParams)
            {
				if (paramInfo.ParameterType == typeof(AppEndUser))
				{
					if (actor is not null) methodInputs.Add(actor);
				}
				else if (paramInfo.ParameterType == typeof(AppEndBackgroundWorkerQueue))
				{
					if (appEndBackgroundWorkerQueue is not null) methodInputs.Add(appEndBackgroundWorkerQueue);
				}
				else
				{
                    if(objects is not null)
                    {
						IEnumerable<JsonProperty> l = ((ObjectEnumerator)objects).Where(i => string.Equals(i.Name, paramInfo.Name));
						if (!l.Any()) throw new AppEndException($"MethodCallMustContainsParameter", System.Reflection.MethodBase.GetCurrentMethod())
								.AddParam("MethodFullName", methodInfo.GetFullName())
								.AddParam("ParameterName", paramInfo.Name.ToStringEmpty())
								.GetEx();
						JsonProperty p = l.First();
						methodInputs.Add(p.Value.ToOrigType(paramInfo));
					}
				}
            }
            return [.. methodInputs];
        }
        private static MethodInfo GetMethodInfo(string methodFullName)
        {
            var parts = MethodPartsNames(methodFullName);
            return  GetMethodInfo(parts.Item1, parts.Item2, parts.Item3);
        }

        public static Tuple<string?, string, string> MethodPartsNames(string methodFullPath)
        {
            if (methodFullPath.Trim() == "") throw new AppEndException("MethodFullPathCanNotBeEmpty", System.Reflection.MethodBase.GetCurrentMethod())
                            .GetEx();
            string[] parts = methodFullPath.Trim().Split('.');
            if (parts.Length < 2 || parts.Length > 3) throw new AppEndException($"MethodMustContainsAtLeast2PartsSeparatedByDot", System.Reflection.MethodBase.GetCurrentMethod())
                    .AddParam("MethodFullPath", methodFullPath)
                    .GetEx();
            return parts.Length == 3 ? new(parts[0], parts[1], parts[2]) : new(null, parts[0], parts[1]);
        }
        private static MethodInfo GetMethodInfo(string? namespaceName, string className, string methodName)
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
		private static Type GetType(string classFullName)
        {
            string tName = classFullName;
            string nsName = "";
            Type? dynamicType;
            if (classFullName.Contains('.'))
            {
                nsName = classFullName.Split('.')[0];
                tName = classFullName.Split(".")[1];
            }
            if (invokeOptions.IsDevelopment || invokeOptions.CompiledIn)
            {
                dynamicType = Assembly.GetEntryAssembly()?.GetTypes().FirstOrDefault(i => i.Name == tName && (nsName == "" || i.Namespace == nsName));
                if (dynamicType == null)
                {
                    dynamicType = DynaAsm?.GetTypes().FirstOrDefault(i => i.Name == tName && (nsName == "" || i.Namespace == nsName));
                }
            }
            else
            {
                dynamicType = DynaAsm?.GetTypes().FirstOrDefault(i => i.Name == tName && (nsName == "" || i.Namespace == nsName));
                if (dynamicType == null)
                {
                    dynamicType = Assembly.GetEntryAssembly()?.GetTypes().FirstOrDefault(i => i.Name == tName && (nsName == "" || i.Namespace == nsName));
                }
            }
            if (dynamicType == null) throw new AppEndException("TypeDoesNotExist", System.Reflection.MethodBase.GetCurrentMethod())
                    .AddParam("ClassFullName", classFullName)
                    .GetEx();
            return dynamicType;
        }
        
        private static string CalculateCacheKey(MethodInfo methodInfo, MethodSettings methodSettings, object[]? inputParams, AppEndUser? dynaUser)
        {
            string paramKey = inputParams is null ? "" : $".{inputParams.ToJsonStringByBuiltIn().GetHashCode()}";
            string levelName = methodSettings.CachePolicy.CacheLevel == CacheLevel.PerUser ? $".{dynaUser?.UserName}" : "";
            return $"{methodInfo.GetFullName()}{levelName}{paramKey}";
        }


        public static Dictionary<string, object> GetAllAllowdAndDeniedActions(AppEndUser? actor)
        {
            if (actor == null) return new Dictionary<string, object> { { "AllowedActions", "".Split(',') } };

			List<DynaClass> dynaClasses = GetDynaClasses();
			List<string> alloweds = [];
			List<string> denieds = [];

			foreach (var dynaC in dynaClasses)
            {
                foreach(DynaMethod dynaM in dynaC.DynaMethods)
                {
					MethodSettings ms = dynaM.MethodSettings;
                    string mFullName = dynaC.Namespace + "." + dynaC.Name + "." + dynaM.Name;
                    if (
						invokeOptions.PublicMethods.ContainsIgnoreCase(mFullName) ||
						invokeOptions.PublicKeyUser.EqualsIgnoreCase(actor.UserName) ||
						actor.Roles.ContainsIgnoreCase(invokeOptions.PublicKeyRole) ||
                        ms.AccessRules.AllowedUsers.ContainsIgnoreCase(actor.UserName) ||
                        ms.AccessRules.AllowedRoles.HasIntersect(actor.Roles)
                        )
                        alloweds.Add(mFullName);

					if (ms.AccessRules.DeniedUsers.ContainsIgnoreCase(actor.UserName))
						denieds.Add(mFullName);
				}
			}

            foreach(string s in denieds) if (alloweds.ContainsIgnoreCase(s)) alloweds.Remove(s);

            return new Dictionary<string, object> { { "AllowedActions", alloweds.ToArray() } };
		}

    }
}