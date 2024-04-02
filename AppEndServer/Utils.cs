using AppEndCommon;
using AppEndDynaCode;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using AngleSharp.Html;
using JWT;
using Microsoft.Extensions.Caching.Memory;
using System.Collections;

namespace AppEndServer
{
    public static class Utils
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
				SV.SharedMemoryCache.Set(cacheKey, hashtable, GetCacheOptions(600));
			}

			return user;
		}

		public static void ClearActorCacheEntries(AppEndUser? Actor)
		{
			if (Actor == null) return;
			ICollection userKeys = SV.SharedMemoryCache.GetKeysStartsWith(AppEndUser.ContextCacheKeyShortName(Actor.Id.ToString()));
			foreach (string key in userKeys)
			{
				SV.SharedMemoryCache.TryRemove(key);
			}
		}

		public static AppEndUser GetNobodyUser()
		{
			return new AppEndUser() { UserName = "nobody" };
		}

        public static string CreateToken(this AppEndUser dynaUser)
        {
            return dynaUser.Encode(AppEndSettings.Secret);
        }

        public static void TryRemove(this IMemoryCache memoryCache, string key)
        {
            if (memoryCache.Get(key) is not null) memoryCache.Remove(key);
        }

		public static string FormatAsHtml(this string text)
		{
			var parser = new HtmlParser();
			IHtmlDocument document = parser.ParseDocument("<body>" + text + "</body>");
			if (document.Body is null) return "";
			using var writer = new StringWriter();
			document.Body.ToHtml(writer, new PrettyMarkupFormatter());
			document = parser.ParseDocument(writer.ToString());
			if (document.Body is null) return "";
			string res = document.Body.InnerHtml.ToStringEmpty();
			res = res.Replace("\n\t", "\n").Trim().Replace("<template>", "<template>\n").Replace("</template>", "\n</template>");
			return res;
		}

		public static MemoryCacheEntryOptions GetCacheOptions(int seconds)
		{
			return new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(seconds) };
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
