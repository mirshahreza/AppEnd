using AppEndCommon;
using Microsoft.Extensions.Caching.Memory;
using System.Collections;

namespace AppEndServer
{
	public static class CacheServices
	{
		public static void RemoveAllCacheItems()
		{
			SV.SharedMemoryCache = new MemoryCache(new MemoryCacheOptions() { TrackStatistics = true });
		}
		public static MemoryCacheState? GetCacheItems(string likeStr)
		{
			MemoryCacheStatistics? ms = SV.SharedMemoryCache.GetCurrentStatistics();
			List<string> keys = [];
			foreach (var key in SV.SharedMemoryCache.GetKeys())
			{
				if (likeStr == "" || key.ToStringEmpty().ContainsIgnoreCase(likeStr)) keys.Add(key.ToStringEmpty());
			}
			if (ms == null) return new MemoryCacheState(0, 0, 0, 0, keys);
			return new(ms.CurrentEntryCount, ms.CurrentEstimatedSize == null ? 0 : (long)ms.CurrentEstimatedSize, ms.TotalMisses, ms.TotalHits, keys);
		}
		public static object? GetCacheItem(string key)
		{
			SV.SharedMemoryCache.TryGetValue(key, out var value);
			return value;
		}
		public static bool RemoveCacheItem(string key)
		{
			SV.SharedMemoryCache.TryRemove(key);
			return true;
		}
		public static void ClearActorCacheEntries(AppEndUser? Actor)
		{
			if (Actor == null) return;
			ICollection userKeys = SV.SharedMemoryCache.GetKeysStartsWith(AppEndUser.ContextCacheKeyShortName(Actor.Id.ToString()));
			foreach (string key in userKeys)
			{
				SV.SharedMemoryCache.TryRemove(key);
			}
		}
		public static MemoryCacheEntryOptions GetCacheOptions(int seconds)
		{
			return new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(seconds) };
		}

	}

	public class MemoryCacheState(long currentEntryCount, long currentEstimatedSize, long totalMisses, long totalHits, List<string> cachedKeys)
	{
		public long CurrentEntryCount { set; get; } = currentEntryCount;
		public long CurrentEstimatedSize { set; get; } = currentEstimatedSize;
		public long TotalMisses { set; get; } = totalMisses;
		public long TotalHits { set; get; } = totalHits;
		public List<string> CachedKeys { set; get; } = cachedKeys;
	}

	


}
