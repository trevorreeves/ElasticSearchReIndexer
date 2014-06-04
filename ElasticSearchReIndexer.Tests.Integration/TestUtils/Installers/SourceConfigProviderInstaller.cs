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
using Newtonsoft.Json.Linq;

namespace ElasticSearchReIndexer.Tests.Integration.TestUtils.Installers
{
    public class SourceConfigProviderInstaller : IWindsorInstaller
    {
        private readonly string _index;
        private readonly string _type;

        public SourceConfigProviderInstaller(string index, string type)
        {
            _index = index;
            _type = type;
        }

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var testConfigProvider = new InMemoryConfigProvider();
            testConfigProvider.AddValue(SourceScrollConfig.SERVER_CONNECTION_STRING_KEY, GlobalTestSettings.TestSourceServerConnectionString);
            testConfigProvider.AddValue(SourceScrollConfig.INDEX_KEY, _index);
            testConfigProvider.AddValue(SourceScrollConfig.TYPE_KEY, _type);
            testConfigProvider.AddValue(SourceScrollConfig.BATCH_SIZE_KEY, 3);
            testConfigProvider.AddValue(SourceScrollConfig.FILTER_DOC_KEY, new JObject());
            testConfigProvider.AddValue(SourceScrollConfig.SCROLL_THROTTLE_TIME_PERIOD_KEY, TimeSpan.FromSeconds(10));
            testConfigProvider.AddValue(SourceScrollConfig.MAX_SCROLLS_PER_THROTTLE_KEY, 10);
            
            container.Register(
                Component.For<IConfigProvider>().Instance(testConfigProvider).Named("testSourceConfigProvider"),
                Component.For<ISourceScrollConfig>()
                         .ImplementedBy<SourceScrollConfig>()
                         .LifestyleTransient()
                         .DependsOn(Dependency.OnComponent(typeof(IConfigProvider), "testSourceConfigProvider"))
                         .Named("TestSourceScrollConfig")
                         .IsDefault());
        }
    }
}
