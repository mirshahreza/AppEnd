using System.Text.Json;
using System.Text.Json.Nodes;
using System;
using System.IO;

namespace AppEndCommon
{
	public static class AppEndSettings
    {

		public const string ConfigSectionName = "AppEnd";
		public static List<string> ReservedFolders = ["a..lib", "a..templates", "appendstudio", "a.Components", "a.SharedComponents", "a.Layouts"];

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
						if (AppSettings == null || AppSettings[ConfigSectionName] == null) return [];
						AppSettings[ConfigSectionName]?[nameof(DbServers)] = JsonNode.Parse("[]")?.AsArray();
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

		private static JsonNode? _serilog;
		public static JsonNode Serilog
		{
			get
			{
				if (_serilog is null)
				{
					if (AppSettings[ConfigSectionName] == null) AppSettings[ConfigSectionName] = JsonNode.Parse("{}")?.AsObject();
					if (AppSettings[ConfigSectionName]?[nameof(Serilog)] == null)
					{
						if (AppSettings == null || AppSettings[ConfigSectionName] == null) return JsonNode.Parse("{}")?.AsObject();
						AppSettings[ConfigSectionName]?[nameof(Serilog)] = JsonNode.Parse("{}")?.AsObject();
						string s = JsonSerializer.Serialize(AppSettings, options: new()
						{
							WriteIndented = true
						});
						File.WriteAllText("appsettings.json", s);
						_appsettings = null;
					}
					_serilog = AppSettings[ConfigSectionName]?[nameof(Serilog)]?.AsObject();
				}
				return _serilog;
			}
		}

		public static DirectoryInfo ProjectRoot => new DirectoryInfo(".");
        public static DirectoryInfo PublishedRoot => new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);


        public static string WorkspacePath => "workspace";

        public static string ServerObjectsPath => $"{WorkspacePath}/server";

        public static string ApiCallsPath => $"{WorkspacePath}/apicalls";

        public static string ClientObjectsPath => $"{WorkspacePath}/client";
        public static string AppEndPackagesPath => $"{WorkspacePath}/appendpackages";

        public static string LoginDbConfName => AppSettings[ConfigSectionName]?[nameof(LoginDbConfName)]?.ToString() ?? "DefaultRepo";

        public static string LogDbConfName => AppSettings[ConfigSectionName]?[nameof(LogDbConfName)]?.ToString() ?? "DefaultRepo";

        public static int LogWriterQueueCap => AppSettings[ConfigSectionName]?[nameof(LogWriterQueueCap)]?.ToIntSafe() ?? 0;

        public static string TalkPoint => AppSettings[ConfigSectionName]?[nameof(TalkPoint)]?.ToString() ?? "talk-to-me";

        public static string PublicKeyRole => AppSettings[ConfigSectionName]?[nameof(PublicKeyRole)]?.ToString() ?? "";

        public static string DefaultSuccessLoggerMethod => AppSettings[ConfigSectionName]?[nameof(DefaultSuccessLoggerMethod)]?.ToString() ?? "";

		public static string DefaultErrorLoggerMethod => AppSettings[ConfigSectionName]?[nameof(DefaultErrorLoggerMethod)]?.ToString() ?? "";
		public static string DefaultDbConfName => AppSettings[ConfigSectionName]?[nameof(DefaultDbConfName)]?.ToString() ?? "";

		public static string PublicKeyUser => AppSettings[ConfigSectionName]?[nameof(PublicKeyUser)]?.ToString() ?? "";
		public static string LogsPath => AppSettings[ConfigSectionName]?[nameof(LogsPath)]?.ToString() ?? "log";
		public static string LogLevel => AppSettings[ConfigSectionName]?[nameof(LogLevel)]?.ToString() ?? "Information";
		public static int MaxLogFileSizeBytes => AppSettings[ConfigSectionName]?[nameof(MaxLogFileSizeBytes)]?.ToIntSafe() ?? 2048;

		

		public static string[]? PublicMethods => AppSettings[ConfigSectionName]?[nameof(PublicMethods)]?.ToString().DeserializeAsStringArray();

		public static string Secret => AppSettings[ConfigSectionName]?[nameof(Secret)]?.ToString() ?? ConfigSectionName;

		

		public static bool IsDevelopment => AppSettings[ConfigSectionName]?[nameof(IsDevelopment)]?.ToBooleanSafe() ?? false;
		public static bool EnableFileLogging => AppSettings[ConfigSectionName]?[nameof(EnableFileLogging)]?.ToBooleanSafe() ?? true;

		private static JsonNode? _appsettings;
        public static JsonNode AppSettings
        {
            get
            {
                if (_appsettings != null) return _appsettings;
                
                // Load base appsettings.json
                if (!File.Exists("appsettings.json")) throw new AppEndException("AppSettingsFileIsNotExist", System.Reflection.MethodBase.GetCurrentMethod()).GetEx();
                _appsettings = JsonNode.Parse(File.ReadAllText("appsettings.json"));
				if (_appsettings is null) throw new AppEndException("AppSettingsFileIsNotExist", System.Reflection.MethodBase.GetCurrentMethod()).GetEx();
				
				// Check if we're in Development environment and merge appsettings.Development.json if it exists
				string? environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
				if (environment != null && environment.Equals("Development", StringComparison.OrdinalIgnoreCase))
				{
					string devSettingsPath = "appsettings.Development.json";
					if (File.Exists(devSettingsPath))
					{
						try
						{
							var devSettings = JsonNode.Parse(File.ReadAllText(devSettingsPath));
							if (devSettings != null)
							{
								_appsettings = MergeJsonNodes(_appsettings, devSettings);
							}
						}
						catch
						{
							// If Development file has errors, continue with base settings
						}
					}
				}
				
				return _appsettings;
            }
        }

		private static JsonNode MergeJsonNodes(JsonNode baseNode, JsonNode overrideNode)
		{
			if (baseNode is JsonObject baseObj && overrideNode is JsonObject overrideObj)
			{
				// Create a deep copy of baseNode
				var merged = JsonNode.Parse(baseObj.ToJsonString())?.AsObject();
				if (merged == null) return overrideNode;
				
				foreach (var property in overrideObj)
				{
					if (property.Value == null)
					{
						merged[property.Key] = null;
						continue;
					}
					
					var existingValue = merged[property.Key];
					var overrideValue = property.Value;
					
					if (existingValue is JsonObject existingObj && overrideValue is JsonObject overrideValueObj)
					{
						// Recursively merge nested objects
						merged[property.Key] = MergeJsonNodes(existingObj, overrideValueObj);
					}
					else if (overrideValue is JsonArray)
					{
						// For arrays, replace with override array (deep copy)
						merged[property.Key] = JsonNode.Parse(overrideValue.ToJsonString());
					}
					else
					{
						// Replace or add the property (deep copy)
						merged[property.Key] = JsonNode.Parse(overrideValue.ToJsonString());
					}
				}
				
				return merged;
			}
			
			// If types don't match, return override
			return overrideNode;
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
