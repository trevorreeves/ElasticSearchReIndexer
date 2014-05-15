
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using ElasticSearchReIndexer.Workers;
using Ploeh.AutoFixture;

namespace ElasticSearchReIndexer.Tests.Integration.TestUtils.Customisations
{
    public class IndexWorkerCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            var container = new WindsorContainer();
            container.Install(
                FromAssembly.InDirectory(new AssemblyFilter(".", "ElasticSearchReIndexer.*")));

            var worker = container.Resolve<IndexWorker>();
        }
    }
}
