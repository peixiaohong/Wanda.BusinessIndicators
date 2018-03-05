using System;
using System.Reflection;
using System.Collections.Generic;

namespace Lib.Core
{
    /// <summary>
    /// Attribute定义的控制器
    /// </summary>
    /// <remarks>
    /// 通过反射，读取类、属性、方法等的Attribute定义
    /// </remarks>
    public static class AttributeHelper
    {
        private struct AttrDictEntry
        {
            public object MemberInfo;
            public System.Type AttributeType;
            public bool Inherited;
        }

        private static Dictionary<AttrDictEntry, Attribute> dictionary = new Dictionary<AttrDictEntry, Attribute>();

        /// <summary>
        /// 读取类、属性或方法上的Attribute定义
        /// </summary>
        /// <typeparam name="T">欲读取的类、属性或方法的类型</typeparam>
        /// <param name="element">类、属性或方法类型的实例</param>
        /// <returns>类、属性或方法等的Attribute实例</returns>
        /// <remarks>
        /// 读取类、属性或方法上的Attribute定义。采用反射机制实现。
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\AttributeHelperTest.cs" region="GetCustomAttributeTest" lang="cs" title="获取CollegeStudent的DescriptionAttribute内容" />
        /// <seealso cref="System.ComponentModel.DescriptionAttribute" />
        /// <seealso cref="MCS.Library.Core.EnumItemDescriptionAttribute"/>
        /// </remarks>
        public static T GetCustomAttribute<T>(MemberInfo element) where T : Attribute
        {
            T result = default(T);
            System.Type attrType = typeof(T);

            AttrDictEntry key = CalculateKey(element, attrType, true);

            lock (AttributeHelper.dictionary)
            {
                if (AttributeHelper.dictionary.ContainsKey(key))
                    result = (T)AttributeHelper.dictionary[key];
                else
                {
                    result = (T)Attribute.GetCustomAttribute(element, attrType);
                    AttributeHelper.dictionary[key] = result;
                }
            }

            return result;
        }

        private static AttrDictEntry CalculateKey(object element, System.Type attrType, bool inherited)
        {
            AttrDictEntry key;

            key.MemberInfo = element;
            key.AttributeType = attrType;
            key.Inherited = inherited;

            return key;
        }
    }
}
