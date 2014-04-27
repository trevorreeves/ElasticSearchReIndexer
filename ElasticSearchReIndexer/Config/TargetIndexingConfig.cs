using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearchReIndexer.Config
{
    public static class ConfigProviderExtensions
    {
        public static T AssertConfigValueIsPresent<T>(
            this IConfigProvider provider, 
            string key,
            T valueIfNotPresent = default(T))
        {
            T val = provider.GetPropertyValue<T>(key);
            if (EqualityComparer<T>.Default.Equals(val, default(T)))
            {
                if (valueIfNotPresent.Equals(default(T)))
                {
                    throw new ArgumentNullException("Config value is not present : " + key);
                }
                else
                {
                    val = valueIfNotPresent;
                }
            }

            return val;
        }
    }

    public class TargetIndexingConfig : ITargetIndexingConfig
    {
        public const string SERVER_CONNECTION_STRING_KEY = "Target.ConnectionString";
        public const string INDEX_KEY = "Target.Index";
        public const string TYPE_KEY = "Target.Type";
        public const string BATCH_SIZE_KEY = "Target.BatchSize";
        public const string SUSPEND_INDEX_REFRESH_KEY = "Target.SuspendIndexRefresh";
        public const string REINSTATE_INDEX_REFRESH_KEY = "Target.ReinstateIndexRefresh";
        public const string INDEX_THROTTLE_TIME_PERIOD_KEY = "Target.IndexThrottleTimePeriod";
        public const string MAX_INDEXES_PER_THROTTLE_KEY = "Target.MaxIndexesPerThrottle";

        private readonly string _serverConnectionString;
        private readonly string _index;
        private readonly string _type;
        private readonly int _batchSize;

        private readonly bool _suspendIndexRefreshDuringIndex;
        private readonly bool _reInstateIndexRefreshAfterIndex;

        private readonly TimeSpan _indexThrottlingTimePeriod;
        private readonly int _maxIndexesPerThrottlingTimePeriod;

        public TargetIndexingConfig(IConfigProvider configProvider)
        {
            _serverConnectionString = configProvider.AssertConfigValueIsPresent<string>(SERVER_CONNECTION_STRING_KEY);
            _index = configProvider.AssertConfigValueIsPresent<string>(INDEX_KEY);
            _type = configProvider.AssertConfigValueIsPresent<string>(TYPE_KEY);
            _batchSize = configProvider.AssertConfigValueIsPresent<int>(BATCH_SIZE_KEY);

            _suspendIndexRefreshDuringIndex = configProvider.AssertConfigValueIsPresent<bool>(SUSPEND_INDEX_REFRESH_KEY);
            _reInstateIndexRefreshAfterIndex = configProvider.AssertConfigValueIsPresent<bool>(REINSTATE_INDEX_REFRESH_KEY);
            _indexThrottlingTimePeriod = configProvider.AssertConfigValueIsPresent<TimeSpan>(INDEX_THROTTLE_TIME_PERIOD_KEY);
            _maxIndexesPerThrottlingTimePeriod = configProvider.AssertConfigValueIsPresent<int>(MAX_INDEXES_PER_THROTTLE_KEY);
        }

        public string ServerConnectionString
        {
            get { return _serverConnectionString; }
        }

        public string Index
        {
            get { return _index; }
        }

        public string Type
        {
            get { return _type; }
        }

        public int BatchSize
        {
            get { return _batchSize; }
        }

        public bool SuspendIndexRefreshDuringIndex
        {
            get { return _suspendIndexRefreshDuringIndex; }
        }

        public bool ReInstateIndexRefreshAfterIndex
        {
            get { return _reInstateIndexRefreshAfterIndex; }
        }

        public TimeSpan IndexThrottlingTimePeriod
        {
            get { return _indexThrottlingTimePeriod; }
        }

        public int MaxIndexesPerThrottlingTimePeriod
        {
            get { return _maxIndexesPerThrottlingTimePeriod; }
        }
    }
}
