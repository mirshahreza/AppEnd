namespace AppEndServer
{
    public class MemoryCacheState(long currentEntryCount, long currentEstimatedSize, long totalMisses, long totalHits, List<string> cachedKeys)
	{
		public long CurrentEntryCount { set; get; } = currentEntryCount;
		public long CurrentEstimatedSize { set; get; } = currentEstimatedSize;
		public long TotalMisses { set; get; } = totalMisses;
		public long TotalHits { set; get; } = totalHits;
		public List<string> CachedKeys { set; get; } = cachedKeys;
	}
}
