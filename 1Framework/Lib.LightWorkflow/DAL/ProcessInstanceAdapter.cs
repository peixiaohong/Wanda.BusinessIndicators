using System;
using System.Text;
using System.Collections.Generic;
using Wanda.Lib.Data.AppBase;
using Wanda.Lib.LightWorkflow.Entities;
using Lib.Data;

namespace Wanda.Lib.LightWorkflow.Dal
{
    internal sealed class ProcessInstanceAdapter : LwfBaseAdapterT<ProcessInstance>, IProcessInstanceAdapter
    {
        private static ProcessInstanceAdapter _instance = new ProcessInstanceAdapter();
        public static ProcessInstanceAdapter Instance { get { return _instance; } }

        public void Delete(int piid)
        {
            base.Delete(new ProcessInstance { ID = piid });
        }


        public ProcessInstance Load(int piid)
        {
            return base.GetModelByID(piid);
        }

        public ProcessInstance LoadByBizProcessID(int bizProcessID)
        {
            WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
            where.AppendItem("BizProcessID", bizProcessID);

            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("SELECT  * FROM {0} WHERE {1} AND {2}  ",
                ORMapping.GetTableName(typeof(ProcessInstance)),
                where.ToSqlString(TSqlBuilder.Instance), NotDeleted
                );
            List<ProcessInstance> List = ExecuteQuery(sql.ToString());
            return List == null || List.Count == 0 ? null : List[0]; //返回第一个
        }

        public List<ProcessInstance> LoadList(List<string> bizProcessIDList)
        {
            string bizProcessIDs = string.Format(" BizProcessID in ('{0}')", string.Join("','", bizProcessIDList.ToArray()));

            //WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
            //where.AppendItem("BizProcessID", bizProcessIDs, "in");

            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("SELECT  * FROM {0} WHERE {1} AND {2} ORDER BY LastUpdatedTime DESC ",
                ORMapping.GetTableName(typeof(ProcessInstance)),
                bizProcessIDs, NotDeleted
                );

            return ExecuteQuery(sql.ToString());
        }

        public List<ProcessInstance> LoadList(CommonConsts.ProcessInstanceStatus? status, string userID, string projectid, string processCode)
        {
            WhereSqlClauseBuilder wc = new WhereSqlClauseBuilder();

            string ProcessInstanceTableName = ORMapping.GetTableName(typeof(ProcessInstance));
            string StakeHolderTableName = ORMapping.GetTableName(typeof(StakeHolder));

            if (status != null)
            {
                wc.AppendItem("status", (int)status);
            }
            if (processCode != "")
            {
                wc.AppendItem("ProcessCode", processCode);
            }
            //if (string.IsNullOrEmpty(projectID) == false)
            //{
            //    //wc.AppendItem("ProjectID", projectID);
            //}
            string strWhere = wc.ToSqlString(TSqlBuilder.Instance).Trim();

            string sqlCommand = null;
            if (string.IsNullOrEmpty(userID))
            {
                if (strWhere == "")
                {
                    sqlCommand = string.Format("SELECT * FROM {0} WHERE {1} ORDER BY LastUpdatedTime DESC",
                        ProcessInstanceTableName, NotDeleted);
                }
                else
                {
                    sqlCommand = string.Format("SELECT * FROM {0} WHERE {1} AND {2} ORDER BY LastUpdatedTime DESC",
                            ProcessInstanceTableName,
                    strWhere, NotDeleted);
                }
            }
            else
            {
                if (strWhere != "")
                {
                    strWhere += " AND ";
                }
                strWhere += string.Format("{0}.UserID = {1}", StakeHolderTableName, userID);
                //从StakeHolder中关联参与过的用户
                sqlCommand = string.Format("SELECT {0}.* FROM {0}, {1} WHERE {2} AND {1}.ProcessInstanceID = {0}.ID AND {1}.IsDeleted<>1 and {0}.IsDeleted<>1 ORDER BY LastUpdatedTime DESC",
                        ProcessInstanceTableName,
                        StakeHolderTableName,
                        strWhere);
            }
            return ExecuteQuery(sqlCommand);
        }

