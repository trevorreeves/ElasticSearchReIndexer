using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbDataFlow
{
    public abstract class BaseTransformer<T, TTransformed> : 
        ITransformer<T, TTransformed>
    {
        public BlockingCollection<TTransformed> StartTransforming(
            JobCancellationUnit cancellationUnit, 
            BlockingCollection<T> source)
        {
            var transformations = new BlockingCollection<TTransformed>();

            var batchTask = new Task(
                () => this.ScheduleTransforming(cancellationUnit, source, transformations),
                cancellationUnit.Token);

            batchTask.Start();

            return transformations;
        }

        protected abstract void Transform(
            JobCancellationUnit cancellationUnit,
            BlockingCollection<T> source, 
            BlockingCollection<TTransformed> transformations);

        private void ScheduleTransforming(
            JobCancellationUnit cancellationUnit,
            BlockingCollection<T> source,
            BlockingCollection<TTransformed> transformations)
        {
            try
            {
                this.ThrowIfSuccessorCancelled(cancellationUnit, source);

                Transform(cancellationUnit, source, transformations);
            }
            catch (TaskCanceledException)
            {
                // our sucessors should have stopped, but lets dot the i's...
                throw;
            }
            catch (Exception)
            {
                //TODO: logging
                cancellationUnit.Cancel(); // cancel predecessors

                throw;
            }
            finally
            {
                transformations.CompleteAdding();
            }
        }

        protected void ThrowIfSuccessorCancelled(
            JobCancellationUnit cancellationUnit,
            BlockingCollection<T> source)
        {
            // if something cancelled the job, but our predecessors stream is still going, we can
            // assume that our successor died, so we should stop sending it data...
            if (cancellationUnit.IsCancellationRequested && !source.IsCompleted)
            {
                cancellationUnit.ThrowIfCancelled();
            }
        }
    }
}
