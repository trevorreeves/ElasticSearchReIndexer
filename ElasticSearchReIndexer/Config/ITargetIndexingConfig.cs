using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearchReIndexer.Config
{
    public interface ITargetIndexingConfig
    {
        string ServerConnectionString { get; }
        string Index { get; }
        string Type { get; }  // optional - original type is preserved if type not specified.
        int BatchSize { get; }

        bool SuspendIndexRefreshDuringIndex { get; }
        bool ReInstateIndexRefreshAfterIndex { get; }

        TimeSpan IndexThrottlingTimePeriod { get; }
        int MaxIndexesPerThrottlingTimePeriod { get; }  // or do it by max concurrent indexes
    }
}
