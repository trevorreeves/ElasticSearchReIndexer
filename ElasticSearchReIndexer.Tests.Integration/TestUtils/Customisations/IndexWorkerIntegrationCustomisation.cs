using Castle.Windsor;
using ElasticSearchReIndexer.Tests.Integration.TestUtils.Installers;
using Ploeh.AutoFixture;

namespace ElasticSearchReIndexer.Tests.Integration.TestUtils.Customisations
{
    public class IndexWorkerIntegrationCustomisation : ICustomization
    {
        private readonly IWindsorContainer _container;

        public IndexWorkerIntegrationCustomisation(
            string index,
            string type,
            IWindsorContainer container = null)
        {
            _container = container ?? new WindsorContainer();

            _container.Install(
                new TargetConfigProviderInstaller(index, type));
        }

        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(new WindsorAdapter(this._container));
        }
    }
}