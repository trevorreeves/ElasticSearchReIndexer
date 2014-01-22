using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElasticSearchReIndexer.Config;

namespace ElasticSearchReIndexer
{
    public class EsScroller
    {
        private readonly ISourceScrollConfig _config;

        public EsScroller(ISourceScrollConfig config)
        {
            _config = config;
        }

        public BlockingCollection<EsDocument> StartScrollingToEnd()
        {

        }
    }
}
