using Lib.Cache;
using Lib.Core;
using Lib.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Transactions;

namespace Wanda.Lib.Data.AppBase
{



    public abstract class BaseAdapterT<T> : DataAdapterBase<T>, IBasicDataAccess<T>, IRetrievable<T>
        where T : BaseModel, new()
    {
     
        protected BaseAdapterT()
        {
        }



        /// <summary>
        /// 注意是标记删除+标记失效， 并非真的删除数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int Remove(T data)
        {
            data.IsDeleted = true;
            int result = 0;
            Dictionary<string, object> context = new Dictionary<string, object>();

            BeforeInnerRemove(data, context);

            using (TransactionScope scope = TransactionScopeFactory.Create())
            {
                result = InnerUpdate(data, null);

                AfterInnerRemove(data, context);

                scope.Complete();
            }

            return result;
        }

        protected virtual void BeforeInnerRemove(T data, Dictionary<string, object> context)
        {

        }
        protected virtual void AfterInnerRemove(T data, Dictionary<string, object> context)
        {

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public T GetModelByID(Guid ID)
        {
            WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();

            string idField = "ID"; //TODO , 通过ORM 反射找到对应的Field; 默认暂时认为都是使用ID字段

            //ORMapping.GetMappingInfo<T>()[]

            where.AppendItem(idField, ID);

            string sqlString = ORMapping.GetSelectSql<T>(TSqlBuilder.Instance)
                + " where "
                + where.ToSqlString(TSqlBuilder.Instance)
                + " AND " + NotDeleted;

            var listResult = this.ExecuteQuery(sqlString, null);


            return listResult.FirstOrDefault();
        }

        /// <summary>
        /// 根据多个ID返回结果（集合）
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        internal IList<T> GetBatchModelObjects(IEnumerable<Guid> ids)
        {

            string idsJoined = string.Format(" ID in  ({0}) ", string.Join(",", ids));


            string sqlString = ORMapping.GetSelectSql<T>(TSqlBuilder.Instance)
                + " where "
                + idsJoined
                + " AND " + NotDeleted;

            var listResult = this.ExecuteQuery(sqlString, null);


            return listResult;
        }

        /// <summary>
        /// 非删除状态数据
        /// </summary>
        protected readonly string NotDeleted = " ([ISDELETED]<1) ";


        // Retrieve
        IList<T> IRetrievable<T>.RetrieveAll()
        {
            WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();

            string isDeletedField = "ISDELETED"; //TODO , 通过ORM 反射找到对应的Field; 默认暂时认为都是使用ISDELETED字段

            where.AppendItem(isDeletedField, true);


            string sqlString = ORMapping.GetSelectSql<T>(TSqlBuilder.Instance)
                                 + " where " + where.ToSqlString(TSqlBuilder.Instance);


            return this.ExecuteQuery(sqlString, null);

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        /// <remarks>必须是返回已删除的数据（未删除的数据， 即使ID对了， 也找不到）</remarks>
        T IRetrievable<T>.RetrieveByID(string ID)
        {
            WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();

            string idField = "ID"; //TODO , 通过ORM 反射找到对应的Field; 默认暂时认为都是使用ID字段
            string isDeletedField = "ISDELETED"; //TODO , 通过ORM 反射找到对应的Field; 默认暂时认为都是使用ISDELETED字段

            where.AppendItem(idField, ID);
            where.AppendItem(isDeletedField, true);

            string sqlString = ORMapping.GetSelectSql<T>(TSqlBuilder.Instance)
                + " where "
                + where.ToSqlString(TSqlBuilder.Instance);

            var listResult = this.ExecuteQuery(sqlString, null);
            return (listResult == null) ? null : listResult.FirstOrDefault();

        }


        protected static readonly string sqlSeperator = "\r\n\r\n";

        protected static void ExceptionWhenUnmatch<T>(T model, Guid id, Func<T, Guid> getCheckProperty)
           where T : BaseModel
        {
            if (id != getCheckProperty(model))
            {
                throw new System.ArgumentException(string.Format("Parameters error: some {0} Objects are not mapped to the modelID: {1}", model.GetType().Name, id));
            }
        }


        /// <summary>
        /// 管理关联关系（先删除旧的， 再添加新的）
        /// self是指中心关系， 即不变的一方
        /// 如A与B 是一对多关系， 比如 Team和Member， 如果需要更新Team内的全部Member， 则可以执行这个方法
        /// </summary>
        /// <typeparam name="T">A类关系表类型， 如"MemberShip：Team-Member"</typeparam>
        /// <param name="majorID">主体唯一识别字段字段值， 如TeamID="1111-xxx-xxxx-xxxx"</param>
        /// <param name="majorFieldName">主体唯一识别字段名称， 如"TeamID"</param>
        /// <param name="items">多个主体-副体关系对象; 如果为空, 则相当于清除所有的旧关联</param>
        /// <param name="getCheckProperty">检查一致性，确保此次批量操作的是关联同一个对象。 和selfID比较核对的方法; 如果为null， 则跳过检查</param>
        /// <returns></returns>
        public int ManageAssociations<T>(Guid majorID, string majorFieldName, List<T> items, Func<T, Guid> getCheckProperty)
            where T : BaseModel
        {
            if (getCheckProperty != null && items != null)
            {
                items.ForEach(item => ExceptionWhenUnmatch(item, majorID, getCheckProperty));
            }

            StringBuilder sqlString = new StringBuilder();
            string sqlSeperator = "\r\n\r\n";

            //先删除旧关系， 
            string associationTableName = ORMapping.GetTableName(typeof(T));
            sqlString.AppendFormat("Update {0} set [ISDELETED]=1  WHERE [{1}]='{2}' and [ISDELETED]<1 ",
                associationTableName,
                majorFieldName,
                majorID);
            sqlString.Append(sqlSeperator);

            if (items == null || items.Count == 0)
            {
                // no insert!
            }
            else
            {

                foreach (T item in items)
                {
                    //item.ID = Guid.NewGuid().ToString();
                    item.IsDeleted = false;


                    sqlString.Append(ORMapping.GetInsertSql<T>(item, TSqlBuilder.Instance));
                    sqlString.Append(sqlSeperator);
                }
            }


            int result = DbHelper.RunSqlWithTransaction(sqlString.ToString(), ConnectionName);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="majorID"></param>
        /// <param name="t1FieldName"></param>
        /// <param name="t2FieldName"></param>
        /// <param name="t2MajorIDFieldName"></param>
        /// <param name="items"></param>
        /// <param name="getCheckProperty"></param>
        /// <returns></returns>
        public int ManageAssociations<T1, T2>(string majorID, string t1FieldName, string t2FieldName, string t2MajorIDFieldName, List<T1> items, Func<T1, string> getCheckProperty)
            where T1 : BaseModel
            where T2 : BaseModel
        {
            if (getCheckProperty != null)
            {
                //TODO , 待做检查 ，由于需要再关联一张表查询， 比较复杂， 暂时搁置 huwz
                //items.ForEach(item => ExceptionWhenUnmatch(item, majorID, getCheckProperty));
            }

            StringBuilder sqlString = new StringBuilder();
            string sqlSeperator = "\r\n\r\n";

            //先删除旧关系， 
            string associationTableName_A = ORMapping.GetTableName(typeof(T1));
            string mediaAssociationTableName_B = ORMapping.GetTableName(typeof(T2));
            sqlString.AppendFormat(@"
Update {0} set [ISDELETED]=1 
FROM {0} A INNER JOIN {1} B ON A.{2}=B.{3} 
WHERE B.[{4}]='{5}' and A.[ISDELETED]<1 and B.[ISDELETED]<1 ",
                associationTableName_A,
                mediaAssociationTableName_B,
                t1FieldName,
                t2FieldName,
                t2MajorIDFieldName,
                majorID);
            sqlString.Append(sqlSeperator);

            foreach (T1 item in items)
            {
                //item.ID = Guid.NewGuid().ToString();
                sqlString.Append(ORMapping.GetInsertSql<T1>(item, TSqlBuilder.Instance));
                sqlString.Append(sqlSeperator);
            }


            int result = DbHelper.RunSqlWithTransaction(sqlString.ToString(), ConnectionName);

            return result;
        }


        protected List<T> ExecuteSqlGet(string sql)
        {
            throw new NotImplementedException();
        }

    }




}
