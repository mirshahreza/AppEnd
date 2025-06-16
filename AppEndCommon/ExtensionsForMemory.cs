using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;

namespace AppEndCommon
{
	public static class ExtensionsForMemory
	{
		// A simple (thread-safe) way to track keys
		private static readonly ConcurrentDictionary<string, bool> _cacheKeys = new ConcurrentDictionary<string, bool>();

		public static void ToCache<TItem>(this IMemoryCache memoryCache, object key, TItem value, MemoryCacheEntryOptions options)
		{
			memoryCache.Set(key, value, options);
			_cacheKeys.TryAdd(key.ToStringEmpty(), true); // Add key to our tracking
		}

		public static void Remove(this IMemoryCache memoryCache, object key)
		{
			memoryCache.Remove(key);
			_cacheKeys.TryRemove(key.ToStringEmpty(), out _); // Remove key from our tracking
		}

		public static ICollection<string> GetKeys(this IMemoryCache memoryCache)
		{
			return _cacheKeys.Keys;
		}

		public static List<string> GetKeysStartsWith(this IMemoryCache memoryCache, string searchPhrase)
		{
			List<string> res = [];
			foreach (var key in GetKeys(memoryCache)) // Use our custom GetKeys
			{
				if (searchPhrase.IsNullOrEmpty() || key.ContainsIgnoreCase(searchPhrase))
				{
					res.Add(key);
				}
			}
			return res;
		}

		public static void TryRemove(this IMemoryCache memoryCache, string key)
		{
			if (memoryCache.Get(key) is not null) memoryCache.Remove(key);
		}
	}
}