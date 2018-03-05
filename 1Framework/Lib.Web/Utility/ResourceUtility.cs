using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using Lib.Core;

namespace Lib.Web
{
    public class ResourceUtility
    {
        #region From WebApp

        public static string GetFileString(string filePath)
        {
            Stream stream = GetFileStream(filePath);
            StreamReader sr = new StreamReader(stream);
            try
            {
                return sr.ReadToEnd();
            }
            finally
            {
                sr.Close();
            }
        }

        public static Stream GetFileStream(string filePath)
        {
            Stream stream = new FileStream(filePath, FileMode.Open);
            return stream;
        }

        #endregion

        #region From Assembly

        /// <summary>
        /// 从资源中读取以内嵌文件方式保存的字符串
        /// </summary>
        /// <param name="strPath">文件的路径</param>
        /// <param name="assembly">Assembly实例</param>
        /// <returns>结果字符串</returns>
        public static string GetFileResourceString(string strPath, Assembly assembly)
        {
            Stream stream = GetFileResourceStream(strPath, assembly);

            StreamReader sr = new StreamReader(stream);
            try
            {
                return sr.ReadToEnd();
            }
            finally
            {
                sr.Close();
            }
        }

        /// <summary>
        /// 从资源中读取以内嵌文件方式保存的字符串
        /// </summary>
        /// <param name="strPath">文件的路径</param>
        /// <param name="encoding">字符的编码方式</param>
        /// <param name="assembly">Assembly实例</param>
        /// <returns>结果字符串</returns>
        public static string GetFileResourceString(string strPath, Encoding encoding, Assembly assembly)
        {
            Stream stream = GetFileResourceStream(strPath, assembly);

            StreamReader sr = new StreamReader(stream, encoding);
            try
            {
                return sr.ReadToEnd();
            }
            finally
            {
                sr.Close();
            }
        }

        /// <summary>
        /// 从Assembly的资源中得到数据流
        /// </summary>
        /// <param name="strPath">资源的路径，例如：Goo.Data.CustomerInfo</param>
        /// <param name="assembly">Assembly实例</param>
        /// <returns>数据流</returns>
        public static Stream GetFileResourceStream(string strPath, Assembly assembly)
        {
            Stream stream = assembly.GetManifestResourceStream(strPath);

            return stream;
        }

        #endregion

        #region Resource

        public static string GetResourceString(Assembly assembly,CultureInfo culture, string baseName, string key)
        {
            ResourceManager manager = new ResourceManager(baseName, assembly);

            return manager.GetString(key, culture);
        }

        public static string GetResourceString(CultureInfo culture, string baseName, string key)
        {
            return GetResourceString(Assembly.GetExecutingAssembly(), culture, baseName, key);
        }

        public static string GetResourceString(string baseName,string key)
        {
            return GetResourceString(Assembly.GetExecutingAssembly(), Thread.CurrentThread.CurrentUICulture, baseName, key);
        }

        public static string GetResourceString(Type type, CultureInfo culture, string key)
        {
            return GetResourceString(type.Assembly, culture, type.FullName, key);
        }

        public static string GetResourceString(Type type, string key)
        {
            return GetResourceString(type, Thread.CurrentThread.CurrentUICulture, key);
        }

        public static string GetResourceString<T>(CultureInfo culture, string key)
        {
            return GetResourceString(typeof(T), culture, key);
        }

        public static string GetResourceString<T>(string key)
        {
            return GetResourceString<T>(Thread.CurrentThread.CurrentUICulture, key);
        }

        public static string GetResourceString(string assemblyName, CultureInfo culture, string baseName, string key)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", assemblyName + ".dll");

            ExceptionHelper.FalseThrow(File.Exists(path), "Can not find the {0} assembly file", assemblyName);

            Assembly assembly = Assembly.LoadFrom(path);

            return GetResourceString(assembly, culture, baseName, key);
        }

        public static string GetResourceString(string assemblyName, string baseName, string key)
        {
            return GetResourceString(assemblyName, Thread.CurrentThread.CurrentUICulture, baseName, key);
        }

        #endregion Resource
    }
}
