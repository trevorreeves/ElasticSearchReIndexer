using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElasticSearchReIndexer.Clients;
using ElasticSearchReIndexer.Config;
using ElasticSearchReIndexer.WindsorExtensions;

namespace ElasticSearchReIndexer.Workers
{
    public interface IIndexWorkerFactory : IReleaser<IndexWorker>
    {
        IndexWorker Create();
    }
}
