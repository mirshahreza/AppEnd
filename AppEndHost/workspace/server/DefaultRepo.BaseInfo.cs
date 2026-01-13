using System.Text.Json;
using AppEndCommon;
using AppEndDynaCode;
using AppEndDbIO;
using AppEndServer;
namespace DefaultRepo
{
	public static class BaseInfo
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
		public static object? MetaInfoUpdate(JsonElement ClientQueryJE, AppEndUser? Actor)
		{
			return AppEndDbIO.ClientQuery.GetInstanceByQueryJson(ClientQueryJE, Actor?.ContextInfo).Exec();
		}
		public static object? UiInfoUpdate(JsonElement ClientQueryJE, AppEndUser? Actor)
		{
			return AppEndDbIO.ClientQuery.GetInstanceByQueryJson(ClientQueryJE, Actor?.ContextInfo).Exec();
		}
		public static object? IsActiveUpdate(JsonElement ClientQueryJE, AppEndUser? Actor)
		{
			return AppEndDbIO.ClientQuery.GetInstanceByQueryJson(ClientQueryJE, Actor?.ContextInfo).Exec();
		}

        public static object? CalculateHID(string ParentId,string ChildDigits)
        {
			string pid = ParentId.ToStringEmpty() == "" ? "NULL" : $"'{ParentId}'";
			if (ChildDigits == "") ChildDigits = "2";
			return DbIO.Instance(DbConf.FromSettings("DefaultRepo")).ToScalar($"EXEC [DBO].[ZzCalculateHID] 'BaseInfo', {pid}, 3, {ChildDigits}, '.'");
        }

    }
}
