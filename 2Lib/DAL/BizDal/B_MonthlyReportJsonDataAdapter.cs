using Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LJTH.BusinessIndicators.Model;

namespace LJTH.BusinessIndicators.DAL
{
    sealed class B_MonthlyReportJsonDataAdapter : AppBaseAdapterT<B_MonthlyReportJsonData>
    {

        public IList<B_MonthlyReportJsonData> GetMonthlyReportJsonDataList()
        {
            string sql = ORMapping.GetSelectSql<B_MonthlyReportJsonData>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;

            return ExecuteQuery(sql);
        }

    }
}
