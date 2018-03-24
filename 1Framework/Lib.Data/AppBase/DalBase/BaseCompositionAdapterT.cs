using Lib.Core;
using Lib.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace LJTH.Lib.Data.AppBase
{
    /// <summary>
    /// 视图（实体组合）对象的数据库访问适配器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// 通常来讲composited的视图对象， 不再需要具体的Adapter ， 因为仅需要查询
    /// </remarks>
    public abstract class BaseCompositionAdapterT<T> : DataAdapterBase<T>
         where T : IBaseComposedModel, new()
    {
        protected string SelectAllString
        {
            get
            {
                return ORMapping.GetSelectSql<T>(TSqlBuilder.Instance);
            }
        }

        /// <summary>
        /// 返回查询结果集合
        /// </summary>
        /// <param name="filter">带分页属性的过滤条件对象</param>
        /// <param name="sortField">排序字段</param>
        /// <returns>返回查询结果集合</returns>
        public PartlyCollection<T> GetList(PagenationDataFilter filter, string sortField="ID")
        {
            WhereSqlClauseBuilder where = filter.ConvertToWhereBuilder();
            int rowIndex = filter.RowIndex;
            int pageSize = filter.PageSize;

            if (string.IsNullOrEmpty(sortField))
            {
                sortField = "ID";
            }

            return GetList(where, rowIndex, pageSize, sortField);
        }

        /// <summary>
        /// 按where 条件
        /// </summary>
        /// <param name="where"></param>
        /// <param name="sortField">排序字段</param>
        /// <returns></returns>
        public PartlyCollection<T> GetList(WhereSqlClauseBuilder where, string sortField = "ID")
        {
            return GetList(where, 0, 0, sortField);
        }



        /// <summary>
        /// 按where 条件 且返回分页
        /// </summary>
        /// <param name="where"></param>
        /// <param name="rowIndex">行序号， 基于0</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="sortField">排序字段</param>
        /// <returns></returns>
        public PartlyCollection<T> GetList(WhereSqlClauseBuilder where, int rowIndex, int pageSize, string sortField = "ID")
        {
            if (where==null)
            {
                throw new ArgumentNullException("where");
            }
            if (string.IsNullOrEmpty(sortField))
            {
                sortField = "ID";
            }
            QueryCondition qc = new QueryCondition(
                rowIndex,
                pageSize,
                " * ",
               ORMapping.GetTableName(typeof(T)),
               SqlTextHelper.SafeQuote(sortField),
               where.ToSqlString(TSqlBuilder.Instance)
              );

            PartlyCollection<T> result = GetPageSplitedCollection(qc);
            return result;
        }

        public override int Insert(T data)
        {
            throw new NotSupportedException();
        }

        public override int Update(T data)
        {
            throw new NotSupportedException();
        }

        public override int Delete(T data)
        {
            throw new NotSupportedException();
        }

    }

}

