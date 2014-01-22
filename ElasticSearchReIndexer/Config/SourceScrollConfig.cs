using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ElasticSearchReIndexer.Config
{
    public interface ISourceScrollConfig
    {
        string ServerConnectionString { get; }
        string IndexIdentifier { get; }
        string TypeIdentifier { get; }
        JObject FilterDoc { get; }
        int BatchSize { get; }

        TimeSpan ScrollThrottlingTimePeriod { get; }
        int MaxScrollsPerThrottlingTimePeriod { get; }
    }

    public class SourceScrollConfig
    {
    }
}
