using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ElasticSearchReIndexer
{
    public class JobCancellationUnit
    {
        private readonly CancellationTokenSource _cancelTokenSource;
        private readonly CancellationToken _cancelToken;

        public JobCancellationUnit()
        {
            _cancelTokenSource = new CancellationTokenSource();
            _cancelToken = _cancelTokenSource.Token;
        }

        public void Cancel()
        {
            _cancelTokenSource.Cancel();
        }

        public void ThrowIfCancelled()
        {
            _cancelToken.ThrowIfCancellationRequested();
        }

        public bool IsCancellationRequested
        {
            get
            {
                return _cancelTokenSource.IsCancellationRequested;
            }
        }

        public CancellationTokenSource TokenSource
        {
            get
            {
                return _cancelTokenSource;
            }
        }

        public CancellationToken Token
        {
            get
            {
                return _cancelToken;
            }
        }

        // add dependency model?
    }
}
