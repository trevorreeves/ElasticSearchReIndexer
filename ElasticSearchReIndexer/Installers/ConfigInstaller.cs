using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using ElasticSearchReIndexer.Config;

namespace ElasticSearchReIndexer.Installers
{
    public class ConfigInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IConfigProvider>().ImplementedBy<InMemoryConfigProvider>().LifestyleSingleton(),
                Component.For<ITargetIndexingConfig>().ImplementedBy<TargetIndexingConfig>().LifestyleSingleton(),
                Component.For<ISourceScrollConfig>().ImplementedBy<SourceScrollConfig>().LifestyleSingleton()
            );
        }
    }
}
