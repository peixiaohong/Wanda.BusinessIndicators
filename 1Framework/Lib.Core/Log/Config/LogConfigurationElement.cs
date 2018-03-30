using System;
using Lib.Config;
using System.Collections.Generic;
using System.Threading;


namespace Lib.Log
{
    /// <summary>
    /// 日志配置节的基类
    /// </summary>
    /// <remarks>
    /// TypeConfigurationElement的派生类，自定义对日志配置信息的处理
    /// 日志配置节的公共基类，在该类中扩展了一个公有属性ExtendedAttributes，她是一个IDictionary对象，可以在其中存放派生类的各种初始化数据
    /// </remarks>
    public class LogConfigurationElement : TypeConfigurationElement
    {
        internal static IDictionary<string, IDictionary<string, string>> extendedAttributes = new Dictionary<string, IDictionary<string, string>>();
        private ReaderWriterLock dictionaryRWLock = new ReaderWriterLock();
        private IDictionary<string, string> instanceDic = new Dictionary<string, string>();
        private string fullPath = string.Empty;

        /// <summary>
        /// 缺省构造函数
        /// </summary>
        public LogConfigurationElement()
        {
        }

        internal LogConfigurationElement(string fullPath)
        {
            this.fullPath = fullPath;
        }

        /// <summary>
        /// 重载配置节的解析方法
        /// </summary>
        /// <param name="reader">在配置文件中进行读取操作的 <seealso cref="System.Xml.XmlReader"/>。</param>
        /// <param name="serializeCollectionKey">为 true，则只序列化集合的键属性；否则为 false。</param>
        protected override void DeserializeElement(System.Xml.XmlReader reader, bool serializeCollectionKey)
        {
            base.DeserializeElement(reader, serializeCollectionKey);

            string instanceKey = this.fullPath + "~" + this.Name;

            dictionaryRWLock.AcquireWriterLock(1000);
            try
            {
                if (extendedAttributes.Keys.Contains(instanceKey) == false)
                    extendedAttributes.Add(instanceKey, this.instanceDic);
                else
                {
                    throw new LogException(string.Format("配置节点解析失败，请检查是否有name属性同为：\"{0}\"的配置节", this.Name));
                }
            }
            finally
            {
                dictionaryRWLock.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// 扩展属性
        /// </summary>
        /// <remarks>
        /// 字典，用于存放自定义配置节的扩展属性
        /// </remarks>
        public IDictionary<string, string> ExtendedAttributes
        {
            get
            {
                IDictionary<string, string> result;
                string instanceKey = this.fullPath + "~" + this.Name;

                dictionaryRWLock.AcquireReaderLock(1000);

                try
                {
                    if (extendedAttributes.TryGetValue(instanceKey, out result) == false)
                        result = new Dictionary<string, string>();

                    return result;
                }
                finally
                {
                    dictionaryRWLock.ReleaseReaderLock();
                }
            }
        }

        /// <summary>
        /// 处理扩展属性
        /// </summary>
        /// <param name="name">属性的名称。</param>
        /// <param name="value">属性的值。</param>
        /// <returns>如果反序列化过程中遇到未知属性，则为 true。</returns>
        protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
        {
            if (this.Properties.Contains(name))
                return base.OnDeserializeUnrecognizedAttribute(name, value);
            else
            {
                this.instanceDic.Add(name, value);

                return true;
            }
        }

    }
}
