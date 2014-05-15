using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElasticSearchReIndexer.Clients;
using ElasticSearchReIndexer.Config;
using ElasticSearchReIndexer.Models;
using ElasticSearchReIndexer.Workers;
using FluentAssertions;
using Nest;
using Newtonsoft.Json.Linq;
using Xunit;

namespace ElasticSearchReIndexer.Tests.Integration
{
    public class ElasticSearchReIndexerFixture
    {
        private const string TEST_INDEX_PREFIX = "indexworkertests";
        private const string TEST_TYPE = "testtype";

        private IndexWorker _testIndexWorker;
        private ElasticClient _testEsClient;

        private readonly string _testIndex;

        public ElasticSearchReIndexerFixture()
        {
            _testIndex = TEST_INDEX_PREFIX + "_" + Guid.NewGuid();

            var testConfigProvider = new InMemoryConfigProvider();
            testConfigProvider.AddValue(TargetIndexingConfig.SERVER_CONNECTION_STRING_KEY, TestServerConnectionString);
            testConfigProvider.AddValue(TargetIndexingConfig.INDEX_KEY, _testIndex);
            testConfigProvider.AddValue(TargetIndexingConfig.TYPE_KEY, TEST_TYPE);
            testConfigProvider.AddValue(TargetIndexingConfig.BATCH_SIZE_KEY, 10);
            testConfigProvider.AddValue(TargetIndexingConfig.INDEX_THROTTLE_TIME_PERIOD_KEY, TimeSpan.FromSeconds(10));
            testConfigProvider.AddValue(TargetIndexingConfig.MAX_INDEXES_PER_THROTTLE_KEY, 10);
            testConfigProvider.AddValue(TargetIndexingConfig.REINSTATE_INDEX_REFRESH_KEY, true);
            testConfigProvider.AddValue(TargetIndexingConfig.SUSPEND_INDEX_REFRESH_KEY, true);

            _testIndexWorker =
                new IndexWorker(
                    new EsIndexClient(
                        new TargetIndexingConfig(
                            testConfigProvider)));
        }

        [Fact]
        public void ReIndex_SingleBatchOfDocsInSource_ReIndexesCorrectly()
        {
            // setup data in source
            var testDocs = new List<EsDocument>() {
                GenerateTestDoc("1"),
                GenerateTestDoc("2"),
                GenerateTestDoc("3"),
            };

            _testIndexWorker
                .Index(testDocs)
                .Should().Be(true);

            TestEsClient.Refresh(_testIndex);

            // setup indexer to test destination

            // run re-indexer

            // verify data in destination.
        }

        private string TestServerConnectionString
        {
            get
            {
                return ConfigurationManager.AppSettings[TargetIndexingConfig.SERVER_CONNECTION_STRING_KEY];
            }
        }

        private EsDocument GenerateTestDoc(string id)
        {
            var testDoc =
                JObject.Parse(
                    string.Format("{{ \"_id\" : \"{0}\", \"dude\" : true }}", id));

            return new EsDocument(_testIndex, TEST_TYPE, testDoc);
        }

        private ElasticClient TestEsClient
        {
            get
            {
                if (_testEsClient == null)
                {
                    _testEsClient = new ElasticClient(
                    new ConnectionSettings(new Uri(this.TestServerConnectionString)));
                }
                return _testEsClient;
            }
        }
    }
}
