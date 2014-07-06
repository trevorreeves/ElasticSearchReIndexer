using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElasticSearchReIndexer.Models;

namespace ElasticSearchReIndexer.Steps
{
    public class EsDocumentBatcherStep : IBatcher<EsDocument>
    {
        private readonly int _batchSize;

        public EsDocumentBatcherStep(int batchSize)
        {
            Contract.Requires(batchSize > 0);

            _batchSize = batchSize;
        }

        public BlockingCollection<List<EsDocument>> StartBatching(
            JobCancellationUnit cancellationUnit,
            BlockingCollection<EsDocument> source)
        {
            var docBatches = new BlockingCollection<List<EsDocument>>();

            var batchTask = new Task(
                () => this.ScheduleDocBatching(cancellationUnit, source, docBatches),
                cancellationUnit.Token);

            batchTask.Start();

            return docBatches;
        }

        private void ScheduleDocBatching(
            JobCancellationUnit cancellationUnit,
            BlockingCollection<EsDocument> source,
            BlockingCollection<List<EsDocument>> destination)
        {
            try
            {
                var currentBatch = new List<EsDocument>(_batchSize);

                this.ThrowIfSuccessorCancelled(cancellationUnit, source);

                while (this.PossiblyMoreInSourceStream(source))
                {
                    // close the current batch if its full
                    if (currentBatch.Count() == _batchSize)
                    {
                        destination.Add(currentBatch);
                        currentBatch = new List<EsDocument>(_batchSize);
                    }

                    // add another doc to the current batch
                    EsDocument currentDoc;
                    if (source.TryTake(out currentDoc, 5 * 1000, cancellationUnit.Token))
                    {
                        currentBatch.Add(currentDoc);
                    }

                    this.ThrowIfSuccessorCancelled(cancellationUnit, source);
                }

                // ensure all docs are pushed when total number is not exact multiple
                // of batch size.
                if (currentBatch.Any())
                {
                    destination.Add(currentBatch);
                }
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
                destination.CompleteAdding();
            }
        }

        private void ThrowIfSuccessorCancelled(
            JobCancellationUnit cancellationUnit,
            BlockingCollection<EsDocument> source)
        {
            // if something cancelled the job, but our predecessors stream is still going, we can
            // assume that our successor died, so we should stop sending it data...
            if (cancellationUnit.IsCancellationRequested && !source.IsCompleted)
            {
                cancellationUnit.ThrowIfCancelled();
            }
        }

        private bool PossiblyMoreInSourceStream(BlockingCollection<EsDocument> source)
        {
            return source.Any() || !source.IsCompleted;
        }
    }
}
