
using System;
using LJTH.Lib.Data.AppBase;
using System.Collections.Generic;

using Lib.Data;
using LJTH.BusinessIndicators.Model;
using System.Data.SqlClient;

namespace LJTH.BusinessIndicators.DAL
{
    sealed class C_ReportTimeAdapter : AppBaseAdapterT<C_ReportTime>
    {
        public IList<C_ReportTime> GetReportTimeList()
        {
            string sql = ORMapping.GetSelectSql<C_ReportTime>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;

            return ExecuteQuery(sql);
        }
    }
}
