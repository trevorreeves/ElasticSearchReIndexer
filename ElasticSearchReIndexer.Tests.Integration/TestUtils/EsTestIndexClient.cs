using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElasticSearchReIndexer.Models;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ElasticSearchReIndexer.Tests.Integration.TestUtils
{
    public class EsTestIndexClient : IDisposable
    {
        private readonly ElasticClient _client;
        private readonly string _indexName;

        public EsTestIndexClient(
            string server,
            string indexName) :
            this(new ConnectionSettings(new Uri(server)), indexName)
        {
        }

        public EsTestIndexClient(
            ConnectionSettings connectionSettings, 
            string indexName)
        {
            _client = new ElasticClient(connectionSettings);
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

        public void Index(params EsDocument[] docs)
        {
            foreach(var doc in docs)
            {
                _client.Index(doc.Data, doc.Index, doc.Type, doc.Id);
            }
        }

        public IEnumerable<JObject> GetAllDocs()
        {
            var res = _client.Search<JObject>(
                    s => s.MatchAll()
                          .Index(_indexName)
                          .AllTypes());

            return res.Documents;
        }

        public EsDocument GetCorrespondingDoc(EsDocument seedDoc)
        {
            var doc = _client.Get<JObject>(seedDoc.Index, seedDoc.Type, seedDoc.Id);
            return new EsDocument(seedDoc.Index, seedDoc.Type, doc);
        }

        public InitializingUnitOfWork ForTestAssertions()
        {
            return new InitializingUnitOfWork(
                () => this.Refresh(),
                () => this.Delete());
        }

        public void Dispose()
        {
            this.Delete();
        }
    }

    public class InitializingUnitOfWork : IDisposable
    {
        private readonly Action _finalisationStep;

        public InitializingUnitOfWork(
            Action initialisationStep,
            Action finalisationStep)
        {
            initialisationStep();
            _finalisationStep = finalisationStep;
        }

        public void Dispose()
        {
            _finalisationStep();
        }
    }
}
