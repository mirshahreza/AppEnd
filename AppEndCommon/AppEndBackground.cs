using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace AppEndCommon
{
	public class AppEndBackgroundWorkerQueue
	{
		public static Dictionary<string, JObject> QueuedWorkers = new Dictionary<string, JObject>();

		private ConcurrentQueue<Func<CancellationToken, Task>> _workItems = new();
		private SemaphoreSlim _signal = new(0);

		public async Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken)
		{
			await _signal.WaitAsync(cancellationToken);
			_workItems.TryDequeue(out var workItem);
			return workItem;
		}

		public void QueueBackgroundWorkItem(string taskName, JObject taskInfo, Func<CancellationToken, Task> workItem)
		{
			ArgumentNullException.ThrowIfNull(workItem);
			taskInfo["StartedOn"] = DateTime.Now.ToString();
			_workItems.Enqueue(workItem);
			_signal.Release();
			RegisterTask(taskName, taskInfo);
		}

		public static Dictionary<string, JObject> GetQueueItems(string likeStr)
		{
			Dictionary<string, JObject> queueItems = [];
			foreach (var item in QueuedWorkers)
			{
				if (likeStr == "" || item.Key.ContainsIgnoreCase(likeStr))
				{
					JObject newV = (JObject)item.Value.DeepClone();
					newV["ProgressState"] = QueueState(item.Key);
					queueItems.Add(item.Key, newV);
				}
			}
			return queueItems;
		}

		private void RegisterTask(string taskName, JObject taskInfo)
		{
			QueuedWorkers.Add(taskName, taskInfo);
		}
		public static void UnRegisterTask()
		{
			if (QueuedWorkers.Count > 0) QueuedWorkers.Remove(QueuedWorkers.First().Key);
		}

		public static string QueueState(string taskName)
		{
			if (!QueuedWorkers.ContainsKey(taskName)) return "NotExist";
			if (QueuedWorkers.Count > 0 && QueuedWorkers.First().Key == taskName) return "Running";
			return "Waiting";
		}

	}

	public class AppEndLongRunningService(AppEndBackgroundWorkerQueue queue) : BackgroundService
	{
		private readonly AppEndBackgroundWorkerQueue queue = queue;

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			try
			{
				while (!stoppingToken.IsCancellationRequested)
				{
					var workItem = await queue.DequeueAsync(stoppingToken);
					await workItem(stoppingToken);
				}
			}
			catch(Exception ex)
			{
				StaticMethods.LogImmed(ex.Message, filePreFix: "ExecuteAsync");
			}
			finally
			{
				
			}
		}
	}

}
