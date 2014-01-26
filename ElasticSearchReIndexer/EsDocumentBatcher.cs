using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearchReIndexer.Workers
{
    public class EsDocumentBatcher
    {
        private readonly int _batchSize;

        public EsDocumentBatcher(int batchSize)
        {
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
                    if (currentBatch.Count() == _batchSize)
                    {
                        destination.Add(currentBatch);
                        currentBatch = new List<EsDocument>(_batchSize);
                    }

                    EsDocument currentDoc;
                    if (source.TryTake(out currentDoc, 5 * 1000, cancellationUnit.Token))
                    {
                        currentBatch.Add(currentDoc);
                    }

                    this.ThrowIfSuccessorCancelled(cancellationUnit, source);
                }

                if (currentBatch.Any())
                {
                    destination.Add(currentBatch);
                }
            }
            catch (TaskCanceledException) 
            {
                // our sucessors should have stopped, but lets dot the i's...
                destination.CompleteAdding();
                throw;
            }
            catch (Exception ex)
            {
                //TODO: logging
                cancellationUnit.Cancel(); // cancel predecessors
                destination.CompleteAdding(); // tell successors there's no more where that came from.
                
                throw;
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
