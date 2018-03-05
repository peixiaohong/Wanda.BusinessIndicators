using Lib.Data;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wanda.BusinessIndicators.Model;

namespace Wanda.BusinessIndicators.DAL
{
    sealed class B_SystemBatchAdapter : AppBaseAdapterT<B_SystemBatch>
    {
        public IList<B_SystemBatch> GetSystemBatchList()
        {
            string sql = ORMapping.GetSelectSql<B_SystemBatch>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;

            return ExecuteQuery(sql);
        }

        public B_SystemBatch GetSystemBatch( string BatchType , int FinYear , int FinMonth )
        {
            string sql = ORMapping.GetSelectSql<B_SystemBatch>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " and BatchType =@BatchType and  FinYear=@FinYear AND FinMonth=@FinMonth   AND WFBatchStatus <> 'Cancel'"; 
            sql += " ORDER BY CreateTime DESC";

            SqlParameter pBatchType = CreateSqlParameter("@BatchType", System.Data.DbType.String, BatchType);
            SqlParameter pYear = CreateSqlParameter("@FinYear", System.Data.DbType.Int32, FinYear);
            SqlParameter pMonth = CreateSqlParameter("@FinMonth", System.Data.DbType.Int32, FinMonth);

            List<B_SystemBatch> list = ExecuteQuery(sql, pBatchType,pYear,pMonth);

            return (list != null && list.Count > 0) ? list.FirstOrDefault() : null;
        }

        public List<B_SystemBatch> GetSystemBatchList(string BatchType, int FinYear, int FinMonth)
        {
            string sql = ORMapping.GetSelectSql<B_SystemBatch>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " and BatchType =@BatchType and  FinYear=@FinYear AND FinMonth=@FinMonth   AND WFBatchStatus <> 'Cancel'";
            sql += " ORDER BY CreateTime DESC";

            SqlParameter pBatchType = CreateSqlParameter("@BatchType", System.Data.DbType.String, BatchType);
            SqlParameter pYear = CreateSqlParameter("@FinYear", System.Data.DbType.Int32, FinYear);
            SqlParameter pMonth = CreateSqlParameter("@FinMonth", System.Data.DbType.Int32, FinMonth);

            List<B_SystemBatch> list = ExecuteQuery(sql, pBatchType, pYear, pMonth);

            return list;
        }

        public B_SystemBatch GetSystemBatchByDraft(string BatchType, int FinYear, int FinMonth)
        {
            string sql = ORMapping.GetSelectSql<B_SystemBatch>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " and BatchType =@BatchType and  FinYear=@FinYear AND FinMonth=@FinMonth   AND WFBatchStatus = 'Draft'";
            sql += " ORDER BY CreateTime DESC";

            SqlParameter pBatchType = CreateSqlParameter("@BatchType", System.Data.DbType.String, BatchType);
            SqlParameter pYear = CreateSqlParameter("@FinYear", System.Data.DbType.Int32, FinYear);
            SqlParameter pMonth = CreateSqlParameter("@FinMonth", System.Data.DbType.Int32, FinMonth);

            List<B_SystemBatch> list = ExecuteQuery(sql, pBatchType, pYear, pMonth);

            return (list != null && list.Count > 0) ? list.FirstOrDefault() : null;
        }

    }
}
