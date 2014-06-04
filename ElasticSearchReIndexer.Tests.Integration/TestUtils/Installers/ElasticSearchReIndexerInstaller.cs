using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Castle.Windsor.Installer;

namespace ElasticSearchReIndexer.Tests.Integration.TestUtils.Installers
{
    public class ElasticSearchReIndexerInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Install(
                FromAssembly.InDirectory(
                    new AssemblyFilter(".", "ElasticSearchReIndexer.exe")));
        }
    }
}
