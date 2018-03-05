using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib.Core;
using System.Data;
using System.IO;
using System.Transactions;
using System.Data.Common;
using System.Data.SqlClient;


namespace Lib.Data
{
    public delegate string ComposeSqlString<T>(T data);

    public abstract class DataAdapterBase
    {
        protected abstract string ConnectionName { get; }

        /// <summary>
        /// 分页查询结构
        /// </summary>
        /// <remarks>
        /// 0 ： select 各字段
        /// 1 ： 数据表 或 数据视图
        /// 2 ： Where 子句条件
        /// 3 ： 结果排序的字段（唯一）
        /// 4 ： 分页开始行
        /// 5 ： 分页结束行
        /// </remarks>
        protected virtual string PageQueryString
        {
            get
            {
                return @"WITH TempQuery AS
									(
										SELECT {0}, ROW_NUMBER() OVER (ORDER BY {3}) AS 'RowNumberForSplit'
										FROM {1}
										WHERE 1 = 1 {2}
									)
									SELECT * 
									FROM TempQuery 
									WHERE RowNumberForSplit BETWEEN {4} AND {5};";
            }
        }

        protected DataSet SplitPageQuery(QueryCondition qc, bool retrieveTotalCount)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(null == qc, "qc");
            ExceptionHelper.CheckStringIsNullOrEmpty(qc.SelectFields, "qc.SelectFields");
            ExceptionHelper.CheckStringIsNullOrEmpty(qc.FromClause, "qc.FromClause");
            ExceptionHelper.CheckStringIsNullOrEmpty(qc.OrderByClause, "qc.OrderByClause");

            DataSet ds = null;

            if (qc.RowIndex == 0 && qc.PageSize == 0)	//一种假设，qc.RowIndex == 0 && qc.PageSize == 0认为不分页
                ds = DoNoSplitPageQuery(qc);
            else
                ds = DoSplitPageQuery(qc, retrieveTotalCount);

            return ds;
        }

        private DataSet DoNoSplitPageQuery(QueryCondition qc)
        {
            string sql = string.Format("SELECT {0} FROM {1} WHERE 1 = 1 {2} ORDER BY {3}",
                        qc.SelectFields,
                        qc.FromClause,
                        string.IsNullOrEmpty(qc.WhereClause) ? string.Empty : " AND " + qc.WhereClause,
                        qc.OrderByClause);

            using (DbContext context = DbContext.GetContext(this.ConnectionName))
            {
                Database db = DatabaseFactory.Create(context);

                DataSet ds = db.ExecuteDataSet(CommandType.Text, sql, "RESULT");

                DataTable table = new DataTable("RESULT_COUNT");

                table.Columns.Add("TOTAL_COUNT", typeof(int));
                table.Rows.Add(ds.Tables[0].Rows.Count);

                ds.Tables.Add(table);

                return ds;
            }
        }

        private DataSet DoSplitPageQuery(QueryCondition qc, bool retrieveTotalCount)
        {
            string query = string.Format(
                this.PageQueryString,
                qc.SelectFields,
                qc.FromClause,
                string.IsNullOrEmpty(qc.WhereClause) ? string.Empty : " AND " + qc.WhereClause,
                qc.OrderByClause,
                qc.RowIndex + 1,
                qc.RowIndex + qc.PageSize);

            if (retrieveTotalCount)
                query += TSqlBuilder.Instance.DBStatementSeperator + GetTotalCountSql(qc);

            using (DbContext context = DbContext.GetContext(this.ConnectionName))
            {
                Database db = DatabaseFactory.Create(context);
                //根据SQL Server版本选择分页语句的写法

                return db.ExecuteDataSet(CommandType.Text, query, "RESULT", "RESULT_COUNT");
            }
        }


        private DataSet DoSplitPageQuery(QueryCondition qc, DbParameter[] dbParams, bool retrieveTotalCount)
        {
            string query = string.Format(
                this.PageQueryString,
                qc.SelectFields,
                qc.FromClause,
                string.IsNullOrEmpty(qc.WhereClause) ? string.Empty : " AND " + qc.WhereClause,
                qc.OrderByClause,
                qc.RowIndex + 1,
                qc.RowIndex + qc.PageSize);

            if (retrieveTotalCount)
                query += TSqlBuilder.Instance.DBStatementSeperator + GetTotalCountSql(qc);

            using (DbContext context = DbContext.GetContext(this.ConnectionName))
            {
                Database db = DatabaseFactory.Create(context);
                //根据SQL Server版本选择分页语句的写法

                return db.ExecuteDataSet(CommandType.Text, query, "RESULT", "RESULT_COUNT");
            }
        }

