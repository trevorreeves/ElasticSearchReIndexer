using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElasticSearchReIndexer.Config;

namespace ElasticSearchReIndexer
{
    public class IndexWorker
    {
        private readonly ITargetIndexingConfig _config;

        public IndexWorker(ITargetIndexingConfig config)
        {
            _config = config;
        }

        public void Index(IEnumerable<EsDocument> docs)
        {
            // bulk update docs to ES.
        }
    }
}
