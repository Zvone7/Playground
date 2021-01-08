using WorkProvider.Infrastructrue;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WorkProvider.Services
{
    public class WorkProvider : IWorkProvider
    {
        private readonly IWorkDequeuer _workDequeuer_;
        private readonly Queue<IWork> _queue_;
        public WorkProvider(Queue<IWork> queue, IWorkDequeuer workDequeuer)
        {
            _workDequeuer_ = workDequeuer;
            _queue_ = queue;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var work = await _workDequeuer_.DequeueAsync();
                if (work != null)
                {
                    CustomLogger.Log(new String[] { $"Dequeued {work.ToString()}" });
                    _queue_.Enqueue(work);
                    CustomLogger.Log(new String[] { $"Enqueued to work queue {work.ToString()}" });
                }
            }
        }
    }
}
