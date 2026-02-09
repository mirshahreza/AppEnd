using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using AppEndCommon;

namespace AppEndDynaCode
{
    internal static class DynaCodeInvocation
    {
        #region Public Invocation Methods
        public static CodeInvokeResult InvokeByJsonInputs(string methodFullPath, JsonElement? inputParams = null, AppEndUser? dynaUser = null, string clientIp = "", string clientAgent = "", bool ignoreCaching = false)
        {
            MethodInfo methodInfo = DynaCodeMapping.GetMethodInfo(methodFullPath);
            return Invoke(methodInfo, ExtractParams(methodInfo, inputParams, dynaUser), dynaUser, ignoreCaching, inputParams, clientIp, clientAgent);
        }

        public static CodeInvokeResult InvokeByParamsInputs(string methodFullPath, object[]? inputParams = null, AppEndUser? dynaUser = null, bool ignoreCaching = false)
        {
            MethodInfo methodInfo = DynaCodeMapping.GetMethodInfo(methodFullPath);
            return Invoke(methodInfo, inputParams, dynaUser, ignoreCaching);
        }
        #endregion

        #region Core Invocation Logic
        private static CodeInvokeResult Invoke(MethodInfo methodInfo, object[]? inputParams = null, AppEndUser? dynaUser = null, bool ignoreCaching = false, JsonElement? rawInputs = null, string clientIp = "", string clientAgent = "")
        {
            string? responseStr = null;
            string methodFullName = methodInfo.GetFullName();
            string methodFilePath = DynaCodeMapping.GetMethodFilePath(methodFullName);
            MethodSettings methodSettings = DynaCodeSettings.ReadMethodSettings(methodFullName, methodFilePath);

            if (methodSettings.CachePolicy != null && methodSettings.CachePolicy.CacheLevel == CacheLevel.PerUser && (dynaUser is null || dynaUser.UserName.Trim() == ""))
                throw new AppEndException($"CachePolicy.CacheLevelIsSetToPerUserButTheCurrentUserIsNull", System.Reflection.MethodBase.GetCurrentMethod())
                    .AddParam("MethodFullName", methodFullName)
                    .GetEx();

            CodeInvokeResult codeInvokeResult;
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                if (DynaCodeSecurity.HasAccess(methodInfo, methodSettings, dynaUser) == false)
                {
                    throw new AppEndException("AccessDenied", System.Reflection.MethodBase.GetCurrentMethod())
                        .AddParam("Method", methodInfo.GetFullName())
                        .AddParam("Actor", dynaUser == null ? "nobody" : dynaUser.UserName)
                        .GetEx();
                }
                string cacheKey = CalculateCacheKey(methodInfo, methodSettings, inputParams, dynaUser);
                if (methodSettings.CachePolicy?.CacheLevel != CacheLevel.None && AppEndCache.TryGet<object>(cacheKey, out var result) && ignoreCaching == false)
                {
                    stopwatch.Stop();
                    codeInvokeResult = new() { Result = result, FromCache = true, IsSucceeded = true, Duration = stopwatch.ElapsedMilliseconds };
                }
                else
                {
                    try
                    {
                        result = methodInfo.Invoke(null, inputParams);
                        if (methodSettings.CachePolicy?.CacheLevel != CacheLevel.None && methodSettings.CachePolicy is not null)
                        {
                            AppEndCache.Set(cacheKey, result, methodSettings.CachePolicy.AbsoluteExpirationSeconds);
                        }
                        stopwatch.Stop();
                        codeInvokeResult = new() { Result = result, IsSucceeded = true, Duration = stopwatch.ElapsedMilliseconds };
                    }
                    catch (Exception ex)
                    {
                        stopwatch.Stop();
                        object? oEx = ex.InnerException is null ? ex : ex.InnerException;
                        codeInvokeResult = new() { Result = oEx, IsSucceeded = false, Duration = stopwatch.ElapsedMilliseconds };
                    }
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                Exception exx = ex.InnerException is null ? ex : ex.InnerException;
                codeInvokeResult = new() { Result = exx, IsSucceeded = false, Duration = stopwatch.ElapsedMilliseconds };
                responseStr = exx.Message + SV.NL + exx.InnerException?.ToString();
            }

            try
            {
                var parts = StaticMethods.MethodPartsNames(methodFullName);
                JsonElement je = default;
                bool hasClientQueryJE = rawInputs is not null && ((JsonElement)rawInputs).TryGetProperty("ClientQueryJE", out je);

                string recordId = hasClientQueryJE ? GetInputsRecordId(je) ?? "" : "";

                string? inputsStr = null;

                if (methodSettings.LogPolicy != LogPolicy.IgnoreLogging)
                {
                    switch (methodSettings.LogPolicy)
                    {
                        case LogPolicy.TrimInputs:
                            inputsStr = hasClientQueryJE ? je.ToJsonStringByBuiltInAllDefaults() : rawInputs?.ShrinkJsonElement(128).ToJsonStringByBuiltInAllDefaults();
                            break;
                        case LogPolicy.Full:
                            inputsStr = hasClientQueryJE ? je.ToJsonStringByBuiltInAllDefaults() : rawInputs.ToJsonStringByBuiltInAllDefaults();
                            break;
                    }

                    if (inputsStr != null && inputsStr.Length < 4) inputsStr = null;

                    LogMan.LogActivity(parts.Item1, parts.Item2, parts.Item3, recordId, codeInvokeResult.IsSucceeded, codeInvokeResult.FromCache,
                            inputsStr,
                            (codeInvokeResult.IsSucceeded ? null : codeInvokeResult.Result.ToHumanReadible()),
                            (int)codeInvokeResult.Duration,
                            clientIp, clientAgent,
                            (dynaUser == null ? -1 : dynaUser.Id), (dynaUser == null ? "" : dynaUser.UserName));
                }
            }
            catch { }

            return codeInvokeResult;
        }
        #endregion

