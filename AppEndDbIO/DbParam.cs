namespace AppEndDbIO
{
    public class DbParam(string name, string dbType)
	{
		public string Name { set; get; } = name;
		public string DbType { set; get; } = dbType;
		public string? Size { set; get; }
        public bool AllowNull { set; get; } = true;
        public string? ValueSharp { set; get; }
        public object? Value { set; get; }
	}
}
