using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Windsor;
using DbDataFlow;
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
        public const int TEST_BATCH_SIZE = 3;

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
                    .Install(new SourceConfigProviderInstaller(sourceIndex, sourceType, TEST_BATCH_SIZE));

                fixture
                    .Customize(new WindsorAdapterCustomization(container))
                    .Customize(new EsDocumentCustomisation(sourceIndex, sourceType))
                    .Customize(new EsTestIndexClientCustomisation(GlobalTestSettings.TestTargetServerConnectionString, targetIndex));
            };
        }

        [Theory]
        [AutoSetup]
        public async void ReIndex_SingleBatchOfDocsInSource_ReIndexesCorrectly(
            IFixture fixture,
            DbDataFlow<EsDocument, List<EsDocument>> reindexer,
            ISourceScrollConfig sourceConfig,
            ITargetIndexingConfig targetConfig)
        {
            // arrange
            var testSourceClient = 
                new EsTestIndexClient(
                    GlobalTestSettings.TestSourceServerConnectionString, sourceConfig.IndexIdentifier);

            var testTargetClient = 
                new EsTestIndexClient(
                    GlobalTestSettings.TestTargetServerConnectionString, targetConfig.Index);

            testSourceClient.Index(
                fixture.CreateMany<EsDocument>(TEST_BATCH_SIZE * 3).ToArray());

            // act
            using (testSourceClient.ForTestAssertions())
            {
                await reindexer.StartFlowAsync();
            }

            // assert
            using (testTargetClient.ForTestAssertions())
            {
                testTargetClient.GetAllDocs().Should().HaveCount(3);    
            }
        }

        [Theory]
        [AutoSetup]
        public async void ReIndex_MultiBatchOfDocsInSource_ReIndexesCorrectly(
            IFixture fixture,
            DbDataFlow<EsDocument, List<EsDocument>> reindexer,
            ISourceScrollConfig sourceConfig,
            ITargetIndexingConfig targetConfig)
        {
            // arange
            var testSourceClient =
                new EsTestIndexClient(
                    GlobalTestSettings.TestSourceServerConnectionString, sourceConfig.IndexIdentifier);

            var testTargetClient =
                new EsTestIndexClient(
                    GlobalTestSettings.TestTargetServerConnectionString, targetConfig.Index);

            testSourceClient.Index(
                fixture.CreateMany<EsDocument>(TEST_BATCH_SIZE * 3).ToArray());

            // act
            using (testSourceClient.ForTestAssertions())
            {
                await reindexer.StartFlowAsync();
            }

            // assert
            using (testTargetClient.ForTestAssertions())
            {
                testTargetClient.GetAllDocs().Should().HaveCount(6);
            }
        }
    }
}
