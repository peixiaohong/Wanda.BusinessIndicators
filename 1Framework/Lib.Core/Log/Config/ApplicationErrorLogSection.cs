using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Diagnostics;
using Lib.Config;

namespace Lib.Log
{
    /// <summary>
    /// 应用错误日志配置
    /// </summary>
    public class ApplicationErrorLogSection : ConfigurationSection
    {
        /// <summary>
        /// 获取Section实例
        /// </summary>
        /// <returns></returns>
        public static ApplicationErrorLogSection GetSection()
        {
            return (ApplicationErrorLogSection)ConfigurationBroker.GetSection("appicationErrorLog");
        }

        /// <summary>
        /// 异常和日志对应配置
        /// </summary>
        [ConfigurationProperty("exceptionLogs", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
        public ExceptionLogElementColletion ExceptionLogs
        {
            get
            {
                return (ExceptionLogElementColletion)this["exceptionLogs"];
            }
        }

        /// <summary>
        /// 默认事件类型
        /// </summary>
        [ConfigurationProperty("defaultLogEventType")]
        public TraceEventType DefaultLogEventType
        {
            get
            {
                return (TraceEventType)this["defaultLogEventType"];
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public TraceEventType GetExceptionLogEventType(Exception ex)
        {
            Type type = ex.GetType();
            string name = type.FullName;
            ExceptionLogElement elt = ExceptionLogs.GetElement(name);
            if (elt == null)
            {
                name = type.Name;
                elt = ExceptionLogs.GetElement(name);
            }

            TraceEventType eventType = elt == null ? this.DefaultLogEventType : elt.LogEventType;

            return eventType;
        }
    }

    public class ExceptionLogElementColletion : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ExceptionLogElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ExceptionLogElement)element).Name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ExceptionLogElement GetElement(string name)
        {
            return (ExceptionLogElement)this.BaseGet(name);
        }

    }

    public class ExceptionLogElement : NamedConfigurationElement
    {
        /// <summary>
        /// 事件类型
        /// </summary>
        [ConfigurationProperty("logEventType")]
        public TraceEventType LogEventType
        {
            get
            {
                return (TraceEventType)this["logEventType"];
            }
        }
    }
}
