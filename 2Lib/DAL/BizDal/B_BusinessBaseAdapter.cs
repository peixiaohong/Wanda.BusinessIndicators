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
    sealed class B_BusinessBaseAdapter : AppBaseAdapterT<B_BusinessBase>
    {

        public IList<B_BusinessBase> GetBusinessBaseList()
        {
            string sql = ORMapping.GetSelectSql<B_BusinessBase>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;

            return ExecuteQuery(sql);
        }


        public IList<B_BusinessBase> GetBusinessBaseList( Guid MonthlyReportID )
        {
            string sql = ORMapping.GetSelectSql<B_BusinessBase>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;

            sql += " AND MonthlyReportID =@MonthlyReportID ";

            SqlParameter p1 = CreateSqlParameter("@MonthlyReportID", System.Data.DbType.Guid, MonthlyReportID);

            return ExecuteQuery(sql, p1);
        }



    }
}
