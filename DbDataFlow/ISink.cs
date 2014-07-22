using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbDataFlow
{
    public interface ISink<T>
    {
        Task StartDrainingAsync(
            JobCancellationUnit cancellationUnit,
            BlockingCollection<T> stream);
    }
}
