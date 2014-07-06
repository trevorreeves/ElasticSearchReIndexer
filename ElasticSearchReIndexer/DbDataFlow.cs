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
    public class DbDataFlow<T>
    {
        private readonly ITargetIndexingConfig _targetConfig;
        private readonly ITap<T> _tap;
        private readonly IBatcherFactory<T> _batcherFactory;
        private readonly ISink<T> _sink;

        public DbDataFlow(
            ITargetIndexingConfig targetConfig,
            IBatcherFactory<T> batcherFactory,
            ITap<T> tap,
            ISink<T> sink)
        {
            _targetConfig = targetConfig;
            _tap = tap;
            _batcherFactory = batcherFactory;
            _sink = sink;
        }

        public Task StartIndexingAsync()
        {
            var sourceCancellationUnit = new JobCancellationUnit();

            // tap - start scrolling - out = es docs
            var sourceStream = _tap.StartFlowingToEnd(sourceCancellationUnit);

            // batcher/transformer - start batching - in = es docs, out = es doc batches
            var batcher = _batcherFactory.Create(_targetConfig.BatchSize);
            var sourceBathStream = batcher.StartBatching(sourceCancellationUnit, sourceStream);

            // TODO: throttler

            // sink - start indexing - in = es doc batches
            var destinationCancellationUnit = new JobCancellationUnit();
            return _sink.StartDrainingAsync(destinationCancellationUnit, sourceBathStream);
        }
    }
}