        #region Parameter Extraction
        private static object[]? ExtractParams(MethodInfo methodInfo, JsonElement? jsonElement, AppEndUser? actor)
        {
            List<object> methodInputs = [];
            ParameterInfo[] methodParams = methodInfo.GetParameters();
            var objects = jsonElement is null ? (System.Text.Json.JsonElement.ObjectEnumerator?)null : ((JsonElement)jsonElement).EnumerateObject();

            foreach (var paramInfo in methodParams)
            {
                if (paramInfo.ParameterType == typeof(AppEndUser))
                {
                    if (actor is not null) methodInputs.Add(actor);
                }
                else
                {
                    if (objects is not null)
                    {
                        var l = ((System.Text.Json.JsonElement.ObjectEnumerator)objects).Where(i => string.Equals(i.Name, paramInfo.Name));
                        if (!l.Any()) throw new AppEndException($"MethodCallMustContainsParameter", System.Reflection.MethodBase.GetCurrentMethod())
                                .AddParam("MethodFullName", methodInfo.GetFullName())
                                .AddParam("ParameterName", paramInfo.Name.ToStringEmpty())
                                .GetEx();
                        var p = l.First();
                        methodInputs.Add(p.Value.ToOrigType(paramInfo));
                    }
                }
            }
            return [.. methodInputs];
        }
        #endregion

        #region Cache Management
        private static string CalculateCacheKey(MethodInfo methodInfo, MethodSettings methodSettings, object[]? inputParams, AppEndUser? dynaUser)
        {
            string paramKey = inputParams is null ? "" : $".{inputParams.ToJsonStringByBuiltIn().GetHashCode()}";
            string levelName = methodSettings.CachePolicy.CacheLevel == CacheLevel.PerUser ? $".{dynaUser?.UserName}" : "";
            return $"{methodInfo.GetFullName()}{levelName}{paramKey}";
        }
        #endregion

        #region Helper Methods
        private static string? GetInputsRecordId(System.Text.Json.JsonElement clientQueryJEObj)
        {
            if (clientQueryJEObj.TryGetProperty("Params", out System.Text.Json.JsonElement paramsToken) && paramsToken.ValueKind == System.Text.Json.JsonValueKind.Array)
                foreach (var jo in paramsToken.EnumerateArray())
                    if (jo.TryGetProperty("Name", out System.Text.Json.JsonElement nameElement) && nameElement.GetString() == "Id")
                        if (jo.TryGetProperty("Value", out System.Text.Json.JsonElement valueElement))
                            return valueElement.ToString();

            return null;
        }
        #endregion
    }
}
