using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;

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
			UnRegisterTask("");
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

		public static Dictionary<string, JObject> GetQueueItems()
		{
			return QueuedWorkers;
        }

		private void RegisterTask(string taskName, JObject taskInfo)
		{
			QueuedWorkers.Add(taskName, taskInfo);
		}
		public static void UnRegisterTask(string taskName)
		{
			if (QueuedWorkers.ContainsKey(taskName)) QueuedWorkers.Remove(taskName);
		}

		public static bool InQueue(string taskName)
		{
			return QueuedWorkers.ContainsKey(taskName);
		}
	}

	public class AppEndLongRunningService(AppEndBackgroundWorkerQueue queue) : BackgroundService
	{
		private readonly AppEndBackgroundWorkerQueue queue = queue;

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				var workItem = await queue.DequeueAsync(stoppingToken);
				await workItem(stoppingToken);
			}
		}
	}

}
