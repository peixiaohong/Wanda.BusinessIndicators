
using System;
using Wanda.Lib.Data.AppBase;
using System.Collections.Generic;

using Lib.Data;
using Wanda.BusinessIndicators.Model;


namespace Wanda.BusinessIndicators.DAL
{
    /// <summary>
    /// Interfaceinstance对象的数据访问适配器
    /// </summary>
    sealed class S_InterfaceinstanceAdapter : AppBaseAdapterT<S_InterfaceInstance>
	{

        public IList<S_InterfaceInstance> GetInterfaceinstanceList()
        {
            string sql = ORMapping.GetSelectSql<S_InterfaceInstance>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;

            return ExecuteQuery(sql);
        }
         
		
		 
	} 
}

