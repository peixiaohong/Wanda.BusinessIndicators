using System;
using System.Collections.Generic;
using System.Web;
using Lib.Core;
using Lib.Core.Properties;

namespace Lib.Config
{
    /// <summary>
    /// 寻找目录层次中最匹配一项的算法
    /// </summary>
    internal sealed class BestDirectoryMatchStrategy : BestPathMatchStrategyBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="instances">MetaConfigurationSourceInstanceElementCollection对象（集合）</param>
        public BestDirectoryMatchStrategy(MetaConfigurationSourceInstanceElementCollection instances)
        {
            if (instances == null) throw new NullReferenceException("instances");

            if (EnvironmentHelper.Mode == InstanceMode.Web)
                this.path = HttpContext.Current.Request.Url.AbsoluteUri;
            else
                this.path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            this.path = FormatPath(path);
            this.candidates = FileterPath(instances, true);
        }


        /// <summary>
        /// 从路径列表里选择与目标路径最贴近的一项。
        /// </summary>
        /// <param name="items">路径列表</param>
        /// <returns>最贴近项目</returns>
        public override string Calculate(IList<KeyValuePair<string, string>> items)
        {
            if ((items == null) || (items.Count <= 0) || string.IsNullOrEmpty(path)) 
                return string.Empty;

            int maxMatchLength = -1;
            string metaConfig = string.Empty;

            if (EnvironmentHelper.Mode == InstanceMode.Web)
            {
                Uri pathAbsolute = new Uri(this.path, UriKind.Absolute);
                this.path = FormatPath(pathAbsolute.GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped));
            }

            foreach (KeyValuePair<string, string> item in items)
            {
                // 部分路径匹配
                if (path.StartsWith(item.Key))
                {
                    if (item.Key.Length > maxMatchLength)
                    {
                        maxMatchLength = item.Key.Length;
                        metaConfig = item.Value;
                    }
                    else
                    {
                        if ((item.Key.Length == maxMatchLength) && !string.Equals(metaConfig, item.Value))
                        {
                            string message = string.Format(Resource.ExceptionConflictPathDefinition,
                                item.Key, metaConfig, item.Value);
                            throw new Exception(message);
                        }
                    }
                }
            }

            return metaConfig;
        }
    }
}
