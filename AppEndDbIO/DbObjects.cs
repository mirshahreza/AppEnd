namespace AppEndDbIO
{
    public class DbObject(string name, DbObjectType dbObjectType)
	{
		public string Name { get; init; } = name;
		public DbObjectType DbObjectType { get; init; } = dbObjectType;
	}

    public class DbTable(string name) : DbObject(name, DbObjectType.Table)
    {
        public List<DbColumnChangeTrackable> Columns { set; get; } = [];
	}

	public class DbTableFunction(string name) : DbObject(name, DbObjectType.TableFunction)
    {
	}

	public class DbScalarFunction(string name) : DbObject(name, DbObjectType.ScalarFunction)
    {
	}

	public class DbView(string name) : DbObject(name, DbObjectType.View)
    {
        public List<DbColumnChangeTrackable> Columns { set; get; } = [];
	}

	public class DbProcedure(string name) : DbObject(name, DbObjectType.Procedure)
    {
	}
}
