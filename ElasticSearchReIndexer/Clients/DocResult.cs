using System.Diagnostics.Contracts;
using Newtonsoft.Json.Linq;

namespace ElasticSearchReIndexer.Clients
{
    public class DocResult
    {
        private readonly string _index;
        private readonly string _type;
        private readonly JObject _doc;

        public DocResult(
            string index,
            string type,
            JObject doc)
        {
            Contract.Requires(index != null);
            Contract.Requires(type != null);
            Contract.Requires(doc != null);

            _index = index;
            _type = type;
            _doc = doc;
        }

        public string Index
        {
            get
            {
                return _index;
            }
        }

        public string Type
        {
            get
            {
                return _type;
            }
        }

        public JObject Doc
        {
            get
            {
                return _doc;
            }
        }
    }
}
