using Lib.Data;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LJTH.BusinessIndicators.Model;

namespace LJTH.BusinessIndicators.DAL
{
    sealed class A_SystemBatchAdapter : AppBaseAdapterT<A_SystemBatch>
    {
        public IList<A_SystemBatch> GetSystemBatchList()
        {
            string sql = ORMapping.GetSelectSql<A_SystemBatch>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;

            return ExecuteQuery(sql);
        }

        public A_SystemBatch GetSystemBatch( string BatchType , int FinYear , int FinMonth )
        {
            string sql = ORMapping.GetSelectSql<A_SystemBatch>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " and BatchType =@BatchType and  FinYear=@FinYear AND FinMonth=@FinMonth ";
            sql += " ORDER BY CreateTime DESC";

            SqlParameter pBatchType = CreateSqlParameter("@BatchType", System.Data.DbType.String, BatchType);
            SqlParameter pYear = CreateSqlParameter("@FinYear", System.Data.DbType.Int32, FinYear);
            SqlParameter pMonth = CreateSqlParameter("@FinMonth", System.Data.DbType.Int32, FinMonth);

            List<A_SystemBatch> list = ExecuteQuery(sql, pBatchType,pYear,pMonth);

            return (list != null && list.Count > 0) ? list.FirstOrDefault() : null;
        }


        internal int DeleteModel(A_SystemBatch Model)
        {
            int i = 0;
            string SQL = ORMapping.GetDeleteSql<A_SystemBatch>(Model, TSqlBuilder.Instance);
            if (SQL.Length > 0)
            {
                i = ExecuteSql(SQL.ToString());
            }
            return i;
        } 


    }
}
