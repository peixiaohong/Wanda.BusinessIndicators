using System;
using System.Configuration;
using System.Transactions;

namespace Lib.Data
{
    /// <summary>
    /// 配置有关TransactionScope的参数
    /// 执行中该套参数仅作为默认配置，应用可以根据需要通过构造参数或属性修改相关参数。
    /// </summary>
    class TransactionConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("timeout", IsRequired = false, DefaultValue = "00:10:00")]
        public virtual TimeSpan TimeOut
        {
            get
            {
                string val = Convert.ToString(this["timeout"]);
                return TimeSpan.Parse(val);
            }
        }

        [ConfigurationProperty("isolationLevel", IsRequired = false, DefaultValue = "ReadCommitted")]
        public virtual IsolationLevel IsolationLevel
        {
            get
            {
                string val = Convert.ToString(this["isolationLevel"]);
                return (IsolationLevel)Enum.Parse(typeof(IsolationLevel), val);
            }
        }

    }
}
