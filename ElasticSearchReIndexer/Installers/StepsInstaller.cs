using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using ElasticSearchReIndexer.Models;
using ElasticSearchReIndexer.Steps;

namespace ElasticSearchReIndexer.Installers
{
    public class StepsInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.AddFacility(new TypedFactoryFacility());

            container.Register(
                Component.For<ITap<EsDocument>>().ImplementedBy<EsScrollerStep>().LifestyleSingleton(),
                Component.For<IBatcher<EsDocument>>().ImplementedBy<EsDocumentBatcherStep>().LifestyleSingleton(),
                Component.For<IBatcherFactory<EsDocument>>().AsFactory().LifestyleSingleton(),
                Component.For<ISink<EsDocument>>().ImplementedBy<EsIndexerStep>().LifestyleSingleton());
        }
    }
}
