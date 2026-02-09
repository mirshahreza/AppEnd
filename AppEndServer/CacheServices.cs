using AppEndCommon;

namespace AppEndServer
{
	public static class CacheServices
	{
		public static void RemoveAllCacheItems()
		{
			AppEndCache.Clear();
		}
		public static AppEndCacheState GetCacheItems(string likeStr)
		{
			return AppEndCache.GetState(likeStr);
		}
		public static object? GetCacheItem(string key)
		{
			return AppEndCache.GetValue(key);
		}
		public static bool RemoveCacheItem(string key)
		{
			AppEndCache.Remove(key);
			return true;
		}
		public static void ClearActorCacheEntries(AppEndUser? Actor)
		{
			if (Actor == null) return;
			var userKeys = AppEndCache.GetKeysContaining(AppEndUser.ContextCacheKeyShortName(Actor.Id.ToString()));
			foreach (string key in userKeys)
			{
				AppEndCache.Remove(key);
			}
		}
	}
}
