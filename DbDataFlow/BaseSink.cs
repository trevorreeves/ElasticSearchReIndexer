using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DbDataFlow;

namespace ElasticSearchReIndexer.Steps
{
    public abstract class BaseSink<T> : ISink<T>
    {
        public Task StartDrainingAsync(
            JobCancellationUnit cancellationUnit,
            BlockingCollection<T> stream)
        {
            var indexingTask = new Task(
                () => this.ScheduleWorkers(cancellationUnit, stream));

            return indexingTask;
        }

        protected abstract Action GetWorker(T data);

        private void ScheduleWorkers(
            JobCancellationUnit cancellationUnit,
            BlockingCollection<T> stream)
        {
            var indexTasks = new List<Task>();
            try
            {
                while (this.PossiblyMoreInSourceStream(stream))
                {
                    T currentItem;
                    if (stream.TryTake(out currentItem, 5 * 1000, cancellationUnit.Token))
                    {
                        var sinkWorkerTask = new Task(this.GetWorker(currentItem));

                        sinkWorkerTask.Start();
                        indexTasks.Add(sinkWorkerTask);
                    }
                }
                Task.WaitAll(indexTasks.ToArray());
            }
            catch (Exception)
            {
                // logging?

                // would be done anyway by TPL, but to be explicit about things...
                cancellationUnit.Cancel();

                throw;
            }
        }

        private bool PossiblyMoreInSourceStream(BlockingCollection<T> source)
        {
            return source.Any() || !source.IsCompleted;
        }
    }
}
