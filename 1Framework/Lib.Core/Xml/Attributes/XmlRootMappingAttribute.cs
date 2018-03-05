using System;

namespace Lib.Xml
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class XmlRootMappingAttribute : System.Attribute
    {
        private string rootName = string.Empty;
        private bool onlyMapMarkedProperties = true;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="rootName">根节点名</param>
        public XmlRootMappingAttribute(string rootName)
        {
            this.rootName = rootName;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="rootName">根节点名</param>
        /// <param name="onlyMapMarkedProperties">仅映射标记的属性</param>
        public XmlRootMappingAttribute(string rootName, bool onlyMapMarkedProperties)
        {
            this.rootName = rootName;
            this.onlyMapMarkedProperties = onlyMapMarkedProperties;
        }

        /// <summary>
        /// 根节点名
        /// </summary>
        public string RootName
        {
            get { return this.rootName; }
            set { this.rootName = value; }
        }

        /// <summary>
        /// 仅映射标记的属性
        /// </summary>
        public bool OnlyMapMarkedProperties
        {
            get { return onlyMapMarkedProperties; }
            set { onlyMapMarkedProperties = value; }
        }
    }
}
