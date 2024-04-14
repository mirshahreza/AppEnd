using AppEndCommon;
using AppEndDynaCode;
using Microsoft.Extensions.Caching.Memory;
using System.Collections;

namespace AppEndServer
{
	public static class HostingActorServices
	{
		public static AppEndUser GetActor(this HttpContext httpContext)
		{
			string token = httpContext.Request.Headers["token"].ToStringEmpty();
			if (token == "") return GetNobodyUser();
			return GetActor(httpContext.Request.Headers["token"].ToString());
		}
		public static AppEndUser GetActor(string token)
		{
			AppEndUser? user = ExtensionsForJson.TryDeserializeTo<AppEndUser>(token.Decode(AppEndSettings.Secret));
			if (user == null) return GetNobodyUser();
			string cacheKey = user.ContextCacheKey();
			if (SV.SharedMemoryCache.TryGetValue(cacheKey, out object? userContext))
			{
				userContext ??= new Hashtable();
				user.ContextInfo = (Hashtable)userContext;
				user.Roles = user.ContextInfo["Roles"] == null ? [] : [.. (List<string>?)user.ContextInfo["Roles"]];
			}
			else
			{
				CodeInvokeResult codeInvokeResult = DynaCode.InvokeByJsonInputs("Zzz.AppEndProxy.CreateUserServerContext", null, user);
				userContext = codeInvokeResult.Result;
				Hashtable hashtable = userContext is not null ? (Hashtable)userContext : [];
				user.ContextInfo = hashtable;
				user.ContextInfo.Add("UserName", user.UserName);
				user.ContextInfo.Add("UserId", user.Id);
				SV.SharedMemoryCache.Set(cacheKey, hashtable, HostingCacheServices.GetCacheOptions(600));
			}

			return user;
		}
		public static AppEndUser GetNobodyUser()
		{
			return new AppEndUser() { UserName = "nobody" };
		}
		public static string CreateToken(this AppEndUser dynaUser)
		{
			return dynaUser.Encode(AppEndSettings.Secret);
		}

		public static string GetClientIp(this HttpRequest httpRequest)
		{
			string ipAdd = httpRequest.Headers["X-Forwarded-For"].ToStringEmpty();
			ipAdd = ipAdd.IsNullOrEmpty() ? httpRequest.Headers["REMOTE_ADDR"].ToStringEmpty().FixNullOrEmpty("0.0.0.0") : ipAdd;
			ipAdd = ipAdd.Contains(',') ? ipAdd.Split(',')[0] : ipAdd;
			return ipAdd.FixNullOrEmpty("0.0.0.0");
		}

		public static string GetClientAgent(this HttpRequest httpRequest)
		{
			return httpRequest.Headers["User-Agent"].ToString();
		}

	}
}
