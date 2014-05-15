using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using ElasticSearchReIndexer.Workers;

namespace ElasticSearchReIndexer.Installers
{
    public class WorkersInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IndexWorker>().LifestyleTransient(),
                Component.For<ScrollWorker>().LifestyleTransient());
        }
    }
}
