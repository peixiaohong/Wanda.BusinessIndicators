using Lib.Data;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Wanda.BusinessIndicators.Model;


namespace Wanda.BusinessIndicators.DAL
{
    sealed class B_MonthlyReportActionAdapter : AppBaseAdapterT<B_MonthlyReportAction>
    {
        public IList<B_MonthlyReportAction> GetMonthlyReportActionList()
        {
            string sql = ORMapping.GetSelectSql<B_MonthlyReportAction>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;

            return ExecuteQuery(sql);
        }

        public IList<B_MonthlyReportAction> GetMonthlyReportActionList(Guid businessID)
        {
            string sql = ORMapping.GetSelectSql<B_MonthlyReportAction>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND MonthlyReportID=@BusinessID ";
            sql += " ORDER BY CreateTime DESC";
            SqlParameter pBusinessID = CreateSqlParameter("@BusinessID", System.Data.DbType.Guid, businessID);
            return ExecuteQuery(sql, pBusinessID);
        }

        public IList<B_MonthlyReportAction> GetsystemActionList(Guid SystemID, int Year, int Month)
        {
            string sql = ORMapping.GetSelectSql<B_MonthlyReportAction>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND SystemID=@SystemID AND FinYear=@FinYear AND FinMonth=@FinMonth ";
            sql += " ORDER BY CreateTime DESC";
            SqlParameter pBusinessID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter pYear = CreateSqlParameter("@FinYear", System.Data.DbType.Int32, Year);
            SqlParameter pMonth = CreateSqlParameter("@FinMonth", System.Data.DbType.Int32, Month);
            return ExecuteQuery(sql, new SqlParameter[] { pBusinessID, pYear, pMonth });
        }
        public IList<B_MonthlyReportAction> GetMonthlyReportActionOneHour(Guid sysid, int finmonth, int year,int time)
        {
            string sql=ORMapping.GetSelectSql<B_MonthlyReportAction>(TSqlBuilder.Instance);
            sql += "WHERE" + base.NotDeleted;
            sql += "AND DATEDIFF(MINUTE,OperatorTime,GETDATE())<=@Time ";//查询一小时内数据
            sql += "AND SystemID=@SystemID AND FinYear=@FinYear AND FinMonth=@FinMonth ORDER BY OperatorTime DESC";
            SqlParameter pSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, sysid);
            SqlParameter pTime = CreateSqlParameter("@Time", System.Data.DbType.Int32, time);
            SqlParameter pFinYear = CreateSqlParameter("@FinYear", System.Data.DbType.Int32, year);
            SqlParameter pFinMonth = CreateSqlParameter("@FinMonth", System.Data.DbType.Int32, finmonth);
            return ExecuteQuery(sql, new SqlParameter[] { pSystemID, pFinYear, pFinMonth, pTime });
        }
    }
}
