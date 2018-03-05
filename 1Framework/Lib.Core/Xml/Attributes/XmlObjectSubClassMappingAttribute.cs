using System;

namespace Lib.Xml
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class XmlObjectSubClassMappingAttribute : XmlObjectMappingAttribute
    {
        private string subPropertyName = string.Empty;

        /// <summary>
        /// 构造方法
        /// </summary>
        protected XmlObjectSubClassMappingAttribute()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subPropertyName"></param>
        /// <param name="nodeName"></param>
        public XmlObjectSubClassMappingAttribute(string subPropertyName, string nodeName)
            : base(nodeName)
        {
            this.subPropertyName = subPropertyName;
        }

        /// <summary>
        /// 源对象的属性名称
        /// </summary>
        public string SubPropertyName
        {
            get
            {
                return this.subPropertyName;
            }
            set
            {
                this.subPropertyName = value;
            }
        }
    }
}
