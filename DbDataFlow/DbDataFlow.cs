using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbDataFlow
{
    public class DbDataFlow<T>
    {
        private readonly ITap<T> _tap;
        private readonly IBatcher<T> _batcher;
        private readonly ISink<T> _sink;

        public DbDataFlow(
            ITap<T> tap,
            IBatcher<T> batcher,
            ISink<T> sink)
        {
            _tap = tap;
            _batcher = batcher;
            _sink = sink;
        }

        public Task StartFlowAsync()
        {
            var sourceCancellationUnit = new JobCancellationUnit();

            // tap - start scrolling - out = es docs
            var sourceStream = _tap.StartFlowingToEnd(sourceCancellationUnit);

            // batcher/transformer - start batching - in = es docs, out = es doc batches
            var sourceBathStream = _batcher.StartBatching(sourceCancellationUnit, sourceStream);

            // TODO: throttler

            // sink - start indexing - in = es doc batches
            var destinationCancellationUnit = new JobCancellationUnit();
            return _sink.StartDrainingAsync(destinationCancellationUnit, sourceBathStream);
        }
    }
}
