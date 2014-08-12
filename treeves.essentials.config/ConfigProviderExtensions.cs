using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace treeves.essentials.config
{
    public static class ConfigProviderExtensions
    {
        public static bool TryGetConfigValue<T>(
            this IConfigProvider provider,
            string key,
            out T value)
        {
            value = provider.GetPropertyValue<T>(key);
            return !EqualityComparer<T>.Default.Equals(value, default(T));
        }
        
        public static bool ConfigValueIsPresent<T>(
            this IConfigProvider provider,
            string key)
        {
            T value;
            return provider.TryGetConfigValue(key, out value);
        }

        public static T AssertConfigValueIsPresent<T>(
            this IConfigProvider provider,
            string key,
            T valueIfNotPresent = default(T))
        {
            T value;
            
            if (!provider.TryGetConfigValue(key, out value))
            {
                if (valueIfNotPresent.Equals(default(T)))
                {
                    throw new ArgumentNullException("Config value is not present : " + key);
                }
                else
                {
                    value = valueIfNotPresent;
                }
            }

            return value;
        }
    }
}
