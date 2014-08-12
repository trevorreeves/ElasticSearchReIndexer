using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace treeves.essentials.config
{
    public class CompositeCascadingConfigProvider : IConfigProvider
    {
        IEnumerable<IConfigProvider> _internalProviders;

        public CompositeCascadingConfigProvider(params IConfigProvider[] providers)
        {
            _internalProviders = providers.ToList();
        }

        public T GetPropertyValue<T>(string key)
        {
            return _internalProviders
                .Select(p => p.GetPropertyValue<T>(key))
                .FirstWithValueOrDefault();
        }
    }
}
