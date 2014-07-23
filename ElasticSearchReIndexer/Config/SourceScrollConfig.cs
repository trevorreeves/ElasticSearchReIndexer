using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ElasticSearchReIndexer.Config
{
    public class SourceScrollConfig : ISourceScrollConfig
    {
        public const string SERVER_CONNECTION_STRING_KEY = "Source.ConnectionString";
        public const string INDEX_KEY = "Source.Index";
        public const string TYPE_KEY = "Source.Type";
        public const string BATCH_SIZE_KEY = "Source.BatchSize";
        public const string FILTER_DOC_KEY = "Source.FilterDoc";
        
        public const string SCROLL_THROTTLE_TIME_PERIOD_KEY = "Target.ScrollThrottleTimePeriod";
        public const string MAX_SCROLLS_PER_THROTTLE_KEY = "Target.MaxScrollsPerThrottle";

        private readonly string _serverConnectionString;
        private readonly string _index;
        private readonly string _type;
        private readonly int _batchSize;

        private readonly JObject _filterDoc;

        private readonly TimeSpan _scrollThrottlingTimePeriod;
        private readonly int _maxScrollsPerThrottlingTimePeriod;

        public SourceScrollConfig(IConfigProvider configProvider)
        {
            _serverConnectionString = configProvider.AssertConfigValueIsPresent<string>(SERVER_CONNECTION_STRING_KEY);
            _index = configProvider.AssertConfigValueIsPresent<string>(INDEX_KEY);
            _type = configProvider.AssertConfigValueIsPresent<string>(TYPE_KEY);
            _batchSize = configProvider.AssertConfigValueIsPresent<int>(BATCH_SIZE_KEY);

            _filterDoc = configProvider.AssertConfigValueIsPresent<JObject>(FILTER_DOC_KEY, new JObject());
            _scrollThrottlingTimePeriod = configProvider.AssertConfigValueIsPresent<TimeSpan>(SCROLL_THROTTLE_TIME_PERIOD_KEY);
            _maxScrollsPerThrottlingTimePeriod = configProvider.AssertConfigValueIsPresent<int>(MAX_SCROLLS_PER_THROTTLE_KEY);
        }

        public string ServerConnectionString
        {
            get { return _serverConnectionString; }
        }

        public string IndexIdentifier
        {
            get { return _index; }
        }

        public string TypeIdentifier
        {
            get { return _type; }
        }

        public JObject FilterDoc
        {
            get { return _filterDoc; }
        }

        public int BatchSize
        {
            get { return _batchSize; }
        }

        public TimeSpan ScrollThrottlingTimePeriod
        {
            get { return _scrollThrottlingTimePeriod; }
        }

        public int MaxScrollsPerThrottlingTimePeriod
        {
            get { return _maxScrollsPerThrottlingTimePeriod; }
        }
    }
}
