using System.Collections.Generic;
using Wanda.Lib.Data.AppBase;
using Wanda.Lib.LightWorkflow.Entities;
using Lib.Data;
using System.Data;


namespace Wanda.Lib.LightWorkflow.Dal
{
    internal sealed class ProcessNodeAdapter : LwfBaseAdapterT<ProcessNode>, IProcessNodeAdapter
    {

        private static ProcessNodeAdapter _instance = new ProcessNodeAdapter();
        public static ProcessNodeAdapter Instance { get { return _instance; } }

        public void Delete(int nid)
        {
            base.Delete(new ProcessNode() { ID = nid });

        }

        public ProcessNode Load(int nid)
        {
            return base.GetModelByID(nid);
        }

        public void RemoveByProcessID(int processID)
        {
            string ProcessNodeTableName = ORMapping.GetTableName(typeof(ProcessNode));
            WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
            where.AppendItem("processID", processID);
            string sqlCommand = string.Format("delete FROM {0} WHERE {1}",
                       ProcessNodeTableName,
                       where.ToSqlString(TSqlBuilder.Instance));
            ExecuteQuery(sqlCommand);
        }

        public List<ProcessNode> LoadList(int processID, string processType)
        {
            string ProcessNodeTableName = ORMapping.GetTableName(typeof(ProcessNode));
            WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
            where.AppendItem("processID", processID);
            where.AppendItem("processType", processType);
            string sqlCommand = string.Format("SELECT * FROM {0} WHERE {1} AND {2}  ORDER BY NodeSeq ASC",
                       ProcessNodeTableName,
                       where.ToSqlString(TSqlBuilder.Instance),NotDeleted);
            return ExecuteQuery(sqlCommand);
        }

        public List<ProcessNode> LoadList(int processID)
        {
            string ProcessNodeTableName = ORMapping.GetTableName(typeof(ProcessNode));
            WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
            where.AppendItem("processID", processID);
            string sqlCommand = string.Format("SELECT * FROM {0} WHERE {1}  ORDER BY NodeSeq ASC",
                       ProcessNodeTableName,
                       where.ToSqlString(TSqlBuilder.Instance), NotDeleted);
            return ExecuteQuery(sqlCommand);
        }
        public int UpdateNodeSeq(int nodeID,int seq)
        {
            string ProcessNodeTableName = ORMapping.GetTableName(typeof(ProcessNode));
            WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
            where.AppendItem("ID", nodeID);
            string sqlCommand = string.Format("Update {0} Set NodeSeq={1} WHERE {2} AND {3}",
                       ProcessNodeTableName,seq,
                       where.ToSqlString(TSqlBuilder.Instance), NotDeleted);
            return ExecuteSql(sqlCommand);
        }

        public DataTable GetProcessNodeListWithDataAuthorization(string ProcessCode, int congID)
        {
            string SQL = string.Format(@"SELECT  distinct
                P.ID AS ProcessID,
                P.ProcessCode,
                P.ProcessName,
                PN.ID AS NodeID,
                PN.NodeName,
                PN.NodeSeq,
                PN.NodeType,
                PN.RoleID,
                R.Name AS RoleName,
                UR.UserID,U.WD_UserID,
                U.LoginName AS LoginName,
                U.Name AS UserName
                FROM [LWF_Process] P WITH(NOLOCK)
                INNER JOIN LWF_ProcessNode PN
                ON PN.ProcessID=P.ID
                INNER JOIN PB_ROLEINFO R WITH(NOLOCK)
                ON R.ID=PN.RoleID
                LEFT JOIN PA_UserRole UR WITH(NOLOCK)
                ON UR.RoleID=R.ID and UR.IsDeleted<1
                LEFT JOIN PB_Userinfo U WITH(NOLOCK)
                ON UR.UserID=U.ID and U.IsDeleted<1
                WHERE ProcessCode='{0}' and R.ScopeID={1} and P.CongID={1} and  P.IsDeleted<1 and PN.IsDeleted<1 
                ORDER BY PN.NodeSeq
                ", SqlTextHelper.SafeQuote(ProcessCode), congID);

            return ExecuteReturnTable(SQL);
        }
      
    }
}

