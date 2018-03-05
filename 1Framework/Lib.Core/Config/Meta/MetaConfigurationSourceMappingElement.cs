using System;
using System.Configuration;
using Lib.Core;

namespace Lib.Config
{
    /// <summary>
    /// Entity of applications in sourceMappings config 
    /// </summary>
    class MetaConfigurationSourceMappingElement : ConfigurationElement
    {
        /// <summary>
        /// Private const and Field
        /// </summary>
        private const string AppItem = "app";


        /// <summary>
        /// Logical name of mapping configuration file.
        /// 配置的应用系统的路径，可以是绝对路径，也可以是相对路径。
        /// 绝对路径需要指定正确的主机名、IP地址、段口号、用户信息等各个字段，匹配要求更严格。
        /// 建议使用相对路径，配置要求更宽松
        /// 如果是配置对所有应用都有效，可以配置app="/"，不过匹配规则是找到一个匹配的全局文件就不再进行后面的匹配，所以，这用配置要慎用。
        /// </summary>
        [ConfigurationProperty(MetaConfigurationSourceMappingElement.AppItem, IsRequired = true)]
        public string Application
        {
            get
            {
                return base[MetaConfigurationSourceMappingElement.AppItem] as string;
            }
        }

        /// <summary>
        /// 对于web访问使用UriMatch();对于winForm访问使用FileMatch()
        /// </summary>
        /// <param name="appPath">外层传来的访问地址的全路径</param>
        /// <returns>bool</returns>
        public bool IsMatched(string appPath)
        {
            bool isMatched = false;

            if (EnvironmentHelper.Mode == InstanceMode.Web)
            {
                isMatched = MetaConfigurationSourceMappingElement.UriMatch(appPath, Application);
            }
            else
            {
                isMatched = MetaConfigurationSourceMappingElement.FileMatch(appPath, Application);
            }
            return isMatched;
        }

        /// <summary>
        /// 如果配置项app是相对路径，则在相对路径前补上当前应用域的路径，使其成为绝对路径
        /// 最终使用绝对路径进行匹配
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="configPath"></param>
        /// <returns></returns>
        private static bool FileMatch(string filePath, string configPath)
        {
            if (System.IO.Path.IsPathRooted(configPath) == false)
                configPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase.TrimEnd('\\') + "\\" + configPath;

            return filePath.IndexOf(configPath, StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        /// 如果配置项app是绝对路径，则要对访问协议、端口号、用户信息、主机名、主机名类型进行逐一匹配
        /// 最终使用相对路径进行匹配。
        /// </summary>
        /// <param name="url"></param>
        /// <param name="configPath"></param>
        /// <returns></returns>
        private static bool UriMatch(string url, string configPath)
        {
            bool result = true;
            Uri uri = new Uri(url, UriKind.RelativeOrAbsolute);
            Uri configUri = new Uri(configPath, UriKind.RelativeOrAbsolute);
            string relativeConfigPath = string.Empty;

            if (configUri.IsAbsoluteUri)
            {
                result = uri.Scheme == configUri.Scheme &&
                            uri.Port == configUri.Port &&
                            uri.UserInfo == configUri.UserInfo &&
                            uri.Host == configUri.Host &&
                            uri.HostNameType == configUri.HostNameType;
                if (result)
                    relativeConfigPath = configUri.GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped);
            }
            else
                relativeConfigPath = configUri.ToString();

            if (result)
            {
                string srcPathAndQuery = uri.GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped);
                result = srcPathAndQuery.IndexOf(relativeConfigPath, StringComparison.OrdinalIgnoreCase) == 0;
            }

            return result;
        }
    }
}
