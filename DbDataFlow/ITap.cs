using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbDataFlow
{
    public interface ITap<T>
    {
        BlockingCollection<T> StartFlowingToEnd(
            JobCancellationUnit cancellationUnit);
    }
}
