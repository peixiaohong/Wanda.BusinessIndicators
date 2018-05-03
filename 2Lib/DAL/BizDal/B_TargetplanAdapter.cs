
using System;
using LJTH.Lib.Data.AppBase;
using System.Collections.Generic;
using System.Linq;

using Lib.Data;
using LJTH.BusinessIndicators.Model;
using System.Data.SqlClient;
using System.Data;

namespace LJTH.BusinessIndicators.DAL
{
    /// <summary>
    /// Targetplan对象的数据访问适配器
    /// </summary>
    sealed class B_TargetplanAdapter : AppBaseAdapterT<B_TargetPlan>
    {

        public IList<B_TargetPlan> GetTargetplanList()
        {
            string sql = ORMapping.GetSelectSql<B_TargetPlan>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;

            return ExecuteQuery(sql);
        }

        public B_TargetPlan GetTargetPlanByID(Guid TargetPlanID)
        {
            string sql = ORMapping.GetSelectSql<B_TargetPlan>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND ID=@ID";
            sql += " ORDER BY CreateTime DESC";

            SqlParameter pTargetPlanID = CreateSqlParameter("@ID", System.Data.DbType.Guid, TargetPlanID);

            List<B_TargetPlan> list = ExecuteQuery(sql, pTargetPlanID);
            return (list != null && list.Count > 0) ? list.FirstOrDefault() : null;
        }

        public B_TargetPlan GetTargetPlanByDraft(Guid SystemID, int Year)
        {
            string sql = ORMapping.GetSelectSql<B_TargetPlan>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND SystemID=@SystemID AND FinYear=@Year AND WFStatus='Draft'";
            sql += " ORDER BY CreateTime DESC";

            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pYear = CreateSqlParameter("@Year", System.Data.DbType.String, Year);
            List<B_TargetPlan> list = ExecuteQuery(sql, pSystemID, pYear);
            return (list != null && list.Count > 0) ? list.FirstOrDefault() : null;
        }


        public B_TargetPlan GetTargetPlanByProgress(Guid SystemID, int Year)
        {
            string sql = ORMapping.GetSelectSql<B_TargetPlan>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND SystemID=@SystemID AND FinYear=@Year AND WFStatus='Progress'";
            sql += " ORDER BY CreateTime DESC";

            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pYear = CreateSqlParameter("@Year", System.Data.DbType.String, Year);
            List<B_TargetPlan> list = ExecuteQuery(sql, pSystemID, pYear);
            return (list != null && list.Count > 0) ? list.FirstOrDefault() : null;
        }