        private string GetTotalCountSql(QueryCondition qc)
        {
            return string.Format("SELECT COUNT(*) AS TOTAL_COUNT FROM {0} WHERE 1 = 1 {1}",
                        qc.FromClause,
                        string.IsNullOrEmpty(qc.WhereClause) ? string.Empty : " AND " + qc.WhereClause);
        }



        protected virtual DataTableCollection ExecuteReturnTableCollection(string query, params DbParameter[] parameters)
        {
            DataSet ds = null;
            if (parameters == null)
                ds = DbHelper.RunSqlReturnDS(query, ConnectionName);
            else
                ds = DbHelper.RunSqlReturnDS(query, ConnectionName, parameters);

            if (ValidateDataSet(ds))
                return ds.Tables;
            return null;
        }

        protected virtual DataTable ExecuteReturnTable(string query, params DbParameter[] parameters)
        {
            DataSet ds = ExecuteReturnDataSet(query, parameters);
            if (ds == null)
                return null;
            return ds.Tables[0];
        }

        protected virtual DataSet ExecuteReturnDataSet(string query, params DbParameter[] parameters)
        {
            LogSql(query);

            try
            {
                DataSet ds = null;
                if (parameters == null)
                    ds = DbHelper.RunSqlReturnDS(query, ConnectionName);
                else
                    ds = DbHelper.RunSqlReturnDS(query, ConnectionName, parameters);
                if (ValidateDataSet(ds))
                    return ds;
            }
            catch (Exception ex)
            {
                LogSql(query, ex);
                throw;
            }

            return null;
        }

        protected virtual int ExecuteSql(string query)
        {
            return DbHelper.RunSql(query, ConnectionName);
        }

        protected virtual int ExecuteSql(string query, params DbParameter[] parameters)
        {
            return DbHelper.RunSql(query, ConnectionName, parameters);
        }

        private void LogSql(string sql, Exception ex = null)
        {
            //if (ex == null)
            //{
            //    Wanda.HR.Common.LogHelper.DevLogger.Write(
            //       new Lib.Log.LogEntity("sql", sql, 0)
            //        );

            //}
            //else
            //{
            //    Wanda.HR.Common.LogHelper.DevLogger.Write(new Lib.Log.LogEntity(ex) { Message = sql });
            //}
        }

        protected SqlParameter CreateSqlParameter(string paramName, DbType paramType, object paramValue)
        {
            return new SqlParameter() { ParameterName = paramName, DbType = paramType, Value = paramValue };
        }

