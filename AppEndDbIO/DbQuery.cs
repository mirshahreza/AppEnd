using System.Data.Common;

namespace AppEndDbIO
{
    public class DbQuery(string name, QueryType type)
	{
		public string Name { set; get; } = name;
		public QueryType Type { set; get; } = type;
		public string? BaseObjectName { set; get; }
        public List<DbQueryColumn>? Columns { set; get; }
        public List<DbParam>? Params { set; get; }
        public Where? Where { set; get; }
        public int? PaginationMaxSize { set; get; }
        public List<DbAggregation>? Aggregations { set; get; }
        public List<string>? Relations { set; get; }
        public string? HistoryTable { set; get; }

        public List<DbParameter> FinalDbParameters = [];
	}
}
