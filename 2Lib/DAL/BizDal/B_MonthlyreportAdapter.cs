
using System;
using LJTH.Lib.Data.AppBase;
using System.Collections.Generic;
using System.Linq;

using Lib.Data;
using LJTH.BusinessIndicators.Model;
using System.Data.SqlClient;
using System.Data;
using System.Transactions;

namespace LJTH.BusinessIndicators.DAL
{
    /// <summary>
    /// Monthlyreport对象的数据访问适配器
    /// </summary>
    sealed class B_MonthlyreportAdapter : AppBaseAdapterT<B_MonthlyReport>

    {

        public IList<B_MonthlyReport> GetMonthlyreportList()
        {
            string sql = ORMapping.GetSelectSql<B_MonthlyReport>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;

            return ExecuteQuery(sql);
        }
        /// <summary>
        /// 获取MonthlyReportDraftList
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <returns></returns>
        public B_MonthlyReport GetMonthlyReportDraftList(Guid ID)
        {
            string sql = ORMapping.GetSelectSql<B_MonthlyReport>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND ID=@ID";
            sql += " ORDER BY CreateTime DESC";

            SqlParameter pSystemID = CreateSqlParameter("@ID", System.Data.DbType.Guid, ID);
            List<B_MonthlyReport> list = ExecuteQuery(sql, pSystemID);
            return (list != null && list.Count > 0) ? list.FirstOrDefault() : null;
        }

        /// <summary>
        /// 通过批次获取，月度数据填报表
        /// </summary>
        /// <param name="SystemBatchID"></param>
        /// <returns></returns>
        public List<B_MonthlyReport> GetMonthlyreportList(Guid SystemBatchID)
        {
            string sql = ORMapping.GetSelectSql<B_MonthlyReport>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " and SystemBatchID=@SystemBatchID ";

            SqlParameter pSystemBatchID = CreateSqlParameter("@SystemBatchID", System.Data.DbType.Guid, SystemBatchID);

            return ExecuteQuery(sql, pSystemBatchID);
        }

