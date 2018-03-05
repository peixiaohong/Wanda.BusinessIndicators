using System;
using System.Reflection;
using Lib.Core;
using Lib.Xml;
using System.Xml;

namespace Lib.Data
{
    /// <summary>
    /// 映射关系类
    /// </summary>
    /// <remarks>
    /// 本类描述了数据实体类与数据库字段间进行映射时的关系
    /// </remarks>
    public class ORMappingItem
    {
        private string propertyName = string.Empty;
        private string subClassPropertyName = string.Empty;
        private string dataFieldName = string.Empty;
        private bool isIdentity = false;
        private bool primaryKey = false;
        private int length = 0;
        private bool isNullable = true;
        private string subClassTypeDescription = string.Empty;
        private ClauseBindingFlags bindingFlags = ClauseBindingFlags.All;
        private string defaultExpression = string.Empty;

        private MemberInfo memberInfo = null;
        private EnumUsageTypes enumUsage = EnumUsageTypes.UseEnumValue;

        /// <summary>
        /// 构造方法
        /// </summary>
        public ORMappingItem()
        {
        }

        /// <summary>
        /// 写入到XmlWriter
        /// </summary>
        /// <param name="writer">XML编写器</param>
        public void WriteToXml(XmlWriter writer)
        {
            XmlHelper.AppendNotNullAttr(writer, "propertyName", this.propertyName);
            XmlHelper.AppendNotNullAttr(writer, "dataFieldName", this.dataFieldName);

            if (this.isIdentity)
                XmlHelper.AppendNotNullAttr(writer, "isIdentity", this.isIdentity);

            if (this.primaryKey)
                XmlHelper.AppendNotNullAttr(writer, "primaryKey", this.primaryKey);

            if (this.length != 0)
                XmlHelper.AppendNotNullAttr(writer, "length", this.length);

            if (this.isNullable == false)
                XmlHelper.AppendNotNullAttr(writer, "isNullable", this.isNullable);

            if (this.bindingFlags != ClauseBindingFlags.All)
                XmlHelper.AppendNotNullAttr(writer, "bindingFlags", this.bindingFlags.ToString());

            XmlHelper.AppendNotNullAttr(writer, "defaultExpression", this.defaultExpression);

            if (this.enumUsage != EnumUsageTypes.UseEnumValue)
                XmlHelper.AppendNotNullAttr(writer, "enumUsage", this.enumUsage.ToString());

            XmlHelper.AppendNotNullAttr(writer, "subClassPropertyName", this.subClassPropertyName);
            XmlHelper.AppendNotNullAttr(writer, "subClassTypeDescription", this.subClassTypeDescription);
        }

        /// <summary>
        /// 从XmlReader中设置属性
        /// </summary>
        /// <param name="reader">Xml阅读器</param>
        /// <param name="mi">成员属性</param>
        public void ReadFromXml(XmlReader reader, MemberInfo mi)
        {
            this.memberInfo = mi;

            this.propertyName = XmlHelper.GetAttributeValue(reader, "propertyName", this.propertyName);
            this.dataFieldName = XmlHelper.GetAttributeValue(reader, "dataFieldName", this.dataFieldName);

            this.isIdentity = XmlHelper.GetAttributeValue(reader, "isIdentity", this.isIdentity);
            this.primaryKey = XmlHelper.GetAttributeValue(reader, "primaryKey", this.primaryKey);
            this.length = XmlHelper.GetAttributeValue(reader, "length", this.length);

            this.isNullable = XmlHelper.GetAttributeValue(reader, "isNullable", this.isNullable);
            this.bindingFlags = XmlHelper.GetAttributeValue(reader, "bindingFlags", this.bindingFlags);

            this.defaultExpression = XmlHelper.GetAttributeValue(reader, "defaultExpression", this.defaultExpression);

            this.enumUsage = XmlHelper.GetAttributeValue(reader, "enumUsage", this.enumUsage);
            this.subClassPropertyName = XmlHelper.GetAttributeValue(reader, "subClassPropertyName", this.subClassPropertyName);
            this.subClassTypeDescription = XmlHelper.GetAttributeValue(reader, "subClassTypeDescription", this.subClassTypeDescription);
        }

        /// <summary>
        /// Enum的值或其描述
        /// </summary>
        public EnumUsageTypes EnumUsage
        {
            get { return this.enumUsage; }
            set { this.enumUsage = value; }
        }

        /// <summary>
        /// 对应的属性为空时，所提供的缺省值表达式
        /// </summary>
        public string DefaultExpression
        {
            get
            {
                return this.defaultExpression;
            }
            set
            {
                this.defaultExpression = value;
            }
        }

        /// <summary>
        /// 对应的属性值会出现在哪些Sql语句中
        /// </summary>
        public ClauseBindingFlags BindingFlags
        {
            get { return this.bindingFlags; }
            set { this.bindingFlags = value; }
        }

        /// <summary>
        /// 字段是否为空
        /// </summary>
        public bool IsNullable
        {
            get { return this.isNullable; }
            set { this.isNullable = value; }
        }

        /// <summary>
        /// 字段长度
        /// </summary>
        public int Length
        {
            get { return this.length; }
            set { this.length = value; }
        }

        /// <summary>
        /// 属性名
        /// </summary>
        public string PropertyName
        {
            get { return this.propertyName; }
            set { this.propertyName = value; }
        }

        /// <summary>
        /// 如果有子对象，对应的子对象属性的名称
        /// </summary>
        public string SubClassPropertyName
        {
            get { return this.subClassPropertyName; }
            set { this.subClassPropertyName = value; }
        }

        /// <summary>
        /// 对应的数据库字段名
        /// </summary>
        public string DataFieldName
        {
            get { return this.dataFieldName; }
            set { this.dataFieldName = value; }
        }

        /// <summary>
        /// 是否标识列
        /// </summary>
        public bool IsIdentity
        {
            get { return this.isIdentity; }
            set { this.isIdentity = value; }
        }

        /// <summary>
        /// 是否主键
        /// </summary>
        public bool PrimaryKey
        {
            get { return this.primaryKey; }
            set { this.primaryKey = value; }
        }

        /// <summary>
        /// MemberInfo类
        /// </summary>
        /// <remarks>
        /// Obtains information about the attributes of a member and provides access to member metadata. 
        /// </remarks>
        public MemberInfo MemberInfo
        {
            get { return this.memberInfo; }
            internal set { this.memberInfo = value; }
        }

        /// <summary>
        /// 子对象的类型描述
        /// </summary>
        public string SubClassTypeDescription
        {
            get { return subClassTypeDescription; }
            internal set { this.subClassTypeDescription = value; }
        }

        /// <summary>
        /// 复制一个MappingItem
        /// </summary>
        /// <returns></returns>
        public ORMappingItem Clone()
        {
            ORMappingItem newItem = new ORMappingItem();

            newItem.dataFieldName = this.dataFieldName;
            newItem.propertyName = this.propertyName;
            newItem.subClassPropertyName = this.subClassPropertyName;
            newItem.isIdentity = this.isIdentity;
            newItem.primaryKey = this.primaryKey;
            newItem.length = this.length;
            newItem.isNullable = this.isNullable;
            newItem.subClassTypeDescription = this.subClassTypeDescription;
            newItem.bindingFlags = this.bindingFlags;
            newItem.defaultExpression = this.defaultExpression;
            newItem.memberInfo = this.memberInfo;
            newItem.enumUsage = this.enumUsage;

            return newItem;
        }
    }
}
