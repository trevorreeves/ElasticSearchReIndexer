using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbDataFlow
{
    public static class BlockingCollectionExtensions
    {
        public static bool PossiblyMoreInStream<T>(this BlockingCollection<T> source)
        {
            return source.Any() || !source.IsCompleted;
        }
    }
}
