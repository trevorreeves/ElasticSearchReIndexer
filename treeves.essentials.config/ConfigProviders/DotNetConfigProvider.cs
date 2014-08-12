using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace treeves.essentials.config
{
    public class DotNetConfigProvider : IConfigProvider
    {
        private readonly IEnumerable<IConfigValueDeserializer> _deserializers;

        public DotNetConfigProvider(IEnumerable<IConfigValueDeserializer> deserializers)
        {
            _deserializers = deserializers;
        }

        public T GetPropertyValue<T>(string key)
        {
            T value = default(T);

            var strValue = ConfigurationManager.AppSettings[key];

            if (strValue == null)
            {
                return default(T);
            }

            var candidateDeserializers = 
                _deserializers.Where(d => d.DeserializerFor == typeof(T));

            if (!candidateDeserializers.Any())
            {
                throw new ArgumentOutOfRangeException(
                            string.Format("Config property '{0}' cannot be retrieved.  No deserializer is registered for type {1}",
                                key,
                                typeof(T).ToString()));
            }

            object rawValue = candidateDeserializers
                .Select(d => d.DeserializeOrDefault(strValue))
                .FirstWithValueOrDefault();

            if (Object.Equals(rawValue, default(T)))
            {
                value = default(T);
            }
            else if (typeof(T) == rawValue.GetType())
            {
                value = (T)rawValue;
            }
            else
            {
                throw new ArgumentOutOfRangeException(
                    string.Format("Config property '{0}' is not of the correct type.  Expected {1} but found {2}",
                        key,
                        typeof(T).ToString(),
                        rawValue.GetType().ToString()));
            }

            return value;
        }
    }
}
