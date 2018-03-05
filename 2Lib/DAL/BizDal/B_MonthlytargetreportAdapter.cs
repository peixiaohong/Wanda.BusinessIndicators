
using System;
using Wanda.Lib.Data.AppBase;
using System.Collections.Generic;

using Lib.Data;
using Wanda.BusinessIndicators.Model;


namespace Wanda.BusinessIndicators.DAL
{
    /// <summary>
    /// Monthlytargetreport对象的数据访问适配器
    /// </summary>
    sealed class B_MonthlytargetreportAdapter : AppBaseAdapterT<B_MonthlyTargetReport>
	{

        public IList<B_MonthlyTargetReport> GetMonthlytargetreportList()
        {
            string sql = ORMapping.GetSelectSql<B_MonthlyTargetReport>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;

            return ExecuteQuery(sql);
        }
         
		
		 
	} 
}

