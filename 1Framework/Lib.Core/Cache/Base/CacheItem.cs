using System;
using System.Collections.Generic;

namespace Lib.Cache
{
    /// <summary>
    /// 组件内部泛型类,封装存储用户提供的Value值,以及相应的Dependency
    /// 此类实现了IDisposable
    /// </summary>
    /// <typeparam name="TValue">值类型</typeparam>
    /// <typeparam name="TKey" >键类型</typeparam>
    internal class CacheItem<TKey, TValue> : CacheItemBase
    {
        private TKey key;

        public TKey Key
        {
            get { return this.key; }
            set { this.key = value; }
        }

        private TValue tValue;

        /// <summary>
        /// 属性,获取或设置CacheItem的value
        /// </summary>
        public TValue Value
        {
            get { return this.tValue; }
            set { this.tValue = value; }
        }

        /// <summary>
        /// 构造函数，初始化没有Dependency的CacheItem
        /// </summary>
        /// <param name="tKey">初始化CacheItem的Key值</param>
        /// <param name="data">初始化CacheItem的Value值</param>
        /// <param name="cacheQueue">本CacheItem所属的CacheQueue的引用</param>
        public CacheItem(TKey tKey, TValue data, CacheQueueBase cacheQueue)
            : base(cacheQueue)
        {
            this.key = tKey;
            this.tValue = data;
        }

        /// <summary>
        /// 构造函数，初始化具有Dependency的CacheItem
        /// </summary>
        /// <param name="tKey">初始化CacheItem的键值</param>
        /// <param name="data">CacheItem的Value</param>
        /// <param name="dependencyBase">与此CacheItem相关的Dependency，用以判断过期</param>
        /// <param name="cacheQueue"> 本CacheItem所属的CacheQueue的引用</param>
        public CacheItem(TKey tKey, TValue data, DependencyBase dependencyBase, CacheQueueBase cacheQueue)
            : base(cacheQueue)
        {
            this.key = tKey;
            this.tValue = data;

            if (dependencyBase != null)
            {
                dependencyBase.CacheItem = this;
                dependencyBase.CacheItemBinded();
            }

            this.Dependency = dependencyBase;
        }

        /// <summary>
        /// 得到CacheItem的KeyValue值
        /// </summary>
        /// <returns></returns>
        public override KeyValuePair<object, object> GetKeyValue()
        {
            return new KeyValuePair<object, object>(this.key, this.tValue);
        }

        /// <summary>
        /// 设置CacheItem的值
        /// </summary>
        /// <param name="value">设置值</param>
        public override void SetValue(object value)
        {
            this.tValue = (TValue)value;
            UpdateDependencyLastModifyTime();
        }

        //重置Cache项的最后修改时间和最后访问时间
        internal void UpdateDependencyLastModifyTime()
        {
            if (this.Dependency != null)
            {
                DateTime now = DateTime.UtcNow;

                this.Dependency.UtcLastModified = now;
                this.Dependency.UtcLastAccessTime = now;
            }
        }
    }
}
