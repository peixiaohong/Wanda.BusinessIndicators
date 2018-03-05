using System;
using System.Configuration;

namespace Lib.Config
{
    /// <summary>
    /// SourceMappings配置节
    /// </summary>
    class MetaConfigurationSourceInstanceSection : ConfigurationSection
    {
        /// <summary>
        /// Public const
        /// </summary>
        public const string Name = "sourceMappings";


        /// <summary>
        /// 所有实例的源映射元素集合
        /// </summary>
        [ConfigurationProperty(MetaConfigurationSourceInstanceElementCollection.Name)]
        public MetaConfigurationSourceInstanceElementCollection Instances
        {
            get
            {
                return base[MetaConfigurationSourceInstanceElementCollection.Name] as
                    MetaConfigurationSourceInstanceElementCollection;
            }
        }

    }
}
