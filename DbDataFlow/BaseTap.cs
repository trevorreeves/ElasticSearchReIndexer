using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbDataFlow;

namespace ElasticSearchReIndexer.Steps
{
    public abstract class BaseTap<T> : ITap<T>
    {
        public BlockingCollection<T> StartFlowingToEnd(
            JobCancellationUnit cancellationUnit)
        {
            var stream = new BlockingCollection<T>();

            var streamTask = new Task(
                () => this.ScheduleWorkers(cancellationUnit, stream),
                cancellationUnit.Token);

            streamTask.Start();

            return stream;
        }

        protected abstract Action<BlockingCollection<T>, Action> GetWorker();

        private void ScheduleWorkers(
            JobCancellationUnit cancellationUnit,
            BlockingCollection<T> stream)
        {
            try
            {
                // single task / synchronous workers pattern...
                this.ThrowIfSuccessorCancelled(cancellationUnit);

                GetWorker()
                    (stream, 
                     () => this.ThrowIfSuccessorCancelled(cancellationUnit));
            }
            catch (TaskCanceledException)
            {
                // our sucessors should have stopped, but lets dot the i's...
                throw;
            }
            catch (Exception ex)
            {
                // because we're the first in the job, don't cause a cancel if we
                // fail, just say we're finished.  This gives everything further down
                // stream a chance to process all the data we've gathered so far.

                // TODO: logging

                throw; // currently throwing.  though I think this will cause a cancel 
                // on the token.  but we do want to report the exceptioned status of this
                // task, but just not cause everything else to stop immediately.
            }
            finally
            {
                stream.CompleteAdding();
            }
        }

        private void ThrowIfSuccessorCancelled(
            JobCancellationUnit cancellationUnit)
        {
            // if something cancelled the job, and we have no predecessor, so we can only
            // assume that is must have been a successor, so we must end.
            if (cancellationUnit.IsCancellationRequested)
            {
                cancellationUnit.ThrowIfCancelled();
            }
        }
    }
}
