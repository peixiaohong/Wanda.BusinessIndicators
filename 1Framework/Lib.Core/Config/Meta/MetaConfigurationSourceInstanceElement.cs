using System;
using System.Configuration;
using Lib.Core;

namespace Lib.Config
{
    /// <summary>
    /// SourceMapping实例的配置元素.
    /// </summary>
    class MetaConfigurationSourceInstanceElement : ConfigurationElement
    {
        /// <summary>
        /// Private const
        /// </summary>
        private const string NameItem = "name";
        private const string PathItem = "path";
        private const string ModeItem = "mode";

        /// <summary>
        /// 站点的名称，不可以重复
        /// </summary>
        [ConfigurationProperty(MetaConfigurationSourceInstanceElement.NameItem, IsRequired = true, IsKey = true)]
        public string Name
        {
            get
            {
                return base[MetaConfigurationSourceInstanceElement.NameItem] as string;
            }
        }

        /// <summary>
        /// 全局配置文件的路径和文件名
        /// </summary>
        [ConfigurationProperty(MetaConfigurationSourceInstanceElement.PathItem, IsRequired = true)]
        public string Path
        {
            get
            {
                return base[MetaConfigurationSourceInstanceElement.PathItem] as string;
            }
        }

        /// <summary>
        /// 配置文件使用的访问类型，web访问方式mode可配置为：web，winForm访问方式mode可配置为：win。
        /// </summary>
        [ConfigurationProperty(MetaConfigurationSourceInstanceElement.ModeItem, IsRequired = true)]
        public string Mode
        {
            get
            {
                return base[MetaConfigurationSourceInstanceElement.ModeItem] as string;
            }
        }

        /// <summary>
        /// 获取某个 Instance 的运行模式
        /// </summary>
        /// <returns></returns>
        public InstanceMode GetMode()
        {
            switch (Mode.ToLower().Trim())
            {
                case "web":
                    return InstanceMode.Web;
                case "win":
                    return InstanceMode.Windows;
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// 当前实例的配置文件映射
        /// </summary>
        [ConfigurationProperty(MetaConfigurationSourceMappingElementCollection.Name)]
        public MetaConfigurationSourceMappingElementCollection Mappings
        {
            get
            {
                return base[MetaConfigurationSourceMappingElementCollection.Name] as
                    MetaConfigurationSourceMappingElementCollection;
            }
        }
    }
}
