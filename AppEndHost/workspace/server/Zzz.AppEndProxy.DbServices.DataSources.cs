namespace Zzz
{
    public static partial class AppEndProxy
    {
		#region DbServices
		public static object? GetDataSources()
		{
			return DbServices.GetDataSources();
		}
		public static object? GetDataSourcesWithCnn()
		{
			return DbServices.GetDataSourcesWithCnn();
		}
		public static object? AddOrAlterDbServer(JsonElement DataSourceInfo)
		{
			return DbServices.AddOrAlterDbServer(DataSourceInfo);
		}
		public static object? RemoveDbServer(string DbServerName)
		{
			return DbServices.RemoveDbServer(DbServerName);
		}
		public static object? TestDbConnection(JsonElement ServerInfo)
		{
			return DbServices.TestDbConnection(ServerInfo);
		}
		#endregion
    }
}
