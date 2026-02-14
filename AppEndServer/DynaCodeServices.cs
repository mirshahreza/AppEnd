using AppEndCommon;
using AppEndDbIO;
using AppEndDynaCode;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using System.Reflection;

namespace AppEndServer
{
	public static class DynaCodeServices
	{
		public static bool CreateController(string namespaceName, string className, bool addSampleMthod)
		{
			AppEndClass appEndClass = new(className, namespaceName);
			if (addSampleMthod)
			{
				appEndClass.NotMappedMethods.Add("SampleMthod");
			}
			File.WriteAllText($"{AppEndSettings.ServerObjectsPath}/{namespaceName}.{className}.cs", appEndClass.ToCode());
			DynaCode.Refresh();
			return true;
		}
		public static object? GetMethodSettings(string namespaceName, string className, string methodName)
		{
			return DynaCode.ReadMethodSettings($"{namespaceName}.{className}.{methodName}");
		}
		public static object? WriteMethodSettings(string namespaceName, string className, string methodName, JsonElement newMethodSettings)
		{
			MethodSettings? methodSettings = ExtensionsForJson.TryDeserializeTo<MethodSettings>(newMethodSettings, new JsonSerializerOptions() { IncludeFields = true }) ?? throw new AppEndException("MethodSettingsIsNotValid", System.Reflection.MethodBase.GetCurrentMethod())
					.AddParam("MethodSettings", newMethodSettings)
					.GetEx();
			DynaCode.WriteMethodSettings($"{namespaceName}.{className}.{methodName}", methodSettings);
			return true;
		}
		public static List<DynaClass> GetDynaClasses()
		{
			return DynaCode.GetDynaClasses();
		}

		public static object? GetRpcMethodCatalog()
		{
			var list = new List<object>();
			var types = DynaCode.DynaAsm?.GetTypes() ?? Array.Empty<Type>();
			foreach (var type in types)
			{
				if (!type.IsClass || !type.IsPublic) continue;
				if (string.IsNullOrWhiteSpace(type.Namespace)) continue;
				var ns = type.Namespace!.Contains('.') ? type.Namespace!.Split('.')[0] : type.Namespace!;
				if (string.IsNullOrWhiteSpace(ns)) continue;

				var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
					.Where(m => !m.IsSpecialName && !m.Name.StartsWith("get_") && !m.Name.StartsWith("set_"))
					.Select(m => new
					{
						Name = m.Name,
						Signature = ToCSharpSignature(m)
					})
					.OrderBy(m => m.Name)
					.ToList();

				list.Add(new
				{
					Namespace = ns,
					ClassName = type.Name,
					Methods = methods
				});
			}

			return list.OrderBy(x => ((dynamic)x).Namespace + "." + ((dynamic)x).ClassName).ToList();
		}

		private static string ToCSharpSignature(MethodInfo m)
		{
			static string TypeName(Type t)
			{
				if (t == typeof(void)) return "void";
				if (t == typeof(string)) return "string";
				if (t == typeof(bool)) return "bool";
				if (t == typeof(int)) return "int";
				if (t == typeof(long)) return "long";
				if (t == typeof(short)) return "short";
				if (t == typeof(byte)) return "byte";
				if (t == typeof(decimal)) return "decimal";
				if (t == typeof(double)) return "double";
				if (t == typeof(float)) return "float";
				if (t == typeof(object)) return "object";
				if (t == typeof(DateTime)) return "DateTime";
				if (t == typeof(Guid)) return "Guid";

				if (t.IsGenericType)
				{
					var name = t.Name;
					var tick = name.IndexOf('`');
					if (tick > -1) name = name.Substring(0, tick);
					var args = string.Join(", ", t.GetGenericArguments().Select(TypeName));
					return $"{name}<{args}>";
				}
				if (t.IsArray)
				{
					return $"{TypeName(t.GetElementType()!)}[]";
				}
				return t.Name;
			}

			var prms = m.GetParameters()
				.Where(p => p.ParameterType != typeof(AppEndUser))
				.Select(p => $"{TypeName(p.ParameterType)} {p.Name}");

			return $"public static {TypeName(m.ReturnType)} {m.Name}({string.Join(", ", prms)})";
		}

		public static void RemoveMethod(string namespaceName, string className, string methodName)
		{
			DynaCode.RemoveMethod($"{namespaceName}.{className}.{methodName}");
		}
		public static void RemoveClass(string namespaceName, string className)
		{
			string classFilePath = $"{AppEndSettings.ServerObjectsPath}/{namespaceName}.{className}.cs";
			string settingsFilePath = $"{AppEndSettings.ServerObjectsPath}/{namespaceName}.{className}.settings.json";
			if (File.Exists(settingsFilePath)) { File.Delete(settingsFilePath); }
			if (File.Exists(classFilePath)) { File.Delete(classFilePath); }
			DynaCode.Refresh();
		}

		public static void CreateMethod(string namespaceName, string className, string methodName)
		{
			DynaCode.CreateMethod($"{namespaceName}.{className}", methodName);
		}

		public static bool HasAccess(string methodFullPath, AppEndUser? appEndUser)
		{
			return DynaCode.HasAccess(methodFullPath, appEndUser);
		}
	}
}
