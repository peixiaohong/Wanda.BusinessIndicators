
using System;
using Wanda.Lib.Data.AppBase;
using System.Collections.Generic;

using Lib.Data;
using Wanda.BusinessIndicators.Model;
using System.Data.SqlClient;
using System.Text;
using System.Transactions;


namespace Wanda.BusinessIndicators.DAL
{
    /// <summary>
    /// Interface对象的数据访问适配器
    /// </summary>
    sealed class TSM_MessagesAdapter : AppBaseAdapterT<TSM_Messages>
    {


        public IList<TSM_Messages> GetMessagesList()
        {
            string sql = ORMapping.GetSelectSql<TSM_Messages>(TSqlBuilder.Instance);
            return ExecuteQuery(sql);
        }


        public override int Delete(TSM_Messages data)
        {
            string sql = "DELETE FROM TSM_Messages WHERE ID=@ID";
            SqlParameter pa = CreateSqlParameter("@ID", System.Data.DbType.Guid, data.ID);
            return ExecuteSql(sql,new SqlParameter[]{pa});
        }
      
    }
}

