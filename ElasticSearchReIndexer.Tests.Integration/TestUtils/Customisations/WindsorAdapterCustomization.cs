using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Windsor;
using Ploeh.AutoFixture;

namespace ElasticSearchReIndexer.Tests.Integration.TestUtils.Customisations
{
    public class WindsorAdapterCustomization : ICustomization
    {
        private readonly IWindsorContainer _container;

        public WindsorAdapterCustomization(IWindsorContainer container = null)
        {
            _container = container ?? new WindsorContainer();
        }

        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(new WindsorAdapter(this._container));
        }
    }
}
