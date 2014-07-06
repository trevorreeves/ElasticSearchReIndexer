using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbDataFlow
{
    public interface IBatcher<T>
    {
        BlockingCollection<List<T>> StartBatching(
            JobCancellationUnit cancellationUnit,
            BlockingCollection<T> source);
    }
}
