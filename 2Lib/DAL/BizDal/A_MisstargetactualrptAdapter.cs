
using System;
using Wanda.Lib.Data.AppBase;
using System.Collections.Generic;
using Lib.Data;
using Wanda.BusinessIndicators.Model;


namespace Wanda.BusinessIndicators.DAL
{
    /// <summary>
    /// Misstargetactualrpt对象的数据访问适配器
    /// </summary>
    sealed class A_MisstargetactualrptAdapter : AppBaseAdapterT<A_MissTargetActualRpt>
	{

        public IList<A_MissTargetActualRpt> GetMisstargetactualrptList()
        {
            string sql = ORMapping.GetSelectSql<A_MissTargetActualRpt>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;

            return ExecuteQuery(sql);
        }
         
		
		 
	} 
}

