using System;
using System.Text;
using Lib.Core;

namespace Lib.Data
{
    /// <summary>
    /// 提供一组字段和值的集合，帮助生成UPDATE语句的SET部分
    /// </summary>
    [Serializable]
    public class UpdateSqlClauseBuilder : SqlClauseBuilderIUW
    {
        /// <summary>
        /// 生成Update语句的SET部分（不包括SET）
        /// </summary>
        /// <param name="sqlBuilder">Sql语句构造器</param>
        /// <returns>构造的Update子句(不含update部分)</returns>
        public override string ToSqlString(ISqlBuilder sqlBuilder)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(sqlBuilder != null, "sqlBuilder");

            StringBuilder strB = new StringBuilder(256);

            foreach (SqlClauseBuilderItemIUW item in List)
            {
                if (strB.Length > 0)
                    strB.Append(", ");

                strB.Append(item.DataField);
                strB.AppendFormat(" {0} ", item.Operation);
                strB.Append(item.GetDataDesp(sqlBuilder));
            }

            return strB.ToString();
        }
    }
}
