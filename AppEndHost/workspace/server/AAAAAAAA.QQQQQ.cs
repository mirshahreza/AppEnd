using System.Text.Json;
using AppEndCommon;
using AppEndDynaCode;
using AppEndDbIO;
using AppEndServer;
namespace AAAAAAAA
{
    public static class QQQQQ
    {
        public static object? SampleMthod()
        {
            return true;
        }
        public static object? fffffff(JsonElement ClientQueryJE, AppEndUser? Actor)
        {
            return AppEndDbIO.ClientQuery.GetInstanceByQueryJson(ClientQueryJE, Actor?.ContextInfo).Exec();
        }
    }
}
