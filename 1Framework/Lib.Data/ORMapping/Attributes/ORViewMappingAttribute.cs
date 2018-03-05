using System;

namespace Lib.Data
{
    /// <summary>
    /// 加在类定义之前，用于表示数据库视图的Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ORViewMappingAttribute : ORTableMappingAttribute
    {

        // 表示sql 视图的查询sql语句
        private string viewTSql = "";

        // 排序字段名, 默认为ID列
        private string orderByFieldName = "ID"; /*查询的时候， 大多数需要按照一定的顺序返回结果，特别是在分页的时候， 此字段非常有必要*/

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="sqlString">构建视图的SQL语句</param>
        /// <param name="tableName">表名</param>
        public ORViewMappingAttribute(string sqlString, string tableName) :
            base(tableName)
        {
            this.viewTSql = sqlString;
        }


        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="sqlString">构建视图的SQL语句</param>
        /// <param name="tableName">表名</param>
        /// <param name="orderByFieldName">排序字段名</orderByFieldName>
        public ORViewMappingAttribute(string sqlString, string tableName, string orderByFieldName) :
            base(tableName)
        {
            this.viewTSql = sqlString;
            this.orderByFieldName = orderByFieldName;
        }

        /// <summary>
        /// select 的数据源
        /// </summary>
        public override string TableName
        {
            get
            {
                return string.Format("({0}) as {1}", viewTSql, base.TableName);
            }
        }

    }
}
