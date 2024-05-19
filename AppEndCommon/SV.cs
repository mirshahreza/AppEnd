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

        public static readonly List<string> AuditingFields = ["CreatedBy", "CreatedOn", "UpdatedBy", "UpdatedOn"];
        public static readonly List<string> UpdatedFields = ["UpdatedBy", "UpdatedOn"];
        public static readonly string UpdatedBy = "UpdatedBy";
        public static readonly string UpdatedOn = "UpdatedOn";
        public static readonly List<string> CreatedFields = ["CreatedBy", "CreatedOn"];
		public static readonly string CreatedBy = "CreatedBy";
		public static readonly string CreatedOn = "CreatedOn";

		public static readonly string ViewOrder = "ViewOrder";
		public static readonly string ReadByKey = "ReadByKey";
		public static readonly string Update = "Update";
		public static readonly string AsStr = " AS ";

	}
}
