using System;

namespace Lib.Xml
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class XmlObjectMappingAttribute : System.Attribute
    {
        private string nodeName = string.Empty;
        private XmlNodeMappingType nodeMappingType = XmlNodeMappingType.Attribute;

        /// <summary>
        /// 默认构造方法，默认值nodeName为Node，映射类型为属性
        /// </summary>
        public XmlObjectMappingAttribute()
        { 
        
        }

        /// <summary>
        /// 以提供的nodeName作为Xml的节点名
        /// </summary>
        /// <param name="nodeName">节点名</param>
        public XmlObjectMappingAttribute(string nodeName)
        {
            this.nodeName = nodeName;
        }

        /// <summary>
        /// 自定义节点名与映射方式
        /// </summary>
        /// <param name="nodeName">节点名</param>
        /// <param name="mappingType">映射方式</param>
        public XmlObjectMappingAttribute(string nodeName, XmlNodeMappingType mappingType)
        {
            this.nodeName = nodeName;
            this.nodeMappingType = mappingType;
        }

        /// <summary>
        /// 
        /// </summary>
        public string NodeName
        {
            get { return this.nodeName; }
            set { this.nodeName = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public XmlNodeMappingType MappingType
        {
            get { return nodeMappingType; }
            set { nodeMappingType = value; }
        }
    }
}
