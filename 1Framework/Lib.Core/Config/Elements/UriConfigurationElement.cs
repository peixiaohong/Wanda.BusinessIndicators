using System;
using System.Configuration;
using Lib.Core;

namespace Lib.Config
{
    /// <summary>
    /// 关于Uri的配置项
    /// </summary>
    public class UriConfigurationElement : NamedConfigurationElement
    {
        /// <summary>
        /// Uri的地址字符串
        /// </summary>
        [ConfigurationProperty("uri")]
        private string UriString
        {
            get
            {
                return (string)this["uri"];
            }
        }

        /// <summary>
        /// 配置的Uri
        /// </summary>
        public Uri Uri
        {
            get
            {
                Uri result;
                //考虑添加缓存
                result = UriHelper.ResolveUri(UriString);

                return result;
            }
        }
    }

    /// <summary>
    /// 关于Uri的配置项集合
    /// </summary>
    public class UriConfigurationCollection : NamedConfigurationElementCollection<UriConfigurationElement>
    {
    }
}
