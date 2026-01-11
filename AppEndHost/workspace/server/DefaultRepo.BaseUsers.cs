using AppEndCommon;
using AppEndDbIO;
using AppEndDynaCode;
using AppEndServer;
using Newtonsoft.Json.Linq;
using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Linq;

namespace DefaultRepo
{
	public static class BaseUsers
	{
		public static object? ReadList(JsonElement ClientQueryJE, AppEndUser? Actor)
		{
            return AppEndDbIO.ClientQuery.GetInstanceByQueryJson(ClientQueryJE, Actor?.ContextInfo).Exec();
		}
		public static object? Create(JsonElement ClientQueryJE, AppEndUser? Actor)
		{
			var uId = AppEndDbIO.ClientQuery.GetInstanceByQueryJson(ClientQueryJE, Actor?.ContextInfo).Exec();

            ClientQuery? cq = ExtensionsForJson.TryDeserializeTo<ClientQuery>(ClientQueryJE);
			var memNode = cq?.Params?.FirstOrDefault(i => i.Name == "MemberId");
			if (memNode != null) {
				string? memId = memNode?.Value?.ToString();
				if (memId != null) 
				{
                    AppEndDbIO.DbIO dbIO = AppEndDbIO.DbIO.Instance(DbConf.FromSettings(AppEndSettings.DefaultDbConfName));
                    dbIO.ToNoneQuery($"UPDATE BasePersons SET UserId={uId} WHERE Id={memId}");
                }
            }

			return uId;
		}
		public static object? ReadByKey(JsonElement ClientQueryJE, AppEndUser? Actor)
		{
			return AppEndDbIO.ClientQuery.GetInstanceByQueryJson(ClientQueryJE, Actor?.ContextInfo).Exec();
		}
		public static object? UpdateByKey(JsonElement ClientQueryJE, AppEndUser? Actor)
		{
			return AppEndDbIO.ClientQuery.GetInstanceByQueryJson(ClientQueryJE, Actor?.ContextInfo).Exec();
		}
		public static object? DeleteByKey(JsonElement ClientQueryJE, AppEndUser? Actor)
		{
			return AppEndDbIO.ClientQuery.GetInstanceByQueryJson(ClientQueryJE, Actor?.ContextInfo).Exec();
		}
		public static object? IsActiveUpdate(JsonElement ClientQueryJE, AppEndUser? Actor)
		{
			return AppEndDbIO.ClientQuery.GetInstanceByQueryJson(ClientQueryJE, Actor?.ContextInfo).Exec();
		}
		public static object? LoginLockedUpdate(JsonElement ClientQueryJE, AppEndUser? Actor)
		{
			return AppEndDbIO.ClientQuery.GetInstanceByQueryJson(ClientQueryJE, Actor?.ContextInfo).Exec();
		}
	}
}
