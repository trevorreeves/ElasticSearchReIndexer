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

                while (source.Any() || !source.IsCompleted)
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
                }

                if (currentBatch.Any())
                {
                    destination.Add(currentBatch);
                }
            }
            //catch (TaskCanceledException) // not doing this yet - we just stop when our predecessor has finished.
            //{
            //
            //}
            catch (Exception ex)
            {
                //TODO: logging
                destination.CompleteAdding();
                
                throw;
            }
        }
    }
}
