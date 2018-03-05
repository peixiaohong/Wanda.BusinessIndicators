using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Lib.Web
{
    /// <summary>
    /// 页面扩展信息配置节
    /// </summary>
    public class PageContentSection : ConfigurationSection
    {
        /// <summary>
        /// Css文件配置节集合
        /// </summary>
        [ConfigurationProperty("cssClasses", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
        public FilePathConfigElementCollection CssClasses
        {
            get
            {
                return (FilePathConfigElementCollection)this["cssClasses"];
            }
        }

        /// <summary>
        /// 脚本文件配置节集合
        /// </summary>
        [ConfigurationProperty("scripts", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
        public FilePathConfigElementCollection Scripts
        {
            get
            {
                return (FilePathConfigElementCollection)this["scripts"];
            }
        }

        /// <summary>
        /// 不自动加载的页面配置节集合
        /// </summary>
        [ConfigurationProperty("notAutoLoadPages", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
        public NotAutoLoadPageConfigElementCollection NotAutoLoadPages
        {
            get
            {
                return (NotAutoLoadPageConfigElementCollection)this["notAutoLoadPages"];
            }
        }

        /// <summary>
        /// 是否自动为每个页面加载扩展
        /// </summary>
        [ConfigurationProperty("autoLoad", DefaultValue = false)]
        public bool AutoLoad
        {
            get
            {
                return (bool)this["autoLoad"];
            }
        }
    }

    /// <summary>
    /// 不自动加载的页面配置节集合
    /// </summary>
    public class NotAutoLoadPageConfigElementCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new NotAutoLoadPageConfigElement();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((NotAutoLoadPageConfigElement)element).Path;
        }
    }

    /// <summary>
    /// 不自动加载的页面配置节
    /// </summary>
    public class NotAutoLoadPageConfigElement : ConfigurationElement
    {
        /// <summary>
        /// page路径
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
