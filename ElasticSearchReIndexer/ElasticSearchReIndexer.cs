using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElasticSearchReIndexer.Config;
using ElasticSearchReIndexer.Workers;
using ElasticSearchReIndexer.Steps;
using ElasticSearchReIndexer.Models;

namespace ElasticSearchReIndexer
{
    public class ElasticSearchReIndexer
    {
        private readonly ISourceScrollConfig _sourceConfig;
        private readonly ITargetIndexingConfig _targetConfig;

        public ElasticSearchReIndexer(
            ISourceScrollConfig sourceConfig,
            ITargetIndexingConfig targetConfig)
        {
            _sourceConfig = sourceConfig;
            _targetConfig = targetConfig;
        }

        public Task StartIndexingAsync()
        {
            var reIndexTaskCancellationUnit = new JobCancellationUnit();

            // tap - start scrolling - out = es docs
            var scroller = new EsScrollerStep(_sourceConfig);
            var sourceDocs = scroller.StartScrollingToEnd(reIndexTaskCancellationUnit);

            // batcher/transformer - start batching - in = es docs, out = es doc batches
            var batcher = new EsDocumentBatcherStep(_targetConfig.BatchSize);
            var sourceDocBatches = batcher.StartBatching(reIndexTaskCancellationUnit, sourceDocs);

            // TODO: throttler

            // sink - start indexing - in = es doc batches
            var indexer = new EsIndexerStep(_targetConfig);
            var indexingCancellationUnit = new JobCancellationUnit();
            return indexer.StartIndexingAsync(indexingCancellationUnit, sourceDocBatches);
        }
    }
}
