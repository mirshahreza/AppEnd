using System.Reflection;

namespace AppEndCommon
{
	public static class ExtensionsForMethodBase
	{
		public static string GetPlaceInfo(this MethodBase? methodBase)
		{
			if (methodBase == null) return "";
			return $"{methodBase?.DeclaringType?.Name}, {methodBase?.Name}";
		}
	}
}