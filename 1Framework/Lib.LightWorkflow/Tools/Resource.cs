using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Resources;
using System.Reflection;

namespace Wanda.Lib.LightWorkflow.Tools
{
	/// <summary>
	/// ��Դ����ĸ����ࡣ����Ӧ�ô���Դ�ڶ�ȡ����
	/// </summary>
	/// <remarks>
	/// ��Դ����ĸ����ࡣ����Ӧ�ô���Դ�ڶ�ȡ�ַ�����XML�����������ȡ�
	/// </remarks>
	public class Resource
	{
		/// <summary>
		/// ��strIDָ������Դ�л�õ��ַ���
		/// </summary>
		/// <param name="strID">��ԴID</param>
		/// <param name="strResourceName">��Դ����</param>
		/// <param name="assembly">��Դ���ڵ�Assembly</param>
		/// <param name="strParams">���ڸ�ʽ�� ȡ��ֵ �Ĳ���</param>
		/// <returns>��Դ�д洢���ַ����ھ����ض���ʽ���Ժ���ı���</returns>
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
		/// ��Assembly����Դ�еõ�������
		/// </summary>
		/// <param name="strPath">��Դ��·�������磺Goo.Data.CustomerInfo</param>
		/// <param name="assembly">Assemblyʵ��</param>
		/// <returns>������</returns>
		public static Stream GetResStream(string strPath, Assembly assembly)
		{
			Stream stream = assembly.GetManifestResourceStream(strPath);

            if (stream == null)
            {
                throw new Exception("������Assembly(" + assembly.FullName + ")�ҵ���Դ" + strPath);
            }
			return stream;
		}

		/// <summary>
		/// ����Դ�ж�ȡ����Ƕ�ļ���ʽ������ַ���
		/// </summary>
		/// <param name="strPath">�ļ���·��</param>
		/// <param name="assembly">Assemblyʵ��</param>
		/// <returns>����ַ���</returns>
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
		/// ����Դ�ж�ȡ����Ƕ�ļ���ʽ������ַ���
		/// </summary>
		/// <param name="strPath">�ļ���·��</param>
		/// <param name="encoding">�ַ��ı��뷽ʽ</param>
		/// <param name="assembly">Assemblyʵ��</param>
		/// <returns>����ַ���</returns>
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
		/// ����Դ�ж�ȡ����Ƕ�ļ���ʽ�����XML
		/// </summary>
		/// <param name="strPath">�ļ���·��</param>
		/// <param name="assembly">Assemblyʵ��</param>
		/// <returns>Xml�ĵ�����</returns>
		public static XmlDocument GetResXml(string strPath, Assembly assembly)
		{
			Stream stream = GetResStream(strPath, assembly);

			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(stream);

			return xmlDoc;
		}
	}
}

