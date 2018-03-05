using System;
using System.Web.Script.Serialization;

namespace Lib.Web.Json
{
    /// <summary>
    /// 带有默认类别的JSON序列化类别解析器，如果此类别解析器无法解析出类别，则会使用默认类别代替
    /// </summary>
    /// <remarks>带有默认类别的JSON序列化类别解析器，如果此类别解析器无法解析出类别，则会使用默认类别代替</remarks>
    public class DefaultTypeResolver : JavaScriptTypeResolver
    {
        private Type _DefaultType;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="defaultType">解析出的默认类别</param>
        /// <remarks>构造函数</remarks>
        public DefaultTypeResolver(Type defaultType)
        {
            _DefaultType = defaultType;
        }

        /// <summary>
        /// 通过类别id解析出类别
        /// </summary>
        /// <param name="id">类别id</param>
        /// <returns>类别</returns>
        /// <remarks>通过类别id解析出类别</remarks>
        public override Type ResolveType(string id)
        {
            Type t = Type.GetType(id, false);

            return t ?? _DefaultType;
        }

        /// <summary>
        /// 通过类别解析出类别id
        /// </summary>
        /// <param name="type">类别</param>
        /// <returns>类别id</returns>
        /// <remarks>通过类别解析出类别id</remarks>
        public override string ResolveTypeId(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return type.AssemblyQualifiedName;
        }
    }
}
