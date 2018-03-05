using System.Collections.Generic;
using Wanda.Lib.Data.AppBase;
using Wanda.Lib.LightWorkflow.Entities;
using Lib.Data;
using System.Linq;
namespace Wanda.Lib.LightWorkflow.Dal
{
    internal sealed class ProcessNodeInstanceAdapter : LwfBaseAdapterT<ProcessNodeInstance>, IProcessNodeInstanceAdapter
    {

        private static ProcessNodeInstanceAdapter _instance = new ProcessNodeInstanceAdapter();
        public static ProcessNodeInstanceAdapter Instance { get { return _instance; } }

        public void Delete(int niid)
        {

            base.Delete(new ProcessNodeInstance { ID = niid });
        }

        public void DeleteList(int processInstanceId)
        {
            string tableName = ORMapping.GetTableName(typeof(ProcessNodeInstance));
            WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();

            where.AppendItem("ProcessInstanceID", processInstanceId);

            string sqlCommand = string.Format("delete from {0} where {1}", tableName,
                where.ToSqlString(TSqlBuilder.Instance));

            ExecuteQuery(sqlCommand);

        }


        public ProcessNodeInstance Load(int niid)
        {
            return base.GetModelByID(niid);
        }

        public PartlyCollection<ProcessNodeInstance> LoadList(int processInstanceId)
        {
            string tableName = ORMapping.GetTableName(typeof(ProcessNodeInstance));
            WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
            where.AppendItem("ProcessInstanceID", processInstanceId);
            //string sqlCommand = string.Format("select * from {0} where {1} AND {2} ORDER BY NodeSeq ASC", tableName,
            //    where.ToSqlString(TSqlBuilder.Instance), NotDeleted);
            string sqlCommand = string.Format("SELECT P.*,JobTitle FROM {0} P INNER JOIN PB_UserInfo U ON p.UserID=U.ID  where {1} AND  p.IsDeleted=0 AND U.IsDeleted=0  ORDER BY NodeSeq ASC", tableName,
               where.ToSqlString(TSqlBuilder.Instance));
            var table = ExecuteReturnTable(sqlCommand);
            List<ProcessNodeInstance> NodeList = ExecuteQuery(sqlCommand);
            for (int i = 0; i < table.Rows.Count; i++)
            {
                NodeList.ForEach(p =>
                {
                    if (p.ID == int.Parse(table.Rows[i]["ID"].ToString()))
                    {
                        p.JobTitle = table.Rows[i]["JobTitle"].ToString();
                    }
                });
            }
            return (PartlyCollection<ProcessNodeInstance>)NodeList;
        }

        public List<ProcessNodeInstance> LoadListByBizID(int bizProcessID)
        {
            string tableName = ORMapping.GetTableName(typeof(ProcessNodeInstance));
            WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();

            where.AppendItem("BizProcessID", bizProcessID);

            string sqlCommand = string.Format("select * from {0} where {1} AND {2} ORDER BY NodeSeq ASC", tableName,
                where.ToSqlString(TSqlBuilder.Instance), NotDeleted);
            return ExecuteQuery(sqlCommand);
        }
    }
}

