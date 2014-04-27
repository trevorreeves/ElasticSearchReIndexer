using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using ElasticSearchReIndexer.Tests.Integration.TestUtils.Customisations;
using ElasticSearchReIndexer.Tests.Integration.TestUtils.Installers;
using Nest;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace ElasticSearchReIndexer.Tests.Integration.TestUtils.CustomisationGroups
{
    public class EsIntegrationCustomisations : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            var index = "index_" + fixture.Create<string>();
            var type = "type_" + fixture.Create<string>();

            var container = new WindsorContainer();

            container.Install(
                FromAssembly.InDirectory(new AssemblyFilter(".", "ElasticSearchReIndexer.dll")),
                new TargetConfigProviderInstaller(index, type));

            var indexWorker = container.Resolve<IndexWorker>();

            var connection = new ConnectionSettings(new Uri(GlobalTestSettings.TestTargetServerConnectionString));

            fixture.Customize(new EsDocumentCustomisation(index, type));
            fixture.Customize(new EsTestIndexClientCustomisation(connection, index));
            fixture.Register<IndexWorker>(() => indexWorker);
        }
    }
}
