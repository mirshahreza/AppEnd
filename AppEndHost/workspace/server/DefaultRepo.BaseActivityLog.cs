using System.Text.Json;
using AppEndCommon;
using AppEndDynaCode;
using AppEndDbIO;
using AppEndServer;
namespace DefaultRepo
{
 public static class BaseActivityLog
 {
 public static object? ReadList(JsonElement ClientQueryJE, AppEndUser? Actor)
 {
 return AppEndDbIO.ClientQuery.GetInstanceByQueryJson(ClientQueryJE, Actor?.ContextInfo).Exec();
 }
 }
}
