using Microsoft.Extensions.Caching.Memory;

namespace AppEndCommon
{
    public static class SV
    {
        public static string NT => "\t";
        public static string NL => Environment.NewLine;
        public static string NL2x => Environment.NewLine + Environment.NewLine;

		private static readonly IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions() { TrackStatistics = true });
		public static IMemoryCache SharedMemoryCache
		{
			get
			{
				return memoryCache;
			}
		}

	}
}
