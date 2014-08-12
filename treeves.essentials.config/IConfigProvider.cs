using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace treeves.essentials.config
{
    public interface IConfigProvider
    {
        T GetPropertyValue<T>(string key);
    }
}
