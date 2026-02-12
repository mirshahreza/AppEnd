using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AppEndCommon;

namespace AppEndDynaCode
{
    public static class LongRunningTaskManager
    {
        private static readonly ConcurrentDictionary<string, (LongRunningTaskInfo Info, CancellationTokenSource Cts)> _tasks = new();
        private static Timer? _cleanupTimer;
        private static readonly object _timerLock = new();
        private const int CleanupIntervalMinutes = 5;
        private const int MaxCompletedAgeMinutes = 30;

        #region Public API
        public static string Enqueue(MethodInfo methodInfo, object[]? inputParams, MethodSettings methodSettings, AppEndUser? dynaUser, string clientIp, string clientAgent)
        {
            EnsureCleanupTimer();

            var info = new LongRunningTaskInfo
            {
                MethodFullName = methodInfo.GetFullName(),
                Status = LongRunningTaskStatus.Pending,
                CreatedBy = dynaUser?.UserName ?? ""
            };

            int timeoutSeconds = methodSettings.LongRunningPolicy?.TimeoutSeconds ?? 300;
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));

            _tasks[info.TaskToken] = (info, cts);

            object[]? finalParams = InjectCancellationToken(methodInfo, inputParams, cts.Token);

            Task.Run(() =>
            {
                info.Status = LongRunningTaskStatus.Running;
                var sw = Stopwatch.StartNew();
                try
                {
                    cts.Token.ThrowIfCancellationRequested();
                    var result = methodInfo.Invoke(null, finalParams);

                    if (methodSettings.CachePolicy?.CacheLevel != CacheLevel.None && methodSettings.CachePolicy is not null)
                    {
                        string cacheKey = DynaCodeInvocation.CalculateCacheKey(methodInfo, methodSettings, inputParams, dynaUser);
                        AppEndCache.Set(cacheKey, result, methodSettings.CachePolicy.AbsoluteExpirationSeconds);
                    }

                    sw.Stop();
                    info.Status = LongRunningTaskStatus.Completed;
                    info.Result = result;
                    info.DurationMs = sw.ElapsedMilliseconds;
                }
                catch (Exception ex) when (ex is OperationCanceledException || ex.InnerException is OperationCanceledException)
                {
                    sw.Stop();
                    info.Status = LongRunningTaskStatus.Cancelled;
                    info.DurationMs = sw.ElapsedMilliseconds;
                    info.Error = "TaskCancelled";
                }
                catch (Exception ex)
                {
                    sw.Stop();
                    var inner = ex.InnerException ?? ex;
                    info.Status = LongRunningTaskStatus.Failed;
                    info.Error = inner.Message;
                    info.Result = inner;
                    info.DurationMs = sw.ElapsedMilliseconds;
                }
                finally
                {
                    info.CompletedAt = DateTime.UtcNow;

                    try
                    {
                        if (methodSettings.LogPolicy != LogPolicy.IgnoreLogging)
                        {
                            var parts = StaticMethods.MethodPartsNames(info.MethodFullName);
                            LogMan.LogActivity(parts.Item1, parts.Item2, parts.Item3, "",
                                info.Status == LongRunningTaskStatus.Completed, false,
                                null, info.Error, (int)info.DurationMs, clientIp, clientAgent,
                                dynaUser?.Id ?? -1, dynaUser?.UserName ?? "");
                        }
                    }
                    catch { }
                }
            }, cts.Token);

            return info.TaskToken;
        }

        public static LongRunningTaskInfo? GetStatus(string taskToken, string requestingUser)
        {
            if (!_tasks.TryGetValue(taskToken, out var entry)) return null;
            if (!IsAuthorized(entry.Info, requestingUser)) return null;
            return entry.Info;
        }

        public static object? GetResult(string taskToken, string requestingUser)
        {
            if (!_tasks.TryGetValue(taskToken, out var entry)) return null;
            if (!IsAuthorized(entry.Info, requestingUser)) return null;
            return entry.Info.Result;
        }

        public static bool Cancel(string taskToken, string requestingUser)
        {
            if (!_tasks.TryGetValue(taskToken, out var entry)) return false;
            if (!IsAuthorized(entry.Info, requestingUser)) return false;
            if (entry.Info.Status == LongRunningTaskStatus.Pending || entry.Info.Status == LongRunningTaskStatus.Running)
            {
                try
                {
                    entry.Cts.Cancel();
                    return true;
                }
                catch { return false; }
            }
            return false;
        }
        #endregion

        #region Private Helpers
        private static bool IsAuthorized(LongRunningTaskInfo info, string requestingUser)
        {
            if (string.IsNullOrEmpty(requestingUser)) return false;
            return string.Equals(info.CreatedBy, requestingUser, StringComparison.OrdinalIgnoreCase);
        }

        private static object[]? InjectCancellationToken(MethodInfo methodInfo, object[]? inputParams, CancellationToken ct)
        {
            if (inputParams is null) return inputParams;
            var parameters = methodInfo.GetParameters();
            for (int i = 0; i < parameters.Length && i < inputParams.Length; i++)
            {
                if (parameters[i].ParameterType == typeof(CancellationToken))
                {
                    inputParams[i] = ct;
                }
            }
            return inputParams;
        }

        private static void EnsureCleanupTimer()
        {
            if (_cleanupTimer is not null) return;
            lock (_timerLock)
            {
                _cleanupTimer ??= new Timer(_ => Cleanup(), null,
                    TimeSpan.FromMinutes(CleanupIntervalMinutes),
                    TimeSpan.FromMinutes(CleanupIntervalMinutes));
            }
        }

        private static void Cleanup()
        {
            var cutoff = DateTime.UtcNow.AddMinutes(-MaxCompletedAgeMinutes);
            foreach (var kvp in _tasks)
            {
                var info = kvp.Value.Info;
                if (info.CompletedAt.HasValue && info.CompletedAt.Value < cutoff)
                {
                    if (_tasks.TryRemove(kvp.Key, out var removed))
                    {
                        removed.Cts.Dispose();
                    }
                }
            }
        }
        #endregion
    }
}
