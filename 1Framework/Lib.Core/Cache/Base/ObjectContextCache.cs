using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lib.Cache
{
    /// <summary>
    /// 存放Key和Value都是object的ContextCache
    /// </summary>
    public sealed class ObjectContextCache : ContextCacheQueueBase<object, object>
    {
        /// <summary>
        /// ObjectContextCache的实例，此处必须是属性，动态计算
        /// </summary>
        public static ObjectContextCache Instance
        {
            get
            {
                return ContextCacheManager.GetInstance<ObjectContextCache>();
            }
        }

        private ObjectContextCache()
        {
        }
    }
}
