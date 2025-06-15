using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.Extensions.Caching.Memory;

namespace AppEndCommon
{
	public static class ExtensionsForMemory
	{
		public static ICollection GetKeys(this IMemoryCache memoryCache)
		{
			if (memoryCache is MemoryCache memCache)
			{
				var field = typeof(MemoryCache).GetField("_entries", BindingFlags.NonPublic | BindingFlags.Instance);
				if (field != null)
				{
					var entries = field.GetValue(memCache) as IDictionary;
					return entries?.Keys ?? new List<string>();
				}
			}
			return new List<string>();	
		}

		public static List<string> GetKeysStartsWith(this IMemoryCache memoryCache, string searchPhrase)
		{
			List<string> res = [];
			foreach(var key in memoryCache.GetKeys())
			{
				if (searchPhrase.IsNullOrEmpty() || key.ToStringEmpty().ContainsIgnoreCase(searchPhrase)) res.Add(key.ToStringEmpty());
			}
			return res;
		}

		public static void TryRemove(this IMemoryCache memoryCache, string key)
		{
			if (memoryCache.Get(key) is not null) memoryCache.Remove(key);
		}

	}
}