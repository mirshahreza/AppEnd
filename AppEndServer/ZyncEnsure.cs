using AppEndCommon;
using AppEndDbIO;
using System.Data;
using System.Data.Common;
using System.Net.Http;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

namespace AppEndServer
{
	/// <summary>
	/// Ensures Zync database package manager and required packages (Base, DbMan, DbMon) are installed/updated at startup.
	/// Uses https://github.com/mirshahreza/Zync - installs or updates packages so the app runs without missing tables/objects.
	/// </summary>
	public static class ZyncEnsure
	{
		private const string ZyncSqlBootstrapUrl = "https://raw.githubusercontent.com/mirshahreza/Zync/main/MsSql/Zync.sql";
		private const string DefaultZyncRepoUrl = "https://raw.githubusercontent.com/mirshahreza/Zync/main/MsSql/Packages/";
		private static readonly string[] DefaultPackages = ["Base", "DbMan", "DbMon"];

		/// <summary>
		/// Ensures Zync procedure exists (bootstraps from GitHub if missing), then installs/updates configured packages.
		/// Safe to call on every startup; logs and skips on failure without crashing the app.
		/// </summary>
		/// <param name="dbConfName">Database configuration name (e.g. DefaultRepo, LoginDbConfName).</param>
		public static void EnsurePackages(string dbConfName)
		{
			if (string.IsNullOrWhiteSpace(dbConfName))
				dbConfName = AppEndSettings.LoginDbConfName;

			bool ensureEnabled = GetEnsureZyncOnStartup();
			if (!ensureEnabled)
			{
				LogMan.LogDebug("[ZyncEnsure] EnsureZyncOnStartup is disabled; skipping.");
				return;
			}

			string repoUrl = GetZyncRepoUrl();
			string[] packages = GetZyncPackages();

			try
			{
				using var dbIO = DbIO.Instance(DbConf.FromSettings(dbConfName));
				bool zyncExists = ZyncProcedureExists(dbIO);
				if (!zyncExists)
				{
					LogMan.LogDebug("[ZyncEnsure] Zync procedure not found; attempting bootstrap from GitHub...");
					BootstrapZyncCore(dbIO);
				}

				// Re-check after bootstrap (bootstrap might have failed)
				zyncExists = ZyncProcedureExists(dbIO);
				if (!zyncExists)
				{
					LogMan.LogError("[ZyncEnsure] Zync procedure still missing after bootstrap. Run Zync.sql manually on database. Skipping package install.");
					return;
				}

				foreach (string package in packages)
				{
					if (string.IsNullOrWhiteSpace(package)) continue;
					try
					{
						RunZyncInstall(dbIO, package.Trim(), repoUrl);
					}
					catch (Exception ex)
					{
						LogMan.LogError($"[ZyncEnsure] Failed to install/update package '{package}': {ex.Message}");
					}
				}
			}
			catch (Exception ex)
			{
				LogMan.LogError($"[ZyncEnsure] EnsurePackages failed for '{dbConfName}': {ex.Message}. App will continue; fix DB connection or Zync setup if needed.");
			}
		}

		private static bool ZyncProcedureExists(DbIO dbIO)
		{
			try
			{
				var result = dbIO.ToScalar(
					"SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Zync]') AND type = 'P'");
				return result is not null && result != DBNull.Value && Convert.ToInt32(result) == 1;
			}
			catch
			{
				return false;
			}
		}

		private static void BootstrapZyncCore(DbIO dbIO)
		{
			using var http = new HttpClient();
			http.Timeout = TimeSpan.FromSeconds(60);
			string sql;
			try
			{
				sql = http.GetStringAsync(ZyncSqlBootstrapUrl).GetAwaiter().GetResult();
			}
			catch (Exception ex)
			{
				LogMan.LogError($"[ZyncEnsure] Could not download Zync.sql from GitHub: {ex.Message}. Run Zync.sql manually.");
				return;
			}

			if (string.IsNullOrWhiteSpace(sql))
			{
				LogMan.LogError("[ZyncEnsure] Zync.sql content empty.");
				return;
			}

			string[] batches = SplitSqlBatches(sql);
			int executed = 0;
			foreach (string batch in batches)
			{
				string b = batch.Trim();
				if (b.Length == 0) continue;
				try
				{
					dbIO.ToNoneQuery(b);
					executed++;
				}
				catch (Exception ex)
				{
					LogMan.LogError($"[ZyncEnsure] Bootstrap batch error: {ex.Message}");
				}
			}
			LogMan.LogDebug($"[ZyncEnsure] Bootstrap executed {executed} batch(es).");
		}

		/// <summary>
		/// Splits T-SQL script by GO (batch separator). Handles GO on its own line.
		/// </summary>
		private static string[] SplitSqlBatches(string script)
		{
			if (string.IsNullOrWhiteSpace(script)) return [];
			// Match "GO" as whole line (optional whitespace, GO, optional whitespace, optional semicolon)
			var goRegex = new Regex(@"^\s*GO\s*;?\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
			string[] parts = goRegex.Split(script);
			return parts.Select(p => p.Trim()).Where(p => p.Length > 0).ToArray();
		}

		private static void RunZyncInstall(DbIO dbIO, string packageName, string repoUrl)
		{
			var dbParams = new List<DbParameter>
			{
				new SqlParameter("@Command", SqlDbType.VarChar, 128) { Value = "i " + packageName },
				new SqlParameter("@Repo", SqlDbType.VarChar, 512) { Value = repoUrl }
			};
			dbIO.ToNoneQuery("EXEC [dbo].[Zync] @Command, @Repo", dbParams);
			LogMan.LogDebug($"[ZyncEnsure] Package '{packageName}' ensured.");
		}

		private static bool GetEnsureZyncOnStartup()
		{
			try
			{
				var v = AppEndSettings.AppSettings[AppEndSettings.ConfigSectionName]?["EnsureZyncOnStartup"];
				if (v is null) return true;
				return v.ToStringEmpty().ToLowerInvariant() is not ("false" or "0" or "no");
			}
			catch { return true; }
		}

		private static string GetZyncRepoUrl()
		{
			try
			{
				var url = AppEndSettings.AppSettings[AppEndSettings.ConfigSectionName]?["ZyncRepoUrl"]?.ToStringEmpty().Trim();
				return string.IsNullOrEmpty(url) ? DefaultZyncRepoUrl : (url.EndsWith("/", StringComparison.Ordinal) ? url : url + "/");
			}
			catch { return DefaultZyncRepoUrl; }
		}

		private static string[] GetZyncPackages()
		{
			try
			{
				var node = AppEndSettings.AppSettings[AppEndSettings.ConfigSectionName]?["ZyncPackages"];
				if (node is null) return DefaultPackages;
				if (node is System.Text.Json.Nodes.JsonArray arr)
				{
					var list = new List<string>();
					foreach (var item in arr)
						if (item?.ToString() is { } s && !string.IsNullOrWhiteSpace(s))
							list.Add(s.Trim());
					return list.Count > 0 ? [.. list] : DefaultPackages;
				}
				if (node.ToString() is { } str && !string.IsNullOrWhiteSpace(str))
					return str.Split(',', ';').Select(s => s.Trim()).Where(s => s.Length > 0).ToArray();
			}
			catch { }
			return DefaultPackages;
		}
	}
}
