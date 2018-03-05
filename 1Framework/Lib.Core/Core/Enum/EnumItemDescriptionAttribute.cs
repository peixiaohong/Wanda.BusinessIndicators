using System;
using System.Collections.Generic;
using System.Reflection;
using Lib.Globalization;
using Lib.Core.Properties;

namespace Lib.Core
{
    /// <summary>
    /// 定义了枚举型中每个枚举项附加的属性，这个属性包含了对该枚举项的描述信息
    /// </summary>
    /// <remarks>
    /// 这些描述信息包括：描述信息，ID号，短名。
    /// </remarks>
    [AttributeUsageAttribute(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class EnumItemDescriptionAttribute : System.Attribute
    {
        internal string description = string.Empty;
        private int sortId = -1;
        private string shortName = string.Empty;
        private string category = string.Empty;

        private static LruDictionary<Type, EnumItemDescriptionList> innerDictionary =
            new LruDictionary<Type, EnumItemDescriptionList>();

        /// <summary>
        /// 缺省的构造方法
        /// </summary>
        /// <remarks>
        /// 缺省的构造方法
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\EnumItemDescriptionAttributeTest.cs" lang="cs" title="EnumItemDescriptionAttribute缺省的构造方法" />
        /// </remarks>
        public EnumItemDescriptionAttribute()
        {
        }

        /// <summary>
        /// 带描述信息参数的构造函数
        /// </summary>
        /// <param name="desp">对枚举项的描述信息</param>
        /// <remarks>
        /// 带描述信息参数的构造函数，同时设置枚举项附加属性的描述值等于desp。
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\EnumItemDescriptionAttributeTest.cs" lang="cs" title="获得带描述信息参数的构造方法" />
        /// </remarks>
        public EnumItemDescriptionAttribute(string desp)
        {
            this.description = desp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="desp"></param>
        /// <param name="category"></param>
        public EnumItemDescriptionAttribute(string desp, string category)
        {
            this.description = desp;
            this.category = category;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="desp">枚举项的描述信息</param>
        /// <param name="sort">枚举项的排序号</param>
        public EnumItemDescriptionAttribute(string desp, int sort)
        {
            this.description = desp;
            this.sortId = sort;
        }

        /// <summary>
        /// 带描述信息和排序号参数的构造函数
        /// </summary>
        /// <param name="desp">枚举项的描述信息</param>
        /// <param name="sort">枚举项的排序号</param>
        /// <param name="category">多语言版的类别</param>
        /// <remarks>
        /// 带描述信息和排序号参数的构造函数，同时设置枚举项附加属性的描述信息和排序号
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\EnumItemDescriptionAttributeTest.cs" lang="cs" title="带描述信息和序号参数的构造方法" />
        /// </remarks>
        public EnumItemDescriptionAttribute(string desp, int sort, string category)
        {
            this.description = desp;
            this.sortId = sort;
            this.category = category;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="desp"></param>
        /// <param name="sName"></param>
        /// <param name="sort"></param>
        public EnumItemDescriptionAttribute(string desp, string sName, int sort)
        {
            this.description = desp;
            this.shortName = sName;
            this.sortId = sort;
        }

        /// <summary>
        /// 带描述信息、短名和排序号参数的构造函数
        /// </summary>
        /// <param name="desp">枚举项的描述信息</param>
        /// <param name="sName">枚举项的短名</param>
        /// <param name="sort">枚举项的排序号</param>
        /// <param name="category">多语言版的类别</param>
        /// <remarks>带描述信息、短名和排序号参数的构造函数，同时设置枚举项附加属性的描述信息、短名和排序号
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\EnumItemDescriptionAttributeTest.cs" lang="cs" title="带描述信息、短名和排序号参数的构造方法" />
        ///</remarks>
        public EnumItemDescriptionAttribute(string desp, string sName, int sort, string category)
        {
            this.description = desp;
            this.shortName = sName;
            this.sortId = sort;
            this.category = category;
        }

        /// <summary>
        /// 枚举项的描述信息
        /// </summary>
        /// <remarks>该属性是可读写的
        /// </remarks>
        public string Description
        {
            get
            {
                string result = this.description;

                if (string.IsNullOrEmpty(this.category) == false)
                    result = Translator.Translate(this.category, this.description);

                return result;
            }
            set
            {
                this.description = value;
            }
        }

        /// <summary>
        /// 枚举项的排序号
        /// </summary>
        /// <remarks>该属性是可读写的
        /// </remarks>
        public int SortId
        {
            get { return this.sortId; }
            set { this.sortId = value; }
        }

        /// <summary>
        /// 枚举项的短名
        /// </summary>
        /// <remarks>该属性是可读写的
        /// </remarks>
        public string ShortName
        {
            get { return this.shortName; }
            set { this.shortName = value; }
        }

        /// <summary>
        /// 翻译的类别
        /// </summary>
        public string Category
        {
            get { return this.category; }
            set { this.category = value; }
        }

        /// <summary>
        /// 获得枚举项附加属性的描述信息属性
        /// </summary>
        /// <param name="enumItem">枚举项</param>
        /// <returns>描述信息属性，若该附加属性没有定义，则返回null</returns>
        /// <remarks>获得枚举项的附加属性，若该附加属性没有定义，则返回null
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\EnumItemDescriptionAttributeTest.cs" region = "GetAttributeTest" lang="cs" title="得到枚举项的描述信息属性" />
        /// </remarks>
        public static EnumItemDescriptionAttribute GetAttribute(System.Enum enumItem)
        {
            return AttributeHelper.GetCustomAttribute<EnumItemDescriptionAttribute>(enumItem.GetType().GetField(enumItem.ToString()));
        }

        /// <summary>
        /// 获得枚举项的描述信息值，若没有定义该附加属性，则返回空串
        /// </summary>
        /// <param name="enumItem">枚举项</param>
        /// <returns>枚举项的描述信息值，若没有定义该枚举项附加属性，则返回空串</returns>
        /// <remarks>获得枚举项的描述信息值，若没有定义该枚举项附加属性，则返回空串
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\EnumItemDescriptionAttributeTest.cs" region = "GetDescriptionTest" lang="cs" title="得到枚举项的描述信息属性" />
        /// </remarks>
        public static string GetDescription(System.Enum enumItem)
        {
            string result = string.Empty;

            EnumItemDescriptionAttribute attr = GetAttribute(enumItem);

            if (attr != null)
                result = attr.Description;

            return result;
        }

        /// <summary>
        /// 获得已排序的枚举型的描述信息表
        /// </summary>
        /// <param name="enumType">枚举型</param>
        /// <returns>已排序的枚举型的描述信息表</returns>
        /// <remarks>得到已排序的枚举型的描述信息表，该表是根据SortID属性排序的。
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\EnumItemDescriptionAttributeTest.cs" region = "GetEnumItemDescriptionListTest" lang="cs" title="获得已排序的枚举项的描述信息表" />
        /// </remarks>
        public static EnumItemDescriptionList GetDescriptionList(Type enumType)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(enumType != null, "enumType");
            ExceptionHelper.FalseThrow<ArgumentException>(enumType.IsEnum, Resource.TypeMustBeEnum, enumType.FullName);

            lock (EnumItemDescriptionAttribute.innerDictionary)
            {
                EnumItemDescriptionList result;

                if (EnumItemDescriptionAttribute.innerDictionary.TryGetValue(enumType, out result) == false)
                {
                    result = GetDescriptionListFromEnumType(enumType);
                    EnumItemDescriptionAttribute.innerDictionary.Add(enumType, result);
                }

                return result;
            }
        }

        private static EnumItemDescriptionList GetDescriptionListFromEnumType(Type enumType)
        {
            List<EnumItemDescription> eidList = new List<EnumItemDescription>();

            FieldInfo[] fileds = enumType.GetFields();

            for (int i = 0; i < fileds.Length; i++)
            {
                FieldInfo fi = fileds[i];

                if (fi.IsLiteral && fi.IsStatic)
                    eidList.Add(EnumItemDescription.CreateFromFieldInfo(fi, enumType));
            }

            eidList.Sort(delegate(EnumItemDescription x, EnumItemDescription y)
            {
                return Math.Sign(x.SortId - y.SortId);
            }
                );

            return new EnumItemDescriptionList(eidList);
        }
    }
}
