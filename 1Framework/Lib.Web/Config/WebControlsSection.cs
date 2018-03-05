using System;
using System.Configuration;
using System.Web.Compilation;
using System.Web.UI;

namespace Lib.Web.Config
{
    /// <summary>
    /// WebControl相关配置信息
    /// </summary>
    /// <remarks>WebControl相关配置信息</remarks>
    public sealed class WebControlsSection : ConfigurationSection
    {
        private static readonly ConfigurationProperty _S_PropWebControls =
            new ConfigurationProperty(null, typeof(WebControlConfigElementCollection), null, ConfigurationPropertyOptions.IsDefaultCollection);

        private static ConfigurationPropertyCollection _S_Properties = BuildProperties();

        private static ConfigurationPropertyCollection BuildProperties()
        {
            ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
            //properties.Add(_S_PropDefaultCssUrl);
            properties.Add(_S_PropWebControls);

            return properties;
        }


        /// <summary>
        /// Gets the collection of properties.
        /// </summary>
        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                return _S_Properties;
            }
        }

        /// <summary>
        /// WebControl配置信息集合
        /// </summary>
        /// <remarks>WebControl配置信息集合</remarks>
        public WebControlConfigElementCollection Controls
        {
            get
            {
                return (WebControlConfigElementCollection)this[_S_PropWebControls];
            }
        }

        /// <summary>
        /// 获取某类型控件的配置CssUrl
        /// </summary>
        /// <param name="controlType">控件类别</param>
        /// <returns>CssUrl</returns>
        /// <remarks>获取某类型控件的配置CssUrl</remarks>
        public string GetConfigCssUrl(Type controlType)
        {
            string url = string.Empty;
            WebControlConfigElement elt = (WebControlConfigElement)this.Controls[controlType];
            if (elt == null)
                url = string.Empty;
            else
            {
                url = elt.CssUrl;
            }
            return url;
        }

        /// <summary>
        /// 获取某类型控件的配置CssUrl
        /// </summary>
        /// <param name="control">控件实例</param>
        /// <returns>CssUrl</returns>
        /// <remarks>获取某类型控件的配置CssUrl</remarks>
        public string GetConfigCssUrl(Control control)
        {
            return GetConfigCssUrl(control.GetType());
        }
    }

    /// <summary>
    /// WebControl配置节点集合
    /// </summary>
    public sealed class WebControlConfigElementCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// creates a new WebControlConfigElement
        /// </summary>
        /// <returns>new WebControlConfigElement</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new WebControlConfigElement();
        }

        /// <summary>
        /// Gets the element key for a specified configuration element 
        /// </summary>
        /// <param name="element">The ConfigurationElement to return the key for.</param>
        /// <returns>An Object that acts as the key for the specified ConfigurationElement.</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((WebControlConfigElement)element).Type;
        }

        /// <summary>
        /// 通过控件类型，返回相应配置节点
        /// </summary>
        /// <param name="controlType">控件类型</param>
        /// <returns>配置节点</returns>
        public WebControlConfigElement this[Type controlType]
        {
            get
            {
                return (WebControlConfigElement)base.BaseGet(controlType);
            }
        }

        /// <summary>
        /// 通过控件实例，返回相应配置节点
        /// </summary>
        /// <param name="control">控件实例</param>
        /// <returns>配置节点</returns>
        public WebControlConfigElement this[Control control]
        {
            get
            {
                return this[control.GetType()];
            }
        }
    }

    /// <summary>
    /// 控件配置节点信息
    /// </summary>
    public sealed class WebControlConfigElement : ConfigurationElement
    {
        private static readonly ConfigurationProperty _S_Type =
           new ConfigurationProperty("type", typeof(string), string.Empty, ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty _S_CssUrl =
            new ConfigurationProperty("cssUrl", typeof(string), string.Empty, ConfigurationPropertyOptions.None);
        //private static readonly ConfigurationProperty _S_UseSysCssUrl =
        //    new ConfigurationProperty("useSysCssUrl", typeof(bool), false, ConfigurationPropertyOptions.None);

        private static ConfigurationPropertyCollection _S_Properties = BuildProperties();

        private static ConfigurationPropertyCollection BuildProperties()
        {
            ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
            properties.Add(_S_Type);
            properties.Add(_S_CssUrl);
            //properties.Add(_S_UseSysCssUrl);

            return properties;
        }

        /// <summary>
        /// Gets the collection of properties.
        /// </summary>
        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                return _S_Properties;
            }
        }

        private Type _Type;

        /// <summary>
        /// 控件类型
        /// </summary>
        public Type Type
        {
            get
            {
                if (this._Type == null)
                {
                    string strType = this.StrType;
                    Type t = BuildManager.GetType(strType, false);
                    if (t == null)
                    {
                        throw new ArgumentException();//(string.Format(Resources.DeluxeWebResource.E_UnknownType, new object[] { strType }));
                    }
                    if (!typeof(Control).IsAssignableFrom(t))
                    {
                        throw new ArgumentException();//(string.Format(Resources.DeluxeWebResource.E_NotControl, new object[] { strType }));
                    }

                    this._Type = t;
                }
                return _Type;
            }
        }

        /// <summary>
        /// 控件类型
        /// </summary>
        internal string StrType
        {
            get
            {
                return (string)this[_S_Type];
            }
        }

        /// <summary>
        /// 控件对应的CssUrl
        /// </summary>
        public string CssUrl
        {
            get
            {
                return (string)this[_S_CssUrl];
            }
        }
    }
}
