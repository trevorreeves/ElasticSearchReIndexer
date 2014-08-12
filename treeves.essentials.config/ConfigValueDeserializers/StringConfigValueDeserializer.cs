using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace treeves.essentials.config
{
    public class StringConfigValueDeserializer : IConfigValueDeserializer
    {
        public Type DeserializerFor
        {
            get { return typeof(string); }
        }

        public bool TryDeserialize(string strValue, out object value)
        {
            value = strValue;
            return value != null;
        }

        public object DeserializeOrDefault(string strValue)
        {
            object value;
            TryDeserialize(strValue, out value);
            return value;
        }
    }
}
