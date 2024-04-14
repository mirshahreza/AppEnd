using Microsoft.Extensions.Hosting;
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

			return workItem;
		}

		public void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem)
		{
			if (workItem == null)
			{
				throw new ArgumentNullException(nameof(workItem));
			}

			_workItems.Enqueue(workItem);
			_signal.Release();
		}
	}

	public class AppEndLongRunningService : BackgroundService
	{
		private readonly AppEndBackgroundWorkerQueue queue;

		public AppEndLongRunningService(AppEndBackgroundWorkerQueue queue)
		{
			this.queue = queue;
		}

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
