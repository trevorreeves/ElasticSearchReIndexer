using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearchReIndexer.Clients
{
    public class ScrollResult
    {
        private readonly string _scrollId;
        private readonly IEnumerable<DocResult> _scrolledDocs;

        public ScrollResult(string scrollId, IEnumerable<DocResult> scrolledDocs)
        {
            _scrollId = scrollId;
            _scrolledDocs = scrolledDocs;
        }

        public IEnumerable<DocResult> ScrolledDocs
        {
            get
            {
                return _scrolledDocs;
            }
        }

        public string ScrollId
        {
            get
            {
                return _scrollId;
            }
        }
    }
}
