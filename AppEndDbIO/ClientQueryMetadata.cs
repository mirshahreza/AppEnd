namespace AppEndDbIO
{
    public class ClientQueryMetadata(string name, string objectTyme)
	{
		public List<DbColumn> ParentObjectColumns { set; get; } = [];
		public string? ParentObjectName { set; get; }
		public string? ParentObjectType { set; get; }

		public string Name { get; set; } = name;
		public string Type { get; set; } = objectTyme;
		public List<string> QueryColumns { set; get; } = [];

		public List<DbColumn> FastSearchColumns { set; get; } = [];
		public List<DbColumn> ExpandableSearchColumns { set; get; } = [];

		public List<string> OptionalQueries { set; get; } = [];
	}
}
