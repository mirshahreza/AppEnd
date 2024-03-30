namespace AppEndDbIO
{
    public class DbFk(string fkName, string targetTable, string targetColumn)
	{
		public string FkName { set; get; } = fkName;
		public string TargetTable { set; get; } = targetTable;
		public string TargetColumn { set; get; } = targetColumn;
		public bool EnforceRelation { set; get; } = false;
        public ClientRequest? Lookup { set; get; }
	}
}
