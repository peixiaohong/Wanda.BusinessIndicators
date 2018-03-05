using System;
using System.Configuration;
using Lib.Config;

namespace Lib.Cache
{
    /// <summary>
    /// Cache的配置信息
    /// </summary>
    public sealed class CacheSettingsSection : ConfigurationSection
    {
        /// <summary>
        /// 获取Cache的配置信息
        /// </summary>
        /// <returns>Cache的配置信息</returns>
        public static CacheSettingsSection GetConfig()
        {
            CacheSettingsSection result = (CacheSettingsSection)ConfigurationBroker.GetSection("cacheSettings");

            if (result == null)
                result = new CacheSettingsSection();

            return result;
        }

        private CacheSettingsSection()
        {
        }

        /// <summary>
        /// 缺省的队列长度
        /// </summary>
        [ConfigurationProperty("defaultQueueLength", DefaultValue = 100)]
        public int DefaultQueueLength
        {
            get
            {
                return (int)this["defaultQueueLength"];
            }
        }

        /// <summary>
        /// 清理间隔
        /// </summary>
        public TimeSpan ScanvageInterval
        {
            get
            {
                return TimeSpan.FromSeconds(this.ScanvageIntervalSeconds);
            }
        }

        [ConfigurationProperty("scanvageInterval", DefaultValue = 60)]
        private int ScanvageIntervalSeconds
        {
            get
            {
                return (int)this["scanvageInterval"];
            }
        }

        /*
        /// <summary>
        /// 如果Cache队列没有定义性能计数器实例名称，可以使用的缺省的Cache队列的性能计数器的实例名称。
        /// 如果DefaultInstanceName也没有定义，可以使用Cache队列的类型名称
        /// </summary>
        [ConfigurationProperty("defaultInstanceName", DefaultValue = "")]
        public string DefaultInstanceName
        {
            get
            {
                return (string)this["defaultInstanceName"];
            }
        }
        */

        /// <summary>
        /// 具体每个Cache队列的设置
        /// </summary>
        [ConfigurationProperty("queueSettings")]
        public QueueSettingCollection QueueSettings
        {
            get
            {
                return (QueueSettingCollection)this["queueSettings"];
            }
        }
    }

}
