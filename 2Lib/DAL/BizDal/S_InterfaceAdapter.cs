
using System;
using LJTH.Lib.Data.AppBase;
using System.Collections.Generic;

using Lib.Data;
using LJTH.BusinessIndicators.Model;


namespace LJTH.BusinessIndicators.DAL
{
    /// <summary>
    /// Interface对象的数据访问适配器
    /// </summary>
    sealed class S_InterfaceAdapter : AppBaseAdapterT<S_Interface>
	{

        public IList<S_Interface> GetInterfaceList()
        {
            string sql = ORMapping.GetSelectSql<S_Interface>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;

            return ExecuteQuery(sql);
        }
         
		
		 
	} 
}

