using Lib.Performance;

namespace Lib.Cache
{
    /// <summary>
    /// 和Cache有关的性能计数器包装
    /// </summary>
    public sealed class CachingPerformanceCounters
    {
        private PerformanceCounterWrapper entriesCounter;
        private PerformanceCounterWrapper hitsCounter;
        private PerformanceCounterWrapper missesCounter;
        private PerformanceCounterWrapper hitRatioCounter;
        private PerformanceCounterWrapper hitRatioBaseCounter;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="instanceName">实例名称</param>
        public CachingPerformanceCounters(string instanceName)
        {
            PerformanceCounterInitData data = new PerformanceCounterInitData(
                "DeluxeWorks Caching", "Cache Entries", instanceName);
            this.entriesCounter = new PerformanceCounterWrapper(data);

            data.CounterName = "Cache Hits";
            this.hitsCounter = new PerformanceCounterWrapper(data);

            data.CounterName = "Cache Misses";
            this.missesCounter = new PerformanceCounterWrapper(data);

            data.CounterName = "Cache Hit Ratio";
            this.hitRatioCounter = new PerformanceCounterWrapper(data);

            data.CounterName = "Cache Hit Ratio Base";
            this.hitRatioBaseCounter = new PerformanceCounterWrapper(data);
        }

        /// <summary>
        /// Cache项的计数
        /// </summary>
        public PerformanceCounterWrapper EntriesCounter
        {
            get
            {
                return this.entriesCounter;
            }
        }

        /// <summary>
        /// 命中次数
        /// </summary>
        public PerformanceCounterWrapper HitsCounter
        {
            get
            {
                return this.hitsCounter;
            }
        }

        /// <summary>
        /// 没有命中的次数
        /// </summary>
        public PerformanceCounterWrapper MissesCounter
        {
            get
            {
                return this.missesCounter;
            }
        }

        /// <summary>
        /// 命令率中的命中次数
        /// </summary>
        public PerformanceCounterWrapper HitRatioCounter
        {
            get
            {
                return this.hitRatioCounter;
            }
        }

        /// <summary>
        /// 命中率中的总访问数
        /// </summary>
        public PerformanceCounterWrapper HitRatioBaseCounter
        {
            get
            {
                return this.hitRatioBaseCounter;
            }
        }
    }
}
