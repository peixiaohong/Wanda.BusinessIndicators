
using System;
using Wanda.Lib.Data.AppBase;
using System.Collections.Generic;

using Lib.Data;
using Wanda.BusinessIndicators.Model;


namespace Wanda.BusinessIndicators.DAL
{
    /// <summary>
    /// Targetmapping对象的数据访问适配器
    /// </summary>
    sealed class C_TargetmappingAdapter : AppBaseAdapterT<C_TargetMapping>
	{

        public IList<C_TargetMapping> GetTargetmappingList()
        {
            string sql = ORMapping.GetSelectSql<C_TargetMapping>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;

            return ExecuteQuery(sql);
        }
         
		
		 
	} 
}

