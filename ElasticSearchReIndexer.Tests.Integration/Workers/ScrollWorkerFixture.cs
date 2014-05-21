using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using ElasticSearchReIndexer.Models;
using ElasticSearchReIndexer.Tests.Integration.TestUtils;
using ElasticSearchReIndexer.Tests.Integration.TestUtils.Customisations;
using ElasticSearchReIndexer.Workers;
using FluentAssertions;
using Ploeh.AutoFixture;
using Xunit;

namespace ElasticSearchReIndexer.Tests.Integration.Workers
{
    public class ScrollWorkerFixture
    {
        private readonly IFixture _fixture;

        public ScrollWorkerFixture()
        {
            var container = new WindsorContainer();
            container.Install(
                FromAssembly.InDirectory(
                    new AssemblyFilter(".", "ElasticSearchReIndexer.exe")));

            _fixture = new Fixture();

            var indexName = _fixture.Create<string>();
            var typeName = _fixture.Create<string>();

            _fixture.Customize(new IndexWorkerIntegrationCustomisation(indexName, typeName, container));
            _fixture.Customize(new ScrollWorkerIntegrationCustomisation(indexName, typeName, container));
            _fixture.Customize(new EsIntegrationCustomisations(indexName, typeName));
        }

        [Fact]
        public void ScrollPage_MultipleDocsExistOnSingleScrollPage_ReturnsDocs()
        {
            AutoTest
                .With(_fixture)
                .Start<EsDocument, EsDocument, EsDocument, IndexWorker, EsTestIndexClient, ScrollWorker>(
                    (doc1, doc2, doc3, indexWorker, testClient, scrollWorker) =>
                    {
                        indexWorker.Index(new [] {doc1, doc2, doc3});

                        using (testClient.ForTestAssertions())
                        {
                            var actuals = scrollWorker.ScrollPage();

                            actuals.Should().NotBeNull();
                            actuals.Should().HaveCount(3);
                        }
                    });
        }

        [Fact]
        public void ScrollPage_MultipleDocsExistOnMultipleScrollPages_ReturnsDocs()
        {
            AutoTest
                .With(_fixture)
                .Start<IFixture, EsDocument, EsDocument, EsDocument, IndexWorker, EsTestIndexClient, ScrollWorker>(
                    (fixture, doc1, doc2, doc3, indexWorker, testClient, scrollWorker) =>
                    {
                        var testDocs =
                                Enumerable.Range(1, scrollWorker.Client.Config.BatchSize * 2)
                                    .Select((i) => fixture.Create<EsDocument>()).ToList();

                        indexWorker.Index(testDocs);

                        using (testClient.ForTestAssertions())
                        {
                            var actuals = scrollWorker.ScrollPage(); // first page
                            actuals = actuals.Concat(scrollWorker.ScrollPage()); // second page
                            scrollWorker.ScrollPage().Should().BeEmpty(); // third page is empty

                            actuals.Should().NotBeNull();
                            actuals.Should().HaveSameCount(testDocs);
                        }
                    });
        }
    }
}
