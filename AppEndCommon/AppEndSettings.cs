using System.Text.Json;
using System.Text.Json.Nodes;

namespace AppEndCommon
{
	public static class AppEndSettings
    {

		public const string ConfigSectionName = "AppEnd";
		public static List<string> ReservedFolders = ["..lib", "..templates", "appendstudio", ".DbComponents", ".PublicComponents", ".SharedComponents", ".Layouts", ".UserComponents"];

		private static JsonArray? _dbServers;
        public static JsonArray DbServers
        {
            get
            {
                if (_dbServers is null)
                {
                    if (AppSettings[ConfigSectionName] == null) AppSettings[ConfigSectionName] = JsonNode.Parse("{}")?.AsObject();
                    if (AppSettings[ConfigSectionName]?[nameof(DbServers)] == null)
                    {
						if (AppSettings[ConfigSectionName] == null)
						{
							throw new AppEndException("AppSettingsFileMustContains")
								.AddParam("Section", "AppEnd:ServerObjectsPath")
								.AddParam("Site", $"{System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name}, {System.Reflection.MethodBase.GetCurrentMethod()?.Name}")
								;
						}
						AppSettings[ConfigSectionName][nameof(DbServers)] = JsonNode.Parse("[]")?.AsArray();
                        string s = JsonSerializer.Serialize(AppSettings, options: new()
                        {
                            WriteIndented = true
                        });
                        File.WriteAllText("appsettings.json", s);
                        _appsettings = null;
                    }
                    _dbServers = AppSettings[ConfigSectionName]?[nameof(DbServers)]?.AsArray();
                }
                return _dbServers;
            }
        }

		private static JsonArray? _nodes;
		public static JsonArray Nodes
		{
			get
			{
				if (_nodes is null)
				{
					if (AppSettings[ConfigSectionName] == null) AppSettings[ConfigSectionName] = JsonNode.Parse("{}")?.AsObject();
					if (AppSettings[ConfigSectionName]?[nameof(Nodes)] == null)
					{
						if (AppSettings[ConfigSectionName] == null)
						{
							throw new AppEndException("AppSettingsFileMustContains")
								.AddParam("Section", "AppEnd:ServerObjectsPath")
								.AddParam("Site", $"{System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name}, {System.Reflection.MethodBase.GetCurrentMethod()?.Name}")
								;
						}
						AppSettings[ConfigSectionName][nameof(Nodes)] = JsonNode.Parse("[]")?.AsArray();
						Save();
					}
					_nodes = AppSettings[ConfigSectionName]?[nameof(Nodes)]?.AsArray();
				}
				return _nodes;
			}
			set
			{
				AppSettings[ConfigSectionName][nameof(Nodes)] = value?.AsArray();
				Save();
			}
		}

		public static string WorkspacePath => "workspace";

        public static string ServerObjectsPath => $"{WorkspacePath}/server";

        public static string ApiCallsPath => $"{WorkspacePath}/apicalls";

        public static string ClientObjectsPath => $"{WorkspacePath}/client";

        public static string LoginDbConfName => AppSettings[ConfigSectionName]?[nameof(LoginDbConfName)]?.ToString() ?? "DefaultRepo";

        public static string LogDbConfName => AppSettings[ConfigSectionName]?[nameof(LogDbConfName)]?.ToString() ?? "DefaultRepo";

        public static int LogWriterQueueCap => AppSettings[ConfigSectionName]?[nameof(LogWriterQueueCap)]?.ToIntSafe() ?? 0;

        public static string TalkPoint => AppSettings[ConfigSectionName]?[nameof(TalkPoint)]?.ToString() ?? "talk-to-me";

        public static string PublicKeyRole => AppSettings[ConfigSectionName]?[nameof(PublicKeyRole)]?.ToString() ?? "";

        public static string DefaultSuccessLoggerMethod => AppSettings[ConfigSectionName]?[nameof(DefaultSuccessLoggerMethod)]?.ToString() ?? "";

        public static string DefaultErrorLoggerMethod => AppSettings[ConfigSectionName]?[nameof(DefaultErrorLoggerMethod)]?.ToString() ?? "";

        public static string PublicKeyUser => AppSettings[ConfigSectionName]?[nameof(PublicKeyUser)]?.ToString() ?? "";

        public static string[]? PublicMethods => AppSettings[ConfigSectionName]?[nameof(PublicMethods)]?.ToString().DeserializeAsStringArray();

		public static string Secret => AppSettings[ConfigSectionName]?[nameof(Secret)]?.ToString() ?? ConfigSectionName;
		//public static string Nodes => AppSettings[ConfigSectionName]?[nameof(Nodes)]?.ToString() ?? ConfigSectionName;


		private static JsonNode? _appsettings;
        public static JsonNode AppSettings
        {
            get
            {
                if (!File.Exists("appsettings.json")) throw new AppEndException("AppSettingsFileIsNotExist")
                    .AddParam("Site", $"{System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name}, {System.Reflection.MethodBase.GetCurrentMethod()?.Name}")
                            ;

                _appsettings ??= JsonNode.Parse(File.ReadAllText("appsettings.json"));

				if (_appsettings is null) throw new AppEndException("AppSettingsFileIsNotExist")
					.AddParam("Site", $"{System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name}, {System.Reflection.MethodBase.GetCurrentMethod()?.Name}")
							;
				return _appsettings;
            }
        }

        public static void Save()
        {
			string appSettingsText = JsonSerializer.Serialize(AppSettings, options: new()
			{
				WriteIndented = true
			});
			File.WriteAllText("appsettings.json", appSettingsText);
			RefereshSettings();
		}

        public static void RefereshSettings()
        {
            _appsettings = null;
        }

    }
}
