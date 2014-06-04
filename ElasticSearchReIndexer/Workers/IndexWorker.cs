using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;
using ElasticSearchReIndexer.Clients;
using Newtonsoft.Json.Linq;
using System.Linq;
using ElasticSearchReIndexer.Models;
using ElasticSearchReIndexer.Config;

namespace ElasticSearchReIndexer.Workers
{
    public class IndexWorker
    {
        private readonly ITargetIndexingConfig _config;
        private readonly IEsIndexClient _client;

        public IndexWorker(ITargetIndexingConfig config, IEsIndexClient client)
        {
            Contract.Requires(config != null);
            Contract.Requires(client != null);

            _config = config;
            _client = client;
        }

        public bool Index(IEnumerable<EsDocument> docs)
        {
            StringBuilder bulkBodyBuilder = new StringBuilder();

            foreach (var doc in docs)
            {
                var newDoc = new EsDocument(_config.Index, _config.Type, doc.Data);
                bulkBodyBuilder.AppendLine(
                    new JObject(
                        new JProperty("index",
                            new JObject(
                                new JProperty("_index", newDoc.Index),
                                new JProperty("_type", newDoc.Type),
                                new JProperty("_id", newDoc.Id)))).ToString(Newtonsoft.Json.Formatting.None));

                bulkBodyBuilder.AppendLine(newDoc.Data.ToString(Newtonsoft.Json.Formatting.None));
            }

            var bulkBody = bulkBodyBuilder.ToString();

            if (!string.IsNullOrWhiteSpace(bulkBody))
            {
               return _client.Bulk(bulkBody);
            }

            return true;
        }
    }
}
