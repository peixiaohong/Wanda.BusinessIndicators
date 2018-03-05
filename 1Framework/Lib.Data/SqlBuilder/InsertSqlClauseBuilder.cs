using System;
using Lib.Core;
using System.Text;

namespace Lib.Data
{
    /// <summary>
    /// 提供一组字段和值的集合，帮助生成INSERT语句的字段名称和VALUES部分
    /// </summary>
    [Serializable]
    public class InsertSqlClauseBuilder : SqlClauseBuilderIUW
    {
        /// <summary>
        /// 生成INSERT语句的字段名称和VALUES部分
        /// </summary>
        /// <param name="sqlBuilder">Sql语句构造器</param>
        /// <returns>构造的Insert子句(不含insert部分)</returns>
        public override string ToSqlString(ISqlBuilder sqlBuilder)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(sqlBuilder != null, "sqlBuilder");

            StringBuilder strBFields = new StringBuilder(256);
            StringBuilder strBValues = new StringBuilder(256);

            foreach (SqlClauseBuilderItemIUW item in List)
            {
                if (item.Data != null && item.Data != DBNull.Value )
                {
                    if(item.Data.ToString() !="00000000-0000-0000-0000-000000000000")
                    {
                    if (strBFields.Length > 0)
                        strBFields.Append(", ");

                    strBFields.Append(item.DataField);

                    if (strBValues.Length > 0)
                        strBValues.Append(", ");

                    strBValues.Append(item.GetDataDesp(sqlBuilder));
                    }
                }
            }

            string result = string.Empty;

            if (strBValues.Length > 0)
                result = string.Format("({0}) VALUES({1})", strBFields.ToString(), strBValues.ToString());

            return result;
        }
    }
}
