using System;

namespace Lib.Data
{
    /// <summary>
    /// Insert、Update、Where语句构造器的基类
    /// </summary>
    public abstract class SqlClauseBuilderIUW : SqlClauseBuilderBase
    {
        /// <summary>
        /// 添加一个构造项
        /// </summary>SqlCaluseBuilderBase
        /// <typeparam name="T">数据的类型</typeparam>
        /// <param name="dataField">Sql语句中的字段名</param>
        /// <param name="data">操作的数据</param>
        public void AppendItem<T>(string dataField, T data)
        {
            AppendItem<T>(dataField, data, SqlClauseBuilderBase.EqualTo);
        }

         
        /// <summary>
        /// 添加一个构造项
        /// </summary>
        /// <typeparam name="T">数据的类型</typeparam>
        /// <param name="dataField">Sql语句中的字段名</param>
        /// <param name="data">操作的数据</param>
        /// <param name="op">操作运算符</param>
        public void AppendItem<T>(string dataField, T data, string op)
        {
            AppendItem<T>(dataField, data, op, false);
        }

        /// <summary>
        /// 添加一个构造项
        /// </summary>
        /// <typeparam name="T">数据的类型</typeparam>
        /// <param name="dataField">Sql语句中的字段名</param>
        /// <param name="data">操作的数据</param>
        /// <param name="op">操作运算符</param>
        /// <param name="isExpression">操作的数据是否是表达式</param>
        public virtual void AppendItem<T>(string dataField, T data, string op, bool isExpression)
        {
            SqlClauseBuilderItemIUW item = new SqlClauseBuilderItemIUW();

            item.DataField = dataField;
            item.IsExpression = isExpression;
            item.Operation = op;
            item.Data = data;

            List.Add(item);
        }
    }
}
