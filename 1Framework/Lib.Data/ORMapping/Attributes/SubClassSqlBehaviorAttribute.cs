using System;
using Lib.Core;

namespace Lib.Data
{
    /// <summary>
    /// 属性为子对象时的Sql语句生成关系
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class SubClassSqlBehaviorAttribute : SqlBehaviorAttribute
    {
        private string subPropertyName = string.Empty;

        /// <summary>
        /// 缺省构造方法
        /// </summary>
        public SubClassSqlBehaviorAttribute()
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="subPropertyName"></param>
        /// <param name="flags">对应的属性值会出现在哪些Sql语句中</param>
        public SubClassSqlBehaviorAttribute(string subPropertyName, ClauseBindingFlags flags)
            : base(flags)
        {
            this.subPropertyName = subPropertyName;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="subPropertyName"></param>
        /// <param name="defaultExpression">如果对应的属性值为空，使用的缺省值表达式</param>
        public SubClassSqlBehaviorAttribute(string subPropertyName, string defaultExpression)
            : base(defaultExpression)
        {
            this.subPropertyName = subPropertyName;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="subPropertyName"></param>
        /// <param name="flags">对应的属性值会出现在哪些Sql语句中</param>
        /// <param name="defaultExpression">如果对应的属性值为空，使用的缺省值表达式</param>
        public SubClassSqlBehaviorAttribute(string subPropertyName, ClauseBindingFlags flags, string defaultExpression)
            : base(flags, defaultExpression)
        {
            this.subPropertyName = subPropertyName;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="subPropertyName"></param>
        /// <param name="usage">如果对应的属性是枚举类型，生成Sql时，是否使用枚举类型的值（整型），否则使用字符串</param>
        public SubClassSqlBehaviorAttribute(string subPropertyName, EnumUsageTypes usage)
            : base(usage)
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
