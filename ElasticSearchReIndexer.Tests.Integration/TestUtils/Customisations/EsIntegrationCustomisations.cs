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
using ElasticSearchReIndexer.Workers;
using Nest;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace ElasticSearchReIndexer.Tests.Integration.TestUtils.Customisations
{
    public class EsIntegrationCustomisations : ICustomization
    {
        private string _index;
        private string _type;

        public EsIntegrationCustomisations(string index, string type)
        {
            _index = index;
            _type = type;
        }

        public void Customize(IFixture fixture)
        {
            var connection = new ConnectionSettings(new Uri(GlobalTestSettings.TestTargetServerConnectionString));
            fixture.Customize(new EsDocumentCustomisation(_index, _type));
            fixture.Customize(new EsTestIndexClientCustomisation(connection, _index));
        }
    }
}
