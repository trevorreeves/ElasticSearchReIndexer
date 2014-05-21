using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using ElasticSearchReIndexer.Clients;
using ElasticSearchReIndexer.Models;

namespace ElasticSearchReIndexer.Workers
{
    public class ScrollWorker
    {
        private readonly IEsScrollClient _client;
        private string _currentScrollId = string.Empty;

        public ScrollWorker(IEsScrollClient client)
        {
            Contract.Requires(client != null);

            _client = client;
        }

        public IEnumerable<EsDocument> ScrollPage()
        {
            ScrollResult res;
            
            if (string.IsNullOrWhiteSpace(_currentScrollId))
            {
                this.InitiateScrollOnServer();
            }

            res = _client.Scroll(_currentScrollId);
            _currentScrollId = res.ScrollId;

            Contract.Ensures(!string.IsNullOrWhiteSpace(_currentScrollId));

            return res.ScrolledDocs.Select(d => new EsDocument(d.Index, d.Type, d.Doc));
        }

        public IEsScrollClient Client
        {
            get
            {
                return _client;
            }
        }

        private void InitiateScrollOnServer()
        {
            _currentScrollId = _client.BeginScroll();
            Contract.Ensures(!string.IsNullOrWhiteSpace(_currentScrollId));
        }
    }
}
