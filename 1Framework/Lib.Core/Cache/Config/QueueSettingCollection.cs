using System;
using System.Configuration;

namespace Lib.Cache
{
    /// <summary>
    /// 每个Cache队列的设置集合
    /// </summary>
    public sealed class QueueSettingCollection : ConfigurationElementCollection
    {
        private QueueSettingCollection()
        {
        }

        /// <summary>
        /// 获取配置元素的键值
        /// </summary>
        /// <param name="element">配置元素</param>
        /// <returns>键值</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((QueueSetting)element).TypeName;
        }

        /// <summary>
        /// 创建配置元素
        /// </summary>
        /// <returns></returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new QueueSetting();
        }

        /// <summary>
        /// 获取指定类型的队列设置
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>队列设置</returns>
        public QueueSetting this[System.Type type]
        {
            get
            {
                return (QueueSetting)BaseGet(type.FullName);
            }
        }
    }

}
