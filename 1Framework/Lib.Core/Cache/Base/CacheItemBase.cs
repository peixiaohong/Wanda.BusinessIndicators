using System;
using System.Collections.Generic;

namespace Lib.Cache
{
    /// <summary>
    /// CacheItem的基类
    /// </summary>
    public abstract class CacheItemBase : IDisposable
    {
        private CacheQueueBase cacheQueue;

        //与此CacheItem相关的Dependency,以判断CacheItem的过期
        private DependencyBase dependency = null;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="cacheQueueBase"></param>
        public CacheItemBase(CacheQueueBase cacheQueueBase)
        {
            this.cacheQueue = cacheQueueBase;
        }

        /// <summary>
        /// 在Cache队列中清除Cache项
        /// </summary>
        public void RemoveCacheItem()
        {
            this.cacheQueue.RemoveItem(this);
        }

        //为了CacheDependency能够从CacheItem获取到所属的Queue实例，添加此公有属性
        //2008年7月，沈峥添加

        /// <summary>
        /// 当前Cache项所属于的CacheQueue。
        /// </summary>
        public CacheQueueBase Queue
        {
            get
            {
                return this.cacheQueue;
            }
        }

        /// <summary>
        /// 属性，获取或设置与此CacheItem相关联的Dependency
        /// </summary>
        public DependencyBase Dependency
        {
            get { return this.dependency; }
            set { this.dependency = value; }
        }

        /// <summary>
        /// 将此CacheItem设置为必须更新
        /// </summary>
        public void SetChanged()
        {
            if (this.dependency != null)
                this.dependency.SetChanged();
        }
        #region IDisposable 成员

        /// <summary>
        /// 实现IDisposable接口
        /// </summary>
        public void Dispose()
        {
            if (this.Dependency != null)
                this.Dependency.Dispose();
        }

        #endregion

        //为了CacheDependency能够从CacheItem获取到Key和Value，添加此虚方法。
        //2008年7月，沈峥添加

        /// <summary>
        /// 得到CacheItem对应的Key和Value
        /// </summary>
        /// <returns>获得CacheItem的Key和Value，需要派生类重载</returns>
        public abstract KeyValuePair<object, object> GetKeyValue();

        //为了CacheDependency能够为CacheItem设置Value，添加此虚方法。
        //2008年7月，沈峥添加

        /// <summary>
        /// 设置CacheItem的值
        /// </summary>
        /// <param name="value">为CacheItem设置Value</param>
        public abstract void SetValue(object value);
    }
}
