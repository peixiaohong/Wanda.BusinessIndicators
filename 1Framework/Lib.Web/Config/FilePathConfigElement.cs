using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Lib.Web
{
    /// <summary>
    /// 文件配置节集合
    /// </summary>
    public class FilePathConfigElementCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new FilePathConfigElement();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((FilePathConfigElement)element).Path;
        }
    }

    /// <summary>
    /// 文件配置节
    /// </summary>
    public class FilePathConfigElement : ConfigurationElement
    {
        /// <summary>
        /// 文件路径
        /// </summary>
        [ConfigurationProperty("path", DefaultValue = "")]
        public string Path
        {
            get
            {
                return (string)this["path"];
            }
        }
    }
}
