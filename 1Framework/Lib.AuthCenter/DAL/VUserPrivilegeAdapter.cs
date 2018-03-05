using Lib.Data;
using System;
using System.Collections.Generic;
using Wanda.Lib.AuthCenter.Model;
using Wanda.Lib.Data.AppBase;

namespace Wanda.Lib.AuthCenter.DAL
{



    class VUserPrivilegeAdapter : AuthBaseCompositionAdapterT<VUserPrivilege>
    {

        private VUserPrivilegeAdapter()
        {

        }

        public static VUserPrivilegeAdapter Instance = new VUserPrivilegeAdapter();



        public IList<VUserPrivilege> GetByUserID(string userID)
        {
            return GetByUserID(SqlTextHelper.SafeQuote(userID), -1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="privilegeType">小于0时不作为查询条件</param>
        /// <returns></returns>
        public IList<VUserPrivilege> GetByUserID(string userID, int privilegeType)
        {
            string sql = ORMapping.GetSelectSql<VUserPrivilege>(TSqlBuilder.Instance);

            WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
            where.AppendItem("userid", SqlTextHelper.SafeQuote(userID));
            if (privilegeType > 0)
            {
                where.AppendItem("privilegeType", privilegeType);
            }

            sql += " where " + where.ToSqlString(TSqlBuilder.Instance);

            return ExecuteQuery(sql);
        }
        /// <summary>
        /// 获取用户操作权限
        /// </summary>
        /// <param name="userID">用户id</param>
        /// <param name="privilegeType">权限类型</param>
        /// <param name="privilegeType">权限类型</param>
        /// <returns></returns>
        public IList<VUserPrivilege> GetByUserIDAndType(string userID, string privilegeType)
        {
            string sql = ORMapping.GetSelectSql<VUserPrivilege>(TSqlBuilder.Instance);

            WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
            where.AppendItem("userid", SqlTextHelper.SafeQuote(userID));
            if (null != privilegeType && privilegeType != "")
            {
                where.AppendItem("privilegeType", privilegeType);
            }

            sql += " where " + where.ToSqlString(TSqlBuilder.Instance);

            return ExecuteQuery(sql);
        }
        public IList<VUserPrivilege> GetByPrivilegeID(Guid privilegeID)
        {
            string sql = base.SelectAllString;

            WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
            where.AppendItem("privilegeID", privilegeID);

            sql += " where " + where.ToSqlString(TSqlBuilder.Instance);

            return ExecuteQuery(sql);
        }

        public IList<VUserPrivilege> GetUserPrivilege(Guid userID, Guid privilegeID)
        {
            string sql = base.SelectAllString;

            WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
            where.AppendItem("userID", userID);
            where.AppendItem("privilegeID", privilegeID);

            sql += " WHERE " + where.ToSqlString(TSqlBuilder.Instance);

            return ExecuteQuery(sql);
        }
    }
}
