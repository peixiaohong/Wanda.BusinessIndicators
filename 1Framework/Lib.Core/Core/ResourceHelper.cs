using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Xml;

namespace Lib.Core
{
    /// <summary>
    /// 资源访问的Helper
    /// </summary>
    public static class ResourceHelper
    {
        /// <summary>
        /// 从资源中加载字符串
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string LoadStringFromResource(Assembly assembly, string path)
        {
            using (Stream stm = GetResourceStream(assembly, path))
            {
                using (StreamReader sr = new StreamReader(stm))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// 从资源中加载xml文档对象
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static XmlDocument LoadXmlFromResource(Assembly assembly, string path)
        {
            using (Stream stm = GetResourceStream(assembly, path))
            {
                XmlDocument xmlDoc = new XmlDocument();

                xmlDoc.Load(stm);

                return xmlDoc;
            }
        }

        /// <summary>
        /// 从资源中得到流
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Stream GetResourceStream(Assembly assembly, string path)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(assembly != null, "assembly");
            ExceptionHelper.CheckStringIsNullOrEmpty(path, "path");

            Stream stm = assembly.GetManifestResourceStream(path);

            ExceptionHelper.FalseThrow(stm != null, "不能在Assembly:{0}中找到资源{1}", assembly.FullName, path);

            return stm;
        }
    }
}
