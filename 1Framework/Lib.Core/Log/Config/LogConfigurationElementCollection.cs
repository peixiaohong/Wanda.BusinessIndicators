using System;
using Lib.Config;
using System.Configuration;

namespace Lib.Log
{
    /// <summary>
    /// LogConfigurationElement集合类
    /// </summary>
    /// <remarks>
    /// TypeConfigurationCollection的派生类
    /// </remarks>
    public class LogConfigurationElementCollection : TypeConfigurationCollection
    {
        /// <summary>
        /// 配置节所在的路径
        /// </summary>
        private string fullPath = string.Empty;

        /// <summary>
        /// 缺省构造函数
        /// </summary>
        protected internal LogConfigurationElementCollection()
        {
        }

        internal LogConfigurationElementCollection(string fullPath)
        {
            this.fullPath = fullPath;
        }

        /// <summary>
        /// 创建新的LogConfigurationElement对象
        /// </summary>
        /// <returns>LogConfigurationElement对象</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new LogConfigurationElement(this.fullPath);
        }

        /// <summary>
        /// 根据键值索引，返回LogConfigurationElement对象
        /// </summary>
        /// <param name="name">LogConfigurationElement名称</param>
        /// <returns>LogConfigurationElement对象</returns>
        /// <remarks>
        /// <code source="..\Framework\src\DeluxeWorks.Library\Logging\Filters\LogFilterFactory.cs" 
        /// lang="cs" region="Get FilterPipeline" title="创建LogFilterPipeline对象">
        /// </code>
        /// </remarks>
        public new LogConfigurationElement this[string name]
        {
            get
            {
                return (LogConfigurationElement)InnerGet(name);
            }
        }

        internal void DeserializeElementDirectly(System.Xml.XmlReader reader, bool serializeCollectionKey)
        {
            this.DeserializeElement(reader, serializeCollectionKey);
        }
    }
}
