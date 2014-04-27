using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

using ElasticSearchReIndexer.Clients;
using ElasticSearchReIndexer.Config;
using ElasticSearchReIndexer.Models;
using ElasticSearchReIndexer.Tests.Integration.TestUtils;
using ElasticSearchReIndexer.Tests.Integration.TestUtils.CustomisationGroups;
using ElasticSearchReIndexer.Tests.Integration.TestUtils.Customisations;

using FluentAssertions;
using Nest;
using Newtonsoft.Json.Linq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace ElasticSearchReIndexer.Tests.Integration.Workers
{
    public class IndexWorkerFixture : IDisposable
    {
        private IndexWorker _testIndexWorker;

        public IndexWorkerFixture()
        {
            var testConfigProvider = new InMemoryConfigProvider();
            testConfigProvider.AddValue(TargetIndexingConfig.SERVER_CONNECTION_STRING_KEY, GlobalTestSettings.TestTargetServerConnectionString);
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

        [Theory]
        [CustomizedAutoData(typeof(EsIntegrationCustomisations))]
        public void BulkIndex_addsSingleDoc_Successfully(
            EsDocument testDoc, 
            EsTestIndexClient testClient)
        {
            _testIndexWorker
                .Index(new List<EsDocument>() { testDoc })
                .Should().Be(true);

            using (testClient.ForTestAssertionQueries())
            {
                testClient.GetAllDocs().Single().ToString().Should().Be(testDoc.Data.ToString());
            }
        }

        [Theory]
        [CustomizedAutoData(typeof(EsIntegrationCustomisations))]
        public void BulkIndex_addsMultipleDoc_Successfully(
            EsDocument doc1, EsDocument doc2, EsDocument doc3, EsTestIndexClient testClient)
        {
            _testIndexWorker
                .Index(new[] { doc1, doc2, doc3 })
                .Should().Be(true);

            using (testClient.ForTestAssertionQueries())
            {
                testClient.GetAllDocs().Should().HaveCount(3);
            }
        }

        public void Dispose()
        {
        }
    }
}
