
using System;
using LJTH.Lib.Data.AppBase;
using System.Collections.Generic;
using Lib.Data;
using LJTH.BusinessIndicators.Model;
using System.Data.SqlClient;


namespace LJTH.BusinessIndicators.DAL
{
    sealed class B_SubscriptionAdapter : AppBaseAdapterT<B_Subscription>
    {
        public IList<B_Subscription> GeSubscriptionList()
        {
            string sql = ORMapping.GetSelectSql<B_Subscription>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;

            return ExecuteQuery(sql);
        }
        public IList<B_Subscription> GeSubscriptionListByFile(Guid id, int month, int year,string name)
        {
            string sql = ORMapping.GetSelectSql<B_Subscription>(TSqlBuilder.Instance);
            sql += "WHERE " + base.NotDeleted;
            sql += "AND SystemID=@ID AND FinYear=@year AND FinMonth=@month AND CreatorName=@name ";
            SqlParameter pid = CreateSqlParameter("@ID", System.Data.DbType.Guid, id);
            SqlParameter pYear = CreateSqlParameter("@year", System.Data.DbType.Int32, year);
            SqlParameter pMonth = CreateSqlParameter("@month", System.Data.DbType.Int32, month);
            SqlParameter pName = CreateSqlParameter("@name", System.Data.DbType.String, name);

            return ExecuteQuery(sql, new SqlParameter[] { pid, pYear, pMonth, pName });

        }
        public IList<B_Subscription> GetAllSubscriptionList(int month, int year)
        {
            string sql = ORMapping.GetSelectSql<B_Subscription>(TSqlBuilder.Instance);
            sql += "WHERE " + base.NotDeleted;
            sql += "AND FinYear=@year AND FinMonth=@month AND Operating=1";

            SqlParameter pYear = CreateSqlParameter("@month", System.Data.DbType.Int32, month);
            SqlParameter pMonth = CreateSqlParameter("@year", System.Data.DbType.Int32, year);
            return ExecuteQuery(sql, new SqlParameter[] {  pYear, pMonth });
        }
    }
}
