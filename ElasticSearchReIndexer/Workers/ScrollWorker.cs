using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElasticSearchReIndexer.Config;

namespace ElasticSearchReIndexer.Workers
{
    public class ScrollWorker
    {
        private readonly ISourceScrollConfig _config;

        public ScrollWorker(ISourceScrollConfig config)
        {
            _config = config;
        }

        public List<EsDocument> ScrollPage()
        {
            return null;
        }
    }
}
