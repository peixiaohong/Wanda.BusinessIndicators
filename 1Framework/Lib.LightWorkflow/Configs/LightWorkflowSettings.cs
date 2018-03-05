using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Wanda.Lib.LightWorkflow.Configs
{
    public class LightWorkflowSettings : ConfigurationSection
    {
        private static LightWorkflowSettings _Instance = ConfigurationManager.GetSection("LightWorkflowSettings") as LightWorkflowSettings;
        public static LightWorkflowSettings Instance
        {
            get
            {
                return _Instance;
            }
        }

        [ConfigurationProperty("WorkflowSettings")]
        public WorkflowSettingCollection WorkflowSettings
        {
            get { return (WorkflowSettingCollection)this["WorkflowSettings"]; }
            set { this["WorkflowSettings"] = value; }
        }

        [ConfigurationProperty("DataExchangeProviderInfos")]
        public DataExchangeProviderInfoCollection DataExchangeProviderInfos
        {
            get { return (DataExchangeProviderInfoCollection)this["DataExchangeProviderInfos"]; }
            set { this["DataExchangeProviderInfos"] = value; }
        }
    }


    /// <summary>
    /// 工作流通用配置项配置节集合
    /// </summary>
    [ConfigurationCollection(typeof(WorkflowSetting))]
    public class WorkflowSettingCollection : ConfigurationElementCollection
    {
        public new WorkflowSetting this[string name]
        {
            get
            {
                return (WorkflowSetting)BaseGet(name);
            }
        }

        public WorkflowSetting this[int index]
        {
            get
            {
                return (WorkflowSetting)BaseGet(index);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new WorkflowSetting();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((WorkflowSetting)element).Key;
        }
    }

    /// <summary>
    /// 工作流通用配置项配置节
    /// </summary>
    public class WorkflowSetting : ConfigurationElement
    {
        [ConfigurationProperty("key", IsRequired = true, IsKey = true)]
        public string Key
        {
            get { return (string)this["key"]; }
            set { this["key"] = value; }
        }

        [ConfigurationProperty("value", IsRequired = true)]
        public string Value
        {
            get { return (string)this["value"]; }
            set { this["value"] = value; }
        }

        [ConfigurationProperty("NeedSendRTXMessage", IsRequired = false)]
        public string NeedSendRTXMessage
        {
            get { return (string)this["NeedSendRTXMessage"]; }
            set { this["NeedSendRTXMessage"] = value; }
        }

        [ConfigurationProperty("RTXReceiver", IsRequired = false)]
        public string RTXReceiver
        {
            get { return (string)this["RTXReceiver"]; }
            set { this["RTXReceiver"] = value; }
        }
    }


    /// <summary>
    /// 数据交换配置节集合
    /// </summary>
    [ConfigurationCollection(typeof(DataExchangeProviderInfo))]
    public class DataExchangeProviderInfoCollection : ConfigurationElementCollection
    {
        public new DataExchangeProviderInfo this[string name]
        {
            get
            {
                return (DataExchangeProviderInfo)BaseGet(name);
            }
        }

        public DataExchangeProviderInfo this[int index]
        {
            get
            {
                return (DataExchangeProviderInfo)BaseGet(index);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new DataExchangeProviderInfo();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DataExchangeProviderInfo)element).Key;
        }
    }

    /// <summary>
    /// 数据交换配置节
    /// </summary>
    public class DataExchangeProviderInfo : WorkflowSetting
    {
    }
}
