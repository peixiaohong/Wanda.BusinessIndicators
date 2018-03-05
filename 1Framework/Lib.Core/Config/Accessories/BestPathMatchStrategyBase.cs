
using System;
using System.Collections.Generic;
using System.IO;

using Lib.Core;

namespace Lib.Config
{
    /// <summary>
    /// 路径最贴近匹配算法实现类
    /// </summary>
    internal abstract class BestPathMatchStrategyBase : IStrategy<IList<KeyValuePair<string, string>>, string>
    {
        #region Protected field
        protected string path;
        protected IList<KeyValuePair<string, string>> candidates;
        #endregion

        /// <summary>
        /// IStrategy IList 的抽象实现
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract string Calculate(IList<KeyValuePair<string, string>> data);


        /// <summary>
        /// 待筛选的文件内容
        /// </summary>
        public virtual IList<KeyValuePair<string, string>> Candidates
        {
            get { return candidates; }
        }


        /// <summary>
        /// 判断某个路径是否为目录
        /// </summary>
		/// <param name="folderPath">路径</param>
        /// <returns></returns>
        protected static bool IsDirectory(string folderPath)
        {
			return string.IsNullOrEmpty(Path.GetFileName(folderPath));
        }

        /// <summary>
        /// 依照路径类型（文件/目录）和格式筛选路径
        /// </summary>
        /// <param name="instances">MetaConfigurationSourceInstanceElementCollection</param>
        /// <param name="isDirectory">是否目录</param>
        /// <returns>KeyValuePair 中 Value 为 meta.config 文件, key 为application 路径</returns>
        protected IList<KeyValuePair<string, string>> FileterPath(MetaConfigurationSourceInstanceElementCollection instances, bool isDirectory)
        {
            if ((instances == null) || (instances.Count <= 0)) return null;

            IList<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();
            InstanceMode mode = EnvironmentHelper.Mode;
            
			Uri pathAbsolute = null;

            foreach (MetaConfigurationSourceInstanceElement instance in instances)
            {
                if ((instance == null) || (instance.Mappings == null) 
                    || (instance.Mappings.Count <= 0) || (instance.GetMode() != mode)) 
                    continue;

                string metaConfig = FormatPath(instance.Path);
 
                foreach (MetaConfigurationSourceMappingElement mapping in instance.Mappings)
                {
                    string applicationPath = mapping.Application;
                    if(false == (isDirectory ^ IsDirectory(applicationPath)))
                    {
                        if (mode == InstanceMode.Web)
                        {
                            //当ｗｅｂ访问时，将路径都转化为通过相对路径进行匹配 
							if (pathAbsolute == null)
								pathAbsolute = new Uri(path, UriKind.Absolute);

                            Uri appAbsolute = new Uri(applicationPath, UriKind.RelativeOrAbsolute);

                            if (appAbsolute.IsAbsoluteUri)
                            {
                                if (pathAbsolute.Scheme == appAbsolute.Scheme &&
                                    pathAbsolute.Port == appAbsolute.Port &&
                                    pathAbsolute.UserInfo == appAbsolute.UserInfo &&
                                    pathAbsolute.Host == appAbsolute.Host &&
                                    pathAbsolute.HostNameType == appAbsolute.HostNameType)
                                {
                                    applicationPath = appAbsolute.GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped);
                                }
                            }
                           
                           applicationPath = FormatPath(applicationPath);
   
                        }
                        else
                        {
                            applicationPath = FormatPath(Path.GetFullPath(applicationPath));
                        }

                        KeyValuePair<string, string> item = new KeyValuePair<string, string>(applicationPath, metaConfig);
                        result.Add(item);
                    }
                }
            }

            return result;
        }


        /// <summary>
        /// 将路径去除空格并转换为小写
        /// </summary>
		/// <param name="filePath"></param>
        /// <returns></returns>
        protected static string FormatPath(string filePath)
        {
			return filePath.ToLower().Trim();
        }
    }
}
