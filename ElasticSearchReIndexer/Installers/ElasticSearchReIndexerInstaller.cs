using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using DbDataFlow;
using ElasticSearchReIndexer.Clients;
using ElasticSearchReIndexer.Config;
using ElasticSearchReIndexer.Models;
using ElasticSearchReIndexer.Steps;
using ElasticSearchReIndexer.Workers;
using treeves.essentials.castle.windsor;

namespace ElasticSearchReIndexer.Installers
{
    public class ElasticSearchReIndexerInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.DependsOnFacility<TypedFactoryFacility>();

            container.Register(
                Component.For<IConfigProvider>().ImplementedBy<InMemoryConfigProvider>().LifestyleSingleton(),
                Component.For<ITargetIndexingConfig>().ImplementedBy<TargetIndexingConfig>().LifestyleSingleton(),
                Component.For<ISourceScrollConfig>().ImplementedBy<SourceScrollConfig>().LifestyleSingleton()
            );

            container.Register(
                Component.For<IEsIndexClient>().ImplementedBy<EsIndexClient>().LifestyleTransient(),
                Component.For<IEsScrollClient>().ImplementedBy<EsScrollClient>().LifestyleTransient()
            );

            container.Register(
               Component.For<IndexWorker>().LifestyleTransient(),
               Component.For<ScrollWorker>().LifestyleTransient(),
               Component.For<IIndexWorkerFactory>().AsFactory().LifestyleSingleton(),
               Component.For<IScrollWorkerFactory>().AsFactory().LifestyleSingleton());

            container.Register(
                Component.For<ITap<EsDocument>>().ImplementedBy<EsScrollerStep>().LifestyleSingleton(),
                Component.For<ITransformer<EsDocument, List<EsDocument>>>()
                         .ImplementedBy<EsDocumentBatcherStep>()
                         .LifestyleSingleton()
                         .DynamicParameters((k, d) => {
                             d["batchSize"] = container.Resolve<ITargetIndexingConfig>().BatchSize;
                         }),
                Component.For<ISink<List<EsDocument>>>().ImplementedBy<EsIndexerStep>().LifestyleSingleton());
        }
    }
}
