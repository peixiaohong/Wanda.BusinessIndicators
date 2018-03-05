using System;

namespace Lib.Data
{
    /// <summary>
    /// 可连接的Sql子句的接口
    /// </summary>
    public interface IConnectiveSqlClause
    {
        /// <summary>
        /// 连接时的逻辑运算符
        /// </summary>
        LogicOperatorDefine LogicOperator
        {
            get;
            set;
        }

        /// <summary>
        /// 子句是否为空
        /// </summary>
        bool IsEmpty
        {
            get;
        }

        /// <summary>
        /// 生成Sql子句
        /// </summary>
        /// <param name="sqlBuilder"></param>
        /// <returns></returns>
        string ToSqlString(ISqlBuilder sqlBuilder);
    }
}
