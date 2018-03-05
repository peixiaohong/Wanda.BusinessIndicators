using Lib.Cache;

namespace Lib.Config
{
    /// <summary>
    /// 用于存放 Configuration 的 Cache
    /// </summary>
    sealed class ConfigurationCache : PortableCacheQueue<string, System.Configuration.Configuration>
    {
        /// <summary>
        /// 获取实例
        /// </summary>
        private static readonly ConfigurationCache instance = CacheManager.GetInstance<ConfigurationCache>();
         
        /// <summary>
        /// 获取实例
        /// </summary>
        public static ConfigurationCache Instance
        {
            get
            {
                return ConfigurationCache.instance;
            }
        }

        private ConfigurationCache()
        {

        }
    } // class end

}
