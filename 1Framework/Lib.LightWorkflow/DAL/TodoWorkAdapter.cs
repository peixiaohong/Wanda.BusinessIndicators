using System;
using System.Text;
using System.Collections.Generic;
using Wanda.Lib.Data.AppBase;
using Wanda.Lib.LightWorkflow.Entities;
using Lib.Data;
using Wanda.Lib.LightWorkflow.Filter;


namespace Wanda.Lib.LightWorkflow.Dal
{
    internal sealed class TodoWorkAdapter : LwfBaseAdapterT<TodoWork>, ITodoWorkAdapter
    {
        private static TodoWorkAdapter _instance = new TodoWorkAdapter();
        public static TodoWorkAdapter Instance { get { return _instance; } }

        public void Delete(int todoWorkID)
        {
            base.Delete(new TodoWork { ID = todoWorkID });
        }

        public void DeleteAll(int piid)
        {
            string tableName = ORMapping.GetTableName(typeof(TodoWork));
            WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();

            where.AppendItem("ProcessInstanceID", piid);

            string sqlCommand = string.Format("delete from {0} where {1}", tableName,
                where.ToSqlString(TSqlBuilder.Instance));

            ExecuteQuery(sqlCommand);
        }


        public TodoWork Load(int piid, int userID)
        {
            string tableName = ORMapping.GetTableName(typeof(TodoWork));
            WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();

            where.AppendItem("ProcessInstanceID", piid);
            where.AppendItem("userID", userID);

            string sqlCommand = string.Format("select top 1 * from {0} where {1} AND {2}", tableName,
                where.ToSqlString(TSqlBuilder.Instance), NotDeleted);
            List<TodoWork> result = ExecuteQuery(sqlCommand);
            return result.Count > 0 ? result[0] : null;
        }
        public List<TodoWork> LoadListByProcessInstanceID(int processInstanceID)
        {
            string tableName = ORMapping.GetTableName(typeof(TodoWork));
            WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();

            where.AppendItem("ProcessInstanceID", processInstanceID);

            string sqlCommand = string.Format("select * from {0} where {1} AND {2}", tableName,
                where.ToSqlString(TSqlBuilder.Instance), NotDeleted);
            return ExecuteQuery(sqlCommand);
        }

        public TodoWork Load(int todoWorkID)
        {
            return GetModelByID(todoWorkID);
        }

        //add czq 2013-06-17
        internal PartlyCollection<TodoWork> LoadList(TodoWorkFilter filter)
        {
            WhereSqlClauseBuilder where = filter.ConvertToWhereBuilder();
            where.AppendItem("Isdeleted", 0);
            QueryCondition qc = new QueryCondition(
                  filter.RowIndex,
                  filter.PageSize,
                  " * ",
                 ORMapping.GetTableName(typeof(TodoWork)),
                  " Createtime desc",
                 where.ToSqlString(TSqlBuilder.Instance)
                );

            PartlyCollection<TodoWork> result = GetPageSplitedCollection(qc);
            return result;
        }

        internal string GetLoginNameByUid(int userID)
        {
            WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
            where.AppendItem("ID", userID);

            string sqlCommand = string.Format("select top 1 LoginName AS UserName from PB_Userinfo where {0} AND IsDeleted<>1",
                where.ToSqlString(TSqlBuilder.Instance));
            return DbHelper.RunSqlReturnScalar(sqlCommand, this.ConnectionName).ToString();
        }
    }
}