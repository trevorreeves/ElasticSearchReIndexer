using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElasticSearchReIndexer.Config;
using Nest;
using Newtonsoft.Json.Linq;

namespace ElasticSearchReIndexer.Clients
{
    public class EsIndexClient : IEsIndexClient
    {
        private readonly ITargetIndexingConfig _config;
        private readonly ElasticClient _client;

        public EsIndexClient(ITargetIndexingConfig config)
        {
            Contract.Requires(config != null);

            var setting = new ConnectionSettings(new Uri(config.ServerConnectionString));
            _client = new ElasticClient(setting);
            _config = config;
        }

        public bool Bulk(string body)
        {
            var status = 
                _client.Raw.BulkPost(body);

            return status.Success;
        }

        public void Refresh()
        {
            _client.Refresh(_config.Index);
        }
    }
}
