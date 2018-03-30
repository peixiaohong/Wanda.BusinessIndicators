using System;
using System.Configuration;

namespace Lib.Log
{
    /// <summary>
    /// LogListener配置节的基类
    /// </summary>
    /// <remarks>
    /// LogConfigurationElement的派生类，扩展LogFormatter的名称信息
    /// </remarks>
    public class LogListenerElement : LogConfigurationElement
    {
        /// <summary>
        /// 缺省构造函数
        /// </summary>
        public LogListenerElement()
        {
        }

        internal LogListenerElement(string fullPath)
            : base(fullPath)
        {
        }

        /// <summary>
        /// LogFormatter的名称
        /// </summary>
        [ConfigurationProperty("formatter", IsRequired = false)]
        public string LogFormatterName
        {
            get
            {
                return (string)this["formatter"];
            }
        }
    }
}
