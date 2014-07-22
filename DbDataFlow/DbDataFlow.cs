using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbDataFlow
{
    public class DbDataFlow<TSource, TTransformed>
    {
        private readonly ITap<TSource> _tap;
        private readonly IBatcher<TSource, TTransformed> _transformer;
        private readonly ISink<TTransformed> _sink;

        public DbDataFlow(
            ITap<TSource> tap,
            IBatcher<TSource, TTransformed> transformer,
            ISink<TTransformed> sink)
        {
            _tap = tap;
            _transformer = transformer;
            _sink = sink;
        }

        public Task StartFlowAsync()
        {
            var sourceCancellationUnit = new JobCancellationUnit();

            // tap - start scrolling - out = es docs
            var sourceStream = _tap.StartFlowingToEnd(sourceCancellationUnit);

            // batcher/transformer - start batching - in = es docs, out = es doc batches
            var batchStream = _transformer.StartBatching(sourceCancellationUnit, sourceStream);

            // TODO: throttler
            
            // sink - start indexing - in = es doc batches
            var destinationCancellationUnit = new JobCancellationUnit();
            return _sink.StartDrainingAsync(destinationCancellationUnit, batchStream);
        }
    }
}
