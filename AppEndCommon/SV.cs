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

        public static List<string> AuditingFields => ["CreatedBy", "CreatedOn", "StateBy", "StateOn"];
        public static List<string> StateFields => ["StateBy", "StateOn"];
        public static string StateByField => "StateBy";
        public static string StateOnField => "StateOn";
        public static List<string> CreatedFields => ["CreatedBy", "CreatedOn"];
		public static string CreatedByField => "CreatedBy";
		public static string CreatedOnField => "CreatedOn";
		public static string ReadByKey => "ReadByKey";

	}
}
