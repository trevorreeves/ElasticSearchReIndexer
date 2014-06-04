using System;
using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using ElasticSearchReIndexer.Models;
using ElasticSearchReIndexer.Tests.Integration.TestUtils;
using ElasticSearchReIndexer.Tests.Integration.TestUtils.Customisations;
using ElasticSearchReIndexer.Tests.Integration.TestUtils.Installers;
using ElasticSearchReIndexer.Workers;
using FluentAssertions;
using Ploeh.AutoFixture;
using Xunit;
using Xunit.Extensions;

namespace ElasticSearchReIndexer.Tests.Integration.Workers
{
    public class ScrollWorkerFixture
    {
        public static Action<IFixture> AutoSetup()
        {
            return fixture =>
            {
                var testIndex = fixture.Create<string>();
                var testType = fixture.Create<string>();

                var container = new WindsorContainer()
                    .Install(new ElasticSearchReIndexerInstaller())
                    .Install(new TargetConfigProviderInstaller(testIndex, testType))
                    .Install(new SourceConfigProviderInstaller(testIndex, testType));

                fixture
                    .Customize(new WindsorAdapterCustomization(container))
                    .Customize(new EsDocumentCustomisation(testIndex, testType))
                    .Customize(new EsTestIndexClientCustomisation(GlobalTestSettings.TestTargetServerConnectionString, testIndex));
            };
        }

        [Theory]
        [AutoSetup]
        public void ScrollPage_MultipleDocsExistOnSingleScrollPage_ReturnsDocs(
            EsDocument doc1, 
            EsDocument doc2, 
            EsDocument doc3, 
            IndexWorker indexWorker, 
            EsTestIndexClient testClient, 
            ScrollWorker scrollWorker)
        {
            indexWorker.Index(new [] {doc1, doc2, doc3});

            using (testClient.ForTestAssertions())
            {
                var actuals = scrollWorker.ScrollPage();

                actuals.Should().NotBeNull();
                actuals.Should().HaveCount(3);
            }
        }

        [Theory]
        [AutoSetup]
        public void ScrollPage_MultipleDocsExistOnMultipleScrollPages_ReturnsDocs(
            IFixture fixture,
            IndexWorker indexWorker,
            EsTestIndexClient testClient,
            ScrollWorker scrollWorker)
        {
            var testDocs = fixture.CreateMany<EsDocument>(scrollWorker.Client.Config.BatchSize * 2);

            indexWorker.Index(testDocs);

            using (testClient.ForTestAssertions())
            {
                var actuals = scrollWorker.ScrollPage(); // first page
                actuals = actuals.Concat(scrollWorker.ScrollPage()); // second page
                scrollWorker.ScrollPage().Should().BeEmpty(); // third page is empty

                actuals.Should().NotBeNull();
                actuals.Should().HaveSameCount(testDocs);
            }
        }
    }
}
