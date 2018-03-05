
namespace Lib.Cache
{
    /// <summary>
    /// Cache队列的虚基类
    /// </summary>
    public abstract class CacheQueueBase
    {
        private CachingPerformanceCounters totalCounters;
        private CachingPerformanceCounters counters;

        /// <summary>
        /// Cache项的数量
        /// </summary>
        public abstract int Count
        {
            get;
        }

        /// <summary>
        /// 清除Cache队列
        /// </summary>
        public abstract void Clear();

        /// <summary>
        /// 是否都标记为更新
        /// </summary>
        public abstract void SetChanged();

        /// <summary>
        /// 虚方法，删除Cache项
        /// </summary>
        /// <param name="cacheItem">被删除的Cache项</param>
        internal protected abstract void RemoveItem(CacheItemBase cacheItem);

        /// <summary>
        /// 构造方法，初始化性能指针
        /// </summary>
        protected CacheQueueBase()
        {
            this.InitPerformanceCounters();
        }

        /// <summary>
        /// 初始化性能监视指针
        /// </summary>
        protected void InitPerformanceCounters()
        {
            if (this.totalCounters == null)
                this.totalCounters = new CachingPerformanceCounters("_Total_");

            if (this.counters == null)
                this.counters = new CachingPerformanceCounters(this.GetType().Name);
        }

        /// <summary>
        /// 所有Cache的性能指针
        /// </summary>
        protected CachingPerformanceCounters TotalCounters
        {
            get
            {
                return this.totalCounters;
            }
        }

        /// <summary>
        /// 性能指针
        /// </summary>
        protected CachingPerformanceCounters Counters
        {
            get
            {
                return this.counters;
            }
        }

    }
}