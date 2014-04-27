using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit;

namespace ElasticSearchReIndexer.Tests.Integration.TestUtils
{
    public class CustomizedAutoDataAttribute : AutoDataAttribute
    {
        private static readonly string AUTOFIXTURE_CUSTOMIZATION_INTERFACE_NAME =
            typeof(ICustomization).FullName;

        private Type[] _customizationTypes;

        public CustomizedAutoDataAttribute(params Type[] customizationTypes)
            : this(new Fixture(), customizationTypes)
        {
        }

        private CustomizedAutoDataAttribute(Type fixtureType)
            : base(fixtureType)
        {
        }

        private CustomizedAutoDataAttribute(IFixture fixture, params Type[] customizationTypes)
			: base(fixture)
        {
            _customizationTypes = customizationTypes ?? new Type[0];
        }

        public override IEnumerable<object[]> GetData(System.Reflection.MethodInfo methodUnderTest, Type[] parameterTypes)
        {
            foreach (var customizationType in
                            _customizationTypes.Where(c => c.GetInterface(AUTOFIXTURE_CUSTOMIZATION_INTERFACE_NAME) != null))
            {
                this.Fixture.Customize((ICustomization)Activator.CreateInstance(customizationType));
            }
            return base.GetData(methodUnderTest, parameterTypes);
        }
    }
}
