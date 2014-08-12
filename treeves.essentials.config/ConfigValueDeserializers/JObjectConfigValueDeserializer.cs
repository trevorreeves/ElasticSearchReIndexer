using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace treeves.essentials.config
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;

    namespace treeves.essentials.config
    {
        public class JObjectConfigValueDeserializer : IConfigValueDeserializer
        {
            public Type DeserializerFor
            {
                get { return typeof(JObject); }
            }

            public bool TryDeserialize(string strValue, out object value)
            {
                JObject val;

                bool res = JObjectExtensions.TryParse(strValue, out val);
                value = val;
                return res;
            }

            public object DeserializeOrDefault(string strValue)
            {
                object value;
                TryDeserialize(strValue, out value);
                return value;
            }
        }

        public static class JObjectExtensions
        {
            public static bool TryParse(string strVal, out JObject obj)
            {
                obj = null;
                try
                {
                    obj = JObject.Parse(strVal);
                    return true;
                }
                catch 
                {
                    return false;
                }
            }
        }
    }

}
