using System;

namespace Lib.Cache
{
    /// <summary>
    /// 所有Dependency的抽象基类，此基类实现了IDisposable接口
    /// </summary>
    public abstract class DependencyBase : IDisposable
    {
        //Cache项的最后修改时间
        private DateTime utcLastModified;

        //Cache项的最后访问时间
        private DateTime utlLastAccessTime;

        //包含本Dependency的CacheItem的引用
        private CacheItemBase cacheItem;

        /// <summary>
        /// Dependency所依赖的CacheItem
        /// </summary>
        public CacheItemBase CacheItem
        {
            get { return this.cacheItem; }
            internal set { this.cacheItem = value; }
        }

        /// <summary>
        /// 属性,获取或设置Cache项最后修改时间的UTC时间值
        /// </summary>
        public virtual DateTime UtcLastModified
        {
            get { return this.utcLastModified; }
            set { this.utcLastModified = value; }
        }

        /// <summary>
        /// 属性,获取或设置Cache项的最后访问时间的UTC时间值
        /// </summary>
        public virtual DateTime UtcLastAccessTime
        {
            get { return this.utlLastAccessTime; }
            set { this.utlLastAccessTime = value; }
        }

        /// <summary>
        /// 属性,获取此Dependency是否过期
        /// </summary>
        public virtual bool HasChanged
        {
            get { return false; }
        }

        /// <summary>
        /// 
        /// </summary>
        internal protected virtual void SetChanged()
        {
        }
        //2008年7月，沈峥添加

        /// <summary>
        /// 当Dependency对象绑定到CacheItem时，会调用此方法。此方法被调用时，保证Dependency的CacheItem属性已经有值
        /// </summary>
        internal protected virtual void CacheItemBinded()
        {
        }

        #region IDisposable 成员

        /// <summary>
        /// 实现IDisposable接口
        /// </summary>
        public virtual void Dispose()
        {
        }

        #endregion
    }

}
