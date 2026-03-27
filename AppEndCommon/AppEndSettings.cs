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
						File.WriteAllText(GetSettingsFilePath(forWrite: true), s);
						_appsettings = null;
					}
					_dbServers = AppSettings[ConfigSectionName]?[nameof(DbServers)]?.AsArray();
				}
				return _dbServers;
			}
		}

		private static JsonArray? _llmProviders;
		public static JsonArray LLMProviders
		{
			get
			{
				if (_llmProviders is null)
				{
					if (AppSettings[ConfigSectionName] == null) AppSettings[ConfigSectionName] = JsonNode.Parse("{}")?.AsObject();
					if (AppSettings[ConfigSectionName]?[nameof(LLMProviders)] == null)
					{
						if (AppSettings == null || AppSettings[ConfigSectionName] == null) return [];
						AppSettings[ConfigSectionName]?[nameof(LLMProviders)] = JsonNode.Parse("[]")?.AsArray();
						string s = JsonSerializer.Serialize(AppSettings, options: new()
						{
							WriteIndented = true
						});
						File.WriteAllText(GetSettingsFilePath(forWrite: true), s);
						_appsettings = null;
					}
					_llmProviders = AppSettings[ConfigSectionName]?[nameof(LLMProviders)]?.AsArray();
				}
				return _llmProviders;
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
						File.WriteAllText(GetSettingsFilePath(forWrite: true), s);
						_appsettings = null;
					}
					_serilog = AppSettings[ConfigSectionName]?[nameof(Serilog)]?.AsObject();
				}
				return _serilog;
			}
		}

		private static JsonNode? _aaa;
		public static JsonNode AAA
		{
			get
			{
				if (_aaa is null)
				{
					if (AppSettings[ConfigSectionName] == null) AppSettings[ConfigSectionName] = JsonNode.Parse("{}")?.AsObject();
					if (AppSettings[ConfigSectionName]?[nameof(AAA)] == null)
					{
						// Backward compatible: if legacy keys exist at root, keep them until next save
						AppSettings[ConfigSectionName]?[nameof(AAA)] = JsonNode.Parse("{}")?.AsObject();
						string s = JsonSerializer.Serialize(AppSettings, options: new() { WriteIndented = true });
						File.WriteAllText(GetSettingsFilePath(forWrite: true), s);
						_appsettings = null;
					}
					_aaa = AppSettings[ConfigSectionName]?[nameof(AAA)]?.AsObject();
				}
				return _aaa;
			}
		}

		public static DirectoryInfo ProjectRoot => new DirectoryInfo(".");
        public static DirectoryInfo PublishedRoot => new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);


        public static string WorkspacePath => "workspace";

        public static string ServerObjectsPath => $"{WorkspacePath}/server";

        public static string ApiCallsPath => $"{WorkspacePath}/apicalls";
        public static string SqlQueriesPath => $"{WorkspacePath}/sqlqueries";

        public static string ClientObjectsPath => $"{WorkspacePath}/client";
        public static string AppEndPackagesPath => $"{WorkspacePath}/appendpackages";

        public static string LoginDbConfName
        {
            get
            {
                // Prefer AAA section; fallback to legacy key for backward compatibility
                return AAA?[nameof(LoginDbConfName)]?.ToString() ?? AppSettings[ConfigSectionName]?[nameof(LoginDbConfName)]?.ToString() ?? "DefaultRepo";
            }
        }

        public static string TalkPoint => AppSettings[ConfigSectionName]?[nameof(TalkPoint)]?.ToString() ?? "talk-to-me";

        public static string PublicKeyRole
        {
            get
            {
                return AAA?[nameof(PublicKeyRole)]?.ToString() ?? AppSettings[ConfigSectionName]?[nameof(PublicKeyRole)]?.ToString() ?? "";
            }
        }

        public static string DefaultSuccessLoggerMethod => AppSettings[ConfigSectionName]?[nameof(DefaultSuccessLoggerMethod)]?.ToString() ?? "";

		public static string DefaultErrorLoggerMethod => AppSettings[ConfigSectionName]?[nameof(DefaultErrorLoggerMethod)]?.ToString() ?? "";
		public static string DefaultDbConfName => AppSettings[ConfigSectionName]?[nameof(DefaultDbConfName)]?.ToString() ?? "";

		/// <summary>Language used for Native fields (HumanTitleNative, NoteNative, KeywordsNative) in schema enrichment. E.g. "Persian", "Farsi", "Arabic".</summary>
		public static string EnrichmentNativeLanguage => AppSettings[ConfigSectionName]?[nameof(EnrichmentNativeLanguage)]?.ToString()?.Trim() ?? "Persian";

		public static string PublicKeyUser
        {
            get
            {
                return AAA?[nameof(PublicKeyUser)]?.ToString() ?? AppSettings[ConfigSectionName]?[nameof(PublicKeyUser)]?.ToString() ?? "";
            }
        }
		public static string LogsPath => AppSettings[ConfigSectionName]?[nameof(LogsPath)]?.ToString() ?? "log";
		public static string LogLevel => AppSettings[ConfigSectionName]?[nameof(LogLevel)]?.ToString() ?? "Information";
		public static int MaxLogFileSizeBytes => AppSettings[ConfigSectionName]?[nameof(MaxLogFileSizeBytes)]?.ToIntSafe() ?? 2048;

		

		public static string[]? PublicMethods
        {
            get
            {
                var pmAAA = AAA?[nameof(PublicMethods)]?.ToString().DeserializeAsStringArray();
                if (pmAAA != null) return pmAAA;
                return AppSettings[ConfigSectionName]?[nameof(PublicMethods)]?.ToString().DeserializeAsStringArray();
            }
        }

		public static string Secret => AppSettings[ConfigSectionName]?[nameof(Secret)]?.ToString() ?? ConfigSectionName;

		/// <summary>Access token validity in minutes. Backend only - frontend reacts to 401.</summary>
		public static int AccessTokenValidMinutes => AAA?[nameof(AccessTokenValidMinutes)]?.ToIntSafe() ?? AppSettings[ConfigSectionName]?[nameof(AccessTokenValidMinutes)]?.ToIntSafe() ?? 15;

		/// <summary>Refresh token validity in days.</summary>
		public static int RefreshTokenValidDays => AAA?[nameof(RefreshTokenValidDays)]?.ToIntSafe() ?? AppSettings[ConfigSectionName]?[nameof(RefreshTokenValidDays)]?.ToIntSafe() ?? 7;

		// Make Development detection robust: honor ASPNETCORE_ENVIRONMENT=Development or config flag
		public static bool IsDevelopment
		{
			get
			{
				var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
				bool envDev = env != null && env.Equals("Development", StringComparison.OrdinalIgnoreCase);
				bool cfgDev = AppSettings[ConfigSectionName]?[nameof(IsDevelopment)]?.ToBooleanSafe() ?? false;
				return envDev || cfgDev;
			}
		}
		public static bool EnableFileLogging => AppSettings[ConfigSectionName]?[nameof(EnableFileLogging)]?.ToBooleanSafe() ?? true;

		private static JsonNode? _appsettings;
        public static JsonNode AppSettings
        {
            get
            {
                if (_appsettings != null) return _appsettings;
                
                // Load base appsettings.json
                string basePath = GetSettingsFilePath(forWrite: false);
                if (!File.Exists(basePath)) throw new AppEndException("AppSettingsFileIsNotExist", System.Reflection.MethodBase.GetCurrentMethod()).GetEx();
                var baseSettings = JsonNode.Parse(File.ReadAllText(basePath));
				if (baseSettings is null) throw new AppEndException("AppSettingsFileIsNotExist", System.Reflection.MethodBase.GetCurrentMethod()).GetEx();
				
				// If Development and dev settings exist, use dev settings entirely (no merge) to avoid leaking base-only sections
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
								_appsettings = devSettings;
								return _appsettings;
							}
						}
						catch
						{
							// If Development file has errors, continue with base settings
						}
					}
				}
				
				// Fallback: use base settings
				_appsettings = baseSettings;
				return _appsettings;
            }
        }

		private static string GetSettingsFilePath(bool forWrite)
		{
			// Prefer Development file when environment is Development
			var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
			bool envDev = env != null && env.Equals("Development", StringComparison.OrdinalIgnoreCase);
			string devPath = "appsettings.Development.json";
			if (envDev)
			{
				// If writing, create dev file if missing; if reading, use dev if exists
				if (forWrite)
				{
					return devPath;
				}
				else if (File.Exists(devPath))
				{
					return devPath;
				}
			}
			return "appsettings.json";
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
			File.WriteAllText(GetSettingsFilePath(forWrite: true), appSettingsText);
			RefereshSettings();
		}

        public static void RefereshSettings()
        {
            _appsettings = null;
			_llmProviders = null;
			_dbServers = null;
			_serilog = null;
			_aaa = null;
        }

    }
}
