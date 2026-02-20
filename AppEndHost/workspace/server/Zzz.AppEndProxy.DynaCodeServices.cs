namespace Zzz
{
    public static partial class AppEndProxy
    {
		#region DynaCodeServices
		public static object? GetMethodSettings(string NamespaceName, string ClassName, string MethodName)
		{
			return DynaCodeServices.GetMethodSettings(NamespaceName, ClassName, MethodName);
		}
		public static object? WriteMethodSettings(string NamespaceName, string ClassName, string MethodName, JsonElement NewMethodSettings)
		{
			return DynaCodeServices.WriteMethodSettings(NamespaceName, ClassName, MethodName, NewMethodSettings);
		}
		public static object? GetDynaClasses()
		{
			return DynaCodeServices.GetDynaClasses();
		}
		public static object? GetRpcMethodCatalog()
		{
			return DynaCodeServices.GetRpcMethodCatalog();
		}
		public static object? CreateController(string NamespaceName, string ClassName, bool AddSampleMthod)
		{
			DynaCodeServices.CreateController(NamespaceName, ClassName, AddSampleMthod);
			return true;
		}
		public static object? CreateMethod(string NamespaceName, string ClassName, string MethodName)
		{
			DynaCodeServices.CreateMethod(NamespaceName, ClassName, MethodName);
			return true;
		}
		public static object? RemoveMethod(string NamespaceName, string ClassName, string MethodName)
		{
			DynaCodeServices.RemoveMethod(NamespaceName, ClassName, MethodName);
			return true;
		}
		public static object? RemoveClass(string NamespaceName, string ClassName)
		{
			DynaCodeServices.RemoveClass(NamespaceName, ClassName);
			return true;
		}
        #endregion
    }
}
