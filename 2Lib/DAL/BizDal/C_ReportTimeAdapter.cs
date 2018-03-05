﻿
using System;
using Wanda.Lib.Data.AppBase;
using System.Collections.Generic;

using Lib.Data;
using Wanda.BusinessIndicators.Model;
using System.Data.SqlClient;

namespace Wanda.BusinessIndicators.DAL
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
