using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElasticSearchReIndexer.Config;

namespace ElasticSearchReIndexer
{
    public class EsIndexerManager
    {
        private readonly ITargetIndexingConfig _config;

        public EsIndexerManager(ITargetIndexingConfig config)
        {
            _config = config;
        }

        public void StartIndexing(BlockingCollection<List<EsDocument>> sourceBatches)
        {
            // wait and get a batch

            // wait to be under the index threshold...

            // create a new index worker and work it y'all
        }
    }
}
