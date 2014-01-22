using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElasticSearchReIndexer.Config;
using ElasticSearchReIndexer.Workers;

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

        public void StartIndexing()
        {
            var reIndexTaskCancellationUnit = new JobCancellationUnit();

            // start scrolling - out = es docs
            var scroller = new EsScroller(_sourceConfig);
            var sourceDocs = scroller.StartScrollingToEnd();

            // start batching - in = es docs, out = es doc batches
            var batcher = new EsDocumentBatcher(_targetConfig.BatchSize);
            var sourceDocBatches = batcher.StartBatching(sourceDocs);

            // start indexing - in = es doc batches
            var indexer = new EsIndexerManager(_targetConfig);
            indexer.StartIndexing(sourceDocBatches);
        }
    }
}
