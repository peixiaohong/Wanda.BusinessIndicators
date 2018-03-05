using System;
using System.Configuration;
namespace Lib.Data
{
    /// <summary>
    /// 自定义数据库执行过程事件参数的配置项
    /// </summary>
    class DbEventArgsConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("type", IsRequired = true)]
        public virtual string Type
        {
            get
            {
                return (string)this["type"];
            }
        }
    }
}
