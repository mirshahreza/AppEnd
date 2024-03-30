namespace AppEndDbIO
{
    public class DbColumnChangeTrackable : DbColumn
    {
		public DbColumnChangeTrackable(string name) : base(name)
		{
		}

		public string State { set; get; } = "";
        public string InitialName { set; get; } = "";
    }

}
