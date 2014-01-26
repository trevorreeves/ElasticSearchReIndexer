using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElasticSearchReIndexer.Config;

namespace ElasticSearchReIndexer
{
    public class EsIndexer
    {
        private readonly ITargetIndexingConfig _config;

        public EsIndexer(ITargetIndexingConfig config)
        {
            _config = config;
        }

        public void StartIndexing(
            JobCancellationUnit cancellationUnit,
            BlockingCollection<List<EsDocument>> sourceBatches)
        {
            var indexingTask = new Task(
                () => ScheduleIndexWorkers(cancellationUnit, sourceBatches),
                cancellationUnit.Token);

            indexingTask.Start();
        }

        private void ScheduleIndexWorkers(
            JobCancellationUnit cancellationUnit,
            BlockingCollection<List<EsDocument>> sourceBatches)
        {
            try
            {
                while (this.PossiblyMoreInSourceStream(sourceBatches))
                {
                    List<EsDocument> currentBatch;
                    if (sourceBatches.TryTake(out currentBatch, 5 * 1000, cancellationUnit.Token))
                    {
                        var batchIndexTask = new Task(
                            () =>
                            {
                                var indexer = new IndexWorker(_config);
                                indexer.Index(currentBatch);
                            },
                            cancellationUnit.Token);

                        batchIndexTask.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                // logging?

                // would be done anyway by TPL, but to be explicit about things...
                cancellationUnit.Cancel();

                throw;
            }
        }

        private bool PossiblyMoreInSourceStream(BlockingCollection<List<EsDocument>> source)
        {
            return source.Any() || !source.IsCompleted;
        }
    }
}
