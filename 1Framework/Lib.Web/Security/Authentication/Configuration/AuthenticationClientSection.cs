using System;
using System.Collections.Generic;
using System.Configuration;
using Lib.Config;

namespace Lib.Web.Security
{
    public class AuthenticationClientSection : ConfigurationSection
    {
        /// <summary>
        /// 单点登录客户端应用配置信息
        /// </summary>
        /// <returns>AuthenticationSection对象</returns>
        public static AuthenticationClientSection GetConfig()
        {
            AuthenticationClientSection result =
                (AuthenticationClientSection)ConfigurationBroker.GetSection("authenticationClient");

            ConfigurationExceptionHelper.CheckSectionNotNull(result, "authenticationClient");

            return result;
        }

        private AuthenticationClientSection()
        {
        }

        /// <summary>
        /// 应用的ID号
        /// </summary>
        [ConfigurationProperty("application", IsRequired = true)]
        public string Application
        {
            get
            {
                return (string)this["application"];
            }
        }

        /// <summary>
        /// 应用的绝对过期时间
        /// </summary>
        [ConfigurationProperty("timeout", DefaultValue = -2)]
        public int Timeout
        {
            get
            {
                return (int)this["timeout"];
            }
        }

        [ConfigurationProperty("slidingExpiration", DefaultValue = 0)]
        public int SlidingExpiration
        {
            get
            {
                return (int)this["slidingExpiration"];
            }
        }

        /// <summary>
        /// 是否滑动过期
        /// </summary>
        public bool HasSlidingExpiration
        {
            get
            {
                return SlidingExpiration > 0;
            }
        }

        /// <summary>
        /// Rsa key
        /// </summary>
        public string RsaKeyValue
        {
            get
            {
                return RsaKeyValueElement.Value;
            }
        }

        /// <summary>
        /// 注销回调地址
        /// </summary>
        public Uri LogoutCallbackUrl
        {
            get
            {
                return Paths["logoutCallbackUrl"].Uri;
            }
        }
        /// <summary>
        /// 认证地址
        /// </summary>
        public Uri LoginUrl
        {
            get
            {
                return Paths["loginUrl"].Uri;
            }
        }
        /// <summary>
        /// 注销地址
        /// </summary>
        public Uri LogoutUrl
        {
            get
            {
                return Paths["logoutUrl"].Uri;
            }
        }

        [ConfigurationProperty("typeFactories", IsRequired = false)]
        private TypeConfigurationCollection TypeFactories
        {
            get
            {
                return (TypeConfigurationCollection)this["typeFactories"];
            }
        }

        [ConfigurationProperty("rsaKeyValue", IsRequired = true)]
        private ClientRsaKeyValueConfigurationElement RsaKeyValueElement
        {
            get
            {
                return (ClientRsaKeyValueConfigurationElement)this["rsaKeyValue"];
            }
        }

        [ConfigurationProperty("paths", IsRequired = true)]
        private UriConfigurationCollection Paths
        {
            get
            {
                return (UriConfigurationCollection)this["paths"];
            }
        }
    }

    /// <summary>
    /// 应用Rsa配置
    /// </summary>
    public class ClientRsaKeyValueConfigurationElement : ConfigurationElement
    {
        private static string sValue = string.Empty;

        /// <summary>
        /// 配置的string 值
        /// </summary>
        public string Value
        {
            get
            {
                return ClientRsaKeyValueConfigurationElement.sValue;
            }
        }
        /// <summary>
        /// 读入配置信息
        /// </summary>
        /// <param name="reader">XmlReader</param>
        /// <param name="serializeCollectionKey"></param>
        protected override void DeserializeElement(System.Xml.XmlReader reader, bool serializeCollectionKey)
        {
            lock (typeof(ClientRsaKeyValueConfigurationElement))
            {
                if (ClientRsaKeyValueConfigurationElement.sValue == string.Empty)
                    ClientRsaKeyValueConfigurationElement.sValue = reader.ReadOuterXml();
                else
                    reader.ReadOuterXml();
            }
        }
    }
}
