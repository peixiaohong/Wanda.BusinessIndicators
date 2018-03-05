using System;
using Lib.Cache;

namespace Lib.Data
{
    internal sealed class ORMappingsCache : CacheQueue<System.Type, ORMappingItemCollection>
    {
        public static readonly ORMappingsCache Instance = CacheManager.GetInstance<ORMappingsCache>();

        private ORMappingsCache()
        {
        }

        internal static object syncRoot = new object();
    }
}
