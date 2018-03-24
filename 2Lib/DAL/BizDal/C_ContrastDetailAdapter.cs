
using System;
using LJTH.Lib.Data.AppBase;
using System.Collections.Generic;
using Lib.Data;
using LJTH.BusinessIndicators.Model;
using System.Data.SqlClient;
using System.Text;
using System.Transactions;
namespace LJTH.BusinessIndicators.DAL
{
    sealed class C_ContrastDetailAdapter : AppBaseAdapterT<C_ContrastDetail>
    {
        public IList<C_ContrastDetail> GetContrastDetailList()
        {
            string sql = ORMapping.GetSelectSql<C_ContrastDetail>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;

            return ExecuteQuery(sql);
        }
        public IList<C_ContrastDetail> GetContrastDetailList(int FinYear, int finMonth, Guid SystemID)
        {
            string sql = ORMapping.GetSelectSql<C_ContrastDetail>(TSqlBuilder.Instance);

            sql += " WHERE  " + base.NotDeleted;
            sql += " AND SystemID=@SystemID";
            sql += " AND FinMonth=@finMonth AND FinYear=@FinYear";

            SqlParameter PSystemID = CreateSqlParameter("@SystemID", System.Data.DbType.Guid, SystemID);
            SqlParameter PFinYear = CreateSqlParameter("@FinYear", System.Data.DbType.Int32, FinYear);
            SqlParameter PfinMonth = CreateSqlParameter("@finMonth", System.Data.DbType.Int32, finMonth);
            return ExecuteQuery(sql, PSystemID, PFinYear, PfinMonth);
        }

        public IList<C_ContrastDetail> GetAllContrastDetailList(int FinYear, int finMonth)
        {
            string sql = ORMapping.GetSelectSql<C_ContrastDetail>(TSqlBuilder.Instance);

            sql += " WHERE  " + base.NotDeleted;
            sql += " AND FinMonth=@finMonth AND FinYear=@FinYear";
            sql += " ORDER BY Sequence";
            SqlParameter PFinYear = CreateSqlParameter("@FinYear", System.Data.DbType.Int32, FinYear);
            SqlParameter PfinMonth = CreateSqlParameter("@finMonth", System.Data.DbType.Int32, finMonth);
            return ExecuteQuery(sql,  PFinYear, PfinMonth);
        }

    }
}