        private bool ValidateDataSet(DataSet ds)
        {
            if (ds.Tables == null)
                return false;
            if (ds.Tables.Count == 0)
                return false;
            if (ds.Tables[0].Rows == null)
                return false;
            if (ds.Tables[0].Rows.Count == 0)
                return false;

            return true;
        }


    }
    ///<summary>
    ///带数据更新功能的DataAdapter的基类
    ///</summary>
    ///<typeparam name="T"></typeparam>
    public abstract class DataAdapterBase<T> : DataAdapterBase where T : new()
    {



        #region Insert

        public virtual int Insert(T data)
        {
            return Insert(data, null);
        }

        public virtual int Insert(T data, ComposeSqlString<T> action)
        {
            if (data == null)
                throw new Exception("data 不能为空");

            Dictionary<string, object> context = new Dictionary<string, object>();

            int result = 0;

            BeforeInnerInsert(data, context);

            using (TransactionScope scope = TransactionScopeFactory.Create())
            {
                result = InnerInsert(data, action);

                AfterInnerInsert(data, context);

                scope.Complete();
            }

            return result;
        }

        protected virtual void BeforeInnerInsert(T data, Dictionary<string, object> context)
        {

        }

        protected virtual int InnerInsert(T data, ComposeSqlString<T> action)
        {
            string sql = GetSqlString(data, action, ORMapping.GetInsertSql(data, TSqlBuilder.Instance));

            return  DbHelper.RunSql(sql, this.ConnectionName);

            //return int.Parse(result.ToString());
        }

        protected virtual void AfterInnerInsert(object data, Dictionary<string, object> context)
        {

        }

        #endregion

        #region Update

        public virtual int Update(T data)
        {
            return Update(data, null);
        }

        public virtual int Update(T data, ComposeSqlString<T> action)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(data != null, "data");

            int result = 0;

            Dictionary<string, object> context = new Dictionary<string, object>();

            BeforeInnerUpdate(data, context);

            using (TransactionScope scope = TransactionScopeFactory.Create())
            {
                result = InnerUpdate(data, action);

                AfterInnerUpdate(data, context);

                scope.Complete();
            }

            return result;
        }

        protected virtual void BeforeInnerUpdate(T data, Dictionary<string, object> context)
        {

        }

        protected virtual int InnerUpdate(T data, ComposeSqlString<T> action)
        {
            string sql = GetSqlString(data, action, ORMapping.GetUpdateSql(data, TSqlBuilder.Instance));

            return DbHelper.RunSql(sql, this.ConnectionName);
        }

        protected virtual void AfterInnerUpdate(T data, Dictionary<string, object> context)
        {

        }

        #endregion

        #region Delete

        public virtual int Delete(T data)
        {
            return Delete(data, null);
        }

        public virtual int Delete(T data, ComposeSqlString<T> action)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(data != null, "data");

            int result = 0;

            Dictionary<string, object> context = new Dictionary<string, object>();

            BeforeInnerDelete(data, context);

            using (TransactionScope scope = TransactionScopeFactory.Create())
            {
                result = InnerDelete(data, action);

                AfterInnerDelete(data, context);

                scope.Complete();
            }

            return result;
        }

        protected virtual void BeforeInnerDelete(T data, Dictionary<string, object> context)
        {

        }

        protected virtual int InnerDelete(T data, ComposeSqlString<T> action)
        {
            string sql = GetSqlString(data, action, ORMapping.GetDeleteSql(data, TSqlBuilder.Instance));

            return DbHelper.RunSql(sql, this.ConnectionName);
        }

        protected virtual void AfterInnerDelete(T data, Dictionary<string, object> context)
        {

        }

        #endregion

        #region ModelMapping

        protected virtual T DataModelMapping(DataRow dataSource, T data)
        {
            T result = data;

            ORMapping.DataRowToObject(dataSource, result);

            return result;

        }

        #endregion

        #region 李刚于2013/4/28添加
        protected List<T> Load(Action<WhereSqlClauseBuilder> whereAction)
        {
            return Load(whereAction, null);
        }

        protected List<T> Load(Action<WhereSqlClauseBuilder> whereAction, Action<OrderBySqlClauseBuilder> orderByAction)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(whereAction != null, "whereAction");

            WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();
            whereAction(builder);
            OrderBySqlClauseBuilder orderByBuilder = new OrderBySqlClauseBuilder();
            if (orderByAction != null)
            {
                orderByAction(orderByBuilder);
            }
            return InnerLoadByBuilder(builder.ToSqlString(TSqlBuilder.Instance),
                orderByBuilder.ToSqlString(TSqlBuilder.Instance));
        }

        protected List<T> InnerLoadByBuilder(string condition, string orderBy)
        {
            ORMappingItemCollection mappings = ORMapping.GetMappingInfo<T>();
            string sql = string.Format("SELECT * FROM {0}", mappings.TableName);
            if (string.IsNullOrEmpty(condition) == false)
                sql = sql + string.Format(" WHERE {0}", condition);
            if (string.IsNullOrEmpty(orderBy) == false)
                sql = sql + string.Format(" ORDER BY {0}", orderBy);

            return ExecuteQuery(sql);
        }
        #endregion


        private string GetSqlString(T data, ComposeSqlString<T> action, string defaultString)
        {
            string sql = string.Empty;

            if (action != null)
                sql = action(data);

            if (string.IsNullOrEmpty(sql))
                sql = defaultString;

            return sql;
        }

        protected virtual List<T> ExecuteQuery(string query, params DbParameter[] parameters)
        {

            DataTable table = ExecuteReturnTable(query, parameters);
            if (table == null)
                return new List<T>(); //返回Null集合， 在后面处理实在太别扭了 huweizheng 

            return DataModelMapping(table);
        }

        protected virtual List<T> DataModelMapping(DataTable dataTable)
        {
            return DataModelMapping(dataTable, 1);
        }

        protected virtual List<T> DataModelMapping(DataTable dataTable, int startIndex)
        {
            List<T> list = new List<T>(); 
            int index = startIndex;

            foreach (DataRow row in dataTable.Rows)
            {
                T obj = new T();
                ORMapping.DataRowToObject(row, obj);


                list.Add(obj);
            }
            return list;
        }

        ///// <summary>
        ///// 将返回的Datatable结果集转换为List object集合
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="dataTable"></param>
        ///// <returns></returns>
        //protected List<T> DataModelMapping<T>(DataTable dataTable)
        //where T : new()
        //{
        //    return DataModelMappingA<T>(dataTable, 1);
        //}

        //protected List<T> DataModelMapping<T>(DataTable dataTable, int startIndex)
        //    where T : new()
        //{
        //    List<T> list = new List<T>();
        //    int index = startIndex;

        //    foreach (DataRow row in dataTable.Rows)
        //    {
        //        T obj = new T();
        //        ORMapping.DataRowToObject(row, obj);

        //        list.Add(obj);
        //    }
        //    return list;
        //}

        protected PartlyCollection<T> DataModelMapping(DataSet dataSet)
        {
            ExceptionHelper.FalseThrow(dataSet != null, "dataset 不能为空");
            ExceptionHelper.FalseThrow(dataSet.Tables.Count == 2, "dataset table数目错误");

            // table[0] , return collection
            // table[1] , return count
            List<T> collection = DataModelMapping(dataSet.Tables[0], 1);
            int count = (dataSet.Tables[1].Rows.Count > 0 ?
                            (dataSet.Tables[1].Rows.Count == 1 ? int.Parse(dataSet.Tables[1].Rows[0][0].ToString()) : dataSet.Tables[1].Rows.Count)
                            : 0);

            return PartlyCollection<T>.Create(collection, count);


        }



        protected PartlyCollection<T> GetPageSplitedCollection(QueryCondition qc)
        {
            DataSet ds = SplitPageQuery(qc, true);

            if (ds == null || ds.Tables.Count < 2)
            {
                throw new ArgumentException(
                    string.Format("Cannot find '{0}' object", typeof(T)));
            }

            PartlyCollection<T> result = DataModelMapping(ds);
            return result;
        }


        /// <summary>
        /// 返回分页的数据
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        protected PartlyCollection<T> GetPageSplitedCollection(int rowIndex, int pageSize)
        {
            return GetPageSplitedCollection(rowIndex, pageSize, string.Empty);
        }

        protected PartlyCollection<T> GetPageSplitedCollection(int rowIndex, int pageSize, Action<WhereSqlClauseBuilder> buildWhere)
        {
            WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();
            buildWhere(builder);
            return GetPageSplitedCollection(rowIndex, pageSize, builder.ToSqlString(TSqlBuilder.Instance));
        }
        /// <summary>
        /// 原则上， 和数据库操作相关的都不要公开
        /// where 条件的写法，暴露了太多数据表的细节， 所以此方法不公开
        /// 只能在DAL层中使用
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="whereClause"></param>
        /// <returns></returns>
        protected PartlyCollection<T> GetPageSplitedCollection(int rowIndex, int pageSize, string whereClause)
        {

            string modelTable = ORMapping.GetTableName(typeof(T));
            string selectFields = ORMapping.GetSelectFields(typeof(T), TSqlBuilder.Instance);
            string createTimeField = "CreateTime desc"; //默认按时间倒序排列
            QueryCondition qc = new QueryCondition(rowIndex, pageSize, selectFields, modelTable, createTimeField);

            // 包含IsDelete判断
            qc.WhereClause = (string.IsNullOrEmpty(whereClause) ? "" : (" " + whereClause));

            return GetPageSplitedCollection(qc);
        }



    }
}
