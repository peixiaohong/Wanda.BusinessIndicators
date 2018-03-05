using System.Collections.Generic;
using System.Configuration;
using System.Web.UI;
using Lib.Core;

namespace Lib.Web
{
    /// <summary>
    /// PageModules配置信息
    /// </summary>
    public class PageModulesSection : ConfigurationSection
    {
        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("", IsDefaultCollection = true)]
        public PageModuleElementCollection PageModules
        {
            get
            {
                return (PageModuleElementCollection)this[string.Empty];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal Dictionary<string, IPageModule> Create()
        {
            PageModuleElementCollection configModules = PageModules;
            Dictionary<string, IPageModule> modules = new Dictionary<string, IPageModule>(configModules.Count);

            foreach (PageModuleElement element in configModules)
            {
                IPageModule module = (IPageModule)TypeCreator.CreateInstance(element.Type);
                modules.Add(element.Name, module);
            }

            return modules;
        }
    }

    /// <summary>
    /// PageModule配置节集合
    /// </summary>
    public class PageModuleElementCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// CreateNewElement
        /// </summary>
        /// <returns></returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new PageModuleElement();
        }

        /// <summary>
        /// GetElementKey
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PageModuleElement)element).Name;
        }
    }

    /// <summary>
    /// PageModule配置节
    /// </summary>
    public class PageModuleElement : ConfigurationElement
    {
        /// <summary>
        /// 名称
        /// </summary>
        [ConfigurationProperty("name", DefaultValue = "")]
        public string Name
        {
            get
            {
                return (string)this["name"];
            }
        }

        /// <summary>
        /// 类型
        /// </summary>
        [ConfigurationProperty("type", DefaultValue = "")]
        public string Type
        {
            get
            {
                return (string)this["type"];
            }
        }
    }

    /// <summary>
    /// IPageModule
    /// </summary>
    public interface IPageModule
    {
        /// <summary>
        /// Init
        /// </summary>
        /// <param name="page"></param>
        void Init(Page page);

        /// <summary>
        /// Dispose
        /// </summary>
        void Dispose();
    }

    /// <summary>
    /// BasePageModule
    /// </summary>
    public class BasePageModule : IPageModule
    {
        /// <summary>
        /// Init
        /// </summary>
        /// <param name="page"></param>
        protected virtual void Init(Page page)
        {
        }

        /// <summary>
        /// Dispose
        /// </summary>
        protected virtual void Dispose()
        {
        }

        void IPageModule.Init(Page page)
        {
            this.Init(page);
        }

        void IPageModule.Dispose()
        {
            this.Dispose();
        }
    }
}
