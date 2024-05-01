using System.Text.Json.Serialization;

namespace AppEndDbIO
{
    public class DbRelation
    {
        public string RelationName { set; get; }
        public string RelationTable { set; get; }
        public string RelationPkColumn { set; get; }
        public string RelationFkColumn { set; get; }

		public RelationType RelationType { set; get; }
		public string? LinkingTargetTable { set; get; }
		public string? LinkingColumnInManyToMany { set; get; }
        

        public string? CreateQuery { set; get; }
        public string? ReadByKeyQuery { set; get; }
        public string? ReadListQuery { set; get; }
        public string? ChangeStateByKeyQuery { set; get; }
        public string? DeleteByKeyQuery { set; get; }
        public string? DeleteQuery { set; get; }

        public bool? IsFileCentric { set; get; }

		public string? MinN { set; get; }
		public string? MaxN { set; get; }

        public RelationUiWidget? RelationUiWidget { set; get; }

        public DbRelation(string relationTable, string relationPkColumn, string relationFkColumn)
        {
            RelationTable = relationTable;
            RelationPkColumn = relationPkColumn;
            RelationFkColumn = relationFkColumn;
            RelationName = $"To_{relationTable}_On_{relationFkColumn}";
        }

        [JsonConstructor]
        public DbRelation() { }
    }
}
