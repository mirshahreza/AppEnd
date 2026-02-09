using System.Collections.Concurrent;
using ZiggyCreatures.Caching.Fusion;

namespace AppEndCommon
{
	public static class AppEndCache
	{
		private static readonly ConcurrentDictionary<string, bool> _keys = new();
		private static IFusionCache _cache = CreateCache();
		private static long _totalHits;
		private static long _totalMisses;

		private static IFusionCache CreateCache()
		{
			var cache = new FusionCache(new FusionCacheOptions());
			cache.Events.Hit += (s, e) => Interlocked.Increment(ref _totalHits);
			cache.Events.Miss += (s, e) => Interlocked.Increment(ref _totalMisses);
			cache.Events.Set += (s, e) => _keys.TryAdd(e.Key, true);
			cache.Events.Remove += (s, e) => _keys.TryRemove(e.Key, out _);
			cache.Events.Memory.Eviction += (s, e) => _keys.TryRemove(e.Key, out _);
			return cache;
		}

		public static void Set<T>(string key, T value, int durationSeconds)
		{
			_cache.Set(key, value, new FusionCacheEntryOptions(TimeSpan.FromSeconds(durationSeconds)));
		}

		public static bool TryGet<T>(string key, out T? value)
		{
			var maybe = _cache.TryGet<T>(key);
			if (maybe.HasValue)
			{
				value = maybe.Value;
				return true;
			}
			value = default;
			return false;
		}

		public static T GetOrSet<T>(string key, Func<T> factory, int durationSeconds)
		{
			return _cache.GetOrSet(key, _ => factory(), new FusionCacheEntryOptions(TimeSpan.FromSeconds(durationSeconds)));
		}

		public static void Remove(string key)
		{
			_cache.Remove(key);
		}

		public static ICollection<string> GetKeys() => _keys.Keys;

		public static List<string> GetKeysContaining(string searchPhrase)
		{
			if (string.IsNullOrEmpty(searchPhrase))
				return [.. _keys.Keys];
			return [.. _keys.Keys.Where(k => k.ContainsIgnoreCase(searchPhrase))];
		}

		public static void Clear()
		{
			var snapshot = _keys.Keys.ToArray();
			foreach (var key in snapshot)
			{
				_cache.Remove(key);
			}
			_keys.Clear();
			Interlocked.Exchange(ref _totalHits, 0);
			Interlocked.Exchange(ref _totalMisses, 0);
		}

		public static AppEndCacheState GetState(string likeStr)
		{
			List<string> keys = GetKeysContaining(likeStr);
			return new AppEndCacheState(_keys.Count, Interlocked.Read(ref _totalMisses), Interlocked.Read(ref _totalHits), keys);
		}

		public static object? GetValue(string key)
		{
			var maybe = _cache.TryGet<object>(key);
			return maybe.HasValue ? maybe.Value : null;
		}
	}

	public class AppEndCacheState(long currentEntryCount, long totalMisses, long totalHits, List<string> cachedKeys)
	{
		public long CurrentEntryCount { set; get; } = currentEntryCount;
		public long CurrentEstimatedSize { set; get; } = 0;
		public long TotalMisses { set; get; } = totalMisses;
		public long TotalHits { set; get; } = totalHits;
		public List<string> CachedKeys { set; get; } = cachedKeys;
	}
}
