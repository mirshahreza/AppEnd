using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;

namespace AppEndCommon
{
	public class AppEndBackgroundWorkerQueue
	{
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
			_workItems.Enqueue(workItem);
			_signal.Release();
			RegisterTask(taskName, taskInfo);
		}

		public static Dictionary<string, JObject> GetQueueItems()
		{
			Dictionary<string, JObject> lst = [];
			foreach (var key in SV.SharedMemoryCache.GetKeys())
			{
				if (key.ToStringEmpty().StartsWith("BGW::"))
				{
					SV.SharedMemoryCache.TryGetValue(key, out object? obj);
					if(obj != null)
					{
						lst.Add(key.ToStringEmpty(), (JObject)obj);
					}
				}
			}
			return lst;
        }

		private void RegisterTask(string taskName, JObject taskInfo)
		{
			SV.SharedMemoryCache.Set(GetWorkerName(taskName), taskInfo);
		}
		public static void UnRegisterTask(string taskName)
		{
			SV.SharedMemoryCache.TryRemove(GetWorkerName(taskName));
		}

		public static bool InQueue(string taskName)
		{
			if (SV.SharedMemoryCache.Get(GetWorkerName(taskName)) == null) return false;
			return true;
		}

		public static string GetWorkerName(string taskName)
		{
			return $"BGW::{taskName}";
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
