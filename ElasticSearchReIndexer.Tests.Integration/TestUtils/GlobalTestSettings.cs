using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElasticSearchReIndexer.Config;

namespace ElasticSearchReIndexer.Tests.Integration.TestUtils
{
    public static class GlobalTestSettings
    {
        public static string TestTargetServerConnectionString
        {
            get
            {
                return ConfigurationManager.AppSettings[TargetIndexingConfig.SERVER_CONNECTION_STRING_KEY];
            }
        }

        public static string TestSourceServerConnectionString
        {
            get
            {
                return ConfigurationManager.AppSettings[SourceScrollConfig.SERVER_CONNECTION_STRING_KEY];
            }
        }
    }
}
