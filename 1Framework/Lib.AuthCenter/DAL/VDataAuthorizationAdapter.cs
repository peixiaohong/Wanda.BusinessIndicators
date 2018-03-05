using Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wanda.Lib.AuthCenter.Model;

namespace Wanda.Lib.AuthCenter.DAL
{
    class VDataAuthorizationAdapter : AuthBaseCompositionAdapterT<VDataAuthorization>
    {

        private VDataAuthorizationAdapter()
        {
        }
        public static VDataAuthorizationAdapter Instance = new VDataAuthorizationAdapter();
        public IList<VDataAuthorization> GetUserRoleData(int roleid, string userid)
        {
            string sql = base.SelectAllString;
            WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
            where.AppendItem("RoleID", roleid);
            where.AppendItem("UserID", userid);
            sql += " where " + where.ToSqlString(TSqlBuilder.Instance);
            return ExecuteQuery(sql);
        }



    }
}
