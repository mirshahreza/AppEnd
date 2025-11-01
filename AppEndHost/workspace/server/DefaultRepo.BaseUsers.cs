using System.Text.Json;
using AppEndCommon;
using AppEndDynaCode;
using AppEndDbIO;
using AppEndServer;
namespace DefaultRepo
{
 public static class BaseUsers
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
 public static object? DeleteByKey(JsonElement ClientQueryJE, AppEndUser? Actor)
 {
 return AppEndDbIO.ClientQuery.GetInstanceByQueryJson(ClientQueryJE, Actor?.ContextInfo).Exec();
 }
 public static object? IsActiveUpdate(JsonElement ClientQueryJE, AppEndUser? Actor)
 {
 return AppEndDbIO.ClientQuery.GetInstanceByQueryJson(ClientQueryJE, Actor?.ContextInfo).Exec();
 }
 public static object? LoginLockedUpdate(JsonElement ClientQueryJE, AppEndUser? Actor)
 {
 return AppEndDbIO.ClientQuery.GetInstanceByQueryJson(ClientQueryJE, Actor?.ContextInfo).Exec();
 }
 }
}
