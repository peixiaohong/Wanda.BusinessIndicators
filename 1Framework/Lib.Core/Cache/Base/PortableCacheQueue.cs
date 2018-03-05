using System;
using System.Threading;
using System.Collections.Generic;
using Lib.Core.Properties;

namespace Lib.Cache
{
    /// <summary>
    /// 为一泛型Cache类，和CacheQueue不同的是PortableCacheQueue内部没有实现LRU算法，
    /// 而且容量大小也不做限制。用户在使用此Cache时同样需要从此类派生一新类，并手工注册，
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class PortableCacheQueue<TKey, TValue> : CacheQueueBase, IScavenge
    {
        /// <summary>
        /// Cache项不存在时的委托定义
        /// </summary>
        /// <param name="cache">Cache对列</param>
        /// <param name="key">键值</param>
        /// <returns>新的Cache项</returns>
        public delegate TValue PortableCacheItemNotExistsAction(PortableCacheQueue<TKey, TValue> cache, TKey key);

        private Dictionary<TKey, CacheItem<TKey, TValue>> innerDictionary = new Dictionary<TKey, CacheItem<TKey, TValue>>();

        private ReaderWriterLock rWLock = new ReaderWriterLock();
        private TimeSpan lockTimeout = TimeSpan.FromSeconds(100);
        private bool overrideExistsItem = true;
        private object syncRoot = new object();

        /// <summary>
        /// 构造方法
        /// </summary>
        protected PortableCacheQueue()
            : base()
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="overrideExists">Add Cache项时，是否覆盖已有的数据</param>
        protected PortableCacheQueue(bool overrideExists)
        {
            this.overrideExistsItem = overrideExists;
        }

        /// <summary>
        /// 向CacheQueue中增加一Cache项值对，如果相应的key已经存在，则抛出异常
        /// 此种构造方法无相关Dependency，所以此增加Cache项不会过期，只可能当CacheQueue
        /// 的长度超过预先设定时，才可能被清理掉
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="data">值</param>
        /// <returns>值</returns>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Caching\PortableCacheQueueTest.cs" region="AddRemoveClearTest" lang="cs" title="增加、移除、获取CahceItem项" />
        /// </remarks>
        public TValue Add(TKey key, TValue data)
        {
            this.Add(key, data, null);

            return data;
        }

        /// <summary>
        /// 向CacheQueue中增加一个有关联Dependency的Cache项，如果相应的key已经存在，则抛出异常
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="data">值</param>
        /// <param name="dependency">依赖：相对时间依赖、绝对时间依赖、文件依赖或混合依赖</param>
        /// <returns>值</returns>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Caching\PortableCacheQueueTest.cs" region="AddRemoveClearTest" lang="cs" title="增加、移除、获取CahceItem项" />
        /// </remarks>
        public TValue Add(TKey key, TValue data, DependencyBase dependency)
        {
            this.rWLock.AcquireWriterLock(this.lockTimeout);

            try
            {
                //删除已经存在而且过期的Cache项
                if (this.innerDictionary.ContainsKey(key) &&
                    ((CacheItem<TKey, TValue>)this.innerDictionary[key]).Dependency != null &&
                    ((CacheItem<TKey, TValue>)this.innerDictionary[key]).Dependency.HasChanged)
                    this.Remove(key);

                CacheItem<TKey, TValue> item = new CacheItem<TKey, TValue>(key, data, dependency, this);

                if (dependency != null)
                {
                    dependency.UtcLastModified = DateTime.UtcNow;
                    dependency.UtcLastAccessTime = DateTime.UtcNow;
                }

                if (this.overrideExistsItem)
                    this.innerDictionary[key] = item;
                else
                    this.innerDictionary.Add(key, item);

                this.Counters.EntriesCounter.RawValue = this.innerDictionary.Count;

                return data;
            }
            finally
            {
                this.rWLock.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// 属性，获取CacheQueue的最大容量
        /// </summary>
        public override int Count
        {
            get
            {
                return this.innerDictionary.Count;
            }
        }

        /// <summary>
        /// 通过Cache项的key获取Cache项Value的索引器
        /// </summary>
        /// <param name="key">cache项key</param>
        /// <returns>cache项Value</returns>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Caching\PortableCacheQueueTest.cs" region="GetCacheItemTest" lang="cs" title="通过Cache项的key获取Cache项Value" />
        /// </remarks>
        public TValue this[TKey key]
        {
            get
            {
                CacheItem<TKey, TValue> item = null;
                this.TotalCounters.HitRatioBaseCounter.Increment();
                this.Counters.HitRatioBaseCounter.Increment();

                this.rWLock.AcquireReaderLock(this.lockTimeout);
                try
                {
                    item = this.innerDictionary[key];
                }
                finally
                {
                    this.rWLock.ReleaseReaderLock();
                }

                this.rWLock.AcquireWriterLock(this.lockTimeout);
                try
                {
                    if (this.CheckDependencyChanged(key, item))
                        throw new DependencyChangedException(string.Format(Resource.DependencyChanged, key, item.Dependency));

                    if (item.Dependency != null)
                        item.Dependency.UtcLastAccessTime = DateTime.UtcNow;
                }
                finally
                {
                    this.rWLock.ReleaseWriterLock();
                }

                this.TotalCounters.HitsCounter.Increment();
                this.TotalCounters.HitRatioCounter.Increment();
                this.Counters.HitsCounter.Increment();
                this.Counters.HitRatioCounter.Increment();

                return item.Value;
            }
            set
            {
                this.rWLock.AcquireWriterLock(this.lockTimeout);
                try
                {
                    CacheItem<TKey, TValue> item = this.innerDictionary[key];

                    item.Value = value;

                    //重置Cache项的最后修改时间和最后访问时间
                    if (item.Dependency != null)
                    {
                        item.Dependency.UtcLastModified = DateTime.UtcNow;
                        item.Dependency.UtcLastAccessTime = DateTime.UtcNow;
                    }
                }
                finally
                {
                    this.rWLock.ReleaseWriterLock();
                }
            }
        }

        /// <summary>
        /// 判断PortableCacheQueue中是否包含key键的Cache项
        /// </summary>
        /// <param name="key">查询的cache项的键值</param>
        /// <returns>如果包含此键值，返回true，否则返回false</returns>
        public bool ContainsKey(TKey key)
        {
            this.TotalCounters.HitRatioBaseCounter.Increment();
            this.Counters.HitRatioBaseCounter.Increment();

            this.rWLock.AcquireReaderLock(this.lockTimeout);
            try
            {
                bool result = ((this.innerDictionary.ContainsKey(key) &&
                        ((CacheItem<TKey, TValue>)this.innerDictionary[key]).Dependency == null) ||
                        (this.innerDictionary.ContainsKey(key) &&
                        ((CacheItem<TKey, TValue>)this.innerDictionary[key]).Dependency != null &&
                        ((CacheItem<TKey, TValue>)this.innerDictionary[key]).Dependency.HasChanged == false));

                if (result)
                {
                    this.TotalCounters.HitsCounter.Increment();
                    this.TotalCounters.HitRatioCounter.Increment();
                    this.Counters.HitsCounter.Increment();
                    this.Counters.HitRatioCounter.Increment();
                }
                else
                {
                    this.TotalCounters.MissesCounter.Increment();
                    this.Counters.MissesCounter.Increment();
                }

                return result;
            }
            finally
            {
                this.rWLock.ReleaseReaderLock();
            }
        }

        /// <summary>
        /// 通过key，获取Cache项的value，如果相应的cache项存在的话
        /// 则将cache项的value作为输出参数，返回给客户端代码
        /// </summary>
        /// <param name="key">cache项的key</param>
        /// <param name="data">cache项的value</param>
        /// <returns>如果CacheQueue中包含此Cache项，则返回true，否则返回false</returns>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Caching\PortableCacheQueueTest.cs" region="GetCacheItemTest" lang="cs" title="通过key，获取Cache项的value" />
        /// </remarks>
        public bool TryGetValue(TKey key, out TValue data)
        {
            data = default(TValue);
            CacheItem<TKey, TValue> item;
            bool result;

            this.TotalCounters.HitRatioBaseCounter.Increment();
            this.Counters.HitRatioBaseCounter.Increment();

            this.rWLock.AcquireReaderLock(this.lockTimeout);
            try
            {
                result = this.innerDictionary.TryGetValue(key, out item);
            }
            finally
            {
                this.rWLock.ReleaseReaderLock();
            }

            if (result)
            {
                this.rWLock.AcquireWriterLock(this.lockTimeout);
                try
                {
                    if (this.CheckDependencyChanged(key, item))
                        result = false;
                    else
                    {
                        data = item.Value;
                        if (item.Dependency != null)
                            item.Dependency.UtcLastAccessTime = DateTime.UtcNow;
                    }
                }
                finally
                {
                    this.rWLock.ReleaseWriterLock();
                }
            }

            if (result)
            {
                this.TotalCounters.HitsCounter.Increment();
                this.TotalCounters.HitRatioCounter.Increment();
                this.Counters.HitsCounter.Increment();
                this.Counters.HitRatioCounter.Increment();
            }
            else
            {
                this.TotalCounters.MissesCounter.Increment();
                this.Counters.MissesCounter.Increment();
            }

            return result;
        }

        /// <summary>
        /// 在Cache中读取Cache项，如果不存在，则调用action
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="action">不存在时的回调</param>
        /// <returns>Cache项的值</returns>
        public TValue GetOrAddNewValue(TKey key, PortableCacheItemNotExistsAction action)
        {
            TValue result = default(TValue);

            if (TryGetValue(key, out result) == false)
            {
                lock (this.syncRoot)
                {
                    if (TryGetValue(key, out result) == false)
                        result = action(this, key);
                }
            }

            return result;
        }

        /// <summary>
        /// 通过key，删除一Cache项
        /// </summary>
        /// <param name="key">键</param>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Caching\PortableCacheQueueTest.cs" region="AddRemoveClearTest" lang="cs" title="增加、移除、获取CahceItem项" />
        /// </remarks>
        public void Remove(TKey key)
        {
            CacheItem<TKey, TValue> item;

            this.rWLock.AcquireWriterLock(this.lockTimeout);
            try
            {
                if (this.innerDictionary.TryGetValue(key, out item))
                    this.InnerRemove(key, item);

                this.Counters.EntriesCounter.RawValue = this.innerDictionary.Count;
            }
            finally
            {
                this.rWLock.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// 重载基类方法，删除传入的CacheItem
        /// </summary>
        /// <param name="cacheItem"></param>
        internal protected override void RemoveItem(CacheItemBase cacheItem)
        {
            this.Remove(((CacheItem<TKey, TValue>)cacheItem).Key);
        }

        /// <summary>
        /// 清空整个CacheQueue，删除CacheQueue中所有的Cache项
        /// </summary>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Caching\PortableCacheQueueTest.cs" region="AddRemoveClearTest" lang="cs" title="增加、移除、获取CahceItem项" />
        /// </remarks>
        public override void Clear()
        {
            this.rWLock.AcquireWriterLock(this.lockTimeout);
            try
            {
                foreach (KeyValuePair<TKey, CacheItem<TKey, TValue>> kp in this.innerDictionary)
                    kp.Value.Dispose();

                this.innerDictionary.Clear();
            }
            finally
            {
                this.rWLock.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// 全部更新了
        /// </summary>
        public override void SetChanged()
        {
            this.rWLock.AcquireWriterLock(this.lockTimeout);
            try
            {
                foreach (KeyValuePair<TKey, CacheItem<TKey, TValue>> kp in this.innerDictionary)
                    kp.Value.SetChanged();
            }
            finally
            {
                this.rWLock.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// 清理方法，清理本CacheQueue中过期的cache项
        /// </summary>
        public void DoScavenging()
        {
            List<KeyValuePair<TKey, CacheItem<TKey, TValue>>> keysToRemove = new List<KeyValuePair<TKey, CacheItem<TKey, TValue>>>();

            this.rWLock.AcquireWriterLock(this.lockTimeout);
            try
            {
                foreach (KeyValuePair<TKey, CacheItem<TKey, TValue>> kp in this.innerDictionary)
                    if (kp.Value.Dependency != null && kp.Value.Dependency.HasChanged)
                        keysToRemove.Add(kp);

                foreach (KeyValuePair<TKey, CacheItem<TKey, TValue>> kp in keysToRemove)
                    this.InnerRemove(kp.Key, kp.Value);
            }
            finally
            {
                this.rWLock.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// 判断一Cache项是否过期
        /// </summary>
        /// <param name="key">Cache项的键值</param>
        /// <param name="item">Cache项</param>
        /// <returns>如果Cache项过期，返回true，并将其删除，否则返回false</returns>
        private bool CheckDependencyChanged(TKey key, CacheItem<TKey, TValue> item)
        {
            bool result = false;
            if (item.Dependency != null && item.Dependency.HasChanged)
            {
                result = true;
                this.InnerRemove(key, item);
            }

            return result;
        }

        /// <summary>
        /// 删除Cache项
        /// </summary>
        /// <param name="key">Cache项键值</param>
        /// <param name="item">Cache项</param>
        private void InnerRemove(TKey key, CacheItem<TKey, TValue> item)
        {
            this.innerDictionary.Remove(key);
            item.Dispose();
        }
    }
}
