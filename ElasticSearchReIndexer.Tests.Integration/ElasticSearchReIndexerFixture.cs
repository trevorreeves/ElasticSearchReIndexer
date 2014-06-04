using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Windsor;
using ElasticSearchReIndexer.Clients;
using ElasticSearchReIndexer.Config;
using ElasticSearchReIndexer.Models;
using ElasticSearchReIndexer.Tests.Integration.TestUtils;
using ElasticSearchReIndexer.Tests.Integration.TestUtils.Customisations;
using ElasticSearchReIndexer.Tests.Integration.TestUtils.Installers;
using ElasticSearchReIndexer.Workers;
using FluentAssertions;
using Nest;
using Newtonsoft.Json.Linq;
using Ploeh.AutoFixture;
using Xunit;
using Xunit.Extensions;

namespace ElasticSearchReIndexer.Tests.Integration
{
    public class ElasticSearchReIndexerFixture
    {
        public static Action<IFixture> AutoSetup()
        {
            return fixture =>
            {
                var sourceIndex = fixture.Create<string>();
                var sourceType = fixture.Create<string>();
                var targetIndex = fixture.Create<string>();
                var targetType = fixture.Create<string>();

                var container = new WindsorContainer()
                    .Install(new ElasticSearchReIndexerInstaller())
                    .Install(new TargetConfigProviderInstaller(targetIndex, targetType))
                    .Install(new SourceConfigProviderInstaller(sourceIndex, sourceType));

                fixture
                    .Customize(new WindsorAdapterCustomization(container))
                    .Customize(new EsDocumentCustomisation(sourceIndex, sourceType))
                    .Customize(new EsTestIndexClientCustomisation(GlobalTestSettings.TestTargetServerConnectionString, targetIndex));
            };
        }

        [Theory]
        [AutoSetup]
        public void ReIndex_SingleBatchOfDocsInSource_ReIndexesCorrectly(
            EsDocument doc1,
            EsDocument doc2,
            EsDocument doc3,
            ElasticSearchReIndexer reindexer,
            ISourceScrollConfig sourceConfig,
            ITargetIndexingConfig targetConfig)
        {
            var testSourceClient = 
                new EsTestIndexClient(
                    GlobalTestSettings.TestSourceServerConnectionString, sourceConfig.IndexIdentifier);

            var testTargetClient = 
                new EsTestIndexClient(
                    GlobalTestSettings.TestTargetServerConnectionString, targetConfig.Index);

            testSourceClient.Index(doc1, doc2, doc3);

            using (testSourceClient.ForTestAssertions())
            {
                reindexer.StartIndexing();
            }

            using (testTargetClient.ForTestAssertions())
            {
                testTargetClient.GetAllDocs().Should().HaveCount(3);    
            }
        }
    }
}
