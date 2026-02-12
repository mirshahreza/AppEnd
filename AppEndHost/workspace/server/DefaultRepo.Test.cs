using System;
using System.Text.Json;
using System.Threading;
using AppEndCommon;
using AppEndDynaCode;
using AppEndDbIO;
using AppEndServer;
namespace DefaultRepo
{
    public static class Test
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
        public static object? MyTT()
        {
            return "lllllllllllllllll";
        }
        public static object? LongRunningDemo(int Seconds, CancellationToken ct)
        {
            int total = Seconds * 10;
            for (int i = 0; i < total; i++)
            {
                ct.ThrowIfCancellationRequested();
                Thread.Sleep(100);
            }
            return new { Message = "LongRunningDemo completed successfully", Duration = Seconds, CompletedAt = DateTime.UtcNow };
        }
    }
}