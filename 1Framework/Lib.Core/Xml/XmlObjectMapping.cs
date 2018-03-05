using System;
using System.Collections.Generic;
using System.Reflection;
using Lib.Core;
using Lib.Core.Properties;

namespace Lib.Xml
{
    /// <summary>
    /// 
    /// </summary>
    public static class XmlObjectMapping
    {
        private class RelativeAttributes
        {
            public XmlObjectMappingAttribute FieldMapping = null;
            public NoXmlObjectMappingAttribute NoMapping = null;
            public List<XmlObjectSubClassMappingAttribute> SubClassFieldMappings = new List<XmlObjectSubClassMappingAttribute>();
            public XmlObjectSubClassTypeAttribute SubClassType = null;

            public XmlCollectionMappingAttribute CollectionMapping = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="graph"></param>
        /// <returns></returns>
        public static XmlObjectMappingItemCollection GetMappingInfoByObject<T>(T graph)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(graph != null, "graph");

            System.Type type = null;

            if (typeof(T).IsInterface)
                type = typeof(T);
            else
                type = graph.GetType();

            return InnerGetMappingInfo(type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static XmlObjectMappingItemCollection GetMappingInfo<T>()
        {
            return InnerGetMappingInfo(typeof(T));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static XmlObjectMappingItemCollection GetMappingInfo(System.Type type)
        {
            return InnerGetMappingInfo(type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static XmlObjectMappingItemCollection InnerGetMappingInfo(System.Type type)
        {
            ExceptionHelper.FalseThrow(type != null, "type");

            return XmlMappingsCache.Instance.GetOrAddNewValue(type, (cache, key) => cache.Add(key, GetMappingItemCollection(type)));
        }

        private static XmlObjectMappingItemCollection GetMappingItemCollection(System.Type type)
        {
            XmlObjectMappingItemCollection result = new XmlObjectMappingItemCollection();
            bool onlyMapMarkedProperties = true;

            XmlRootMappingAttribute rootAttr =
                (XmlRootMappingAttribute)XmlRootMappingAttribute.GetCustomAttribute(type, typeof(XmlRootMappingAttribute), true);

            if (rootAttr != null)
            {
                result.RootName = rootAttr.RootName;
                onlyMapMarkedProperties = rootAttr.OnlyMapMarkedProperties;
            }
            else
                result.RootName = type.Name;

            MemberInfo[] mis = GetTypeMembers(type);

            foreach (MemberInfo mi in mis)
            {
                if (mi.MemberType == MemberTypes.Field || mi.MemberType == MemberTypes.Property)
                {
                    XmlObjectMappingItemCollection items = CreateMappingItems(mi, onlyMapMarkedProperties);

                    MergeMappingItems(result, items);
                }
            }

            return result;
        }

        private static void MergeMappingItems(XmlObjectMappingItemCollection dest, XmlObjectMappingItemCollection src)
        {
            foreach (XmlObjectMappingItem item in src)
                if (dest.Contains(item.NodeName) == false)
                    dest.Add(item);
        }

        private static MemberInfo[] GetTypeMembers(Type type)
        {
            List<MemberInfo> list = new List<MemberInfo>();

            PropertyInfo[] pis = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);

            Array.ForEach(pis, delegate(PropertyInfo pi) { list.Add(pi); });

            FieldInfo[] fis = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);

            Array.ForEach(fis, delegate(FieldInfo fi) { list.Add(fi); });

            return list.ToArray();
        }

        private static MemberInfo GetMemberInfoByName(string name, MemberInfo[] mis)
        {
            MemberInfo result = null;

            foreach (MemberInfo mi in mis)
            {
                if (mi.Name == name)
                {
                    result = mi;
                    break;
                }
            }

            return result;
        }

        private static XmlObjectMappingItemCollection CreateMappingItems(MemberInfo mi, bool onlyMapMarkedProperties)
        {
            XmlObjectMappingItemCollection result = null;
            bool isDoMapping = false;

            RelativeAttributes attrs = null;

            if (mi.Name != "Item")
            {
                attrs = GetRelativeAttributes(mi);

                if (onlyMapMarkedProperties)
                    isDoMapping = attrs.FieldMapping != null;
                else
                    isDoMapping = true;

                if (isDoMapping && attrs.NoMapping == null)
                    isDoMapping = true;
                else
                    isDoMapping = false;
            }

            if (isDoMapping == true)
            {
                if (attrs != null)
                {
                    if (attrs.SubClassFieldMappings.Count > 0)
                        result = GetMappingItemsBySubClass(attrs, mi);
                    else
                        result = GetMappingItems(attrs, mi);
                }
            }
            else
                result = new XmlObjectMappingItemCollection();

            return result;
        }

        internal static System.Type GetRealType(MemberInfo mi)
        {
            System.Type type = null;

            switch (mi.MemberType)
            {
                case MemberTypes.Property:
                    type = ((PropertyInfo)mi).PropertyType;
                    break;
                case MemberTypes.Field:
                    type = ((FieldInfo)mi).FieldType;
                    break;
                default:
                    ThrowInvalidMemberInfoTypeException(mi);
                    break;
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition().FullName == "System.Nullable`1")
                type = type.GetGenericArguments()[0];

            return type;
        }

        private static XmlObjectMappingItemCollection GetMappingItems(RelativeAttributes attrs, MemberInfo mi)
        {
            XmlObjectMappingItemCollection items = new XmlObjectMappingItemCollection();

            XmlObjectMappingItem item = new XmlObjectMappingItem(attrs.FieldMapping);

            item.PropertyName = mi.Name;
            if (string.IsNullOrEmpty(item.NodeName))
                item.NodeName = mi.Name;

            item.MemberInfo = mi;

            items.Add(item);

            return items;
        }

        private static XmlObjectMappingItemCollection GetMappingItemsBySubClass(RelativeAttributes attrs, MemberInfo sourceMI)
        {
            XmlObjectMappingItemCollection items = new XmlObjectMappingItemCollection();
            System.Type subType = attrs.SubClassType != null ? attrs.SubClassType.Type : GetRealType(sourceMI);

            MemberInfo[] mis = GetTypeMembers(subType);

            foreach (XmlObjectSubClassMappingAttribute attr in attrs.SubClassFieldMappings)
            {
                MemberInfo mi = GetMemberInfoByName(attr.SubPropertyName, mis);

                if (mi != null)
                {
                    if (items.Contains(attr.NodeName) == false)
                    {
                        XmlObjectMappingItem item = new XmlObjectMappingItem(attr);

                        item.PropertyName = sourceMI.Name;
                        item.SubClassPropertyName = attr.SubPropertyName;
                        item.MemberInfo = mi;

                        if (attrs.SubClassType != null)
                            item.SubClassTypeDescription = attrs.SubClassType.TypeDescription;

                        items.Add(item);
                    }
                }
            }

            return items;
        }

        internal static MemberInfo GetSubClassMemberInfoByName(string subClassPropertyName, MemberInfo sourceMI)
        {
            RelativeAttributes attrs = GetRelativeAttributes(sourceMI);

            System.Type subType = attrs.SubClassType != null ? attrs.SubClassType.Type : GetRealType(sourceMI);

            return GetSubClassMemberInfoByName(subClassPropertyName, subType);
        }

        internal static MemberInfo GetSubClassMemberInfoByName(string subClassPropertyName, System.Type subType)
        {
            MemberInfo[] mis = GetTypeMembers(subType);

            return GetMemberInfoByName(subClassPropertyName, mis);
        }

        internal static void ThrowInvalidMemberInfoTypeException(MemberInfo mi)
        {
            throw new InvalidOperationException(string.Format(Resource.InvalidMemberInfoType,
                mi.Name,
                mi.MemberType));
        }

        private static RelativeAttributes GetRelativeAttributes(MemberInfo mi)
        {
            RelativeAttributes result = new RelativeAttributes();

            Attribute[] attrs = Attribute.GetCustomAttributes(mi, true);

            foreach (Attribute attr in attrs)
            {
                if (attr is NoXmlObjectMappingAttribute)
                {
                    result.NoMapping = (NoXmlObjectMappingAttribute)attr;
                    break;
                }
                else
                    if (attr is XmlObjectSubClassMappingAttribute)
                        result.SubClassFieldMappings.Add((XmlObjectSubClassMappingAttribute)attr);
                    else
                        if (attr is XmlObjectSubClassTypeAttribute)
                            result.SubClassType = (XmlObjectSubClassTypeAttribute)attr;
                        else
                            if (attr is XmlObjectMappingAttribute)
                                result.FieldMapping = (XmlObjectMappingAttribute)attr;
                            else
                                if (attr is XmlCollectionMappingAttribute)
                                    result.FieldMapping = (XmlCollectionMappingAttribute)attr;
            }

            return result;
        }
    }
}
