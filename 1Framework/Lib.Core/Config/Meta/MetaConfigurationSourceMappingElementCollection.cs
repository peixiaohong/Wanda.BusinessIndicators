using System;
using System.Configuration;

namespace Lib.Config
{
    /// <summary>
    /// 每一个应用的配置文件映射集合
    /// </summary>
    [ConfigurationCollection(typeof(MetaConfigurationSourceMappingElement),
    CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    class MetaConfigurationSourceMappingElementCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Public const
        /// </summary>
        public const string Name = "applications";


        /// <summary>
        /// 由索引获取MetaConfigurationSourceMappingElement
        /// </summary>
        public MetaConfigurationSourceMappingElement this[int index]
        {
            get
            {
                return (MetaConfigurationSourceMappingElement)base.BaseGet(index);
            }
        }

        /// <summary>
        /// 由名称获取MetaConfigurationSourceMappingElement
        /// </summary>
        public new MetaConfigurationSourceMappingElement this[string name]
        {
            get
            {
                return base.BaseGet(name) as MetaConfigurationSourceMappingElement;
            }
        }

        /// <summary>
        /// 创建配置元素实例
        /// </summary>
        /// <returns></returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new MetaConfigurationSourceMappingElement();
        }

        /// <summary>
        /// 获取配置元素的键值
        /// </summary>
        /// <param name="element">配置元素</param>
        /// <returns>键值</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as MetaConfigurationSourceMappingElement).Application;
        }

        /// <summary>
        /// 如果该集合中有一个配置项app匹配成功，则返回true
        /// </summary>
        /// <param name="appPath"></param>
        /// <returns></returns>
        public bool IsMatched(string appPath)
        {
            bool isMatched = false;

            foreach (MetaConfigurationSourceMappingElement element in this)
            {
                if (element.IsMatched(appPath))
                {
                    isMatched = true;
                    break;
                }
            }

            return isMatched;
        }
    }
}
