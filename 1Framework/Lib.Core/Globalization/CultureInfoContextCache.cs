using System;
using System.Globalization;
using Lib.Cache;

namespace Lib.Globalization
{
    internal sealed class CultureInfoContextCache : ContextCacheQueueBase<string, CultureInfo>
    {
        public static CultureInfoContextCache Instance
        {
            get
            {
                return ContextCacheManager.GetInstance<CultureInfoContextCache>();
            }
        }

        private CultureInfoContextCache()
        {
        }
    }
}
