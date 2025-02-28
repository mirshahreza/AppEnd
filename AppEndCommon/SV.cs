﻿using Microsoft.Extensions.Caching.Memory;

namespace AppEndCommon
{
    public static class SV
    {
        public static string NT => "\t";
        public static string NL => Environment.NewLine;


		private static IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions() { TrackStatistics = true });
		public static IMemoryCache SharedMemoryCache
		{
			get
			{
				return memoryCache;
			}
			set
			{
				memoryCache = value;
			}
		}
	}
}
