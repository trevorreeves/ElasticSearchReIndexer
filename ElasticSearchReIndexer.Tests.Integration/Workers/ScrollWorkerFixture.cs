using System;
using System.Configuration;
using System.Linq;
using ElasticSearchReIndexer.Clients;
using ElasticSearchReIndexer.Config;
using ElasticSearchReIndexer.Models;
using ElasticSearchReIndexer.Workers;
using FluentAssertions;
using Moq;
using Nest;
using Newtonsoft.Json.Linq;
using Xunit;

namespace ElasticSearchReIndexer.Tests.Integration.Workers
{
    public class ScrollWorkerFixture : IDisposable
    {
        private const string TEST_INDEX_PREFIX = "indexworkertests";
        private const string TEST_TYPE = "testtype";

        private ScrollWorker _testScrollWorker;
        private IndexWorker _dogFoodIndexer;
        private ElasticClient _testEsClient;

        private readonly string _testIndex;

        public ScrollWorkerFixture()
        {
            _testIndex = TEST_INDEX_PREFIX + "_" + Guid.NewGuid();

            var configProvider = Mock.Of<IConfigProvider>(

                    // index settings
                p => p.GetPropertyValue<string>(TargetIndexingConfig.SERVER_CONNECTION_STRING_KEY) == TestServerConnectionString &&
                     p.GetPropertyValue<string>(TargetIndexingConfig.INDEX_KEY) == _testIndex &&
                     p.GetPropertyValue<string>(TargetIndexingConfig.TYPE_KEY) == TEST_TYPE &&
                     p.GetPropertyValue<int>(TargetIndexingConfig.BATCH_SIZE_KEY) == 10 &&
                     p.GetPropertyValue<TimeSpan>(TargetIndexingConfig.INDEX_THROTTLE_TIME_PERIOD_KEY) == TimeSpan.FromSeconds(10) &&
                     p.GetPropertyValue<int>(TargetIndexingConfig.MAX_INDEXES_PER_THROTTLE_KEY) == 10 &&
                     p.GetPropertyValue<bool>(TargetIndexingConfig.REINSTATE_INDEX_REFRESH_KEY) == true &&
                     p.GetPropertyValue<bool>(TargetIndexingConfig.SUSPEND_INDEX_REFRESH_KEY) == true &&
            
                     // scrolling settings
                     p.GetPropertyValue<string>(SourceScrollConfig.SERVER_CONNECTION_STRING_KEY) == TestServerConnectionString &&
                     p.GetPropertyValue<string>(SourceScrollConfig.INDEX_KEY) == _testIndex &&
                     p.GetPropertyValue<string>(SourceScrollConfig.TYPE_KEY) == TEST_TYPE &&
                     p.GetPropertyValue<int>(SourceScrollConfig.BATCH_SIZE_KEY) == 3 &&
                     p.GetPropertyValue<JObject>(SourceScrollConfig.FILTER_DOC_KEY) == new JObject() &&
                     p.GetPropertyValue<TimeSpan>(SourceScrollConfig.SCROLL_THROTTLE_TIME_PERIOD_KEY) == TimeSpan.FromSeconds(10) &&
                     p.GetPropertyValue<int>(SourceScrollConfig.MAX_SCROLLS_PER_THROTTLE_KEY) == 10);

            _dogFoodIndexer =
                new IndexWorker(
                    new EsIndexClient(
                        new TargetIndexingConfig(
                            configProvider)));

            _testScrollWorker = new ScrollWorker(
                    new EsScrollClient(
                        new SourceScrollConfig(configProvider)));
        }

        public void Dispose()
        {
            TestEsClient.DeleteIndex(_testIndex);
        }

        [Fact]
        public void ScrollPage_MultipleDocsExistOnSingleScrollPage_ReturnsDocs()
        {
            var testDocs = new[] { "1", "2", "3" }.Select(GenerateTestDoc);

            _dogFoodIndexer.Index(testDocs);

            TestEsClient.Refresh(_testIndex);

            var actuals = _testScrollWorker.ScrollPage();

            actuals.Should().NotBeNull();
            actuals.Should().HaveSameCount(testDocs);
        }

        [Fact]
        public void ScrollPage_MultipleDocsExistOnMultipleScrollPage_ReturnsDocs()
        {
            var testDocs = new[] { "1", "2", "3", "4", "5", "6" }.Select(GenerateTestDoc);

            _dogFoodIndexer.Index(testDocs);

            TestEsClient.Refresh(_testIndex);

            var actuals = _testScrollWorker.ScrollPage(); // first page
            actuals = actuals.Concat(_testScrollWorker.ScrollPage()); // second page
            _testScrollWorker.ScrollPage().Should().BeEmpty(); // third page is empty

            actuals.Should().NotBeNull();
            actuals.Should().HaveSameCount(testDocs);
        }

        private EsDocument GenerateTestDoc(string id)
        {
            var testDoc =
                JObject.Parse(
                    string.Format("{{ \"_id\" : \"{0}\", \"dude\" : true }}", id));

            return new EsDocument(_testIndex, TEST_TYPE, testDoc);
        }

        private string TestServerConnectionString
        {
            get
            {
                return ConfigurationManager.AppSettings[TargetIndexingConfig.SERVER_CONNECTION_STRING_KEY];
            }
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
