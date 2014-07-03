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
        private readonly ITargetIndexingConfig _targetConfig;
        private readonly IEsScrollerStep _scroller;
        private readonly IEsDocumentBatcherStepFactory _batcherFactory;
        private readonly IEsIndexerStep _indexer;

        public ElasticSearchReIndexer(
            ITargetIndexingConfig targetConfig,
            IEsDocumentBatcherStepFactory batcherFactory,
            IEsScrollerStep scroller,
            IEsIndexerStep indexerStep)
        {
            _targetConfig = targetConfig;
            _scroller = scroller;
            _batcherFactory = batcherFactory;
            _indexer = indexerStep;
        }

        public Task StartIndexingAsync()
        {
            var reIndexTaskCancellationUnit = new JobCancellationUnit();

            // tap - start scrolling - out = es docs
            var sourceDocs = _scroller.StartScrollingToEnd(reIndexTaskCancellationUnit);

            // batcher/transformer - start batching - in = es docs, out = es doc batches
            var batcher = _batcherFactory.Create(_targetConfig.BatchSize);
            var sourceDocBatches = batcher.StartBatching(reIndexTaskCancellationUnit, sourceDocs);

            // TODO: throttler

            // sink - start indexing - in = es doc batches
            var indexingCancellationUnit = new JobCancellationUnit();
            return _indexer.StartIndexingAsync(indexingCancellationUnit, sourceDocBatches);
        }
    }
}
