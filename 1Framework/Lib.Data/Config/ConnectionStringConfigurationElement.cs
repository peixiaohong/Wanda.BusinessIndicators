using System;
using System.Configuration;
using Lib.Config;

namespace Lib.Data
{
    /// <summary>
    /// 连接串对象的配置元素
    /// </summary>
    /// <remarks>由于Builders是在该节点构造完毕后才有，因此对各属性的处理采用后加载处置的办法</remarks>
    sealed class ConnectionStringConfigurationElement : ConnectionStringConfigurationElementBase
    {
    }

    [ConfigurationCollection(typeof(ConnectionStringConfigurationElement), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    sealed class ConnectionStringConfigurationElementCollection : NamedConfigurationElementCollection<ConnectionStringConfigurationElement>
    {
    }
}
