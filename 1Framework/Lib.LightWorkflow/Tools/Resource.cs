using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Resources;
using System.Reflection;

namespace Wanda.Lib.LightWorkflow.Tools
{
	/// <summary>
	/// 资源处理的辅助类。帮助应用从资源内读取数据
	/// </summary>
	/// <remarks>
	/// 资源处理的辅助类。帮助应用从资源内读取字符串、XML、二进制流等。
	/// </remarks>
	public class Resource
	{
		/// <summary>
		/// 从strID指定的资源中获得到字符串
		/// </summary>
		/// <param name="strID">资源ID</param>
		/// <param name="strResourceName">资源名称</param>
		/// <param name="assembly">资源所在的Assembly</param>
		/// <param name="strParams">用于格式化 取出值 的参数</param>
		/// <returns>资源中存储的字符串在经过特定格式化以后的文本串</returns>
		/// <example>
		/// <code>
		/// string strRes = LoadStringFromStringTable("welcomrMessage", "Goo.Resource", Assembly.GetExecutingAssembly);
		/// </code>
		/// </example>
		public static string LoadStringFromStringTable(string strID, string strResourceName, Assembly assembly, params string[] strParams)
		{
			ResourceManager rm = new ResourceManager(strResourceName, assembly);

			string strText = rm.GetString(strID);

			if (strParams.Length > 0)
				strText = string.Format(strText, strParams);

			return strText;
		}

		/// <summary>
		/// 从Assembly的资源中得到数据流
		/// </summary>
		/// <param name="strPath">资源的路径，例如：Goo.Data.CustomerInfo</param>
		/// <param name="assembly">Assembly实例</param>
		/// <returns>数据流</returns>
		public static Stream GetResStream(string strPath, Assembly assembly)
		{
			Stream stream = assembly.GetManifestResourceStream(strPath);

            if (stream == null)
            {
                throw new Exception("不能在Assembly(" + assembly.FullName + ")找到资源" + strPath);
            }
			return stream;
		}

		/// <summary>
		/// 从资源中读取以内嵌文件方式保存的字符串
		/// </summary>
		/// <param name="strPath">文件的路径</param>
		/// <param name="assembly">Assembly实例</param>
		/// <returns>结果字符串</returns>
		public static string GetResString(string strPath, Assembly assembly)
		{
			Stream stream = GetResStream(strPath, assembly);

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
		public static string GetResString(string strPath, Encoding encoding, Assembly assembly)
		{
			Stream stream = GetResStream(strPath, assembly);

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
		/// 从资源中读取以内嵌文件方式保存的XML
		/// </summary>
		/// <param name="strPath">文件的路径</param>
		/// <param name="assembly">Assembly实例</param>
		/// <returns>Xml文档对象</returns>
		public static XmlDocument GetResXml(string strPath, Assembly assembly)
		{
			Stream stream = GetResStream(strPath, assembly);

			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(stream);

			return xmlDoc;
		}
	}
}

