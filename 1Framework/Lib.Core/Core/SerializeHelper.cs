using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;

namespace Lib.Core
{

    /// <summary>
    /// xml、binary序列化帮手
    /// </summary>
    public static class SerializationHelper
    {
        /// <summary>
        /// Xml 序列化
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="xmlFile">保存文件</param>
        public static void XmlSerialize(object obj, string xmlFile)
        {
            Type type = obj.GetType();
            CheckPath(xmlFile);
            XmlSerializer serializer = new XmlSerializer(type);
            using (FileStream file = new FileStream(xmlFile, FileMode.OpenOrCreate, FileAccess.Write))
            {
                serializer.Serialize(file, obj);
            }
        }

        /// <summary>
        /// 序列化为XML字符串
        /// </summary>
        /// <param name="obj">待序列化对象</param>
        /// <returns></returns>
        public static string XmlSerialize(object obj)
        {
            Type type = obj.GetType();
            XmlSerializer serializer = new XmlSerializer(type);

            // 指定命名空间为空
            XmlSerializerNamespaces xsns = new XmlSerializerNamespaces();
            xsns.Add(string.Empty, string.Empty);
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Serialize(stream, obj, xsns);

                byte[] buffer = stream.ToArray();

                // 使用UTF-8编码
                string result = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                return result;
            }
        }

        /// <summary>
        /// Xml字序串反序列化为对象
        /// </summary>
        /// <typeparam name="T">返回对象类型</typeparam>
        /// <param name="xml">xml数据</param>
        /// <returns>反序列化后的对象</returns>
        public static T XmlDeserialize<T>(string xml)
        {
            XmlSerializer xmls = new XmlSerializer(typeof(T));
            MemoryStream memstream = new MemoryStream(new UTF8Encoding().GetBytes(xml));
            return (T)xmls.Deserialize(memstream);
        }

        /// <summary>
        /// Xml字序串反序列化为对象
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <param name="xmlFile">对象文件</param>
        /// <returns>反序列化后的对象</returns>
        public static object XmlDeserialize(Type type, string xmlFile)
        {
            XmlSerializer serializer = new XmlSerializer(type);
            CheckPath(xmlFile);
            using (Stream file = new FileStream(xmlFile, FileMode.Open, FileAccess.Read))
            {
                return serializer.Deserialize(file);
            }
        }




        /// <summary>
        /// 检查路径是否存在
        /// </summary>
        /// <param name="fullPath"></param>
        public static void CheckPath(string fullPath)
        {
            string path = Path.GetDirectoryName(fullPath);
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// 二进制序列化
        /// </summary>
        /// <param name="obj">待序列化的对象</param>
        /// <param name="file">序列化后的文件路径</param>
        public static void BinSerialize(object obj, string file)
        {
            if (obj == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(file))
            {
                throw new ArgumentNullException("尚未指定文件名参数！");
            }
            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(stream, obj);
                stream.Close();
            }
        }

        /// <summary>
        /// 序列化返回字节数组
        /// </summary>
        /// <param name="obj">待序列化的对象</param>
        /// <returns></returns>
        public static byte[] BinSerialize(object obj)
        {
            if (obj == null)
            {
                return null;
            }
            IFormatter bformatter = new BinaryFormatter();
            MemoryStream memory = new MemoryStream();
            bformatter.Serialize(memory, obj);
            byte[] result = memory.GetBuffer();
            return result;
        }


        /// <summary>
        /// 二进制反序列化
        /// </summary>
        /// <param name="file">保存对象的文件</param>
        /// <returns></returns>
        public static object BinDeserialize(string file)
        {
            object result = null;

            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                result = formatter.Deserialize(stream);
                stream.Close();
            }
            return result;
        }

        /// <summary>
        /// 根据字节数组反序列化
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static object BinDeserialize(byte[] bytes)
        {
            if (bytes == null)
            {
                return null;
            }
            MemoryStream stream = new MemoryStream(bytes);
            return BinDeserialize(stream);
        }

        /// <summary>
        /// 根据STREAM反序列化
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private static object BinDeserialize(Stream stream)
        {
            if (stream == null)
            {
                return null;
            }
            IFormatter bformatter = new BinaryFormatter();
            return bformatter.Deserialize(stream);
        }
    }
}
