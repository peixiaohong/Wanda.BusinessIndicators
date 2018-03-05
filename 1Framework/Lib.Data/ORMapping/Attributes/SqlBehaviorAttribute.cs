using System;
using Lib.Core;

namespace Lib.Data
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class SqlBehaviorAttribute : System.Attribute
    {
        private ClauseBindingFlags bindingFlags = ClauseBindingFlags.All;
        private string defaultExpression = string.Empty;
        private EnumUsageTypes enumUsage = EnumUsageTypes.UseEnumValue;

        /// <summary>
        /// 如果对应的属性是枚举类型，生成Sql时，是否使用枚举类型的值（整型），否则使用字符串
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
            get { return this.defaultExpression; }
            set { this.defaultExpression = value; }
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
        /// 缺省构造方法
        /// </summary>
        public SqlBehaviorAttribute()
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="flags">对应的属性值会出现在哪些Sql语句中</param>
        public SqlBehaviorAttribute(ClauseBindingFlags flags)
        {
            this.bindingFlags = flags;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="defaultExpression">如果对应的属性值为空，使用的缺省值表达式</param>
        public SqlBehaviorAttribute(string defaultExpression)
        {
            this.defaultExpression = defaultExpression;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="flags">对应的属性值会出现在哪些Sql语句中</param>
        /// <param name="defaultExpression">如果对应的属性值为空，使用的缺省值表达式</param>
        public SqlBehaviorAttribute(ClauseBindingFlags flags, string defaultExpression)
            : this(flags)
        {
            this.defaultExpression = defaultExpression;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="usage">如果对应的属性是枚举类型，生成Sql时，是否使用枚举类型的值（整型），否则使用字符串</param>
        public SqlBehaviorAttribute(EnumUsageTypes usage)
        {
            this.enumUsage = usage;
        }
    }
}
