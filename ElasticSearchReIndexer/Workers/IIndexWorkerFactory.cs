using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElasticSearchReIndexer.Clients;
using ElasticSearchReIndexer.Config;

namespace ElasticSearchReIndexer.Workers
{
    public interface IIndexWorkerFactory
    {
        IndexWorker Create();

        void Release(IndexWorker childObj);
    }
}
