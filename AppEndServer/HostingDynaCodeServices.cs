using AppEndCommon;
using AppEndDynaCode;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace AppEndServer
{
	public static class HostingDynaCodeServices
	{
		public static object? GetMethodSettings(string namespaceName, string className, string methodName)
		{
			return DynaCode.ReadMethodSettings($"{namespaceName}.{className}.{methodName}");
		}
		public static object? WriteMethodSettings(string namespaceName, string className, string methodName, JsonElement newMethodSettings)
		{
			MethodSettings? methodSettings = ExtensionsForJson.TryDeserializeTo<MethodSettings>(newMethodSettings, new JsonSerializerOptions() { IncludeFields = true }) ?? throw new AppEndException("MethodSettingsIsNotValid")
					.AddParam("MethodSettings", newMethodSettings)
					.AddParam("Site", $"{System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name}, {System.Reflection.MethodBase.GetCurrentMethod()?.Name}");
			DynaCode.WriteMethodSettings($"{namespaceName}.{className}.{methodName}", methodSettings);
			return true;
		}
		public static List<DynaClass> GetDynaClasses()
		{
			return DynaCode.GetDynaClasses();
		}
	}
}
