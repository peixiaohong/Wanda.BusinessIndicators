using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lib.Web
{
    /// <summary>
    /// 关联控件与脚本资源，控件通过此属性可获取客户端控件的类型和脚本资源。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments", Justification = "The composition of baseType, resourceName, and fullResourceName is available as ResourcePath")]
    public sealed class ClientScriptResourceAttribute : Attribute
    {
        private string _resourcePath;
        private string _componentType;
        private int _loadOrder;

        /// <summary>
        /// 客户端对象类型
        /// </summary>        
        public string ComponentType
        {
            get { return _componentType; }
            set { _componentType = value; }
        }

        /// <summary>
        /// 加载顺序
        /// </summary>
        public int LoadOrder
        {
            get { return _loadOrder; }
            set { _loadOrder = value; }
        }

        /// <summary>
        /// 资源文件全名称，命名空间+文件名称
        /// This is the path to the resource in the assembly.  This is usually defined as
        /// [default namespace].[Folder name].FileName.  In a project called "ControlLibrary1", a
        /// JScript file called Foo.js in the "Script" subdirectory would be named "ControlLibrary1.Script.Foo.js" by default.
        /// </summary>
        public string ResourcePath
        {
            get { return _resourcePath; }
            set { _resourcePath = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ClientScriptResourceAttribute()
        {
        }

        /// <summary>
        /// Called from other constructors to set the prefix and the name.
        /// </summary>
        /// <param name="componentType">The name given to the class in the Web.TypeDescriptor.addType call</param>        
        public ClientScriptResourceAttribute(string componentType)
        {
            _componentType = componentType;
        }

        /// <summary>
        /// Associates a client script resource with the class.
        /// </summary>
        /// <param name="componentType">The name given to the class in the Web.TypeDescriptor.addType call</param>
        /// <param name="baseType">A Type that lives in the same folder as the script file</param>
        /// <param name="resourceName">The name of the script file itself (e.g. 'foo.cs')</param>
        public ClientScriptResourceAttribute(string componentType, Type baseType, string resourceName)
        {
            if (baseType == null)
                throw new ArgumentNullException("baseType");
            if (resourceName == null)
                throw new ArgumentNullException("resourceName");
            string typeName = baseType.FullName;

            int lastDot = typeName.LastIndexOf('.');

            if (lastDot != -1)
            {
                typeName = typeName.Substring(0, lastDot);
            }

            ResourcePath = typeName + "." + resourceName;
            this._componentType = componentType;
        }

        /// <summary>
        /// Associates a client script resource with the class.
        /// </summary>
        /// <param name="componentType">The name given to the class in the Web.TypeDescriptor.addType call</param>
        /// <param name="fullResourceName">The name of the script resource, e.g. 'ControlLibrary1.FooExtender.Foo.js'</param>       
        public ClientScriptResourceAttribute(string componentType, string fullResourceName)
            : this(componentType)
        {
            if (fullResourceName == null) throw new ArgumentNullException("fullResourceName");
            ResourcePath = fullResourceName;
        }
    }
}
