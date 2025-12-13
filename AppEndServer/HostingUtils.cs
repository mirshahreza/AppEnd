using AppEndCommon;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Net;
using System.Text.Json.Nodes;

namespace AppEndServer
{
    public static class HostingUtils
    {
		public static List<string> GetTranslationKeys(string folderName)
		{
			List<string> Keys = [];
			DirectoryInfo diApp = new(AppEndSettings.ClientObjectsPath + "/" + folderName);
			foreach (string f in diApp.GetFilesRecursive())
			{
				if (f.EndsWith(".vue"))
				{
					FileInfo fi = new(f);
					string fileContent = File.ReadAllText(fi.FullName);
					MatchCollection translations = ExtensionsForString.JsTranslationRegex().Matches(fileContent);
					foreach (Match match in translations)
					{
						string v = match.Value.Replace("shared.translate(", "").Replace(")", "").Replace("\"", "").Replace("'", "");
						if (v != "i" && v != "m" && !v.ContainsIgnoreCase("i.") && !v.ContainsIgnoreCase("inputs.")) Keys.Add(v);
					}
				}
			}
			return Keys;
		}

        public static object? GetAppEndSummary()
		{
			JObject appendSummary = [];

			appendSummary["ServerPhisicalAddress"] = (new DirectoryInfo(".")).FullName;
			appendSummary["ServerDateTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:MM");
			appendSummary["ServerTimeZone"] = TimeZoneInfo.Local.ToString();

			appendSummary["HostName"] = Dns.GetHostName();
			appendSummary["IpAddress"] = StaticMethods.GetServerIP();

            // Serilog info
            try
            {
                JsonNode serilog = AppEndSettings.Serilog;
                int batchLimit = serilog?["BatchPostingLimit"].ToString().ToIntSafe() ?? 0;
                int batchPeriodSeconds = serilog?["BatchPeriodSeconds"].ToString().ToIntSafe() ?? 0;
                if (batchLimit > 0) appendSummary["SerilogBatchPostingLimit"] = batchLimit;
                if (batchPeriodSeconds > 0) appendSummary["SerilogBatchPeriodSeconds"] = batchPeriodSeconds;
            }
            catch { }

            // DB server names
            try
            {
                List<string> dbNames = [];
                foreach (var n in AppEndSettings.DbServers)
                {
                    var jo = n.AsObject();
                    string name = jo["Name"].ToStringEmpty();
                    if (!string.IsNullOrWhiteSpace(name)) dbNames.Add(name);
                }
                appendSummary["DbServerNames"] = JArray.FromObject(dbNames);
            }
            catch { }

            // LLM provider names
            try
            {
                List<string> llmNames = [];
                foreach (var n in AppEndSettings.LLMProviders)
                {
                    var jo = n.AsObject();
                    string name = jo["Name"].ToStringEmpty();
                    if (!string.IsNullOrWhiteSpace(name)) llmNames.Add(name);
                }
                appendSummary["LLMProviderNames"] = JArray.FromObject(llmNames);
            }
            catch { }

			return appendSummary;
		}

    }
}
