
using System;
using LJTH.Lib.Data.AppBase;
using System.Collections.Generic;
using System.Linq;

using Lib.Data;
using LJTH.BusinessIndicators.Model;
using System.Data.SqlClient;


namespace LJTH.BusinessIndicators.DAL
{
    /// <summary>
    /// Monthlyreport对象的数据访问适配器
    /// </summary>
    sealed class A_MonthlyreportAdapter : AppBaseAdapterT<A_MonthlyReport>
    {

        public IList<A_MonthlyReport> GetMonthlyreportList()
        {
            string sql = ORMapping.GetSelectSql<A_MonthlyReport>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;

            return ExecuteQuery(sql);
        }

        public A_MonthlyReport GetLatestAMonthlyReport(Guid SystemID, int Year, int Month)
        {
            string sql = ORMapping.GetSelectSql<A_MonthlyReport>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND SystemID=@SystemID AND FinYear=@Year AND FinMonth=@Month";
            sql += " ORDER BY CreateTime DESC";

            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pYear = CreateSqlParameter("@Year", System.Data.DbType.String, Year);
            SqlParameter pMonth = CreateSqlParameter("@Month", System.Data.DbType.String, Month);
            List<A_MonthlyReport> list = ExecuteQuery(sql, pSystemID, pYear, pMonth);
            return (list != null && list.Count > 0) ? list.FirstOrDefault() : null;
        }
        public A_MonthlyReport GetLatestAMonthlyReport(Guid SystemID, int Year, int Month,Guid targetPlan)
        {
            string sql = ORMapping.GetSelectSql<A_MonthlyReport>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND SystemID=@SystemID AND FinYear=@Year AND FinMonth=@Month AND TargetPlanID=@TargetPlanID ";
            sql += " ORDER BY CreateTime DESC";

            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pYear = CreateSqlParameter("@Year", System.Data.DbType.String, Year);
            SqlParameter pMonth = CreateSqlParameter("@Month", System.Data.DbType.String, Month);
            SqlParameter pPlanID = CreateSqlParameter("@TargetPlanID", System.Data.DbType.Guid, targetPlan);
            List<A_MonthlyReport> list = ExecuteQuery(sql, pSystemID, pYear, pMonth, pPlanID);
            return (list != null && list.Count > 0) ? list.FirstOrDefault() : null;
        }

        public A_MonthlyReport GetLatestAMonthlyReport(Guid SystemID, Guid AreaID, int Year, int Month)
        {
            string sql = ORMapping.GetSelectSql<A_MonthlyReport>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND SystemID=@SystemID AND AreaID=@AreaID AND FinYear=@Year AND FinMonth=@Month";
            sql += " ORDER BY CreateTime DESC";

            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pSystemBatchID = CreateSqlParameter("@AreaID", System.Data.DbType.Guid, AreaID);
            SqlParameter pYear = CreateSqlParameter("@Year", System.Data.DbType.String, Year);
            SqlParameter pMonth = CreateSqlParameter("@Month", System.Data.DbType.String, Month);
            List<A_MonthlyReport> list = ExecuteQuery(sql, pSystemID, pSystemBatchID, pYear, pMonth);
            return (list != null && list.Count > 0) ? list.FirstOrDefault() : null;
        }


        public A_MonthlyReport GetLatestAMonthlyReport(Guid SystemID, Guid AreaID, int Year, int Month,Guid TargetPlanID)
        {
            string sql = ORMapping.GetSelectSql<A_MonthlyReport>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND SystemID=@SystemID AND ISNULL(AreaID,'00000000-0000-0000-0000-000000000000')=@AreaID AND FinYear=@Year AND FinMonth=@Month AND TargetPlanID=@TargetPlanID";
            sql += " ORDER BY CreateTime DESC";

            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pSystemBatchID = CreateSqlParameter("@AreaID", System.Data.DbType.Guid, AreaID);
            SqlParameter pYear = CreateSqlParameter("@Year", System.Data.DbType.String, Year);
            SqlParameter pMonth = CreateSqlParameter("@Month", System.Data.DbType.String, Month);
            SqlParameter pTargetPlanID = CreateSqlParameter("@TargetPlanID", System.Data.DbType.Guid, TargetPlanID);
            List<A_MonthlyReport> list = ExecuteQuery(sql, pSystemID, pSystemBatchID, pYear, pMonth,pTargetPlanID);
            return (list != null && list.Count > 0) ? list.FirstOrDefault() : null;
        }

        public IList<A_MonthlyReport> GetMonthlyreportListID(Guid MonthlyreportID)
        {
            string sql = ORMapping.GetSelectSql<A_MonthlyReport>(TSqlBuilder.Instance);
            sql += "WHERE " + base.NotDeleted;
            sql += " AND ID=@MonthlyreportID ";
            SqlParameter pMonthlyreportID = CreateSqlParameter("@MonthlyreportID", System.Data.DbType.Guid, MonthlyreportID);
            return ExecuteQuery(sql, pMonthlyreportID);
        }
        internal void InsertAllFromB(Guid businessID)
        {
            string sql = "B2A";
            var p1 = new SqlParameter { ParameterName = "@BusinessID", Value = businessID };
            var p2 = new SqlParameter { ParameterName = "@Flag", Value = 0 };
            DbHelper.RunSPReturnDS(sql, ConnectionName, p1, p2);
        }
    }
}

