namespace AppEndServer
{
    public record DbObjectStack(string objectName, string objectType)
	{
		public string ObjectName { get; set; } = objectName;
		public string ObjectType { get; set; } = objectType;
		public bool HasServerObjects { get; set; }
        public List<string> ClientComponents { get; set; } = [];

		public DateTime LastWriteTime { get; set; }

	}
}
