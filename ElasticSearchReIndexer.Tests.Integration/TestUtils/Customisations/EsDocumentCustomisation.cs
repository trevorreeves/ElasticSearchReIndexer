using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElasticSearchReIndexer.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ploeh.AutoFixture;

namespace ElasticSearchReIndexer.Tests.Integration.TestUtils.Customisations
{
    public class EsDocumentCustomisation : ICustomization
    {
        private readonly string _index;
        private readonly string _type;

        public EsDocumentCustomisation(string index = "", string type = "")
        {
            _index = index;
            _type = type;
        }

        public void Customize(IFixture fixture)
        {
            fixture.Register<EsDocument>(() => {

                var index = string.IsNullOrWhiteSpace(_index) ? "index_" + fixture.Create<string>() : _index;
                var type = string.IsNullOrWhiteSpace(_type) ? "type_" + fixture.Create<string>() : _type;

                return new EsDocument(
                    index,
                    type,
                    JObject.FromObject(
                        new
                        {
                            _id = "doc_" + fixture.Create<string>(),
                            boolProp = fixture.Create<bool>(),
                            strProp = fixture.Create<string>(),
                            dateProp = fixture.Create<DateTime>(),
                            doubleProp = fixture.Create<double>()
                        }));
            });
        }
    }
}
