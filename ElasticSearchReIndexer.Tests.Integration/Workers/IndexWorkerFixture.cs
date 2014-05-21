using System.Collections.Generic;
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
    public class IndexWorkerFixture
    {
        private readonly IFixture _fixture;

        public IndexWorkerFixture()
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
        public void BulkIndex_addsSingleDoc_Successfully()
        {
            AutoTest
                .With(_fixture)
                .Start<EsDocument, EsTestIndexClient, IndexWorker>(
                    (  doc1, testClient, indexWorker) =>
                    {
                        indexWorker
                            .Index(new List<EsDocument>() { doc1 })
                            .Should().Be(true);

                        using (testClient.ForTestAssertions())
                        {
                            testClient.GetAllDocs().Single().ToString().Should().Be(doc1.Data.ToString());
                        }
                    });
        }

        [Fact]
        public void BulkIndex_addsMultipleDoc_Successfully()
        {
            AutoTest
                .With(_fixture)
                .Start<EsDocument, EsDocument, EsDocument, EsTestIndexClient, IndexWorker>(
                      (doc1, doc2, doc3, testClient, indexWorker) => 
                {
                    indexWorker
                        .Index(new[] { doc1, doc2, doc3 })
                        .Should().Be(true);

                    using (testClient.ForTestAssertions())
                    {
                        testClient.GetAllDocs().Should().HaveCount(3);
                    }
                });
        }
    }
}
