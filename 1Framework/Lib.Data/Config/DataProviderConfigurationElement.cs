using System;
using System.Configuration;
using Lib.Config;

namespace Lib.Data
{
    sealed class DataProviderConfigurationElement : TypeConfigurationElement
    {

    }

    [ConfigurationCollection(typeof(DataProviderConfigurationElement), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    sealed class DataProviderConfigurationElementCollection : NamedConfigurationElementCollection<DataProviderConfigurationElement>
    {

    }
}
