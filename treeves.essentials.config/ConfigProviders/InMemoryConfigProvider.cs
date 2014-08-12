using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace treeves.essentials.config
{
    public class InMemoryConfigProvider : IConfigProvider
    {
        private readonly Dictionary<string, object> _configDict;

        public InMemoryConfigProvider()
        {
            _configDict = new Dictionary<string, object>(10);
        }

        public void AddValue(string key, object value)
        {
            _configDict.Add(key, value);
        }

        public T GetPropertyValue<T>(string key)
        {
            T value = default(T);
            object rawValue;

            if (_configDict.TryGetValue(key, out rawValue))
            {
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
            }

            return value;
        }
    }
}
