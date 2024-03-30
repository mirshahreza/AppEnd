using System.Reflection;

namespace AppEndCommon
{
	public static class ExtensionsForTypeAndAssembly
    {
        public static Type[] GetTypesReal(this Assembly asm)
        {
            return asm.GetTypes().Where(i => !i.Name.ContainsIgnoreCase("EmbeddedAttribute") && !i.Name.ContainsIgnoreCase("RefSafetyRulesAttribute")).ToArray();
        }
        public static MethodInfo[] GetMethodsReal(this Type type)
        {
            return type.GetMethods().Where(m => !m.Name.Equals("GetType") && !m.Name.Equals("ToString") && !m.Name.Equals("Equals") && !m.Name.Equals("GetHashCode")).ToArray();
        }
    }
}