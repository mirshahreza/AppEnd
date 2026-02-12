namespace DefaultRepo
{
	public static class BaseUsersRoles
	{

		public static object? ReadList(JsonElement ClientQueryJE, AppEndUser? Actor)
		{
			return AppEndDbIO.ClientQuery.GetInstanceByQueryJson(ClientQueryJE, Actor?.ContextInfo).Exec();
		}

		public static object? Create(JsonElement ClientQueryJE, AppEndUser? Actor)
		{
			return AppEndDbIO.ClientQuery.GetInstanceByQueryJson(ClientQueryJE, Actor?.ContextInfo).Exec();
		}

		public static object? ReadByKey(JsonElement ClientQueryJE, AppEndUser? Actor)
		{
			return AppEndDbIO.ClientQuery.GetInstanceByQueryJson(ClientQueryJE, Actor?.ContextInfo).Exec();
		}

		public static object? UpdateByKey(JsonElement ClientQueryJE, AppEndUser? Actor)
		{
			return AppEndDbIO.ClientQuery.GetInstanceByQueryJson(ClientQueryJE, Actor?.ContextInfo).Exec();
		}

		public static object? Delete(JsonElement ClientQueryJE, AppEndUser? Actor)
		{
			return AppEndDbIO.ClientQuery.GetInstanceByQueryJson(ClientQueryJE, Actor?.ContextInfo).Exec();
		}

		public static object? DeleteByKey(JsonElement ClientQueryJE, AppEndUser? Actor)
		{
			return AppEndDbIO.ClientQuery.GetInstanceByQueryJson(ClientQueryJE, Actor?.ContextInfo).Exec();
		}

	}
}
