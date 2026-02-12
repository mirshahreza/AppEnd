using AppEndCommon;
using AppEndDynaCode;
using Azure;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace AppEndServer
{
    public class RpcNet(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public Task Invoke(HttpContext httpContext)
        {
            return _next(httpContext);
        }
    }

    public static class RpcNetExtensions
    {
        public static IApplicationBuilder UseRpcNet(this IApplicationBuilder builder)
        {
            WebApplication wa = (WebApplication)builder;

            CodeInvokeOptions codeInvokeOptions = new(AppEndSettings.ServerObjectsPath)
            {
                ReferencesPath = "References",
                PublicKeyRole = AppEndSettings.PublicKeyRole,
                PublicKeyUser = AppEndSettings.PublicKeyUser,
                PublicMethods = AppEndSettings.PublicMethods,
                CompiledIn = true,
                IsDevelopment = true
            };
            DynaCode.Init(codeInvokeOptions);

            wa.MapPost(AppEndSettings.TalkPoint, async delegate (HttpContext context)
            {
                using var reader = new StreamReader(context.Request.Body, Encoding.UTF8);
                string s = await reader.ReadToEndAsync();
                List<RpcNetRequest>? requests = ExtensionsForJson.TryDeserializeTo<List<RpcNetRequest>>(s, new() { IncludeFields = true });
                InjectRefreshTokenFromCookie(context, requests);
                var (actor, tokenInvalid) = context.TryGetActor();
                if (tokenInvalid && requests != null && !AllRequestsArePublic(requests))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Unauthorized");
                    return;
                }
                List<RpcNetResponse> responses = requests.Exec(actor, context.Request.GetClientIp(), context.Request.GetClientAgent());
                ApplyAuthCookies(context, requests, responses);
                string res = Newtonsoft.Json.JsonConvert.SerializeObject(responses, Newtonsoft.Json.Formatting.None);
                await context.Response.WriteAsJsonAsync(res);
            });

            wa.MapGet(AppEndSettings.TalkPoint, () =>
            {
                return "I am working ...";
            });

            return builder.UseMiddleware<RpcNet>();
        }

        public static List<RpcNetResponse> Exec(this List<RpcNetRequest>? requests, AppEndUser actor, string clientIp, string clientAgent)
        {
            if (requests == null) return [];
            List<RpcNetResponse> result = [];
            foreach (var request in requests)
            {
                RpcNetResponse response;
                try
                {
                    if (request.Method.StartsWith("__LR_"))
                    {
                        response = HandleLongRunningRequest(request, actor);
                    }
                    else
                    {
					    actor.ContextInfo?.SetOrAdd("Lang", request.Lang ?? "En");
					    actor.ContextInfo?.SetOrAdd("ClientIp", clientIp);
                        actor.ContextInfo?.SetOrAdd("ClientAgent", clientAgent);
					    actor.ContextInfo?.SetOrAdd("MethodFullName", request.Method);
					    var r = DynaCode.InvokeByJsonInputs(request.Method, request.Inputs, actor, clientIp, clientAgent);
                        response = new() { Id = request.Id, Result = r.Result, IsSucceeded = r.IsSucceeded == true ? true : false, FromCache = r.FromCache, Duration = r.Duration, TaskToken = r.TaskToken, IsLongRunning = r.IsLongRunning };
                    }
                }
                catch (Exception ex)
                {
                    Exception exx = ex.InnerException is null ? ex : ex.InnerException;
                    response = new() { Id = request.Id, Result = exx, IsSucceeded = false, FromCache = false, Duration = 0 };
                }
                result.Add(response);
            }
            return result;
        }

		public class RpcNetRequest
        {
            public string Id { set; get; } = "";
            public string Method { set; get; } = "";
            public string Lang { set; get; } = "";
            public JsonElement Inputs { get; set; }
        }

        public class RpcNetResponse
        {
            public string Id { set; get; } = "";
            public long Duration { set; get; }
            public object? Result { get; set; }
			public bool IsSucceeded { get; set; }
			public bool FromCache { get; set; }
			public string? TaskToken { get; set; }
			public bool IsLongRunning { get; set; }
		}

		private static RpcNetResponse HandleLongRunningRequest(RpcNetRequest request, AppEndUser actor)
		{
			try
			{
				string taskToken = request.Inputs.GetProperty("TaskToken").GetString() ?? "";
				string userName = actor.UserName ?? "";

				return request.Method switch
				{
					"__LR_GetStatus" => new()
					{
						Id = request.Id,
						Result = DynaCode.GetLongRunningTaskStatus(taskToken, userName),
						IsSucceeded = DynaCode.GetLongRunningTaskStatus(taskToken, userName) is not null
					},
					"__LR_GetResult" => BuildGetResultResponse(request.Id, taskToken, userName),
					"__LR_Cancel" => new()
					{
						Id = request.Id,
						Result = DynaCode.CancelLongRunningTask(taskToken, userName),
						IsSucceeded = true
					},
					_ => new() { Id = request.Id, Result = "UnknownLongRunningCommand", IsSucceeded = false }
				};
			}
			catch (Exception ex)
			{
				return new() { Id = request.Id, Result = ex.InnerException ?? ex, IsSucceeded = false };
			}
		}

		private static RpcNetResponse BuildGetResultResponse(string requestId, string taskToken, string userName)
		{
			var info = DynaCode.GetLongRunningTaskStatus(taskToken, userName);
			if (info is null)
				return new() { Id = requestId, Result = "TaskNotFound", IsSucceeded = false };

			if (info.Status == AppEndDynaCode.LongRunningTaskStatus.Completed)
				return new() { Id = requestId, Result = DynaCode.GetLongRunningTaskResult(taskToken, userName), IsSucceeded = true, Duration = info.DurationMs };

			if (info.Status == AppEndDynaCode.LongRunningTaskStatus.Failed)
				return new() { Id = requestId, Result = info.Error, IsSucceeded = false, Duration = info.DurationMs };

			return new() { Id = requestId, Result = info.Status.ToString(), IsSucceeded = true, Duration = info.DurationMs };
		}

		private static bool AllRequestsArePublic(List<RpcNetRequest> requests)
		{
			var pm = AppEndSettings.PublicMethods;
			if (pm == null) return false;
			foreach (var req in requests)
			{
				if (string.IsNullOrEmpty(req.Method)) continue;
				if (!pm.ContainsIgnoreCase(req.Method))
					return false;
			}
			return true;
		}

		private static void InjectRefreshTokenFromCookie(HttpContext context, List<RpcNetRequest>? requests)
		{
			if (requests == null) return;
			string? refreshToken = context.Request.Cookies[ActorServices.CookieRefreshToken];
			if (string.IsNullOrEmpty(refreshToken)) return;
			foreach (var req in requests)
			{
				if (req.Method != "Zzz.AppEndProxy.RefreshToken") continue;
				var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(req.Inputs.GetRawText()) ?? [];
				dict["RefreshToken"] = JsonSerializer.SerializeToElement(refreshToken);
				req.Inputs = JsonSerializer.SerializeToElement(dict);
			}
		}

		private static void ApplyAuthCookies(HttpContext context, List<RpcNetRequest>? requests, List<RpcNetResponse> responses)
		{
			if (requests == null || responses == null) return;
			bool isDev = AppEndSettings.IsDevelopment;
			var cookieOptions = new CookieOptions
			{
				HttpOnly = true,
				Secure = !isDev,
				SameSite = SameSiteMode.Lax
			};
			for (int i = 0; i < requests.Count && i < responses.Count; i++)
			{
				var req = requests[i];
				var resp = responses[i];
				if (req.Method == "Zzz.AppEndProxy.Logout" && resp.IsSucceeded)
				{
					context.Response.Cookies.Delete(ActorServices.CookieAccessToken);
					context.Response.Cookies.Delete(ActorServices.CookieRefreshToken);
					continue;
				}
				if ((req.Method == "Zzz.AppEndProxy.Login" || req.Method == "Zzz.AppEndProxy.LoginAs" || req.Method == "Zzz.AppEndProxy.RefreshToken") && resp.IsSucceeded && resp.Result != null)
				{
					string? accessToken = GetStringFromResult(resp.Result, "access_token");
					string? refreshTokenVal = GetStringFromResult(resp.Result, "refresh_token");
					if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshTokenVal))
					{
						context.Response.Cookies.Append(ActorServices.CookieAccessToken, accessToken, new CookieOptions { HttpOnly = true, Secure = !isDev, SameSite = SameSiteMode.Lax, MaxAge = TimeSpan.FromMinutes(15) });
						context.Response.Cookies.Append(ActorServices.CookieRefreshToken, refreshTokenVal, new CookieOptions { HttpOnly = true, Secure = !isDev, SameSite = SameSiteMode.Lax, MaxAge = TimeSpan.FromDays(7) });
						RemoveTokensFromResult(resp.Result);
						if (req.Method == "Zzz.AppEndProxy.RefreshToken")
						{
							LogMan.LogActivity("Zzz", "AppEndProxy", "RefreshTokenRotation", "", true, false, "", "Applied", 0,
								context.Request.GetClientIp(), context.Request.GetClientAgent(), -1, "");
						}
					}
				}
			}
		}

		private static string? GetStringFromResult(object? result, string key)
		{
			if (result == null) return null;
			if (result is Dictionary<string, object> dict && dict.TryGetValue(key, out var v) && v != null)
				return v.ToString();
			var jObj = result as Newtonsoft.Json.Linq.JObject ?? Newtonsoft.Json.Linq.JObject.FromObject(result);
			return jObj[key]?.ToString();
		}

		private static void RemoveTokensFromResult(object? result)
		{
			if (result == null) return;
			if (result is Dictionary<string, object> dict)
			{
				dict.Remove("access_token");
				dict.Remove("refresh_token");
				return;
			}
			var jObj = result as Newtonsoft.Json.Linq.JObject;
			jObj?.Remove("access_token");
			jObj?.Remove("refresh_token");
		}
	}
}