        public B_TargetPlan GetTargetPlanByProgressOrApproved(Guid SystemID, int Year)
        {
            string sql = ORMapping.GetSelectSql<B_TargetPlan>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND SystemID=@SystemID AND FinYear=@Year AND (WFStatus='Progress' or WFStatus='Approved')";
            sql += " ORDER BY CreateTime DESC";

            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pYear = CreateSqlParameter("@Year", System.Data.DbType.String, Year);
            List<B_TargetPlan> list = ExecuteQuery(sql, pSystemID, pYear);
            return (list != null && list.Count > 0) ? list.FirstOrDefault() : null;
        }
        public IList<B_TargetPlan> GetTargetPlanByApproved(Guid SystemID, int Year)
        {
            string sql = ORMapping.GetSelectSql<B_TargetPlan>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND SystemID=@SystemID AND FinYear=@Year AND WFStatus='Approved'";
            sql += " ORDER BY CreateTime DESC";

            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pYear = CreateSqlParameter("@Year", System.Data.DbType.String, Year);
            return ExecuteQuery(sql, pSystemID, pYear);

        }
        /// <summary>
        /// 获取所有状态的B_TargetPlan集合/获取
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <returns></returns>
        public List<B_TargetPlan> GetTargetPlanByAllList(int Year)
        {
            string sql = ORMapping.GetSelectSql<B_TargetPlan>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            //sql += " AND WFStatus!='Draft' AND WFStatus!='Cancel' ";
            sql += " AND FinYear=@Year ORDER BY CreateTime DESC";
            SqlParameter pYear = CreateSqlParameter("@Year", System.Data.DbType.String, Year);

            return ExecuteQuery(sql, pYear);
        }
        /// <summary>
        /// 获取所有状态的B_TargetPlan集合/分组获取
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <returns></returns>
        public DataTable GetTargetPlanByGroupList(int Year)
        {
            string sql = "SELECT COUNT(1) SysCount, SystemID, MAX(CreateTime) CreateTime  FROM B_TargetPlan ";
            sql += "WHERE " + base.NotDeleted;
            sql += " AND ReportApprove IS Not Null AND ReportApprove <> '' ";
            sql += " AND FinYear=@Year GROUP BY SystemID";
            SqlParameter pYear = CreateSqlParameter("@Year", System.Data.DbType.String, Year);

            return ExecuteReturnTable(sql, pYear);
        }
        /// <summary>
        /// 获取所有状态的B_TargetPlan集合
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <returns></returns>
        public List<B_TargetPlan> GetTargetPlanByApproveList(Guid SystemID, int Year)
        {
            string sql = ORMapping.GetSelectSql<B_TargetPlan>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            //sql += " AND WFStatus!='Draft' AND WFStatus!='Cancel' ";
            sql += " AND SystemID=@SystemID AND FinYear=@Year ORDER BY CreateTime DESC";
            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pYear = CreateSqlParameter("@Year", System.Data.DbType.String, Year);

            return ExecuteQuery(sql, pSystemID, pYear);
        }
        /// <summary>
        /// 取出该系统下所有审批中和审批完成的指标
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="Year"></param>
        /// <returns></returns>
        public IList<B_TargetPlan> GetTargetPlanByApprovedAndApproved(Guid SystemID, int Year)
        {
            string sql = ORMapping.GetSelectSql<B_TargetPlan>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND SystemID=@SystemID AND FinYear=@Year AND (WFStatus='Progress' or WFStatus='Approved')";
            sql += " ORDER BY CreateTime DESC";

            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pYear = CreateSqlParameter("@Year", System.Data.DbType.String, Year);
            return ExecuteQuery(sql, pSystemID, pYear);

        }
        

        public IList<B_TargetPlan> GetTargetPlanByApprovedAndApproved(int Year)
        {
            string sql = ORMapping.GetSelectSql<B_TargetPlan>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND FinYear=@Year AND (WFStatus='Progress' or WFStatus='Approved')";
            sql += " ORDER BY CreateTime DESC";

            SqlParameter pYear = CreateSqlParameter("@Year", System.Data.DbType.String, Year);
            return ExecuteQuery(sql, pYear);

        }

        /// <summary>
        /// 获取分解指标版本类型集合
        /// </summary>
        /// <param name="systemID"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public Dictionary<Guid, string> GetTargetVersionType(string systemID, int year, int month)
        {
            Dictionary<Guid, string> dc = new Dictionary<Guid, string>();
            string sql = @"SELECT  b.ID,isnull(b.VersionName,'') AS VersionName  FROM B_TargetPlanDetail a INNER JOIN B_TargetPlan b ON a.TargetPlanID=b.ID AND a.IsDeleted=b.IsDeleted
            WHERE a.SystemID =@SystemID AND a.FinYear =@FinYear AND a.FinMonth =@FinMonth AND a.IsDeleted = 0 ";
            sql += " ORDER BY b.VersionDefault desc, b.CreateTime DESC";
            var result = ExecuteQuery(sql, CreateSqlParameter("@SystemID", System.Data.DbType.Guid, systemID), CreateSqlParameter("@FinYear", System.Data.DbType.Int32, year), CreateSqlParameter("@FinMonth", System.Data.DbType.Int32, month));
            if (result != null && result.Count > 0)
            {
                foreach (var item in result)
                {
                    if (!dc.ContainsKey(item.ID))
                    {
                        dc.Add(item.ID, item.VersionName);
                    }
                }
            }
            return dc;
        }

    }
}

