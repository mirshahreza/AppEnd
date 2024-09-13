using AppEndCommon;
using AppEndDbIO;
using AppEndDynaCode;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace AppEndServer
{
	public static class DynaCodeServices
	{
		public static bool CreateController(string namespaceName, string className, bool addSampleMthod)
		{
			AppEndClass appEndClass = new(className, namespaceName);
			if (addSampleMthod)
			{
				appEndClass.EmptyMethods.Add("SampleMthod");
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
