using System;
using System.Configuration;
using Lib.Config;

namespace Lib.Data
{
    class BuilderConfigurationElement : TypeConfigurationElement
    {
        /// <summary>
        /// Builder适用于ConnectionString的Attribute名称
        /// </summary>
        [ConfigurationProperty("attributeName")]
        public string AttributeName
        {
            get
            {
                return (string)this["attributeName"];
            }
        }
    }

    [ConfigurationCollection(typeof(BuilderConfigurationElement), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    class BuildersConfiguratonElementCollection : NamedConfigurationElementCollection<BuilderConfigurationElement>
    {
    }
}
