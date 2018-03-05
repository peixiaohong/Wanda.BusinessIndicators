using System;
using System.Reflection;
using System.Collections.Generic;
using Lib.Globalization;

namespace Lib.Core
{
    /// <summary>
    /// 枚举项描述类
    /// </summary>
    /// <remarks>用于描述枚举项的类，其中描述信息包括：Name，Description，ShortName，EnumValue，SortID。
    /// </remarks>
    public sealed class EnumItemDescription : IComparer<EnumItemDescription>
    {
        private string name = string.Empty;
        private string description = string.Empty;
        private string shortName = string.Empty;
        private int enumValue;
        private int sortId = -1;
        private string category = string.Empty;

        /// <summary>
        /// 通过一个字段属性创建一个对枚举项描述的实例
        /// </summary>
        /// <param name="fi">字段属性实例</param>
        /// <param name="enumType">枚举型</param>
        /// <returns>描述枚举项的实例</returns>
        /// <remarks>通过一个字段属性值创建一个枚举项描述的实例
        /// <seealso cref="MCS.Library.Core.EnumItemDescriptionAttribute"/>
        /// </remarks>
        internal static EnumItemDescription CreateFromFieldInfo(FieldInfo fi, Type enumType)
        {
            EnumItemDescription eid = new EnumItemDescription();

            eid.Name = fi.Name;
            eid.EnumValue = (int)fi.GetValue(enumType);
            eid.SortId = eid.EnumValue;

            eid.FillDescriptionAttributeInfo(AttributeHelper.GetCustomAttribute<EnumItemDescriptionAttribute>(fi));

            return eid;
        }

        private void FillDescriptionAttributeInfo(EnumItemDescriptionAttribute attr)
        {
            if (attr != null)
            {
                this.Description = attr.description;
                this.ShortName = attr.ShortName;
                this.Category = attr.Category;

                if (attr.SortId != -1)
                    this.sortId = attr.SortId;
            }
        }

        private EnumItemDescription()
        {
        }

        /// <summary>
        /// 枚举项排序的ID
        /// </summary>
        /// <remarks>枚举项排序的ID，当对枚举项进行排序时，使用该属性进行排序，该属性是只读的
        /// </remarks>
        public int SortId
        {
            get { return this.sortId; }
            internal set { this.sortId = value; }
        }

        /// <summary>
        /// 枚举项的值
        /// </summary>
        /// <remarks>该属性是只读的
        /// </remarks>
        public int EnumValue
        {
            get { return this.enumValue; }
            internal set { this.enumValue = value; }
        }

        /// <summary>
        /// 枚举项的短名
        /// </summary>
        /// <remarks>该属性是只读的
        /// </remarks>
        public string ShortName
        {
            get { return this.shortName; }
            internal set { this.shortName = value; }
        }

        /// <summary>
        /// 枚举项的描述信息
        /// </summary>
        /// <remarks>该属性是只读的
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
            internal set
            {
                this.description = value;
            }
        }

        /// <summary>
        /// 类别
        /// </summary>
        public string Category
        {
            get { return this.category; }
            internal set { this.category = value; }
        }

        /// <summary>
        /// 枚举项的名字
        /// </summary>
        /// <remarks>
        /// 该属性是只读的
        /// </remarks>
        public string Name
        {
            get { return this.name; }
            internal set { this.name = value; }
        }

        #region IComparer<EnumItemDescription> 成员

        /// <summary>
        /// 枚举项的比较方法
        /// </summary>
        /// <param name="x">需要比较的枚举项描述类的实例</param>
        /// <param name="y">需要比较的枚举项描述类的实例</param>
        /// <returns>比较结果</returns>
        /// <remarks>枚举项的比较方法,返回值是两个实例的排序号相减的结果
        /// </remarks>
        public int Compare(EnumItemDescription x, EnumItemDescription y)
        {
            return x.SortId - y.SortId;
        }

        #endregion
    }
}
