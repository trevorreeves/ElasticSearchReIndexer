using System;
using System.Collections.Generic;
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
    public class IndexWorkerFixture
    {
        public static Action<IFixture> AutoSetup()
        {
            return fixture => 
            {
                var testIndex = fixture.Create<string>();
                var testType = fixture.Create<string>();

                var container = new WindsorContainer()
                    .Install(new ElasticSearchReIndexerInstaller())
                    .Install(new TargetConfigProviderInstaller(testIndex, testType));

                fixture
                    .Customize(new WindsorAdapterCustomization(container))
                    .Customize(new EsDocumentCustomisation(testIndex, testType))
                    .Customize(new EsTestIndexClientCustomisation(GlobalTestSettings.TestTargetServerConnectionString, testIndex));
            };
        }

        [Theory]
        [AutoSetup]
        public void BulkIndex_addsSingleDoc_Successfully(
            EsDocument doc1, 
            EsTestIndexClient testClient, 
            IndexWorker indexWorker)
        {
            indexWorker
                .Index(new List<EsDocument>() { doc1 })
                .Should().Be(true);

            using (testClient.ForTestAssertions())
            {
                testClient.GetAllDocs().Single().ToString().Should().Be(doc1.Data.ToString());
            }
        }

        [Theory]
        [AutoSetup]
        public void BulkIndex_addsMultipleDoc_Successfully(
            EsDocument doc1,
            EsDocument doc2,
            EsDocument doc3,
            EsTestIndexClient testClient,
            IndexWorker indexWorker)
        {
            indexWorker
                .Index(new[] { doc1, doc2, doc3 })
                .Should().Be(true);

            using (testClient.ForTestAssertions())
            {
                testClient.GetAllDocs().Should().HaveCount(3);
            }
        }
    }
}
