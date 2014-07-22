using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using ElasticSearchReIndexer.Tests.Integration.TestUtils.Installers;
using Ploeh.AutoFixture;

namespace ElasticSearchReIndexer.Tests.Integration.TestUtils.Customisations
{
    public class FullIntegrationTestCustomization : ICustomization
    {
        private readonly string _sourceIndexName;
        private readonly string _sourceTypeName;
        private readonly string _targetIndexName;
        private readonly string _targetTypeName;

        public FullIntegrationTestCustomization(
            string sourceIndexName,
            string sourceTypeName,
            string targetIndexName,
            string targetTypeName)
        {
            _sourceIndexName = sourceIndexName;
            _sourceTypeName = sourceTypeName;
            _targetIndexName = targetIndexName;
            _targetTypeName = targetTypeName;
        }

        public void Customize(IFixture fixture)
        {
            var container = new WindsorContainer();

            container.Install(
                new TargetConfigProviderInstaller(_targetIndexName, _targetTypeName));

            container.Install(
                new SourceConfigProviderInstaller(_sourceIndexName, _sourceTypeName, 3));

            fixture.Customize(new WindsorAdapterCustomization(container));
            fixture.Customize(new EsDocumentCustomisation(_sourceIndexName, _sourceIndexName));
            fixture.Customize(new EsTestIndexClientCustomisation(GlobalTestSettings.TestTargetServerConnectionString, _targetIndexName));
        }
    }
}
