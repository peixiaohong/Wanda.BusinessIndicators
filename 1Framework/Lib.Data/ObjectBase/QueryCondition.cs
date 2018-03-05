using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lib.Data
{
    /// <summary>
    /// 包装分页查询的查询参数
    /// </summary>
    [Serializable]
    public class QueryCondition
    {
        private string selectFields = string.Empty;
        private string fromClause = string.Empty;
        private string whereClause = string.Empty;
        private string orderByClause = string.Empty;
        private int rowIndex = 0;
        private int pageSize = 10;

        private string primaryKey = string.Empty;

        /// <summary>
        /// 构造方法
        /// </summary>
        public QueryCondition()
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="rowIndex">从第几行开始</param>
        /// <param name="pageSize">每页的行数</param>
        /// <param name="select">sql语句返回的字段</param>
        /// <param name="from">sql语句的from部分</param>
        /// <param name="orderBy">sql语句的orderBy部分</param>
        public QueryCondition(int rowIndex, int pageSize, string select, string from, string orderBy)
        {
            this.rowIndex = rowIndex;
            this.pageSize = pageSize;
            this.selectFields = select;
            this.fromClause = from;
            this.orderByClause = orderBy;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="rowIndex">从第几行开始</param>
        /// <param name="pageSize">每页的行数</param>
        /// <param name="select">sql语句返回的字段</param>
        /// <param name="from">sql语句的from部分</param>
        /// <param name="orderBy">sql语句的orderBy部分</param>
        /// <param name="where">sql语句的where部分</param>
        public QueryCondition(int rowIndex, int pageSize, string select, string from, string orderBy, string where)
            : this(rowIndex, pageSize, select, from, orderBy)
        {
            this.whereClause = where;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="rowIndex">从第几行开始</param>
        /// <param name="pageSize">每页的行数</param>
        /// <param name="select">sql语句返回的字段</param>
        /// <param name="from">sql语句的from部分</param>
        /// <param name="orderBy">sql语句的orderBy部分</param>
        /// <param name="where">sql语句的where部分</param>
        /// <param name="key">表的主键</param>
        public QueryCondition(int rowIndex, int pageSize, string select, string from, string orderBy, string where, string key)
            : this(rowIndex, pageSize, select, from, orderBy, where)
        {
            this.PrimaryKey = key;
        }

        /// <summary>
        /// 每页的行数
        /// </summary>
        public int PageSize
        {
            get { return this.pageSize; }
            set { this.pageSize = value; }
        }

        /// <summary>
        /// 从第几行开始
        /// </summary>
        public int RowIndex
        {
            get { return this.rowIndex; }
            set { this.rowIndex = value; }
        }

        /// <summary>
        /// sql语句的orderBy部分
        /// </summary>
        public string OrderByClause
        {
            get { return this.orderByClause; }
            set { this.orderByClause = value; }
        }

        /// <summary>
        /// sql语句的where部分
        /// </summary>
        public string WhereClause
        {
            get { return this.whereClause; }
            set { this.whereClause = value; }
        }

        /// <summary>
        /// sql语句的from部分
        /// </summary>
        public string FromClause
        {
            get { return this.fromClause; }
            set { this.fromClause = value; }
        }

        /// <summary>
        /// sql语句返回的字段
        /// </summary>
        public string SelectFields
        {
            get { return this.selectFields; }
            set { this.selectFields = value; }
        }

        /// <summary>
        /// 表的主键
        /// </summary>
        public string PrimaryKey
        {
            get { return this.primaryKey; }
            set { this.primaryKey = value; }
        }
    }
}
