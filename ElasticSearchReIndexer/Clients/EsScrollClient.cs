using System;
using System.Diagnostics.Contracts;
using System.Linq;
using ElasticSearchReIndexer.Config;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ElasticSearchReIndexer.Clients
{
    public class EsScrollClient : IEsScrollClient
    {
        private readonly ISourceScrollConfig _config;
        private readonly ElasticClient _client;

        public EsScrollClient(ISourceScrollConfig config)
        {
            Contract.Requires(config != null);

            _config = config;
            var setting = new ConnectionSettings(new Uri(config.ServerConnectionString));
            _client = new ElasticClient(setting);
        }

        public string BeginScroll()
        {
            var res = 
                _client.Raw.SearchGet(
                    _config.IndexIdentifier, 
                    _config.TypeIdentifier,
                    new { 
                        query = new { 
                            match_all = new { } 
                        } 
                    },
                    qs => qs
                        .Size(_config.BatchSize)
                        .Scroll("5m")
                        .SearchType(SearchTypeOptions.Scan));

            var scrollResponse = 
                JsonConvert.DeserializeObject<QueryResponse<JObject>>(res.Result);

            return scrollResponse.ScrollId;
        }

        public ScrollResult Scroll(string scrollId)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(scrollId));

            var results = this._client.Scroll<JObject>("4s", scrollId);

            return new ScrollResult(
                results.ScrollId,
                results.DocumentsWithMetaData.Select(d => new DocResult(
                    d.Index,
                    d.Type,
                    d.Source)));
        }

        public ISourceScrollConfig Config
        {
            get
            {
                return _config;
            }
        }
    }
}
