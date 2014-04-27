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
                Component.For<IConfigProvider>().ImplementedBy<InMemoryConfigProvider>().LifestyleTransient(),
                Component.For<ITargetIndexingConfig>().ImplementedBy<TargetIndexingConfig>().LifestyleTransient(),
                Component.For<ISourceScrollConfig>().ImplementedBy<SourceScrollConfig>().LifestyleTransient()
            );
        }
    }
}
