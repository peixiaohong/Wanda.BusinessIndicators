using System;

namespace Lib.Data
{
    /// <summary>
    /// 属性为子对象时的ORMaping关系
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class SubClassORFieldMappingAttribute : ORFieldMappingAttribute
    {
        private string subPropertyName = string.Empty;

        /// <summary>
        /// 构造方法
        /// </summary>
        protected SubClassORFieldMappingAttribute()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subPropertyName"></param>
        /// <param name="fieldName"></param>
        public SubClassORFieldMappingAttribute(string subPropertyName, string fieldName)
            : base(fieldName)
        {
            this.subPropertyName = subPropertyName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subPropertyName"></param>
        /// <param name="fieldName"></param>
        /// <param name="nullable"></param>
        public SubClassORFieldMappingAttribute(string subPropertyName, string fieldName, bool nullable)
            : base(fieldName, nullable)
        {
            this.subPropertyName = subPropertyName;
        }

        /// <summary>
        /// 源对象的属性名称
        /// </summary>
        public string SubPropertyName
        {
            get
            {
                return this.subPropertyName;
            }
            set
            {
                this.subPropertyName = value;
            }
        }
    }
}
