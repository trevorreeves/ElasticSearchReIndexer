using System;
using System.Collections.Generic;
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
        private readonly ElasticClient _client;

        public EsIndexClient(ITargetIndexingConfig config)
        {
            var setting = new ConnectionSettings(new Uri(config.ServerConnectionString));
            _client = new ElasticClient(setting);
        }

        public bool Bulk(string body)
        {
            var status = 
                _client.Raw.BulkPost(body);

            return status.Success;
        }
    }
}
