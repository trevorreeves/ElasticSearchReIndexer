using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace treeves.essentials.config
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Returns the first value in the source that has a value not equal to
        /// the value types default value.  Returns the default value if none of
        /// the source objects have a non-default value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T FirstWithValueOrDefault<T>(this IEnumerable<T> source)
        {
            return source
                .FirstOrDefault(v => !EqualityComparer<T>.Default.Equals(v, default(T)));
        }
    }
}
