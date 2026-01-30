using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using AppEndCommon;

namespace AppEndDynaCode
{
    internal static class DynaCodeSettings
    {
        #region Settings Read/Write
        public static MethodSettings ReadMethodSettings(string methodFullName)
        {
            string methodFilePath = DynaCodeMapping.GetMethodFilePath(methodFullName);
            return ReadMethodSettings(methodFullName, methodFilePath);
        }

        public static MethodSettings ReadMethodSettings(string methodFullName, string methodFilePath)
        {
            string settingsFileName = GetSettingsFile(methodFilePath);
            string settingsRaw = File.Exists(settingsFileName) ? File.ReadAllText(settingsFileName) : "{}";
            try
            {
                var dict = JsonSerializer.Deserialize<Dictionary<string, MethodSettings>>(settingsRaw, new JsonSerializerOptions { IncludeFields = true }) ?? new Dictionary<string, MethodSettings>();
                if (!dict.TryGetValue(methodFullName, out var methodSettings) || methodSettings is null) return new();
                return methodSettings;
            }
            catch
            {
                throw new AppEndException($"SettingsAreNotValid", System.Reflection.MethodBase.GetCurrentMethod())
                    .AddParam("MethodFullName", methodFullName)
                    .AddParam("MethodFilePath", methodFilePath)
                    .AddParam("Settings", settingsRaw)
                    .GetEx();
            }
        }

        public static void WriteMethodSettings(string methodFullName, MethodSettings methodSettings)
        {
            string methodFilePath = DynaCodeMapping.GetMethodFilePath(methodFullName);
            WriteMethodSettings(methodFullName, methodFilePath, methodSettings);
        }

        public static void WriteMethodSettings(string methodFullName, string methodFilePath, MethodSettings methodSettings)
        {
            string settingsFileName = GetSettingsFile(methodFilePath);
            string settingsRaw = File.Exists(settingsFileName) ? File.ReadAllText(settingsFileName) : "{}";
            Dictionary<string, MethodSettings>? dict = null;
            try
            {
                dict = JsonSerializer.Deserialize<Dictionary<string, MethodSettings>>(settingsRaw, new JsonSerializerOptions { IncludeFields = true });
            }
            catch
            {
                dict = null;
            }
            dict ??= new Dictionary<string, MethodSettings>();
            dict[methodFullName] = methodSettings;
            File.WriteAllText(settingsFileName, JsonSerializer.Serialize(dict, new JsonSerializerOptions { WriteIndented = false, IncludeFields = true }));
        }

        public static void RemoveMethodSettings(string methodFullName)
        {
            CodeMap? codeMap = DynaCodeMapping.CodeMaps.FirstOrDefault(cm => cm.MethodFullName == methodFullName) ?? throw new AppEndException($"MethodDoesNotExist : [ {methodFullName} ]", System.Reflection.MethodBase.GetCurrentMethod())
                    .AddParam("MethodFullName", methodFullName)
                    .GetEx();
            string settingsFileName = codeMap.FilePath + ".settings.json";
            if (!File.Exists(settingsFileName)) return;
            File.Delete(settingsFileName);
        }
        #endregion

        #region Helper Methods
        private static string GetSettingsFile(string methodFilePath)
        {
            return methodFilePath.Replace(".cs", "") + ".settings.json";
        }
        #endregion
    }
}
