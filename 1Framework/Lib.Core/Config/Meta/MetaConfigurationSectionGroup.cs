using System.Configuration;

namespace Lib.Config
{
    /// <summary>
    /// DeluxeWorks root meta configuration section group entity.
    /// 
    /// <example>
    ///  <Lib.Core.metaConfig>
    ///  <instance name="risk.qd">
    ///    <configMappings>
    ///      <add name="db" value="db.xml"/>
    ///      <add name="authorization" value="authorization.xml"/>
    ///    </configMappings>
    ///  </instance>
    ///  <instance name="risk.sh">
    ///    <configMappings>
    ///      <add name="db" value="db2.xml"/>
    ///      <add name="authorization" value="authorization2.xml"/>
    ///      <add name="olap" value="olap.xml"/>
    ///    </configMappings>
    ///  </instance>
    ///</Lib.Core.metaConfig>
    /// </example>
    /// </summary>
    sealed class MetaConfigurationSectionGroup : ConfigurationSectionGroup
    {

        ///// <summary>
        ///// Private const
        ///// </summary>
        //private const string _InstanceItem = "instance";

        /// <summary>
        /// 构造函数
        /// </summary>
        public MetaConfigurationSectionGroup()
            : base()
        {
#if DELUXEWORKSTEST
            Trace.WriteLine("'Lib.Core.metaConfig' configuration section group  constructor ...");
#endif
        }

        /// <summary>
        /// 源配置映射节
        /// </summary>
        [ConfigurationProperty(MetaConfigurationSourceInstanceSection.Name)]
        public MetaConfigurationSourceInstanceSection SourceConfigurationMapping
        {
            get
            {
                return base.Sections[MetaConfigurationSourceInstanceSection.Name]
                    as MetaConfigurationSourceInstanceSection;
            }
        }

    } // class end
}
