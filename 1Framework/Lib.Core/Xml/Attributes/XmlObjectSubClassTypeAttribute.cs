using System;
using Lib.Core;

namespace Lib.Xml
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class XmlObjectSubClassTypeAttribute : System.Attribute
    {
        private System.Type type = null;
        private string typeDescription = null;

        /// <summary>
        /// 构造方法，通过类型信息构造
        /// </summary>
        /// <param name="type">类型信息</param>
        public XmlObjectSubClassTypeAttribute(System.Type type)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(type != null, "type");

            this.type = type;
        }

        /// <summary>
        /// 构造方法，通过类型描述构造
        /// </summary>
        /// <param name="typeDesp">类型描述</param>
        public XmlObjectSubClassTypeAttribute(string typeDesp)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(typeDesp, "typeDesp");
            this.typeDescription = typeDesp;
        }

        /// <summary>
        /// 类型信息
        /// </summary>
        public System.Type Type
        {
            get
            {
                if (this.type == null && string.IsNullOrEmpty(this.typeDescription) == false)
                    this.type = TypeCreator.GetTypeInfo(this.typeDescription);

                return this.type;
            }
        }

        /// <summary>
        /// 类型描述
        /// </summary>
        public string TypeDescription
        {
            get
            {
                if (string.IsNullOrEmpty(this.typeDescription) == true && this.type != null)
                    this.typeDescription = this.type.FullName + ", " + this.type.Assembly.FullName;

                return this.typeDescription;
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
