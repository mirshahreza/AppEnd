using AppEndCommon;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System.Data;
using System.Text.Json.Nodes;

namespace AppEndApi
{
	public static class LogMan
	{
		private static readonly Lazy<ColumnOptions> _columnOptions = new(CreateColumnOptions);

		public static void SetupLoggers()
		{
			var loggerConf = new LoggerConfiguration().MinimumLevel.Verbose();

			loggerConf.WriteTo.Logger(lc => lc
				.Filter.ByIncludingOnly(e => (e.Level == Serilog.Events.LogEventLevel.Information))
				.WriteTo.Console(
					outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message}{NewLine}{Exception}"
				));

			loggerConf.WriteTo.Logger(lc => lc
				.Filter.ByIncludingOnly(e => (e.Level == Serilog.Events.LogEventLevel.Error))
				.WriteTo.File(
					path: "log/error-.txt",
					rollingInterval: RollingInterval.Hour,
					outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message}{NewLine}{Exception}",
					fileSizeLimitBytes: 10 * 1024 * 1024,
					retainedFileCountLimit: 30
				));

			loggerConf.WriteTo.Logger(lc => lc
				.Filter.ByIncludingOnly(e => (e.Level == Serilog.Events.LogEventLevel.Debug))
				.WriteTo.File(
					path: "log/debug-.txt",
					rollingInterval: RollingInterval.Hour,
					outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message}{NewLine}{Exception}",
					fileSizeLimitBytes: 10 * 1024 * 1024,
					retainedFileCountLimit: 30
				));

			loggerConf.WriteTo.Logger(lc => lc
				.Filter.ByIncludingOnly(e => (e.Level == Serilog.Events.LogEventLevel.Warning))
				.WriteTo.File(
					path: "log/warning-.txt",
					rollingInterval: RollingInterval.Hour,
					outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message}{NewLine}{Exception}",
					fileSizeLimitBytes: 10 * 1024 * 1024,
					retainedFileCountLimit: 30
				));

			loggerConf.WriteTo.Logger(lc => lc
				.Filter.ByIncludingOnly(e => (e.Level == Serilog.Events.LogEventLevel.Verbose))
				.WriteTo.MSSqlServer(
					connectionString: GetSerilogConnectionString(),
					sinkOptions: new MSSqlServerSinkOptions
					{
						TableName = GetSerilogTableName(),
						BatchPostingLimit = GetBatchPostingLimit(),
						BatchPeriod = GetBatchPeriodSeconds()
					},
					columnOptions: _columnOptions.Value // Use cached column options
				));

			Log.Logger = loggerConf.CreateLogger();
		}
		public static void LogConsole(string message)
		{
			Console.WriteLine(message);
		}
		public static void LogDebug(string message)
		{
			Log.Debug(message);
		}
		public static void LogError(string message)
		{
			Log.Error(message);
		}
		public static void LogError(Exception ex)
		{
			var errorMessage = new System.Text.StringBuilder(ex.Message.Length + (ex.StackTrace?.Length ?? 0) + 10);
			errorMessage.Append(ex.Message);
			errorMessage.Append(Environment.NewLine);
			if (ex.StackTrace != null)
				errorMessage.Append(ex.StackTrace);
			Log.Error(errorMessage.ToString());
		}
		public static void LogWarning(string message)
		{
			Log.Warning(message);
		}

		public static void LogActivity(string? namespaceName, string controller, string method, string? recordId, bool isSucceeded,bool fromCache ,string inputs,string? response, int duration, string clientIp, string clientAgent, int userId, string userName)
		{
			Log.Logger.Verbose("{Namespace}{Controller}{Method}{RecordId}{IsSucceeded}{FromCache}{Inputs}{Response}{Duration}{ClientIp}{ClientAgent}{EventById}{EventByName}{EventOn}",
				namespaceName, controller, method, recordId, isSucceeded, fromCache, inputs, response, duration, clientIp, clientAgent, userId, userName, DateTime.Now);
		}

		private static string GetSerilogConnectionString()
		{
			string cnnName = (AppEndSettings.Serilog["Connection"]?.ToString() ?? "DefaultRepo");
			JsonNode? dbNode = AppEndSettings.DbServers.FirstOrDefault(i => i?["Name"]?.ToString() == cnnName);
			if(dbNode== null) throw new AppEndException("SerilogDbConnectionIsNotExist", System.Reflection.MethodBase.GetCurrentMethod());
			return dbNode["ConnectionString"].ToStringEmpty();
		}
		private static string GetSerilogTableName()
		{
			return AppEndSettings.Serilog["TableName"]?.ToString() ?? "BaseActivityLog";
		}

		private static int GetBatchPostingLimit()
		{
			return (AppEndSettings.Serilog["BatchPostingLimit"]?.ToString() ?? "100").ToIntSafe();
		}
		private static TimeSpan GetBatchPeriodSeconds()
		{
			return new TimeSpan(0, 0, (AppEndSettings.Serilog["BatchPeriodSeconds"]?.ToString() ?? "15").ToIntSafe());
		}

		private static ColumnOptions CreateColumnOptions()
		{
			var columnOptions = new ColumnOptions();

			columnOptions.Store.Remove(StandardColumn.MessageTemplate);
			columnOptions.Store.Remove(StandardColumn.Message);
			columnOptions.Store.Remove(StandardColumn.Exception);
			columnOptions.Store.Remove(StandardColumn.Level);
			columnOptions.Store.Remove(StandardColumn.LogEvent);
			columnOptions.Store.Remove(StandardColumn.Properties);
			columnOptions.Store.Remove(StandardColumn.TimeStamp);

			columnOptions.Id.ColumnName = "Id";

			columnOptions.AdditionalColumns =
			[
				new SqlColumn() { ColumnName = "Namespace", DataType = SqlDbType.VarChar, DataLength = 64, AllowNull=true },
				new SqlColumn() { ColumnName = "Controller", DataType = SqlDbType.VarChar, DataLength = 64 },
				new SqlColumn() { ColumnName = "Method", DataType = SqlDbType.VarChar, DataLength = 64 },
				new SqlColumn() { ColumnName = "RecordId", DataType = SqlDbType.VarChar, DataLength = 64 },
				new SqlColumn() { ColumnName = "IsSucceeded", DataType = SqlDbType.Bit },
				new SqlColumn() { ColumnName = "FromCache", DataType = SqlDbType.Bit },
				new SqlColumn() { ColumnName = "Inputs", DataType = SqlDbType.NVarChar, DataLength=4000 },
				new SqlColumn() { ColumnName = "Response", DataType = SqlDbType.NVarChar, DataLength = 4000 },
				new SqlColumn() { ColumnName = "Duration", DataType = SqlDbType.Int },
				new SqlColumn() { ColumnName = "ClientIp", DataType = SqlDbType.VarChar, DataLength = 32 },
				new SqlColumn() { ColumnName = "ClientAgent", DataType = SqlDbType.VarChar, DataLength = 256 },
				new SqlColumn() { ColumnName = "EventById", DataType = SqlDbType.Int },
				new SqlColumn() { ColumnName = "EventByName", DataType = SqlDbType.NVarChar, DataLength = 64 },
				new SqlColumn() { ColumnName = "EventOn", DataType = SqlDbType.DateTime },
			];

			return columnOptions;
		}


	}
}