using System.Reflection;
using System.Collections.Concurrent;

namespace AppEndCommon
{
	public static class ExtensionsForTypeAndAssembly
    {
        private static readonly ConcurrentDictionary<string, Type[]> _typesCache = new();
        private static readonly ConcurrentDictionary<string, MethodInfo[]> _methodsCache = new();

        public static Type[] GetTypesReal(this Assembly asm)
        {
            string key = asm.FullName ?? asm.GetName().Name ?? "unknown";
            if (_typesCache.TryGetValue(key, out var cached)) return cached;
            var types = asm.GetTypes().Where(i => !i.Name.ContainsIgnoreCase("EmbeddedAttribute") && !i.Name.ContainsIgnoreCase("RefSafetyRulesAttribute")).ToArray();
            _typesCache[key] = types;
            return types;
        }
        public static MethodInfo[] GetMethodsReal(this Type type)
        {
            string key = type.FullName ?? type.Name;
            if (_methodsCache.TryGetValue(key, out var cached)) return cached;
            var methods = type.GetMethods().Where(m => !m.Name.Equals("GetType") && !m.Name.Equals("ToString") && !m.Name.Equals("Equals") && !m.Name.Equals("GetHashCode")).ToArray();
            _methodsCache[key] = methods;
            return methods;
        }
    }
}