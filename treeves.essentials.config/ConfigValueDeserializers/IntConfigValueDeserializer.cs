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

    namespace treeves.essentials.config
    {
        public class IntConfigValueDeserializer : IConfigValueDeserializer
        {
            public Type DeserializerFor
            {
                get { return typeof(int); }
            }

            public bool TryDeserialize(string strValue, out object value)
            {
                int val;
                bool res = int.TryParse(strValue, out val);
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
    }

}
