using Lib.Data;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wanda.BusinessIndicators.Model;

namespace Wanda.BusinessIndicators.DAL
{
    sealed class B_TargetPlanActionAdapter : AppBaseAdapterT<B_TargetPlanAction>
    {
        public IList<B_TargetPlanAction> GetTargetPlanActionList()
        {
            string sql = ORMapping.GetSelectSql<B_TargetPlanAction>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;

            return ExecuteQuery(sql);
        }
        public IList<B_TargetPlanAction> GetActionByTargetplanID(Guid TargetPlanID)
        {
            string sql = ORMapping.GetSelectSql<B_TargetPlanAction>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;
            sql += " AND TargetPlanID=@TargetPlanID";
            sql += " ORDER BY CreateTime DESC";
            SqlParameter pSystemID = CreateSqlParameter("@TargetPlanID", System.Data.DbType.Guid, TargetPlanID);
            return ExecuteQuery(sql, pSystemID);
        }
    }
}
