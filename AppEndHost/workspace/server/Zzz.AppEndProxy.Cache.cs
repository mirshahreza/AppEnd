namespace Zzz
{
    public static partial class AppEndProxy
    {
        #region CacheServices
        public static object? RemoveAllCacheItems()
        {
            CacheServices.RemoveAllCacheItems();
            return true;
        }

        public static object? GetCacheItems(string LikeStr)
        {
            return CacheServices.GetCacheItems(LikeStr);
        }

        public static object? GetCacheItem(string Key)
        {
            return CacheServices.GetCacheItem(Key);
        }

        public static object? RemoveCacheItem(string Key)
        {
            return CacheServices.RemoveCacheItem(Key);
        }
        #endregion
    }
}
