using Lib.Data;
/*
 * Create by Hu Wei Zheng
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wanda.Lib.AuthCenter.Model;
using Wanda.Lib.Data.AppBase;
using System.Data;
using System.Data.SqlClient;
namespace Wanda.Lib.AuthCenter.DAL
{
    /// <summary>
    /// Roleinfo对象的数据访问适配器
    /// </summary>
    sealed class RoleinfoAdapter : AuthBaseAdapterT<BRoleinfo>, IUsage
    {

        public static readonly RoleinfoAdapter Instance = new RoleinfoAdapter();
        /// <summary>
        /// 获得所有的角色列表
        /// </summary>
        /// <returns></returns>
        public IList<BRoleinfo> GetRoleinfoList()
        {
            string sql = ORMapping.GetSelectSql<BRoleinfo>(TSqlBuilder.Instance);

            sql += " WHERE " + base.NotDeleted;

            return ExecuteQuery(sql);
        }

        /// <summary>
        /// 根据分组获取角色列表  
        /// </summary>
        /// <param name="groupId">分组Id</param>
        /// <returns></returns>
        public IList<BRoleinfo> GetRoleinfoListByGroupId(Guid groupId)
        {
            string sql = ORMapping.GetSelectSql<BRoleinfo>(TSqlBuilder.Instance);

            sql += " WHERE " + base.NotDeleted + " and ScopeID='" + groupId.ToString() + "'";

            return ExecuteQuery(sql);
        }



        /// <summary>
        /// 获得指定用户所属的角色列表
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public IList<BRoleinfo> GetBelongsToRoles(Guid userID)
        {
            //StringBuilder sql = new StringBuilder(ORMapping.GetSelectSql<Roleinfo>(TSqlBuilder.Instance));
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(" SELECT  A.* from {0} as A ", ORMapping.GetTableName(typeof(BRoleinfo)));
            sql.AppendFormat("  INNER JOIN  {0} as B on A.ID=B.RoleID", ORMapping.GetTableName(typeof(AUserRole)));
            sql.AppendFormat("  WHERE  B.UserID='{0}'", userID);
            sql.Append(" AND  A.IsDeleted<1 AND B.IsDeleted<1");

            return ExecuteQuery(sql.ToString());

        }


        ///// <summary>
        ///// 获得指定用户所属的角色列表根据万达用户ID
        ///// </summary>
        ///// <param name="userID"></param>
        ///// <returns></returns>
        //public IList<BRoleinfo> GetBelongsToRolesByWD(string WDUserId)
        //{
        //    StringBuilder sql = new StringBuilder();
        //    sql.AppendFormat(" SELECT  A.* from {0} as A ", ORMapping.GetTableName(typeof(BRoleinfo)));
        //    sql.AppendFormat("  INNER JOIN  {0} as B on A.ID=B.RoleID", ORMapping.GetTableName(typeof(AUserRole)));
        //    sql.AppendFormat("  WHERE  B.UserID in (select C.ID from UserInfo as C where WD_UserID='{0}')", SqlTextHelper.SafeQuote(WDUserId));
        //    sql.Append(" AND  A.IsDeleted<1 AND B.IsDeleted<1");

        //    return ExecuteQuery(sql.ToString());
        //}

        /// <summary>
        /// 获得指定授权匹配的角色列表
        /// </summary>
        /// <param name="privilegeID"></param>
        /// <returns></returns>
        public IList<BRoleinfo> GetGrantedRoles(Guid privilegeID)
        {
            //StringBuilder sql = new StringBuilder(ORMapping.GetSelectSql<Roleinfo>(TSqlBuilder.Instance));
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(" SELECT  A.* from {0} as A ", ORMapping.GetTableName(typeof(BRoleinfo)));
            // sql.AppendFormat(" SELECT  A.* from {0} as A ", ORMapping.GetTableName(typeof(BRoleinfo)));
            sql.AppendFormat(" INNER JOIN  {0} as B on A.ID=B.RoleID", ORMapping.GetTableName(typeof(ARolePrivilege)));
            sql.AppendFormat(" WHERE  B.PrivilegeID='{0}'", privilegeID);
            sql.Append(" AND  A.IsDeleted<1 AND B.IsDeleted<1");

            return ExecuteQuery(sql.ToString());
        }
        /// <summary>
        /// 根据角色名获取角色信息
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public BRoleinfo GetRoleInfoByRoleName(string roleName)
        {
            string sql = "SELECT * FROM  PB_RoleInfo WHERE Name=@RoleName  ";
            SqlParameter p = CreateSqlParameter("@RoleName", DbType.String, roleName);
            BRoleinfo info = ExecuteQuery(sql, new SqlParameter[] { p }).FirstOrDefault();
            return info;
        }
        /// <summary>
        /// 返回重名但ID不重复的项
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        internal IList<BRoleinfo> GetExists(string name, Guid id, Guid congId)
        {
            var list = this.Load(p =>
            {
                p.AppendItem("Name", SqlTextHelper.SafeQuote(name));
                if (id != null && id!=Guid.Empty)
                {
                    p.AppendItem("ID", id, "<>");
                }
                p.AppendItem("ScopeID", congId);
                p.AppendItem("ISDELETED", "1", "<>");

            });
            return list;
        }
        #region IUsage
        public int UsageCount(Guid ID)
        {

            int result = 0;

            string sql_Permission = string.Format(@"
SELECT COUNT(*) FROM {0} A 
WHERE A.RoleID='{1}' AND A.ISDELETED<1",
                                            ORMapping.GetTableName(typeof(ARolePrivilege)), ID);


            string sql_UserRole = string.Format(@"
SELECT COUNT(*) FROM {0} A 
WHERE A.RoleID='{1}' AND A.ISDELETED<1",
                                             ORMapping.GetTableName(typeof(AUserRole)), ID);

            result = (int)DbHelper.RunSqlReturnScalar(sql_Permission, ConnectionName);
            result += (int)DbHelper.RunSqlReturnScalar(sql_UserRole, ConnectionName);
            return result;

        }

        #endregion

        /// <summary>
        /// 维护角色下所有的授权
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="rolePermissions"></param>
        /// <returns></returns>
        internal int ManagedRolePermissions(Guid roleID, List<ARolePrivilege> rolePermissions)
        {

            int result = base.ManageAssociations<ARolePrivilege>(roleID, "RoleID", rolePermissions, p => p.RoleID);
            return result;
        }

        /// <summary>
        /// 维护角色下对应的用户
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="userRoles"></param>
        /// <returns></returns>
        internal int ManagedRoleUsers(Guid roleID, List<AUserRole> userRoles)
        {
            int result = base.ManageAssociations<AUserRole>(roleID, "RoleID", userRoles, p => p.RoleID);
            return result;
        }
        /// <summary>
        /// 筛选角色信息
        /// </summary>
        /// <param name="filter"></param>

        /// <returns></returns>
        internal PartlyCollection<BRoleinfo> GetRoleList(RoleFilter filter)
        {

            WhereSqlClauseBuilder where = filter.ConvertToWhereBuilder();
            where.AppendItem("isdeleted", 0);

            QueryCondition qc = new QueryCondition(
                  filter.RowIndex,
                  filter.PageSize,
                  " * ",
                  ORMapping.GetTableName<BRoleinfo>(),
                  " Createtime ",
                 where.ToSqlString(TSqlBuilder.Instance)
                );


            PartlyCollection<BRoleinfo> result = GetPageSplitedCollection(qc);

            return result;
        }
    }
}

