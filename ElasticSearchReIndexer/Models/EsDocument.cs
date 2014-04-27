using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Diagnostics.Contracts;

namespace ElasticSearchReIndexer.Models
{
    public class EsDocument
    {
        private readonly string _index;
        private readonly string _type;
        private readonly JObject _data;
        private readonly string _id;

        public EsDocument(
            string index,
            string type,
            JObject data)
        {
            Contract.Assert(index != null);
            Contract.Assert(type != null);
            Contract.Assert(data != null);

            _index = index;
            _type = type;
            _data = data;
            _id = _data.Property("_id").Value.ToString();
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

        public string Id
        {
            get
            {
                return _id;
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
