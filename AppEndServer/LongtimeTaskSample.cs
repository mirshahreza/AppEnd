using AppEndCommon;
using Newtonsoft.Json.Linq;

namespace AppEndServer
{
	public static class LongtimeTaskSample
	{

		public static object? PrepareTastToStart(AppEndBackgroundWorkerQueue backgroundWorker, string theKey)
		{
			if (!SV.SharedMemoryCache.TryGetValue(theKey, out object? result))
			{
				JObject joInfo = new()
				{
					["Key"] = theKey,
					["StartedOn"] = DateTime.Now.ToString(),
					["ProgressState"] = "Queued"
				};

				joInfo.TryRemoveProperty("Password");
				joInfo.TryRemoveProperty("FilesToDo");
				joInfo.TryRemoveProperty("ProgressState");

				backgroundWorker.QueueBackgroundWorkItem(theKey, joInfo, async token =>
				{
					await StartTheTask(backgroundWorker, joInfo);
				});
			}
			return true;
		}

		private static Task StartTheTask(AppEndBackgroundWorkerQueue backgroundWorker, JObject joInfo)
		{
			try
			{
				Console.WriteLine($"started at {DateTime.Now}.");
				// a log time task
				Console.WriteLine($"finishet at {DateTime.Now}.");
			}
			catch (Exception ex)
			{
				StaticMethods.LogImmed(ex.Message, "log", "", "deploy_");
			}
			finally
			{
				
			}
			return Task.CompletedTask;
		}

	}
}
