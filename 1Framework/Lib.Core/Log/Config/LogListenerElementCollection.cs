using System;
using System.Configuration;

namespace Lib.Log
{
    /// <summary>
    /// LogListenerElement集合类
    /// </summary>
    /// <remarks>
    /// LogConfigurationElementCollection的派生类
    /// </remarks>
    public sealed class LogListenerElementCollection : LogConfigurationElementCollection
    {
        private string fullPath = string.Empty;

        internal LogListenerElementCollection()
        {
        }

        internal LogListenerElementCollection(string fullPath)
            : base(fullPath)
        {
            this.fullPath = fullPath;
        }

        /// <summary>
        /// 创建新的LogListenerElement对象
        /// </summary>
        /// <returns>LogListenerElement对象</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new LogListenerElement(this.fullPath);
        }

        /// <summary>
        /// 获取键值
        /// </summary>
        /// <param name="element">LogListenerElement对象</param>
        /// <returns>键值</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((LogListenerElement)element).Name;
        }

        /// <summary>
        /// 根据键值索引，返回LogListenerElement对象
        /// </summary>
        /// <param name="name">LogListenerElement名称</param>
        /// <returns>LogListenerElement对象</returns>
        /// <remarks>
        /// <code source="..\Framework\src\DeluxeWorks.Library\Logging\Listeners\TraceListenerFactory.cs" 
        /// lang="cs" region="Get Listeners" title="创建Listeners对象" />
        /// </remarks>
        new public LogListenerElement this[string name]
        {
            get
            {
                return (LogListenerElement)BaseGet(name);
            }
        }
    }
}
