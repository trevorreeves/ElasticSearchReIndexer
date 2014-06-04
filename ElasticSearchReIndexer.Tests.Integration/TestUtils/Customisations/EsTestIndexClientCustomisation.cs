using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;
using Ploeh.AutoFixture;

namespace ElasticSearchReIndexer.Tests.Integration.TestUtils.Customisations
{
    public class EsTestIndexClientCustomisation : ICustomization
    {
        private readonly string _indexName;
        private readonly ConnectionSettings _connectionSettings;

        public EsTestIndexClientCustomisation(
            string serverAddress, 
            string index = "")
        {
            _indexName = index;
            _connectionSettings = new ConnectionSettings(new Uri(serverAddress));
        }

        public void Customize(IFixture fixture)
        {
            var index = string.IsNullOrWhiteSpace(_indexName) ? "index_" + fixture.Create<string>() : _indexName;

            fixture.Register<EsTestIndexClient>(() =>
                new EsTestIndexClient(_connectionSettings, index));
        }
    }
}
