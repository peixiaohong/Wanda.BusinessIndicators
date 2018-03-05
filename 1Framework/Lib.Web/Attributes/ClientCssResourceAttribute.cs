using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lib.Web
{
    /// <summary>
    /// 客户端Css资源文件引用Attribute
    /// </summary>
    /// <remarks>客户端Css资源文件引用Attribute</remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments", Justification = "The composition of baseType and resourceName is available as ResourcePath")]
    public sealed class ClientCssResourceAttribute : Attribute
    {
        private string _resourcePath;
        private int _loadOrder;

        ///// <summary>
        ///// 构造函数
        ///// </summary>
        ///// <param name="baseType"></param>
        ///// <param name="resourceName"></param>
        //public ClientCssResourceAttribute(Type baseType, string resourceName)
        //{
        //    if (baseType == null) throw new ArgumentNullException("baseType");
        //    if (resourceName == null) throw new ArgumentNullException("resourceName");

        //    string typeName = baseType.FullName;
        //    int lastDot = typeName.LastIndexOf('.');
        //    if (lastDot != -1)
        //    {
        //        typeName = typeName.Substring(0, lastDot);
        //    }
        //    _resourcePath = typeName + '.' + resourceName;
        //}

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fullResourceName">css资源文件名称</param>
        /// <remarks>构造函数</remarks>
        public ClientCssResourceAttribute(string fullResourceName)
        {
            if (fullResourceName == null) throw new ArgumentNullException("fullResourceName");
            _resourcePath = fullResourceName;
        }

        /// <summary>
        /// css资源文件全名称，命名空间+文件名称
        /// </summary>
        /// <remarks>css资源文件全名称，命名空间+文件名称</remarks>
        public string ResourcePath
        {
            get { return _resourcePath; }
        }

        /// <summary>
        /// 加载顺序
        /// </summary>
        /// <remarks>加载顺序</remarks>
        public int LoadOrder
        {
            get { return _loadOrder; }
            set { _loadOrder = value; }
        }
    }
}
