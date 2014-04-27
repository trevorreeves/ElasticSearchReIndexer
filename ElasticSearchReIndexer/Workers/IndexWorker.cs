using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;
using ElasticSearchReIndexer.Clients;
using Newtonsoft.Json.Linq;
using System.Linq;
using ElasticSearchReIndexer.Models;

namespace ElasticSearchReIndexer
{
    public class IndexWorker
    {
        private readonly IEsIndexClient _client;

        public IndexWorker(IEsIndexClient client)
        {
            Contract.Requires(client != null);

            _client = client;
        }

        public bool Index(IEnumerable<EsDocument> docs)
        {
            StringBuilder bulkBodyBuilder = new StringBuilder();

            foreach (var doc in docs)
            {
                bulkBodyBuilder.AppendLine(
                    new JObject(
                        new JProperty("index",
                            new JObject(
                                new JProperty("_index", doc.Index),
                                new JProperty("_type", doc.Type),
                                new JProperty("_id", doc.Id)))).ToString(Newtonsoft.Json.Formatting.None));

                bulkBodyBuilder.AppendLine(doc.Data.ToString(Newtonsoft.Json.Formatting.None));
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
