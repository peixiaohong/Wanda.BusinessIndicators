using System;
using System.Collections.Generic;
using System.Threading;
using Lib.Core;
using Lib.Core.Properties;

namespace Lib.Cache
{
    /// <summary>
    /// 一泛型Cache类，内部通过LRU算法实现了一个Cache项的队列，
    /// 当Cache项的数量超过预先设定的Cache容量时，将自动把队列尾部的Cache项清除
    /// 用户在使用此Cache时需要从此类派生
    /// </summary>
    /// <typeparam name="TKey">键值类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    public class CacheQueue<TKey, TValue> : CacheQueueBase, IScavenge
    {
        /// <summary>
        /// Cache项不存在时的委托定义
        /// </summary>
        /// <param name="cache">Cache对列</param>
        /// <param name="key">键值</param>
        /// <returns>新的Cache项</returns>
        public delegate TValue CacheItemNotExistsAction(CacheQueue<TKey, TValue> cache, TKey key);

        /// <summary>
        /// 采用LRU算法保存Cache项的字典
        /// </summary>
        private LruDictionary<TKey, CacheItem<TKey, TValue>> innerDictionary;

        private ReaderWriterLock rWLock = new ReaderWriterLock();
        private TimeSpan lockTimeout = TimeSpan.FromSeconds(100);
        private bool overrideExistsItem = true;
        private object syncRoot = new object();

        #region 构造方法
        /// <summary>
        /// 构造函数，没有设置CacheQueue的容量大小，则使用默认值100
        /// </summary>
        protected CacheQueue()
            : base()
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="overrideExists">Add Cache项时，是否覆盖已有的数据</param>
        protected CacheQueue(bool overrideExists)
            : this()
        {
            this.overrideExistsItem = overrideExists;
        }

        #endregion 构造方法

        #region 公有属性

        /// <summary>
        /// 属性，获取CacheQueue的最大容量
        /// </summary>
        public int MaxLength
        {
            get
            {
                int maxLength;

                QueueSetting qs = CacheSettingsSection.GetConfig().QueueSettings[this.GetType()];

                if (qs != null)
                    maxLength = qs.QueueLength;
                else
                    maxLength = CacheSettingsSection.GetConfig().DefaultQueueLength;

                return maxLength;
            }
        }
        #endregion 公有属性

        /// <summary>
        /// 向CacheQueue中增加一Cache项值对，如果相应的key已经存在，则抛出异常
        /// 此种构造方法无相关Dependency，所以此增加Cache项不会过期，只可能当CacheQueue
        /// 的长度超过预先设定时，才可能被清理掉
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="data">值</param>
        /// <returns>值</returns>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Caching\CacheQueueTest.cs" region="AddRemoveClearTest" lang="cs" title="增加、移除、获取CacheItem项" />
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
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Caching\CacheQueueTest.cs" region="AddRemoveClearTest" lang="cs" title="增加、移除、获取CacheItem项" />
        /// </remarks>
        public TValue Add(TKey key, TValue data, DependencyBase dependency)
        {
            this.rWLock.AcquireWriterLock(this.lockTimeout);
            try
            {
                InnerDictionary.MaxLength = this.MaxLength;

                //先删除已经存在而且过期的Cache项
                if (InnerDictionary.ContainsKey(key) &&
                    ((CacheItem<TKey, TValue>)InnerDictionary[key]).Dependency != null &&
                    ((CacheItem<TKey, TValue>)InnerDictionary[key]).Dependency.HasChanged)
                    this.Remove(key);

                CacheItem<TKey, TValue> item = new CacheItem<TKey, TValue>(key, data, dependency, this);

                if (dependency != null)
                {
                    dependency.UtcLastModified = DateTime.UtcNow;
                    dependency.UtcLastAccessTime = DateTime.UtcNow;
                }

                if (this.overrideExistsItem)
                    InnerDictionary[key] = item;
                else
                    InnerDictionary.Add(key, item);

                this.Counters.EntriesCounter.RawValue = InnerDictionary.Count;

                return data;
            }
            finally
            {
                this.rWLock.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// 属性，获取CacheQueue中的存储的Cache项的数量
        /// </summary>
        public override int Count
        {
            get
            {
                return InnerDictionary.Count;
            }
        }

        /// <summary>
        /// 通过Cache项的key获取Cache项Value的索引器
        /// </summary>
        /// <param name="key">cache项key</param>
        /// <returns>cache项Value</returns>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Caching\CacheQueueTest.cs" region="GetCacheItemTest" lang="cs" title="通过Cache项的key获取Cache项Value" />
        /// </remarks>
        public TValue this[TKey key]
        {
            get
            {
                CacheItem<TKey, TValue> item = null;
                this.TotalCounters.HitRatioBaseCounter.Increment();
                this.Counters.HitRatioBaseCounter.Increment();

                try
                {
                    this.rWLock.AcquireReaderLock(this.lockTimeout);
                    try
                    {

                        item = InnerDictionary[key];
                    }
                    finally
                    {
                        this.rWLock.ReleaseReaderLock();
                    }

                    this.rWLock.AcquireWriterLock(this.lockTimeout);
                    try
                    {
                        //如果Cache项过期则抛出异常
                        if (this.CheckDependencyChanged(key, item))
                            throw new DependencyChangedException(string.Format(Resource.DependencyChanged, key, item.Dependency));

                        //重置cache项的最后访问时间
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

                    return InnerDictionary[key].Value;
                }
                catch (System.Exception)
                {
                    this.TotalCounters.MissesCounter.Increment();
                    this.Counters.MissesCounter.Increment();
                    throw;
                }
            }
            set
            {
                this.rWLock.AcquireWriterLock(this.lockTimeout);
                try
                {
                    CacheItem<TKey, TValue> item;

                    if (InnerDictionary.TryGetValue(key, out item) == false)
                    {
                        this.Add(key, value);
                        item = InnerDictionary[key];
                    }
                    else
                        item.Value = value;

                    item.UpdateDependencyLastModifyTime();
                }
                finally
                {
                    this.rWLock.ReleaseWriterLock();
                }
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
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Caching\CacheQueueTest.cs" region="GetCacheItemTest" lang="cs" title="通过key，获取Cache项的value" />
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
                result = InnerDictionary.TryGetValue(key, out item);
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
                    //判断cache项是否过期
                    if (this.CheckDependencyChanged(key, item))
                        result = false;
                    else
                    {
                        data = item.Value;
                        //修改Cache项的最后访问时间
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
        public TValue GetOrAddNewValue(TKey key, CacheItemNotExistsAction action)
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
        /// <param name="key">缓存唯一标识</param>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Caching\CacheQueueTest.cs" region="AddRemoveClearTest" lang="cs" title="增加、移除、获取CacheItem项" />
        /// </remarks>
        public void Remove(TKey key)
        {
            CacheItem<TKey, TValue> item;

            this.rWLock.AcquireWriterLock(this.lockTimeout);
            try
            {
                if (InnerDictionary.TryGetValue(key, out item))
                    this.InnerRemove(key, item);

                this.Counters.EntriesCounter.RawValue = InnerDictionary.Count;
            }
            finally
            {
                this.rWLock.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// 重载基类方法，删除传入的CacheItem
        /// </summary>
        /// <param name="cacheItem">缓存队列对象</param>
        internal protected override void RemoveItem(CacheItemBase cacheItem)
        {
            this.Remove(((CacheItem<TKey, TValue>)cacheItem).Key);
        }

        /// <summary>
        /// 判断CacheQueue中是否包含key键的Cache项
        /// </summary>
        /// <param name="key">查询的cache项的键值</param>
        /// <returns>如果包含此键值，返回true，否则返回false</returns>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Caching\CacheQueueTest.cs" region="AddRemoveClearTest" lang="cs" title="增加、移除、获取CacheItem项" />
        /// </remarks>
        public bool ContainsKey(TKey key)
        {
            this.TotalCounters.HitRatioBaseCounter.Increment();
            this.Counters.HitRatioBaseCounter.Increment();

            this.rWLock.AcquireReaderLock(this.lockTimeout);
            try
            {
                bool result = ((InnerDictionary.ContainsKey(key) &&
                            ((CacheItem<TKey, TValue>)InnerDictionary[key]).Dependency == null) ||
                                (InnerDictionary.ContainsKey(key) &&
                                ((CacheItem<TKey, TValue>)InnerDictionary[key]).Dependency != null
                                && !((CacheItem<TKey, TValue>)InnerDictionary[key]).Dependency.HasChanged));

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
        /// 清空整个CacheQueue，删除CacheQueue中所有的Cache项
        /// </summary>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Caching\CacheQueueTest.cs" region="AddRemoveClearTest" lang="cs" title="增加、移除、获取CacheItem项" />
        /// </remarks>
        public override void Clear()
        {
            this.rWLock.AcquireWriterLock(this.lockTimeout);
            try
            {
                foreach (KeyValuePair<TKey, CacheItem<TKey, TValue>> kp in InnerDictionary)
                    kp.Value.Dispose();

                InnerDictionary.Clear();
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
                foreach (KeyValuePair<TKey, CacheItem<TKey, TValue>> kp in InnerDictionary)
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
            InnerDictionary.Remove(key);
            item.Dispose();
        }

        private LruDictionary<TKey, CacheItem<TKey, TValue>> InnerDictionary
        {
            get
            {
                if (this.innerDictionary == null)
                    this.innerDictionary = new LruDictionary<TKey, CacheItem<TKey, TValue>>(MaxLength);

                return this.innerDictionary;
            }
        }
    }
}