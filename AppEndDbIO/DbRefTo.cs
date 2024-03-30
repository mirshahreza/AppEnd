namespace AppEndDbIO
{
    public class DbRefTo(string targetTable, string targetColumn)
	{
		public string TargetTable { set; get; } = targetTable;
		public string TargetColumn { set; get; } = targetColumn;
		public List<DbQueryColumn> Columns { set; get; } = [];
        public DbRefTo? RefTo { set; get; }
	}
}
