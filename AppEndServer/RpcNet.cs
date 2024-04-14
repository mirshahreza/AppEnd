using AppEndCommon;
using AppEndDynaCode;
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

            wa.Lifetime.ApplicationStopping.Register(AppEndEventLogger.SatrtWriting);

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

            wa.MapPost(AppEndSettings.TalkPoint, async delegate (HttpContext context, AppEndBackgroundWorkerQueue appEndBackgroundWorkerQueue)
            {
                string clientInfo = $"{context.Request.GetClientIp()}::{context.Request.GetClientAgent()}";
                string s = await new StreamReader(context.Request.Body, Encoding.UTF8).ReadToEndAsync();
                List<RpcNetRequest>? requests = ExtensionsForJson.TryDeserializeTo<List<RpcNetRequest>>(s, new() { IncludeFields = true });
                List<RpcNetResponse> responses = requests.Exec(context.GetActor(), appEndBackgroundWorkerQueue, clientInfo);
                string res = Newtonsoft.Json.JsonConvert.SerializeObject(responses, Newtonsoft.Json.Formatting.None);
                await context.Response.WriteAsJsonAsync(res);
            });

            wa.MapGet(AppEndSettings.TalkPoint, () =>
            {
                return "I am working ...";
            });

            return builder.UseMiddleware<RpcNet>();
        }

        public static List<RpcNetResponse> Exec(this List<RpcNetRequest>? requests, AppEndUser actor, AppEndBackgroundWorkerQueue appEndBackgroundWorkerQueue, string clientInfo)
        {
            if (requests == null) return [];
			List<RpcNetResponse> result = [];
            foreach (var request in requests)
            {
                RpcNetResponse response;
                CodeInvokeResult codeInvokeResult;
                try
                {
                    codeInvokeResult = DynaCode.InvokeByJsonInputs(request.Method, request.Inputs, actor, appEndBackgroundWorkerQueue, clientInfo);
                    response = new() { Id = request.Id, Result = codeInvokeResult.Result, IsSucceeded = codeInvokeResult.IsSucceeded, Duration = codeInvokeResult.Duration };
                }
				catch (AppEndException ex)
				{
					response = new() { Id = request.Id, Result = ex, IsSucceeded = false, Duration = 0 };
				}
				catch (Exception ex)
				{
					Exception exx = ex.InnerException is null ? ex : ex.InnerException;
					response = new() { Id = request.Id, Result = exx, IsSucceeded = false, Duration = 0 };
				}
				result.Add(response);
            }
            return result;
        }

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
    }
}
