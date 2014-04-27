using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using ElasticSearchReIndexer.Clients;

namespace ElasticSearchReIndexer.Installers
{
    public class ClientsInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IEsIndexClient>().ImplementedBy<EsIndexClient>().LifestyleTransient(),
                Component.For<IEsScrollClient>().ImplementedBy<EsScrollClient>().LifestyleTransient()
            );
        }
    }
}
