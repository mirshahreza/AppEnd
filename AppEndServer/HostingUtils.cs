using AppEndCommon;
using AppEndDynaCode;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using AngleSharp.Html;
using JWT;
using Microsoft.Extensions.Caching.Memory;
using System.Collections;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Net;

namespace AppEndServer
{
    public static class HostingUtils
    {
		public static JObject CreateStandardLogContent(MethodInfo methodInfo, string actor, string methodFullPath, string clientInfo, CodeInvokeResult codeInvokeResult, object[]? inputParams)
		{
			JObject methodInputs = inputParams is null ? "{}".ToJObjectByNewtonsoft() : inputParams.ExtractInputItems(methodInfo).ToJsonStringByBuiltIn().ToJObjectByNewtonsoft();
			string? recordId = null;
			if (methodInputs["ClientQueryJE"] != null && methodInputs["ClientQueryJE"]?["Params"] != null)
			{
				if (methodInputs["ClientQueryJE"]?["Params"] is JArray paramsArr)
				{
					foreach (JObject jo in paramsArr.Cast<JObject>())
					{
						if (jo["Name"]?.ToString() == "Id")
						{
							recordId = jo["Value"]?.ToString();
							break;
						}
					}
				}
			}

			return new()
			{
				["Method"] = methodFullPath,
				["IsSucceeded"] = codeInvokeResult.IsSucceeded,
				["FromCache"] = codeInvokeResult.FromCache,
				["RecordId"] = recordId,
				["EventBy"] = actor,
				["EventOn"] = DateTime.Now,
				["Duration"] = codeInvokeResult.Duration,
				["ClientInfo"] = clientInfo
			};
		}

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

        public static DirectoryInfo GetHostRootDirectory()
        {
            //return new DirectoryInfo(".");
            return new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
        }
        public static DirectoryInfo GetHostRootDirectoryDeep()
        {
            return new DirectoryInfo(".");
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


        public static bool PathIsManagable(this string path)
        {
            if (path.StartsWithIgnoreCase(".")) return false;
            if (path.StartsWithIgnoreCase("/properties")) return false;
            if (path.StartsWithIgnoreCase("/bin")) return false;
            if (path.StartsWithIgnoreCase("/obj")) return false;
            if (path.StartsWithIgnoreCase("/.config")) return false;
            if (path.ContainsIgnoreCase("DynaAsm")) return false;
            if (path.ContainsIgnoreCase(".csproj")) return false;
            if (path.ContainsIgnoreCase(".Development.")) return false;
            return true;
        }

		public static string NormalizePath(this string p, bool removeBasePath = true)
		{
			string s = p.Replace("\\", "/").Replace("//", "/");

            if (removeBasePath)
			{
				s = s.Replace(AppEndSettings.RootDeep.NormalizePath(false), "");
				return s;
			}
			else return s;
		}

    }
}
