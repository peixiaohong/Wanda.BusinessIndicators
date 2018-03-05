using System.Configuration;
using Lib.Config;

namespace Lib.Web.Security
{
    public class AuthenticationServerSection : ConfigurationSection
    {
        /// <summary>
        /// 单点登录客户端应用配置信息
        /// </summary>
        /// <returns>AuthenticationSection对象</returns>
        public static AuthenticationServerSection GetConfig()
        {
            AuthenticationServerSection result =
                (AuthenticationServerSection)ConfigurationBroker.GetSection("authenticationServer");

            ConfigurationExceptionHelper.CheckSectionNotNull(result, "authenticationServer");

            return result;
        }

        private AuthenticationServerSection()
        { }

        /// <summary>
        /// 认证方式
        /// </summary>
        [ConfigurationProperty("mode", DefaultValue = "lib")]
        public string Mode
        {
            get
            {
                return (string)this["mode"];
            }
        }

        /// <summary>
        /// CookieName
        /// </summary>
        [ConfigurationProperty("name", DefaultValue = "lib.auth")]
        public string Name
        {
            get
            {
                return (string)this["name"];
            }
        }

        /// <summary>
        /// CookiePath
        /// </summary>
        [ConfigurationProperty("path", DefaultValue = "/")]
        public string Path
        {
            get
            {
                return (string)this["path"];
            }
        }

        /// <summary>
        /// CookieTimeout
        /// </summary>
        [ConfigurationProperty("timeout", DefaultValue = "-2")]
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
    }
}
