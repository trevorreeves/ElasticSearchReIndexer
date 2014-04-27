using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearchReIndexer.Clients
{
    public interface IEsScrollClient
    {
        ScrollResult Scroll(string scrollId);

        string BeginScroll();
    }
}
