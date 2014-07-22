using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DbDataFlow;
using ElasticSearchReIndexer.Clients;
using ElasticSearchReIndexer.Config;
using ElasticSearchReIndexer.Models;
using ElasticSearchReIndexer.Workers;
using treeves.essentials.castle.windsor;

namespace ElasticSearchReIndexer.Steps
{
    public class EsIndexerStep : BaseSink<List<EsDocument>>
    {
        private readonly ITargetIndexingConfig _config;
        private readonly IIndexWorkerFactory _workerFactory;
        private readonly IEsIndexClient _flushingClient;

        public EsIndexerStep(
            ITargetIndexingConfig config,
            IIndexWorkerFactory workerFactory,
            IEsIndexClient flushingClient)
        {
            _config = config;
            _workerFactory = workerFactory;
            _flushingClient = flushingClient;
        }

        protected override Action GetWorker(List<EsDocument> data)
        {
            return new Action(() =>
                {
                    using (var workerWrapper = _workerFactory.CreateReleasable((f) => f.Create()))
                    {
                        IndexWorker worker = workerWrapper;
                        worker.Index(data);
                    }
                });
        }
    }
}
