using System;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using Wanda.Lib.Data.AppBase;
using Wanda.Lib.LightWorkflow.Entities;
using Lib.Data;
using Wanda.Lib.LightWorkflow.Interface;


namespace Wanda.Lib.LightWorkflow.Dal
{
    internal sealed class ApprovalLogAdapter : LwfBaseAdapterT<ApprovalLog>, IApprovalLogAdapter
    {
        private static ApprovalLogAdapter _instance = new ApprovalLogAdapter();
        public static ApprovalLogAdapter Instance { get { return _instance; } }


        public void Delete(int apid)
        {
            base.Delete(new ApprovalLog() { ID=apid });
        }

       

        public ApprovalLog Load(int apid)
        {
            return base.GetModelByID(apid);
        }



        public PartlyCollection<ApprovalLog> LoadList(int processInstanceId)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append( ORMapping.GetSelectSql(typeof(ApprovalLog), TSqlBuilder.Instance));

            WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
            where.AppendItem("ProcessInstanceID", processInstanceId);

            sql.AppendFormat(" where {0} AND {1}", where.ToSqlString(TSqlBuilder.Instance),NotDeleted);

            sql.Append(" ORDER BY CompletedTime desc");
            return (PartlyCollection<ApprovalLog>)ExecuteQuery(sql.ToString());
        }



        internal int RemoveApprovalLog(int processInstanceId)
        {
            string tableName = ORMapping.GetTableName(typeof(ApprovalLog));
            string sqlCommand = string.Format("UPDATE {0} SET ISDELETED=1 WHERE ProcessInstanceID={1} AND {2}",
                    tableName, processInstanceId, NotDeleted);
            return ExecuteSql(sqlCommand);
        }
    }
}

