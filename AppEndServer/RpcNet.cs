using AppEndApi;
using AppEndCommon;
using AppEndDynaCode;
using Azure;
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
                string s = await new StreamReader(context.Request.Body, Encoding.UTF8).ReadToEndAsync();
                List<RpcNetRequest>? requests = ExtensionsForJson.TryDeserializeTo<List<RpcNetRequest>>(s, new() { IncludeFields = true });
                List<RpcNetResponse> responses = requests.Exec(context.GetActor(), context.Request.GetClientIp(), context.Request.GetClientAgent());
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
                    var r = DynaCode.InvokeByJsonInputs(request.Method, request.Inputs, actor);
                    response = new() { Id = request.Id, Result = r.Result, IsSucceeded = r.IsSucceeded == true ? true : false, FromCache = r.FromCache, Duration = r.Duration };
                }
                catch (Exception ex)
                {
                    Exception exx = ex.InnerException is null ? ex : ex.InnerException;
                    response = new() { Id = request.Id, Result = exx, IsSucceeded = false, FromCache = false, Duration = 0 };
                }
                LogActivity(request, response, clientIp, clientAgent, actor);
                result.Add(response);
            }
            return result;
        }

        private static void LogActivity(RpcNetRequest request, RpcNetResponse response, string clientIp, string clientAgent, AppEndUser actor)
        {
            var parts = StaticMethods.MethodPartsNames(request.Method);
            bool hasClientQueryJE = request.Inputs.TryGetProperty("ClientQueryJE", out JsonElement je);

            LogMan.LogActivity(
                    parts.Item1, parts.Item2, parts.Item3,
                    (hasClientQueryJE ? GetInputsRecordId(je) : ""),
                    response.IsSucceeded, response.FromCache,
                    (hasClientQueryJE ? je.ToJsonStringByBuiltIn() : request.Inputs.ToJsonStringByBuiltIn()),
                    (response.IsSucceeded ? null : response.Result.ToJsonStringByNewtonsoft()),
                    response.Duration.ToIntSafe(),
                    clientIp, clientAgent,
                    (actor == null ? -1 : actor.Id), (actor == null ? "" : actor.UserName));
        }

		private static string? GetInputsRecordId(JsonElement clientQueryJEObj)
		{
            if (clientQueryJEObj.TryGetProperty("Params", out JsonElement paramsToken) && paramsToken.ValueKind == JsonValueKind.Array)
                foreach (var jo in paramsToken.EnumerateArray())
                    if (jo.TryGetProperty("Name", out JsonElement nameElement) && nameElement.GetString() == "Id")
                        if (jo.TryGetProperty("Value", out JsonElement valueElement))
                            return valueElement.ToString();

			return null;
		}

		public class RpcNetRequest
        {
            public string Id { set; get; } = "";
            public string Method { set; get; } = "";
            public JsonElement Inputs { get; set; }
        }

        public class RpcNetResponse
        {
            public string Id { set; get; } = "";
            public long Duration { set; get; }
            public object? Result { get; set; }
			public bool IsSucceeded { get; set; }
			public bool FromCache { get; set; }
		}
	}
}
