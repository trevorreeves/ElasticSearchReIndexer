using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElasticSearchReIndexer.Config;
using Ploeh.AutoFixture;

namespace ElasticSearchReIndexer.Tests.Integration.TestUtils.Customisations
{
    public class TargetInMemoryConfigProviderCustomisation : ICustomization
    {
        private readonly string _indexName;
        private readonly string _type;

        public TargetInMemoryConfigProviderCustomisation(string index = "", string type = "")
        {
            _indexName = index;
            _type = type;
        }

        public void Customize(IFixture fixture)
        {
            var index = string.IsNullOrWhiteSpace(_indexName) ? "index_" + fixture.Create<string>() : _indexName;

            var testConfigProvider = new InMemoryConfigProvider();
            testConfigProvider.AddValue(TargetIndexingConfig.SERVER_CONNECTION_STRING_KEY, GlobalTestSettings.TestTargetServerConnectionString);
            testConfigProvider.AddValue(TargetIndexingConfig.INDEX_KEY, index);
            testConfigProvider.AddValue(TargetIndexingConfig.TYPE_KEY, _type);
            testConfigProvider.AddValue(TargetIndexingConfig.BATCH_SIZE_KEY, 10);
            testConfigProvider.AddValue(TargetIndexingConfig.INDEX_THROTTLE_TIME_PERIOD_KEY, TimeSpan.FromSeconds(10));
            testConfigProvider.AddValue(TargetIndexingConfig.MAX_INDEXES_PER_THROTTLE_KEY, 10);
            testConfigProvider.AddValue(TargetIndexingConfig.REINSTATE_INDEX_REFRESH_KEY, true);
            testConfigProvider.AddValue(TargetIndexingConfig.SUSPEND_INDEX_REFRESH_KEY, true);

            fixture.Register<IConfigProvider>(() => testConfigProvider);
        }
    }
}
