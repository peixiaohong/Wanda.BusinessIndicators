using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib.Core;

namespace Lib.Xml
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class XmlCollectionMappingAttribute : XmlObjectMappingAttribute
    {
        private System.Type childrenType = null;
        private string childrenTypeDescription = null;

        /// <summary>
        /// 构造方法，通过类型信息构造
        /// </summary>
        /// <param name="childrenType">类型信息</param>
        public XmlCollectionMappingAttribute(System.Type childrenType)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(childrenType != null, "type");

            this.childrenType = childrenType;
        }

        /// <summary>
        /// 构造方法，通过类型描述构造
        /// </summary>
        /// <param name="childrenTypeDesp">类型描述</param>
        public XmlCollectionMappingAttribute(string childrenTypeDesp)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(childrenTypeDesp, "typeDesp");
            this.childrenTypeDescription = childrenTypeDesp;
        }

        /// <summary>
        /// 类型信息
        /// </summary>
        public System.Type Type
        {
            get
            {
                if (this.childrenType == null && string.IsNullOrEmpty(this.childrenTypeDescription) == false)
                    this.childrenType = TypeCreator.GetTypeInfo(this.childrenTypeDescription);

                return this.childrenType;
            }
        }

        /// <summary>
        /// 类型描述
        /// </summary>
        public string TypeDescription
        {
            get
            {
                if (string.IsNullOrEmpty(this.childrenTypeDescription) == true && this.childrenType != null)
                    this.childrenTypeDescription = this.childrenType.FullName + ", " + this.childrenType.Assembly.FullName;

                return this.childrenTypeDescription;
            }
        }

        /// <summary>
        /// 输出类型描述
        /// </summary>
        /// <returns>类型描述</returns>
        public override string ToString()
        {
            return TypeDescription;
        }
    }
}
