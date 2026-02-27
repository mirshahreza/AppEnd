namespace AppEndDbIO
{
    public class DbQueryColumn
    {
        public bool? Hidden { set; get; }
		public string? Name { set; get; }
		public string? Phrase { set; get; }
        public string? As { set; get; }
        public DbRefTo? RefTo { set; get; }
		public ColumnAccessDeny? AccessDeny { set; get; }
        
    }

	public class ColumnAccessDeny
	{
		public string[] DeniedRoles { set; get; } = [];
		public string[] DeniedUsers { set; get; } = [];
	}
}
