using Microsoft.Extensions.Caching.Memory;

namespace AppEndCommon
{
    public static class SV
    {
        public static string NT => "\t";
        public static string NL => Environment.NewLine;
        public static string NL2x => Environment.NewLine + Environment.NewLine;

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

        public static readonly List<string> AuditingFields = ["CreatedBy", "CreatedOn", "StateBy", "StateOn"];
        public static readonly List<string> StateFields = ["StateBy", "StateOn"];
        public static readonly string StateByField = "StateBy";
        public static readonly string StateOnField = "StateOn";
        public static readonly List<string> CreatedFields = ["CreatedBy", "CreatedOn"];
		public static readonly string CreatedByField = "CreatedBy";
		public static readonly string CreatedOnField = "CreatedOn";
		public static readonly string ReadByKey = "ReadByKey";

		public static readonly string[] AsStr = [" AS "];

	}
}
