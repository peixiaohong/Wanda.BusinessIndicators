using System;

namespace Lib.Cache
{
    /// <summary>
    /// 混合Cache依赖类，是AbsoluteTimeDependency、SlidingTimeDependency和FileCacheDependency的任意组合
    /// 当其中任何一项过期时，认为与此MixedDependency相关的Cache项过期
    /// </summary>
    public sealed class MixedDependency : DependencyBase
    {
        private DependencyBase[] dependencyArray = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dependencyArray">包含AbsoluteTimeDependency、SlidingTimeDependency、FileCacheDependency或自身的数组</param>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Caching\MixedDependencyTest.cs" region="HasChangedTest" lang="cs" title="组合的Dependency" />
        /// </remarks>
        public MixedDependency(params DependencyBase[] dependencyArray)
        {
            this.dependencyArray = dependencyArray;

            //对最后修改时间和最后访问时间进行初始化
            this.UtcLastModified = DateTime.UtcNow;
            this.UtcLastAccessTime = DateTime.UtcNow;
        }

        /// <summary>
        /// 属性，判断此Cache依赖是否过期
        /// </summary>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Caching\MixedDependencyTest.cs" region="HasChangedTest" lang="cs" title="组合的Dependency" />
        /// </remarks>
        public override bool HasChanged
        {
            get
            {
                bool result = false;

                for (int index = 0; index < this.dependencyArray.Length; index++)
                    if (this.dependencyArray[index].HasChanged)
                    {
                        result = true;
                        break;
                    }

                return result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal protected override void SetChanged()
        {
            for (int index = 0; index < this.dependencyArray.Length; index++)
                this.dependencyArray[index].SetChanged();
        }

        /// <summary>
        /// 实现IDisposable接口
        /// </summary>
        public override void Dispose()
        {
            foreach (DependencyBase bcd in this.dependencyArray)
                bcd.Dispose();
        }

        /// <summary>
        /// 属性,获取或设置Cache项的最后访问时间的UTC时间值
        /// </summary>
        public override DateTime UtcLastAccessTime
        {
            get
            {
                return base.UtcLastAccessTime;
            }
            set
            {
                for (int i = 0; i < this.dependencyArray.Length; i++)
                    this.dependencyArray[i].UtcLastAccessTime = value;

                base.UtcLastAccessTime = value;
            }
        }

        /// <summary>
        /// 属性,获取或设置Cache项最后修改时间的UTC时间值
        /// </summary>
        public override DateTime UtcLastModified
        {
            get
            {
                return base.UtcLastModified;
            }
            set
            {
                for (int i = 0; i < this.dependencyArray.Length; i++)
                    this.dependencyArray[i].UtcLastModified = value;

                base.UtcLastModified = value;
            }
        }

        //2008年7月，沈峥添加

        /// <summary>
        /// 当Dependency对象绑定到CacheItem时调用此方法。回递归调用子Dependency的CacheItemBinded方法
        /// </summary>
        protected internal override void CacheItemBinded()
        {
            for (int i = 0; i < this.dependencyArray.Length; i++)
            {
                this.dependencyArray[i].CacheItem = this.CacheItem;
                this.dependencyArray[i].CacheItemBinded();
            }
        }
    }
}
