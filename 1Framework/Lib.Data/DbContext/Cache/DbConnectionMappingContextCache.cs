using System;
using Lib.Cache;

namespace Lib.Data
{
    internal class DbConnectionMappingContextCache : ContextCacheQueueBase<string, DbConnectionMappingContext>
    {
        public static DbConnectionMappingContextCache Instance
        {
            get
            {
                return ContextCacheManager.GetInstance<DbConnectionMappingContextCache>();
            }
        }
    }
}
