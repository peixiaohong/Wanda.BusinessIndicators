using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace Lib.Web.Json
{
    /// <summary>
    /// 标记控件某属性序列化到客户端时需要特定的JavaScriptConverter类别
    /// </summary>
    /// <remarks>标记控件某属性序列化到客户端时需要特定的JavaScriptConverter类别</remarks>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class JavaScriptConverterAttribute : Attribute
    {
        private Type _ConverterType = null;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="converterType">序列化时所需特定的JavaScriptConverter类别</param>
        public JavaScriptConverterAttribute(Type converterType)
        {
            if (!typeof(JavaScriptConverter).IsAssignableFrom(converterType))
                throw new ArgumentException();//(string.Format(Resources.DeluxeWebResource.E_NotJavaScriptConverter, new object[] { converterType.Name }));
            _ConverterType = converterType;
        }

        /// <summary>
        /// 获取序列化时所需特定的JavaScriptConverter类别
        /// </summary>
        /// <remarks>获取序列化时所需特定的JavaScriptConverter类别</remarks>
        public Type ConverterType
        {
            get { return _ConverterType; }
        }
    }
}
