#region 李刚于2013/4/28添加
#endregion
using Lib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lib.Data
{

    [Serializable]
    public class OrderBySqlClauseBuilder : SqlClauseBuilderBase
    {
        /// <summary>
        /// 添加一个构造项
        /// </summary>
        /// <param name="dataField">操作的数据</param>
        /// <param name="sortDirection">排序方式</param>
        public void AppendItem(string dataField, FieldSortDirection sortDirection)
        {
            SqlClauseBuilderItemOrd item = new SqlClauseBuilderItemOrd();

            item.DataField = dataField;
            item.SortDirection = sortDirection;
            List.Add(item);
        }

        /// <summary>
        /// 帮助生成ORDER BY语句的字段排序部分
        /// </summary>
        /// <param name="sqlBuilder">Sql语句构造器</param>
        /// <returns>构造出的Order By子句</returns>
        public override string ToSqlString(ISqlBuilder sqlBuilder)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(sqlBuilder != null, "sqlBuilder");

            StringBuilder strB = new StringBuilder(256);

            foreach (SqlClauseBuilderItemOrd item in List)
            {
                if (strB.Length > 0)
                    strB.Append(", ");

                item.ToSqlString(strB, sqlBuilder);
            }

            return strB.ToString();
        }
    }

    /// <summary>
    /// 构造排序表达式的构造项
    /// </summary>
    [Serializable]
    public class SqlClauseBuilderItemOrd : SqlClauseBuilderItemBase
    {
        /// <summary>
        /// 
        /// </summary>
        private FieldSortDirection sortDirection = FieldSortDirection.Ascending;
        /// <summary>
        /// 
        /// </summary>
        private string dataField = string.Empty;

        /// <summary>
        /// 构造方法
        /// </summary>
        public SqlClauseBuilderItemOrd()
        {
        }

        /// <summary>
        /// Sql语句中的字段名
        /// </summary>
        public string DataField
        {
            get { return this.dataField; }
            set
            {
                ExceptionHelper.TrueThrow<ArgumentException>(string.IsNullOrEmpty(value), "DataField属性不能为空或空字符串");
                this.dataField = value;
            }
        }

        /// <summary>
        /// 排序方向
        /// </summary>
        public FieldSortDirection SortDirection
        {
            get
            {
                return this.sortDirection;
            }
            set
            {
                this.sortDirection = value;
            }
        }

        /// <summary>
        /// 得到Data的Sql字符串描述
        /// </summary>
        /// <param name="builder">构造器</param>
        /// <returns>返回将data翻译成sql语句的结果</returns>
        public override string GetDataDesp(ISqlBuilder builder)
        {
            string result = string.Empty;

            if (this.sortDirection == FieldSortDirection.Descending)
                result = "DESC";

            return result;
        }

        /// <summary>
        /// 生成SQL子句（“字段 ASC|DESC”）
        /// </summary>
        /// <param name="strB"></param>
        /// <param name="builder"></param>
        internal void ToSqlString(StringBuilder strB, ISqlBuilder builder)
        {
            strB.Append(this.DataField);

            string desp = this.GetDataDesp(builder);

            if (false == string.IsNullOrEmpty(desp))
                strB.Append(" " + desp);
        }
    }

    /// <summary>
    /// 字段的排序方向定义
    /// </summary>
    public enum FieldSortDirection
    {
        /// <summary>
        /// 升序
        /// </summary>
        Ascending,

        /// <summary>
        /// 降序
        /// </summary>
        Descending
    }
}
