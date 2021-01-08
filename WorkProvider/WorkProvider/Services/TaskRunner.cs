using WorkProvider.Infrastructrue;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WorkProvider.Services
{
    public class TaskRunner
    {
        private readonly IWorkDequeuer _workDequeuer_;
        private readonly Int32 _cancelAfterSeconds_;
        public TaskRunner(IWorkDequeuer workDequeuer, Int32 cancelAfterSeconds)
        {
            _workDequeuer_ = workDequeuer;
            _cancelAfterSeconds_ = cancelAfterSeconds;
        }

        public async Task RunTaskAsync()
        {
            var workQueue = new Queue<IWork>();
            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(_cancelAfterSeconds_));
            var startedAt = DateTime.UtcNow;

            var workProvider = new WorkProvider(workQueue, _workDequeuer_);

            Task task = Task.Run(async () => await workProvider.RunAsync(cancellationTokenSource.Token));

            while (!cancellationTokenSource.IsCancellationRequested)
            {
                CustomLogger.LogInGray(new String[] { $"Application running for {(Int32)(DateTime.UtcNow - startedAt).TotalSeconds + 1} seconds. Exiting after {_cancelAfterSeconds_} seconds." });

                if (workQueue.Count > 0)
                {
                    if (workQueue.TryDequeue(out IWork work))
                    {
                        await work.Execute();
                    }
                }

                // not needed, just to simplify the app logs
                await Task.Delay(1000);
            }

            CustomLogger.Log(new String[] { $"Task finished." });
        }

    }
}
