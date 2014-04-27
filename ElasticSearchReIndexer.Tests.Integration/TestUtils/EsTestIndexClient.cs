using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElasticSearchReIndexer.Models;
using Nest;
using Newtonsoft.Json.Linq;

namespace ElasticSearchReIndexer.Tests.Integration.TestUtils
{
    public class EsTestIndexClient : IDisposable
    {
        private readonly ElasticClient _client;
        private readonly string _indexName;
        private readonly string _type;

        public EsTestIndexClient(
            ConnectionSettings connectionSettings, 
            string indexName)
        {
            _client = new ElasticClient(connectionSettings));
            _indexName = indexName;
        }

        public void Refresh()
        {
            _client.Refresh(_indexName);
        }

        public void Delete()
        {
            _client.DeleteIndex(_indexName);
        }

        public IEnumerable<JObject> GetAllDocs()
        {
            var d = new SearchDescriptor<JObject>();
            d.Index(_indexName);
            d.MatchAll();

            return _client.Search<JObject>(d).Documents;
        }

        public EsDocument GetCorrespondingDoc(EsDocument seedDoc)
        {
            var doc = _client.Get<JObject>(seedDoc.Index, seedDoc.Type, seedDoc.Id);
            return new EsDocument(seedDoc.Index, seedDoc.Type, doc);
        }

        private EsDocument GenerateTestDoc(string id)
        {
            var testDoc =
                JObject.Parse(
                    string.Format("{{ \"_id\" : \"{0}\", \"dude\" : true }}", id));

            return new EsDocument(_indexName, _type, testDoc);
        }

        public InitializingUnitOfWork ForTestAssertionQueries()
        {
            return new InitializingUnitOfWork(
                () => this.Refresh());
        }

        public void Dispose()
        {
            this.Delete();
        }
    }

    public class InitializingUnitOfWork : IDisposable
    {
        public InitializingUnitOfWork(Action initialisationStep)
        {
            initialisationStep();
        }

        public void Dispose()
        {
        }
    }
}
