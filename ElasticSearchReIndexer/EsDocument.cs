using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ElasticSearchReIndexer
{
    public class EsDocument
    {
        private readonly string _index;
        private readonly string _type;
        private readonly JObject _data;

        public EsDocument(
            string index,
            string type,
            JObject data)
        {
            _index = index;
            _type = type;
            _data = data;
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

        public JObject Data
        {
            get
            {
                return _data;
            }
        }
    }
}