        public List<ProcessInstance> LoadListByCreateUserID(string userID, Dictionary<string, string> whereString)
        {
            WhereSqlClauseBuilder wc = new WhereSqlClauseBuilder();
            string ProcessInstanceTableName = ORMapping.GetTableName(typeof(ProcessInstance));
            if (!string.IsNullOrEmpty(userID))
            {
                wc.AppendItem("UserID", userID);
            }

            string strWhere = wc.ToSqlString(TSqlBuilder.Instance).Trim();
            string sqlCommand = "";

            if (strWhere == "")
            {
                sqlCommand = string.Format("SELECT * FROM {0} WHERE {1} ORDER BY LastUpdatedTime DESC",
                        ProcessInstanceTableName, NotDeleted);
            }
            else
            {
                #region Where语句
                StringBuilder burWhere = new StringBuilder();

                if (whereString.Count > 0)
                {
                    if (whereString["InstanceName"] != string.Empty)//标题
                    {
                        burWhere.AppendFormat(" AND {1}.InstanceName like '%{0}%'", WhereSqlClauseBuilder.EscapeLikeString(whereString["InstanceName"]), ProcessInstanceTableName);
                    }

                    if (whereString["CreatedTime"] != string.Empty)//创建时间
                    {
                        //创建时间开始
                        burWhere.AppendFormat(" AND  {0}.CreatedTime>= Convert(NVARCHAR(10),'{1}',120) ", ProcessInstanceTableName, whereString["CreatedTime"]);
                        //创建时间结束
                        burWhere.AppendFormat(" AND  {0}.CreatedTime< Convert(NVARCHAR(10),dateadd(d,1,'{1}'),120)", ProcessInstanceTableName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    if (whereString["LastUpdatedTime"] != string.Empty)//最后更新时间
                    {
                        //最后更新时间开始
                        burWhere.AppendFormat(" AND  {0}.LastUpdatedTime>= Convert(NVARCHAR(10),'{1}',120) ", ProcessInstanceTableName, whereString["LastUpdatedTime"]);
                        //最后更新时间结束
                        burWhere.AppendFormat(" AND  {0}.LastUpdatedTime< Convert(NVARCHAR(10),dateadd(d,1,'{1}'),120)", ProcessInstanceTableName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    if (whereString["ProcessCode"] != string.Empty)//流程类型
                    {
                        burWhere.AppendFormat(" AND {1}.ProcessCode='{0}'", whereString["ProcessCode"], ProcessInstanceTableName);
                    }

                    if (whereString["Status"] != string.Empty)//状态
                    {
                        burWhere.AppendFormat(" AND {1}.Status='{0}'", whereString["Status"], ProcessInstanceTableName);
                    }
                }

                #endregion

                sqlCommand = string.Format("SELECT * FROM {0} WHERE {1} {2} AND {3} ORDER BY LastUpdatedTime DESC",
                            ORMapping.GetTableName(typeof(ProcessInstance)),
                            wc.ToSqlString(TSqlBuilder.Instance),
                            burWhere, NotDeleted);

            }
            return ExecuteQuery(sqlCommand);
        }

        public List<ProcessInstance> LoadListByRelatedUserID(string userID, Dictionary<string, string> whereString)
        {
            //string sql = string.Format("SELECT {0}.* FROM {0}, {1} WHERE {1}.UserID = {2} AND {1}.ProcessInstanceID = {0}.ProcessInstanceID " +
            //    "AND {0}.ProcessInstanceID NOT IN (SELECT ProcessInstanceID FROM {3} WHERE UserID = {2}) ORDER BY LastUpdatedTime DESC",
            //            ProcessInstance.SourceTable, StakeHolder.SourceTable, userID, TodoWork.SourceTable);//(原始sql)

            StringBuilder strSql = new StringBuilder();
            string ProcessInstanceTableName = ORMapping.GetTableName(typeof(ProcessInstance));
            string ApprovalLogTableName = ORMapping.GetTableName(typeof(ApprovalLog));
            //strSql.AppendFormat("SELECT {0}.* FROM {0}, {1} WHERE {1}.UserID = {2} AND {1}.ProcessInstanceID = {0}.ProcessInstanceID " +
            //               "AND {0}.ProcessInstanceID NOT IN (SELECT ProcessInstanceID FROM {3} WHERE UserID = {2}) ", 
            //               ProcessInstance.SourceTable, StakeHolder.SourceTable, userID, TodoWork.SourceTable);
            strSql.AppendFormat("SELECT DISTINCT {0}.* FROM {0}, {1} WHERE {1}.UserID = '{2}' AND {1}.ProcessInstanceID = {0}.ID AND {0}.IsDeleted<>1 AND {1}.IsDeleted<>1 ",
                               ProcessInstanceTableName,
                               ApprovalLogTableName,
                               userID);
            #region where条件
            if (whereString.Count > 0)
            {
                if (whereString["InstanceName"] != string.Empty)//标题
                {
                    strSql.AppendFormat(" AND {1}.InstanceName like '%{0}%'", WhereSqlClauseBuilder.EscapeLikeString(whereString["InstanceName"]),
                        ProcessInstanceTableName);
                }
                if (whereString["UserName"] != string.Empty)//创建人
                {
                    //strSql.AppendFormat(" AND ({1}.UserName='{0}' OR {1}.UserCode= '{0}')", whereString["UserName"],
                    //    ProcessInstanceTableName);

                    strSql.AppendFormat(" AND ({1}.UserName LIKE '%{0}%')", WhereSqlClauseBuilder.EscapeLikeString(whereString["UserName"]),
    ProcessInstanceTableName);
                }
                if (whereString["LastUpdatedTime"] != string.Empty)//处理时间
                {
                    //处理时间开始
                    strSql.AppendFormat(" AND  {0}.LastUpdatedTime>= Convert(NVARCHAR(10),'{1}',120) ",
                        ProcessInstanceTableName,
                        whereString["LastUpdatedTime"]);
                    //处理时间结束
                    strSql.AppendFormat(" AND  {0}.LastUpdatedTime< Convert(NVARCHAR(10),dateadd(d,1,'{1}'),120)",
                        ProcessInstanceTableName,
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                if (whereString["CreatedTime"] != string.Empty)//接收时间
                {
                    //接收时间开始
                    strSql.AppendFormat(" AND  {0}.CreatedTime>= Convert(NVARCHAR(10),'{1}',120) ", ProcessInstanceTableName, whereString["CreatedTime"]);
                    //接收时间结束
                    strSql.AppendFormat(" AND  {0}.CreatedTime< Convert(NVARCHAR(10),dateadd(d,1,'{1}'),120)", ProcessInstanceTableName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                if (whereString["Status"] != string.Empty)//状态
                {
                    strSql.AppendFormat(" AND {1}.Status='{0}'", whereString["Status"],
                        ProcessInstanceTableName);
                }
                if (whereString["ProcessCode"] != string.Empty)//流程类型
                {
                    strSql.AppendFormat(" AND {1}.ProcessCode='{0}'", whereString["ProcessCode"],
                        ProcessInstanceTableName);
                }
            }
            #endregion

            return ExecuteQuery(strSql.ToString());
        }

        #region add czq

        //已办流程,分页查询
        public PartlyCollection<ProcessInstance> LoadListByRelatedUserID(int pageIndex, int pageSize, Dictionary<string, string> whereString)
        {
            StringBuilder strSql = new StringBuilder();
            string ProcessInstanceTableName = ORMapping.GetTableName(typeof(ProcessInstance));
            string ApprovalLogTableName = ORMapping.GetTableName(typeof(ApprovalLog));

            string userID = string.Empty;
            strSql.AppendFormat("{0}.IsDeleted<>1 AND {1}.IsDeleted<>1 ", ProcessInstanceTableName, ApprovalLogTableName);
            if (whereString.ContainsKey("UserID") && !string.IsNullOrEmpty(whereString["UserID"]))
                userID = whereString["UserID"];
            strSql.AppendFormat(" AND {1}.UserID = '{2}' AND {1}.ProcessInstanceID = {0}.ID ",
                               ProcessInstanceTableName,
                               ApprovalLogTableName,
                               userID);
            #region where条件
            if (whereString.ContainsKey("InstanceName") && !string.IsNullOrEmpty(whereString["InstanceName"]))//标题
            {
                strSql.AppendFormat(" AND {1}.InstanceName like '%{0}%'", WhereSqlClauseBuilder.EscapeLikeString(whereString["InstanceName"]),
                    ProcessInstanceTableName);
            }
            if (whereString.ContainsKey("UserName") && !string.IsNullOrEmpty(whereString["UserName"]))//创建人
            {
                strSql.AppendFormat(" AND ({1}.UserName LIKE '%{0}%')", WhereSqlClauseBuilder.EscapeLikeString(whereString["UserName"]), ProcessInstanceTableName);
            }
            if (whereString.ContainsKey("LastUpdatedTime") && !string.IsNullOrEmpty(whereString["LastUpdatedTime"]))//处理时间
            {
                //处理时间开始
                strSql.AppendFormat(" AND  {0}.LastUpdatedTime>= Convert(NVARCHAR(10),'{1}',120) ",
                    ProcessInstanceTableName,
                    whereString["LastUpdatedTime"]);
                //处理时间结束
                strSql.AppendFormat(" AND  {0}.LastUpdatedTime< Convert(NVARCHAR(10),dateadd(d,1,'{1}'),120)",
                    ProcessInstanceTableName,
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            if (whereString.ContainsKey("CreatedTime") && !string.IsNullOrEmpty(whereString["CreatedTime"]))//接收时间
            {
                //接收时间开始
                strSql.AppendFormat(" AND  {0}.CreateTime>= Convert(NVARCHAR(10),'{1}',120) ", ProcessInstanceTableName, whereString["CreatedTime"]);
                //接收时间结束
                strSql.AppendFormat(" AND  {0}.CreateTime< Convert(NVARCHAR(10),dateadd(d,1,'{1}'),120)", ProcessInstanceTableName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            if (whereString.ContainsKey("Status") && !string.IsNullOrEmpty(whereString["Status"]))//状态
            {
                strSql.AppendFormat(" AND {1}.Status='{0}'", whereString["Status"],
                    ProcessInstanceTableName);
            }
            if (whereString.ContainsKey("ProcessCode") && !string.IsNullOrEmpty(whereString["ProcessCode"]))//流程类型
            {
                strSql.AppendFormat(" AND {1}.ProcessCode='{0}'", whereString["ProcessCode"],
                    ProcessInstanceTableName);
            }

            #endregion
            string selectSQL = string.Format("(SELECT DISTINCT {0}.* FROM {0},{1} WHERE {2}) as temp", ProcessInstanceTableName, ApprovalLogTableName, strSql.ToString());
            string selectFields = "DISTINCT *";
            QueryCondition qc = new QueryCondition(pageIndex, pageSize, selectFields, selectSQL, "temp.CreateTime DESC");

            PartlyCollection<ProcessInstance> tuple = GetPageSplitedCollection(qc);
            return tuple;
        }

        //我的流程分页查询
        public PartlyCollection<ProcessInstance> LoadListByCreateUserID(int pageIndex, int pageSize, Dictionary<string, string> whereString)
        {
            string tableName = ORMapping.GetTableName(typeof(ProcessInstance));
            #region Where语句

            StringBuilder burWhere = new StringBuilder();
            burWhere.Append("1=1 ");
            if (whereString.ContainsKey("UserID") && !string.IsNullOrEmpty(whereString["UserID"]))
                burWhere.AppendFormat(" AND {1}.UserID='{0}'", whereString["UserID"], tableName);

            if (whereString.ContainsKey("InstanceName") && !string.IsNullOrEmpty(whereString["InstanceName"]))//标题
            {
                burWhere.AppendFormat(" AND {1}.InstanceName like '%{0}%'", WhereSqlClauseBuilder.EscapeLikeString(whereString["InstanceName"]), tableName);
            }
            if (whereString.ContainsKey("CreatedTime") && !string.IsNullOrEmpty(whereString["CreatedTime"]))//创建时间
            {
                //创建时间开始
                burWhere.AppendFormat(" AND  {0}.CreateTime>= Convert(NVARCHAR(10),'{1}',120) ", tableName, whereString["CreatedTime"]);
                //创建时间结束
                burWhere.AppendFormat(" AND  {0}.CreateTime< Convert(NVARCHAR(10),dateadd(d,1,'{1}'),120)", tableName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            if (whereString.ContainsKey("LastUpdatedTime") && !string.IsNullOrEmpty(whereString["LastUpdatedTime"]))//接收时间
            {
                //最后更新时间开始
                burWhere.AppendFormat(" AND  {0}.LastUpdatedTime>= Convert(NVARCHAR(10),'{1}',120) ", tableName, whereString["LastUpdatedTime"]);
                //最后更新时间结束
                burWhere.AppendFormat(" AND  {0}.LastUpdatedTime< Convert(NVARCHAR(10),dateadd(d,1,'{1}'),120)", tableName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            if (whereString.ContainsKey("ProcessCode") && !string.IsNullOrEmpty(whereString["ProcessCode"]))//流程类型
            {
                burWhere.AppendFormat(" AND {1}.ProcessCode='{0}'", whereString["ProcessCode"], tableName);
            }
            if (whereString.ContainsKey("Status") && !string.IsNullOrEmpty(whereString["Status"]))//状态
            {
                burWhere.AppendFormat(" AND {1}.Status='{0}'", whereString["Status"], tableName);
            }
            burWhere.AppendFormat(" AND {0}", NotDeleted);
            #endregion
            PartlyCollection<ProcessInstance> result = GetPageSplitedCollection(pageIndex, pageSize, burWhere.ToString());

            return result;
        }

        //流程分页查询
        public PartlyCollection<ProcessInstance> LoadList(int pageIndex, int pageSize, Dictionary<string, string> whereString)
        {
            StringBuilder strSql = new StringBuilder();
            string ProcessInstanceTableName = ORMapping.GetTableName(typeof(ProcessInstance));
            string StakeHolderTableName = ORMapping.GetTableName(typeof(StakeHolder));

            string userID = string.Empty;
            strSql.Append("1=1");
            strSql.AppendFormat(" AND {0}.IsDeleted<>1 AND {1}.IsDeleted<>1 ", ProcessInstanceTableName, StakeHolderTableName);
            if (whereString.ContainsKey("UserID") && !string.IsNullOrEmpty(whereString["UserID"]))
            {
                userID = whereString["UserID"];
                strSql.AppendFormat(" AND {0}.UserID = '{1}' ",
                                   StakeHolderTableName,
                                   userID);
            }
            strSql.AppendFormat("AND {1}.ProcessInstanceID = {0}.ID", ProcessInstanceTableName, StakeHolderTableName);
            #region where条件
            if (whereString.ContainsKey("InstanceName") && !string.IsNullOrEmpty(whereString["InstanceName"]))//标题
            {
                strSql.AppendFormat(" AND {1}.InstanceName like '%{0}%'", WhereSqlClauseBuilder.EscapeLikeString(whereString["InstanceName"]),
                    ProcessInstanceTableName);
            }
            if (whereString.ContainsKey("UserName") && !string.IsNullOrEmpty(whereString["UserName"]))//创建人
            {
                strSql.AppendFormat(" AND ({1}.UserName LIKE '%{0}%')", WhereSqlClauseBuilder.EscapeLikeString(whereString["UserName"]), ProcessInstanceTableName);
            }
            if (whereString.ContainsKey("LastUpdatedTime") && !string.IsNullOrEmpty(whereString["LastUpdatedTime"]))//处理时间
            {
                //处理时间开始
                strSql.AppendFormat(" AND  {0}.LastUpdatedTime>= Convert(NVARCHAR(10),'{1}',120) ",
                    ProcessInstanceTableName,
                    whereString["LastUpdatedTime"]);
                //处理时间结束
                strSql.AppendFormat(" AND  {0}.LastUpdatedTime< Convert(NVARCHAR(10),dateadd(d,1,'{1}'),120)",
                    ProcessInstanceTableName,
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            if (whereString.ContainsKey("CreatedTime") && !string.IsNullOrEmpty(whereString["CreatedTime"]))//接收时间
            {
                //接收时间开始
                strSql.AppendFormat(" AND  {0}.CreateTime>= Convert(NVARCHAR(10),'{1}',120) ", ProcessInstanceTableName, whereString["CreatedTime"]);
                //接收时间结束
                strSql.AppendFormat(" AND  {0}.CreateTime< Convert(NVARCHAR(10),dateadd(d,1,'{1}'),120)", ProcessInstanceTableName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            if (whereString.ContainsKey("Status") && !string.IsNullOrEmpty(whereString["Status"]))//状态
            {
                strSql.AppendFormat(" AND {1}.Status='{0}'", whereString["Status"],
                    ProcessInstanceTableName);
            }
            if (whereString.ContainsKey("ProcessCode") && !string.IsNullOrEmpty(whereString["ProcessCode"]))//流程类型
            {
                strSql.AppendFormat(" AND {1}.ProcessCode='{0}'", whereString["ProcessCode"],
                    ProcessInstanceTableName);
            }
            #endregion

            string selectSQL = string.Format("(SELECT DISTINCT {0}.* FROM {0},{1} WHERE {2}) as temp", ProcessInstanceTableName, StakeHolderTableName, strSql.ToString());
            string selectFields = "DISTINCT *";
            QueryCondition qc = new QueryCondition(pageIndex, pageSize, selectFields, selectSQL, "temp.CreateTime DESC");

            PartlyCollection<ProcessInstance> result = GetPageSplitedCollection(qc);

            return result;
        }
        #endregion

        //add czq 2013-06-17
        internal PartlyCollection<ProcessInstance> LoadListByCreateUserID(Filter.WorkFlowFilter filter)
        {
            WhereSqlClauseBuilder where = filter.ConvertToWhereBuilder();
            where.AppendItem("Isdeleted", 0);
            QueryCondition qc = new QueryCondition(
                  filter.RowIndex,
                  filter.PageSize,
                  " * ",
                 ORMapping.GetTableName(typeof(ProcessInstance)),
                  " Createtime desc",
                 where.ToSqlString(TSqlBuilder.Instance)
                );

            PartlyCollection<ProcessInstance> result = GetPageSplitedCollection(qc);
            return result;
        }
    }
}