        /// <summary>
        /// 获取B_MonthlyReport表中创建时间最后的数据 ，包含状态是 ：所有
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <returns></returns>
        public B_MonthlyReport GetLastMonthlyReportList(Guid SystemID, int Year, int Month)
        {
            string sql = ORMapping.GetSelectSql<B_MonthlyReport>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND SystemID=@SystemID AND FinYear=@Year AND FinMonth=@Month and WFStatus <>'Cancel'";
            sql += " ORDER BY CreateTime DESC";

            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pYear = CreateSqlParameter("@Year", System.Data.DbType.String, Year);
            SqlParameter pMonth = CreateSqlParameter("@Month", System.Data.DbType.String, Month);
            List<B_MonthlyReport> list = ExecuteQuery(sql, pSystemID, pYear, pMonth);
            return (list != null && list.Count > 0) ? list.FirstOrDefault() : null;
        }
        public B_MonthlyReport GetLastMonthlyReportList(Guid SystemID, int Year, int Month,Guid PlanId)
        {
            string sql = ORMapping.GetSelectSql<B_MonthlyReport>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND SystemID=@SystemID AND FinYear=@Year AND FinMonth=@Month AND TargetPlanID=@TargetPlanID and WFStatus <>'Cancel'";
            sql += " ORDER BY CreateTime DESC";

            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pYear = CreateSqlParameter("@Year", System.Data.DbType.String, Year);
            SqlParameter pMonth = CreateSqlParameter("@Month", System.Data.DbType.String, Month);
            SqlParameter pPlanID = CreateSqlParameter("@TargetPlanID", System.Data.DbType.Guid, PlanId);
            List<B_MonthlyReport> list = ExecuteQuery(sql, pSystemID, pYear, pMonth, pPlanID);
            return (list != null && list.Count > 0) ? list.FirstOrDefault() : null;
        }
        public List<B_MonthlyReport> GetLastMonthlyReportList(Guid PlanId)
        {
            string sql = ORMapping.GetSelectSql<B_MonthlyReport>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND TargetPlanID=@TargetPlanID and WFStatus <>'Cancel'";
            sql += " ORDER BY CreateTime DESC";
            
            SqlParameter pPlanID = CreateSqlParameter("@TargetPlanID", System.Data.DbType.Guid, PlanId);
            List<B_MonthlyReport> list = ExecuteQuery(sql, pPlanID);
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="AreaID"></param>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <param name="DefaultVersionStatus"></param>
        /// <param name="WFStatus"></param>
        /// <returns></returns>
        public B_MonthlyReport GetMonthlyReportModel(Guid SystemID, Guid AreaID, int Year, int Month, int DefaultVersionStatus, string WFStatus)
        {
            string sql = ORMapping.GetSelectSql<B_MonthlyReport>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND SystemID=@SystemID AND FinYear=@Year AND FinMonth=@Month and WFStatus <>'Cancel'";
            sql += " AND ISNULL(AreaID,'00000000-0000-0000-0000-000000000000')=@AreaID";
            sql += " AND WFStatus=@WFStatus";
            sql += " AND DefaultVersionStatus=@IsDefaultTargetPlan";
            sql += " ORDER BY CreateTime DESC";

            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pAreaID = CreateSqlParameter("@AreaID", System.Data.DbType.Guid, AreaID);
            SqlParameter pYear = CreateSqlParameter("@Year", System.Data.DbType.String, Year);
            SqlParameter pMonth = CreateSqlParameter("@Month", System.Data.DbType.String, Month);
            SqlParameter pDefaultVersionStatus = CreateSqlParameter("@IsDefaultTargetPlan", System.Data.DbType.Int32, DefaultVersionStatus);
            SqlParameter pWFStatus = CreateSqlParameter("@WFStatus", System.Data.DbType.String, WFStatus);
            List<B_MonthlyReport> list = ExecuteQuery(sql, pSystemID,pAreaID, pYear, pMonth, pDefaultVersionStatus, pWFStatus);
            return (list != null && list.Count > 0) ? list.FirstOrDefault() : null;
        }
        /// <summary>
        /// 获取MonthlyReportDraftList 状态是 ：Approved   审批完成状态
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <returns></returns>
        public B_MonthlyReport GetMonthlyReportApprovedList(Guid SystemID, int Year, int Month)
        {
            string sql = ORMapping.GetSelectSql<B_MonthlyReport>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND SystemID=@SystemID AND FinYear=@Year AND FinMonth=@Month AND WFStatus='Approved'";
            sql += " ORDER BY CreateTime DESC";

            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pYear = CreateSqlParameter("@Year", System.Data.DbType.String, Year);
            SqlParameter pMonth = CreateSqlParameter("@Month", System.Data.DbType.String, Month);
            List<B_MonthlyReport> list = ExecuteQuery(sql, pSystemID, pYear, pMonth);
            return (list != null && list.Count > 0) ? list.FirstOrDefault() : null;
        }

        /// <summary>
        /// 获取MonthlyReportDraftList 状态是 ：Draft   草稿状态
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <returns></returns>
        public B_MonthlyReport GetMonthlyReportDraftList(Guid SystemID, int Year, int Month)
        {
            string sql = ORMapping.GetSelectSql<B_MonthlyReport>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND SystemID=@SystemID AND FinYear=@Year AND FinMonth=@Month AND WFStatus='Draft' AND DefaultVersionStatus=1 ";
            sql += " ORDER BY CreateTime DESC";

            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pYear = CreateSqlParameter("@Year", System.Data.DbType.String, Year);
            SqlParameter pMonth = CreateSqlParameter("@Month", System.Data.DbType.String, Month);
            List<B_MonthlyReport> list = ExecuteQuery(sql, pSystemID, pYear, pMonth);
            return (list != null && list.Count > 0) ? list.FirstOrDefault() : null;
        }

        /// <summary>
        /// 获取MonthlyReportDraftList 状态是 ：Draft   草稿状态
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <returns></returns>
        public B_MonthlyReport GetMonthlyReportDraftList(Guid SystemID, Guid AreaID, int Year, int Month)
        {
            string sql = ORMapping.GetSelectSql<B_MonthlyReport>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND SystemID=@SystemID AND AreaID=@AreaID AND FinYear=@Year AND FinMonth=@Month AND WFStatus='Draft' AND DefaultVersionStatus=1 ";
            sql += " ORDER BY CreateTime DESC";

            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pAreaID = CreateSqlParameter("@AreaID", System.Data.DbType.Guid, AreaID);
            SqlParameter pYear = CreateSqlParameter("@Year", System.Data.DbType.String, Year);
            SqlParameter pMonth = CreateSqlParameter("@Month", System.Data.DbType.String, Month);
            List<B_MonthlyReport> list = ExecuteQuery(sql, pSystemID, pAreaID, pYear, pMonth);
            return (list != null && list.Count > 0) ? list.FirstOrDefault() : null;
        }

        /// <summary>
        /// 获取MonthlyReportList 状态是 ：Progress   审批中的
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <returns></returns>
        public B_MonthlyReport GetMonthlyReportList(Guid SystemID, Guid SystemBatchID, int Year, int Month)
        {
            string sql = ORMapping.GetSelectSql<B_MonthlyReport>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND SystemID=@SystemID AND SystemBatchID=@SystemBatchID AND FinYear=@Year AND FinMonth=@Month  and WFStatus ='Progress' ";
            sql += " ORDER BY CreateTime DESC";

            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pSystemBatchID = CreateSqlParameter("@SystemBatchID", System.Data.DbType.Guid, SystemBatchID);
            SqlParameter pYear = CreateSqlParameter("@Year", System.Data.DbType.String, Year);
            SqlParameter pMonth = CreateSqlParameter("@Month", System.Data.DbType.String, Month);
            List<B_MonthlyReport> list = ExecuteQuery(sql, pSystemID, pSystemBatchID, pYear, pMonth);
            return (list != null && list.Count > 0) ? list.FirstOrDefault() : null;
        }

        /// <summary>
        /// 获取MonthlyReportList 状态是 ：Progress   审批中的
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <returns></returns>
        public B_MonthlyReport GetMonthlyReportList(Guid SystemID, int Year, int Month)
        {
            string sql = ORMapping.GetSelectSql<B_MonthlyReport>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND SystemID=@SystemID  AND FinYear=@Year AND FinMonth=@Month  and WFStatus ='Progress' ";
            sql += " ORDER BY CreateTime DESC";

            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pYear = CreateSqlParameter("@Year", System.Data.DbType.String, Year);
            SqlParameter pMonth = CreateSqlParameter("@Month", System.Data.DbType.String, Month);
            List<B_MonthlyReport> list = ExecuteQuery(sql, pSystemID, pYear, pMonth);
            return (list != null && list.Count > 0) ? list.FirstOrDefault() : null;
        }

        /// <summary>
        /// 获取MonthlyReportList 状态是 ：Progress   审批中的 ,Approved : 审批完成 
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <returns></returns>
        public B_MonthlyReport GetLatestMonthlyReport(Guid SystemID, int Year, int Month)
        {
            string sql = ORMapping.GetSelectSql<B_MonthlyReport>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND SystemID=@SystemID AND FinYear=@Year AND FinMonth=@Month  and (WFStatus ='Progress' or  WFStatus ='Approved')";
            sql += " ORDER BY CreateTime DESC";

            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pYear = CreateSqlParameter("@Year", System.Data.DbType.String, Year);
            SqlParameter pMonth = CreateSqlParameter("@Month", System.Data.DbType.String, Month);
            List<B_MonthlyReport> list = ExecuteQuery(sql, pSystemID, pYear, pMonth);
            return (list != null && list.Count > 0) ? list.FirstOrDefault() : null;
        }

        public B_MonthlyReport GetLatestMonthlyReport(Guid SystemID, int Year, int Month,Guid TargetPlanId,string aa)
        {
            string sql = ORMapping.GetSelectSql<B_MonthlyReport>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND SystemID=@SystemID AND FinYear=@Year AND FinMonth=@Month AND TargetPlanID=@TargetPlanID ";
            sql += " ORDER BY CreateTime DESC";

            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pYear = CreateSqlParameter("@Year", System.Data.DbType.String, Year);
            SqlParameter pMonth = CreateSqlParameter("@Month", System.Data.DbType.String, Month);
            SqlParameter pPlanID = CreateSqlParameter("@TargetPlanID", System.Data.DbType.Guid, TargetPlanId);
            List<B_MonthlyReport> list = ExecuteQuery(sql, pSystemID, pYear, pMonth,pPlanID);
            return (list != null && list.Count > 0) ? list.FirstOrDefault() : null;
        }
        /// <summary>
        /// 获取不包含MonthlyreportID的B_MonthlyReport集合
        /// </summary>
        /// <param name="SystemID">系统ID</param>
        /// <param name="Year">年</param>
        /// <param name="Month">月</param>
        /// <param name="MonthlyReportID">月报ID</param>
        /// <returns></returns>
        public B_MonthlyReport GetLatestMonthlyReport(Guid SystemID, int Year, int Month, Guid MonthlyReportID)
        {
            string sql = ORMapping.GetSelectSql<B_MonthlyReport>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND SystemID=@SystemID AND FinYear=@Year AND FinMonth=@Month AND ID not in(@MonthlyReportID) AND (WFStatus ='Progress' or  WFStatus ='Approved')";
            sql += " ORDER BY CreateTime DESC";

            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pYear = CreateSqlParameter("@Year", System.Data.DbType.String, Year);
            SqlParameter pMonth = CreateSqlParameter("@Month", System.Data.DbType.String, Month);
            SqlParameter pMonthlyReportID = CreateSqlParameter("@MonthlyReportID", System.Data.DbType.Guid, MonthlyReportID);
            List<B_MonthlyReport> list = ExecuteQuery(sql, pSystemID, pYear, pMonth, pMonthlyReportID);

            return (list != null && list.Count > 0) ? list.FirstOrDefault() : null;
        }

        /// <summary>
        /// 获取不包含MonthlyreportID的B_MonthlyReport集合
        /// </summary>
        /// <param name="SystemID">系统ID</param>
        /// <param name="Year">年</param>
        /// <param name="Month">月</param>
        /// <param name="MonthlyReportID">月报ID</param>
        /// <returns></returns>
        public B_MonthlyReport GetLatestMonthlyReport(Guid SystemID, Guid AreaID, int Year, int Month, Guid MonthlyReportID, Guid TargetPlanID)
        {
            string sql = ORMapping.GetSelectSql<B_MonthlyReport>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND SystemID=@SystemID AND ISNULL(AreaID,'00000000-0000-0000-0000-000000000000')=@AreaID AND FinYear=@Year AND FinMonth=@Month AND ID not in(@MonthlyReportID) AND (WFStatus ='Progress' or  WFStatus ='Approved') AND TargetPlanID=@TargetPlanID";
            sql += " ORDER BY CreateTime DESC";

            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pAreaID = CreateSqlParameter("@AreaID", System.Data.DbType.Guid, AreaID);
            SqlParameter pYear = CreateSqlParameter("@Year", System.Data.DbType.String, Year);
            SqlParameter pMonth = CreateSqlParameter("@Month", System.Data.DbType.String, Month);
            SqlParameter pMonthlyReportID = CreateSqlParameter("@MonthlyReportID", System.Data.DbType.Guid, MonthlyReportID);
            SqlParameter pTargetPlanID = CreateSqlParameter("@TargetPlanID", System.Data.DbType.Guid, TargetPlanID);
            List<B_MonthlyReport> list = ExecuteQuery(sql, pSystemID, pAreaID, pYear, pMonth, pMonthlyReportID,pTargetPlanID);

            return (list != null && list.Count > 0) ? list.FirstOrDefault() : null;
        }


        /// <summary>
        /// 获取所有状态的B_MonthlyReport集合
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <returns></returns>
        public List<B_MonthlyReport> GetMonthlyReportByApproveList(int Year, int Month)
        {
            string sql = ORMapping.GetSelectSql<B_MonthlyReport>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND WFStatus!='Draft' AND WFStatus!='Cancel' ";
            sql += " AND FinMonth=@Month AND FinYear=@Year ORDER BY CreateTime DESC";
            SqlParameter pYear = CreateSqlParameter("@Year", System.Data.DbType.String, Year);
            SqlParameter pMonth = CreateSqlParameter("@Month", System.Data.DbType.String, Month);

            return ExecuteQuery(sql, pYear, pMonth);
        }
        /// <summary>
        /// 获取所有状态的B_MonthlyReport集合（无SystemID）
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <returns></returns>
        public List<B_MonthlyReport> GetMonthlyReportByAllList(int Year, int Month)
        {
            string sql = ORMapping.GetSelectSql<B_MonthlyReport>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            //sql += " AND WFStatus!='Draft' AND WFStatus!='Cancel' ";
            sql += " AND FinMonth=@Month AND FinYear=@Year ORDER BY CreateTime DESC";
            SqlParameter pYear = CreateSqlParameter("@Year", System.Data.DbType.String, Year);
            SqlParameter pMonth = CreateSqlParameter("@Month", System.Data.DbType.String, Month);

            return ExecuteQuery(sql, pYear, pMonth);
        }
        /// <summary>
        /// 获取所有状态的B_MonthlyReport集合（无SystemID且分组）
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <returns></returns>
        public DataTable GetMonthlyReportByGroupList(int Year, int Month)
        {
            string sql = "SELECT COUNT(1) SysCount, SystemID, MAX(CreateTime) CreateTime  FROM B_MonthlyReport ";
            sql += "WHERE " + base.NotDeleted;
            sql += " AND ReportApprove IS Not Null AND ReportApprove <> '' ";
            sql += " AND FinMonth=@Month AND FinYear=@Year GROUP BY SystemID";
            SqlParameter pYear = CreateSqlParameter("@Year", System.Data.DbType.String, Year);
            SqlParameter pMonth = CreateSqlParameter("@Month", System.Data.DbType.String, Month);

            return ExecuteReturnTable(sql, pYear, pMonth);
        }
        /// <summary>
        /// 根据SystemID 获取所有状态的B_MonthlyReport集合/月报
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <returns></returns>
        public List<B_MonthlyReport> GetMonthlyReportBySysIDList(Guid SystemID, int Year, int Month)
        {
            string sql = ORMapping.GetSelectSql<B_MonthlyReport>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            //sql += " AND WFStatus!='Draft' AND WFStatus!='Cancel' ";
            sql += " AND FinMonth=@Month AND FinYear=@Year  AND SystemID=@SystemID ORDER BY CreateTime DESC ";
            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pYear = CreateSqlParameter("@Year", System.Data.DbType.String, Year);
            SqlParameter pMonth = CreateSqlParameter("@Month", System.Data.DbType.String, Month);

            return ExecuteQuery(sql, pSystemID, pYear, pMonth);
        }

        /// <summary>
        /// 根据SystemID 获取所有状态的B_MonthlyReport集合/指标
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <returns></returns>
        public List<B_MonthlyReport> GetTargetPlanBySysIDList(Guid SystemID, int Year)
        {
            string sql = ORMapping.GetSelectSql<B_MonthlyReport>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            //sql += " AND WFStatus!='Draft' AND WFStatus!='Cancel' ";
            sql += " AND FinMonth=@Month AND FinYear=@Year  AND SystemID=@SystemID ORDER BY CreateTime DESC";
            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pYear = CreateSqlParameter("@Year", System.Data.DbType.String, Year);

            return ExecuteQuery(sql, pSystemID, pYear);
        }
        /// <summary>
        /// 去最新的数据ID
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <param name="SystemID"></param>
        /// <returns></returns>
        public B_MonthlyReport GetMonthlyReportNew(int Year, int Month, Guid SystemID)
        {
            string sql = @"SELECT * FROM dbo.B_MonthlyReport WHERE SystemID=@SystemID
AND (WFStatus ='Progress' or  WFStatus ='Approved')
AND FinYear=2015 AND FinMonth=8 ORDER BY CreateTime DESC ";
            SqlParameter pYear = CreateSqlParameter("@FinYear", System.Data.DbType.String, Year);
            SqlParameter pMonth = CreateSqlParameter("@FinMonth", System.Data.DbType.String, Month);
            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            List<B_MonthlyReport> list = ExecuteQuery(sql, pSystemID, pYear, pMonth);
            return (list != null && list.Count > 0) ? list.FirstOrDefault() : null;
        }

        /// <summary>
        /// 获取已有上传年份
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="Year"></param>
        /// <returns></returns>
        public List<B_MonthlyReport> GetMonthlyReportYearList()
        {
            string sql = "select distinct FinYear from dbo.B_MonthlyReport ";

            sql += "WHERE " + base.NotDeleted;
            sql += " ORDER BY FinYear DESC";

            return ExecuteQuery(sql);
        }

        public bool DeleteNoDefaultVersionMonthlyReport(B_MonthlyReport MonthlyReport)
        {
            var result = false;
            try
            {
                string sql = string.Format(@"
                    DELETE  B
                    FROM    dbo.B_MonthlyReport AS A
                            INNER JOIN dbo.B_MonthlyReportDetail AS B ON B.MonthlyReportID = A.ID
                    WHERE   A.SystemID = '{0}'
                            AND A.FinMonth = {1}
                            AND A.FinYear = {2}
                            AND A.ID <> '{3}'
                            AND ISNULL(A.AreaID,'00000000-0000-0000-0000-000000000000') = '{4}'
                            AND WFStatus<>'Approved';       
		 
                    DELETE  dbo.B_MonthlyReport
                    WHERE   SystemID = '{0}'
                            AND FinMonth = {1}
                            AND FinYear = {2}
                            AND ID <> '{3}'
                            AND ISNULL(AreaID,'00000000-0000-0000-0000-000000000000') = '{4}'
                            AND WFStatus<>'Approved';  ", MonthlyReport.SystemID,
                            MonthlyReport.FinMonth, MonthlyReport.FinYear, MonthlyReport.ID, MonthlyReport.AreaID);
                using (TransactionScope scope = TransactionScopeFactory.Create())
                {
                    var con = new SqlConnection(DbConnectionManager.GetConnectionString(base.ConnectionName));
                    con.Open();
                    SqlCommand cmd = new SqlCommand(sql, con);
                    result = cmd.ExecuteNonQuery() > 0;
                    con.Close();
                    scope.Complete();
                }


            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }
        public int UpdateReportApprove(Guid id,string json)
        {
            string sql = "UPDATE B_MonthlyReport SET ReportApprove=@ReportApprove";

            sql += " WHERE Id=@Id"; 

            return ExecuteSql(sql,CreateSqlParameter("@Id",DbType.Guid,id),CreateSqlParameter("@ReportApprove", DbType.String,json));
        }
    }

}

