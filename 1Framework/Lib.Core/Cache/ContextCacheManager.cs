using System;
using System.Collections.Generic;
using System.Web;
using Lib.Core;

namespace Lib.Cache
{
    /// <summary>
    /// 在调用上下文中缓存的Cache管理器，该Cache的生命期，仅仅在当前线程（WinForm）或一次Http（Web）)请求过程中
    /// </summary>
    public static class ContextCacheManager
    {
        [ThreadStatic]
        private static Dictionary<System.Type, ContextCacheQueueBase> cacheDictionary;

        /// <summary>
        /// 获取Cache队列的实例，如果该队列没有被缓存，可以动态创建一个实例
        /// </summary>
        /// <typeparam name="T">Cache队列的类型</typeparam>
        /// <returns>Cache队列的实例</returns>
        public static T GetInstance<T>() where T : ContextCacheQueueBase
        {
            System.Type type = typeof(T);

            ContextCacheQueueBase instance;
            Dictionary<System.Type, ContextCacheQueueBase> cacheDict = GetCacheDictionary();

            if (cacheDict.TryGetValue(type, out instance) == false)
            {
                instance = (ContextCacheQueueBase)Activator.CreateInstance(typeof(T), true);
                cacheDict.Add(type, instance);
            }

            return (T)instance;
        }

        private static Dictionary<System.Type, ContextCacheQueueBase> GetCacheDictionary()
        {
            Dictionary<System.Type, ContextCacheQueueBase> cacheDict;

            if (EnvironmentHelper.Mode == InstanceMode.Web)
                cacheDict = (Dictionary<System.Type, ContextCacheQueueBase>)HttpContext.Current.Items["ContextCacheDictionary"];
            else
                cacheDict = ContextCacheManager.cacheDictionary;

            if (cacheDict == null)
                cacheDict = new Dictionary<Type, ContextCacheQueueBase>();

            if (EnvironmentHelper.Mode == InstanceMode.Web)
                HttpContext.Current.Items["ContextCacheDictionary"] = cacheDict;
            else
                ContextCacheManager.cacheDictionary = cacheDict;

            return cacheDict;
        }
    }
}
