
using System;
using LJTH.Lib.Data.AppBase;
using System.Collections.Generic;
using Lib.Data;
using LJTH.BusinessIndicators.Model;


namespace LJTH.BusinessIndicators.DAL
{
    /// <summary>
    /// Misstargetcategory对象的数据访问适配器
    /// </summary>
    sealed class C_MisstargetcategoryAdapter : AppBaseAdapterT<C_MissTargetCategory>
	{

        public IList<C_MissTargetCategory> GetMisstargetcategoryList()
        {
            string sql = ORMapping.GetSelectSql<C_MissTargetCategory>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;

            return ExecuteQuery(sql);
        }
         
		
		 
	} 
}

