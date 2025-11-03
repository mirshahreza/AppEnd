using AppEndCommon;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Net;

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
						string v = match.Value.Replace("shared.translate(", "").Replace(")", "").Replace(@"""", "").Replace(@"'", "");
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

			return appendSummary;
		}

    }
}
