using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace treeves.essentials.config
{
    public interface IConfigValueDeserializer
    {
        Type DeserializerFor { get; }

        bool TryDeserialize(string strValue, out object value);

        object DeserializeOrDefault(string strValue);
    }
}
