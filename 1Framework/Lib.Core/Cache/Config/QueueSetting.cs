using System.Configuration;

namespace Lib.Cache
{
    /// <summary>
    /// 每个Cache队列的设置
    /// </summary>
    public sealed class QueueSetting : ConfigurationElement
    {
        private const int CacheDefaultQueueLength = 100;

        internal QueueSetting()
        {
        }

        /// <summary>
        /// 对象的类型名称
        /// </summary>
        [ConfigurationProperty("typeName", Options = ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired)]
        public string TypeName
        {
            get
            {
                return (string)this["typeName"];
            }
        }

        /// <summary>
        /// 队列的深度
        /// </summary>
        [ConfigurationProperty("queueLength", DefaultValue = CacheDefaultQueueLength)]
        public int QueueLength
        {
            get
            {
                return (int)this["queueLength"];
            }
        }

        /*
        /// <summary>
        /// 该Cache队列所对应的性能计数器的实例名称
        /// </summary>
        [ConfigurationProperty("instanceName", DefaultValue = "")]
        public string InstanceName
        {
            get
            {
                return (string)this["instanceName"];
            }
        }
         * */
    }
}
