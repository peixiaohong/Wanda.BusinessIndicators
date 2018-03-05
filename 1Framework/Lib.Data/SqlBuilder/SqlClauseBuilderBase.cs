using System;
using System.Collections;

namespace Lib.Data
{
    /// <summary>
    /// Sql子句构造器的抽象基类
    /// </summary>
    public abstract class SqlClauseBuilderBase : CollectionBase
    {
        #region 条件项的运算符常量定义
        /// <summary>
        /// 等号
        /// </summary>
        internal const string EqualTo = "=";

        /// <summary>
        /// 大于等于
        /// </summary>
        internal const string GreaterThanOrEqualTo = ">=";

        /// <summary>
        /// 大于
        /// </summary>
        internal const string GreaterThan = ">";

        /// <summary>
        /// 小于等于
        /// </summary>
        internal const string LessThanOrEqualTo = "<=";

        /// <summary>
        /// 小于
        /// </summary>
        internal const string LessThan = "<";

        /// <summary>
        /// 不等于
        /// </summary>
        internal const string NotEqualTo = "<>";

        /// <summary>
        /// LIKE运算符
        /// </summary>
        internal const string Like = "LIKE";

        /// <summary>
        /// IS运算符
        /// </summary>
        internal const string Is = "IS";

        /// <summary>
        /// IN运算符
        /// </summary>
        internal const string In = "IN";
        #endregion 条件项的运算符常量定义

        /// <summary>
        /// 按照下标访问构造项
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public SqlClauseBuilderItemBase this[int i]
        {
            get
            {
                return (SqlClauseBuilderItemBase)List[i];
            }
        }

        /// <summary>
        /// 抽象方法，将所有的构造项生成一个SQL
        /// </summary>
        /// <param name="sqlBuilder"></param>
        /// <returns></returns>
        public abstract string ToSqlString(ISqlBuilder sqlBuilder);

    }


}
