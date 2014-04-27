using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearchReIndexer.Clients
{
    public interface IEsIndexClient
    {
        bool Bulk(string body);
    }
}
