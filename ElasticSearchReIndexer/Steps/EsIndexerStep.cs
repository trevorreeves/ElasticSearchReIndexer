using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElasticSearchReIndexer.Clients;
using ElasticSearchReIndexer.Config;
using ElasticSearchReIndexer.Models;
using ElasticSearchReIndexer.Workers;

namespace ElasticSearchReIndexer.Steps
{
    public class EsIndexerStep : IEsIndexerStep
    {
        private readonly ITargetIndexingConfig _config;

        public EsIndexerStep(ITargetIndexingConfig config)
        {
            _config = config;
        }

        public Task StartIndexingAsync(
            JobCancellationUnit cancellationUnit,
            BlockingCollection<List<EsDocument>> sourceBatches)
        {
            var indexingTask = new Task(
                () => ScheduleIndexWorkers(cancellationUnit, sourceBatches));

            return indexingTask;
        }

        private void ScheduleIndexWorkers(
            JobCancellationUnit cancellationUnit,
            BlockingCollection<List<EsDocument>> sourceBatches)
        {
            var indexTasks = new List<Task>();
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
                                var indexer = new IndexWorker(_config, new EsIndexClient(_config));
                                indexer.Index(currentBatch);
                            });

                        batchIndexTask.Start();
                        indexTasks.Add(batchIndexTask);
                    }
                }
                Task.WaitAll(indexTasks.ToArray());

                new EsIndexClient(_config).Refresh();
            }
            catch (Exception)
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
