using System;
using System.Configuration;

namespace Lib.Log
{
    /// <summary>
    /// LoggerElement集合类
    /// </summary>
    /// <remarks>
    /// ConfigurationElementCollection的派生类，不可继承
    /// </remarks>
    public sealed class LoggerElementCollection : ConfigurationElementCollection
    {
        private LoggerElementCollection()
        {
        }

        /// <summary>
        /// 获取键值
        /// </summary>
        /// <param name="element">LoggerElement对象</param>
        /// <returns>键值</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((LoggerElement)element).Name;
        }

        /// <summary>
        /// 创建新的LoggerElement对象
        /// </summary>
        /// <returns>LoggerElement对象</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new LoggerElement();
        }

        /// <summary>
        /// 根据键值索引，返回LoggerElement对象
        /// </summary>
        /// <param name="name">LoggerElement名称</param>
        /// <returns>LoggerElement对象</returns>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\\Logging\LoggerTest.cs"
        /// lang="cs" region="Create Logger Test" tittle="获取Logger对象"></code>
        /// </remarks>
        new public LoggerElement this[string name]
        {
            get
            {
                return (LoggerElement)BaseGet(name);
            }
            //set
            //{
            //    if (BaseGet(name) != null)
            //        BaseRemoveAt(name);

            //    BaseAdd(name, value);
            //}
        }
    }
}
