using AppEndCommon;
using AppEndDynaCode;
using System.Collections;

namespace AppEndServer
{
	public static class ActorServices
	{
		public const string CookieAccessToken = "access_token";
		public const string CookieRefreshToken = "refresh_token";

		public static AppEndUser GetActor(this HttpContext httpContext)
		{
			string token = (httpContext.Request.Cookies[CookieAccessToken] ?? "").ToStringEmpty();
			if (token == "") token = httpContext.Request.Headers["token"].ToStringEmpty();
			if (token == "") return GetNobodyUser();
			return GetActor(token);
		}

		/// <summary>
		/// Gets the actor without throwing. Returns (actor, tokenInvalid) where tokenInvalid is true
		/// when an access token was present but failed to decode (expired or invalid).
		/// </summary>
		public static (AppEndUser actor, bool tokenInvalid) TryGetActor(this HttpContext httpContext)
		{
			string token = (httpContext.Request.Cookies[CookieAccessToken] ?? "").ToStringEmpty();
			if (token == "") token = httpContext.Request.Headers["token"].ToStringEmpty();
			if (token == "") return (GetNobodyUser(), false);
			try
			{
				return (GetActor(token), false);
			}
			catch
			{
				return (GetNobodyUser(), true);
			}
		}
		public static AppEndUser GetActor(string token)
		{
			AppEndUser? user = ExtensionsForJson.TryDeserializeTo<AppEndUser>(token.Decode(AppEndSettings.Secret));
			if (user == null) return GetNobodyUser();
			string cacheKey = user.ContextCacheKey();
			if (AppEndCache.TryGet<Hashtable>(cacheKey, out var userContext))
			{
				userContext ??= new Hashtable();
				user.ContextInfo = userContext;
				user.Roles = user.ContextInfo["Roles"] == null ? [] : [.. (List<string>?)user.ContextInfo["Roles"]];
			}
			else
			{
				CodeInvokeResult codeInvokeResult = DynaCode.InvokeByJsonInputs("Zzz.AppEndProxy.CreateUserServerContext", null, user);
				var result = codeInvokeResult.Result;
				Hashtable hashtable = result is not null ? (Hashtable)result : [];
				user.ContextInfo = hashtable;
				user.ContextInfo.Add("UserName", user.UserName);
				user.ContextInfo.Add("UserId", user.Id);
				AppEndCache.Set(cacheKey, hashtable, 600);
			}

			return user;
		}
		public static AppEndUser GetNobodyUser()
		{
			Hashtable contextInfo = [];
			contextInfo.Add("UserName", "nobody");
			contextInfo.Add("UserId", -1);
			return new AppEndUser() { UserName = "nobody", Id = -1, ContextInfo = contextInfo };
		}
		public static string CreateToken(this AppEndUser dynaUser)
		{
			return dynaUser.Encode(AppEndSettings.Secret);
		}

		public static string CreateAccessToken(this AppEndUser dynaUser, int validMinutes = 15)
		{
			var payload = new Dictionary<string, object>
			{
				["Id"] = dynaUser.Id,
				["UserName"] = dynaUser.UserName ?? "",
				["Roles"] = dynaUser.Roles ?? [],
				["RoleNames"] = dynaUser.RoleNames ?? [],
				["exp"] = DateTimeOffset.UtcNow.AddMinutes(validMinutes).ToUnixTimeSeconds()
			};
			return payload.Encode(AppEndSettings.Secret);
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
