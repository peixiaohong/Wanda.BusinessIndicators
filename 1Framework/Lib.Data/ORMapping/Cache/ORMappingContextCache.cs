using System;
using Lib.Cache;

namespace Lib.Data
{
    /// <summary>
    /// 上下文中的ORMapping Cache，通常先在这里面查找，如果没有，再去ORMappingsCache查找
    /// </summary>
    public sealed class ORMappingContextCache : ContextCacheQueueBase<System.Type, ORMappingItemCollection>
    {
        /// <summary>
        /// 获取实例
        /// </summary>
        public static readonly ORMappingContextCache Instance = ContextCacheManager.GetInstance<ORMappingContextCache>();

        private ORMappingContextCache()
        {
        }
    }
}
