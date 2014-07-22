using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElasticSearchReIndexer.Config;
using ElasticSearchReIndexer.Clients;
using ElasticSearchReIndexer.Workers;
using ElasticSearchReIndexer.Models;
using DbDataFlow;

namespace ElasticSearchReIndexer.Steps
{
    public class EsScrollerStep : BaseTap<EsDocument>
    {
        private readonly IScrollWorkerFactory _workerFactory;

        public EsScrollerStep(IScrollWorkerFactory workerFactory)
        {
            _workerFactory = workerFactory;
        }

        protected override Action<BlockingCollection<EsDocument>, Action> GetWorker()
        {
            return new Action<BlockingCollection<EsDocument>, Action>(
                (scrolledDocs, throwIfTaskCancelled) =>
                {
                    try
                    {
                        var worker = _workerFactory.Create();

                        var docs = worker.ScrollPage();
                        while (docs.Any())
                        {
                            foreach (var doc in docs)
                            {
                                throwIfTaskCancelled();
                                scrolledDocs.Add(doc);
                            }
                            docs = worker.ScrollPage();
                        }
                    }
                    finally
                    {
                        scrolledDocs.CompleteAdding();
                    }
                });
        }
    }
}
