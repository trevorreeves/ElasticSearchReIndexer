using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using ElasticSearchReIndexer.Clients;
using ElasticSearchReIndexer.Config;

namespace ElasticSearchReIndexer.Tests.Integration.TestUtils.Installers
{
    public class TargetConfigProviderInstaller : IWindsorInstaller
    {
        private readonly string _index;
        private readonly string _type;

        public TargetConfigProviderInstaller(string index, string type)
        {
            _index = index;
            _type = type;
        }

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var testConfigProvider = new InMemoryConfigProvider();
            testConfigProvider.AddValue(TargetIndexingConfig.SERVER_CONNECTION_STRING_KEY, GlobalTestSettings.TestTargetServerConnectionString);
            testConfigProvider.AddValue(TargetIndexingConfig.INDEX_KEY, _index);
            testConfigProvider.AddValue(TargetIndexingConfig.TYPE_KEY, _type);
            testConfigProvider.AddValue(TargetIndexingConfig.BATCH_SIZE_KEY, 10);
            testConfigProvider.AddValue(TargetIndexingConfig.INDEX_THROTTLE_TIME_PERIOD_KEY, TimeSpan.FromSeconds(10));
            testConfigProvider.AddValue(TargetIndexingConfig.MAX_INDEXES_PER_THROTTLE_KEY, 10);
            testConfigProvider.AddValue(TargetIndexingConfig.REINSTATE_INDEX_REFRESH_KEY, true);
            testConfigProvider.AddValue(TargetIndexingConfig.SUSPEND_INDEX_REFRESH_KEY, true);

            container.Register(
                Component.For<IConfigProvider>().Instance(testConfigProvider).Named("testTargetConfigProvider"),
                Component.For<ITargetIndexingConfig>()
                         .ImplementedBy<TargetIndexingConfig>()
                         .LifestyleTransient()
                         .DependsOn(Dependency.OnComponent(typeof(IConfigProvider), "testTargetConfigProvider"))
                         .Named("TestTargetIndexConfig")
                         .IsDefault());
        }
    }
}
