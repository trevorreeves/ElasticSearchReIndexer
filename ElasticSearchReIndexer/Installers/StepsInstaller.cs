using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using ElasticSearchReIndexer.Steps;

namespace ElasticSearchReIndexer.Installers
{
    public class StepsInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.AddFacility(new TypedFactoryFacility());

            container.Register(
                Component.For<IEsScrollerStep>().ImplementedBy<EsScrollerStep>().LifestyleSingleton(),
                Component.For<IEsDocumentBatcherStep>().ImplementedBy<EsDocumentBatcherStep>().LifestyleSingleton(),
                Component.For<IEsDocumentBatcherStepFactory>().AsFactory().LifestyleSingleton(),
                Component.For<IEsIndexerStep>().ImplementedBy<EsIndexerStep>().LifestyleSingleton());
        }
    }
}
