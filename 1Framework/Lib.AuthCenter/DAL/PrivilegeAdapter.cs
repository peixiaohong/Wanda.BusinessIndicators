using Lib.Core;
using Lib.Data;
/*
 * Create by Hu Wei Zheng
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Wanda.Lib.AuthCenter.Model;
using Wanda.Lib.Data.AppBase;




namespace Wanda.Lib.AuthCenter.DAL
{
    /// <summary>
    /// BPrivilege对象的数据访问适配器
    /// </summary>
    sealed class PrivilegeAdapter : AuthBaseAdapterT<BPrivilege>, IUsage
    {
        /// <summary>
        /// 获得所有的权限项列表
        /// </summary>
        /// <returns></returns>
        public List<BPrivilege> GetPrivilegeList()
        {
            string sql = ORMapping.GetSelectSql<BPrivilege>(TSqlBuilder.Instance);

            sql += " WHERE " + base.NotDeleted;

            return ExecuteQuery(sql);
        }

        /// <summary>
        /// 根据类型Id获取权限
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>
        public List<BPrivilege> GetPrivilegeListByTypeId(int typeId)
        {
            string sql = ORMapping.GetSelectSql<BPrivilege>(TSqlBuilder.Instance);

            sql += " WHERE " + base.NotDeleted + " and PrivilegeType=" + typeId;

            return ExecuteQuery(sql);
        }


        public PartlyCollection<BPrivilege> GetResearchResult(PrivilegeFilter filter)
        {
            WhereSqlClauseBuilder where = filter.ConvertToWhereBuilder();
            where.AppendItem("Isdeleted", 0);

            QueryCondition qc = new QueryCondition(
                  filter.RowIndex,
                  filter.PageSize,
                  " * ",
                  ORMapping.GetTableName<BPrivilege>(),
                  " Createtime ",
                 where.ToSqlString(TSqlBuilder.Instance)
                );


            PartlyCollection<BPrivilege> result = GetPageSplitedCollection(qc);
            return result;


        }

        /// <summary>
        /// 获得指定角色对应的权限项
        /// </summary>
        /// <param name="roleID"></param>
        /// <returns></returns>
        public List<BPrivilege> GetPrivilegesOfRole(Guid roleID)
        {
            //   StringBuilder sql = new StringBuilder(ORMapping.GetSelectSql<BPrivilege>(TSqlBuilder.Instance));
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(" SELECT  A.* from {0} as A ", ORMapping.GetTableName(typeof(BPrivilege)));
            sql.AppendFormat("  INNER JOIN  {0} as B on A.ID=B.PrivilegeID", ORMapping.GetTableName(typeof(ARolePrivilege)));
            sql.AppendFormat(" WHERE  B.RoleID='{0}'", roleID);
            sql.Append(" AND  A.IsDeleted<1 AND B.IsDeleted<1");

            return ExecuteQuery(sql.ToString());
        }


        /// <summary>
        /// 获得指定用户直接对应的权限项
        /// </summary>
        /// <param name="roleID"></param>
        /// <returns></returns>
        public List<BPrivilege> GetDirectedPrivilegesOfUser(Guid userID)
        {
            //StringBuilder sql = new StringBuilder(ORMapping.GetSelectSql<BPrivilege>(TSqlBuilder.Instance));
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(" SELECT  A.* from {0} as A ", ORMapping.GetTableName(typeof(BPrivilege)));
            sql.AppendFormat("  INNER JOIN  {0} as B on A.ID=B.PrivilegeID", ORMapping.GetTableName(typeof(AUserPrivilege)));
            sql.AppendFormat(" WHERE  B.UserID='{0}'", userID);
            sql.Append(" AND  A.IsDeleted<1 AND B.IsDeleted<1");

            return ExecuteQuery(sql.ToString());
        }

        /// <summary>
        /// 获得指定权限项组下的权限项
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public List<BPrivilege> GetPrivilegesOfGroup(string groupID)
        {
            string sql = ORMapping.GetSelectSql<BPrivilege>(TSqlBuilder.Instance);

            sql += " WHERE " + base.NotDeleted;
            sql += string.Format(" AND groupID='{0}'", SqlTextHelper.SafeQuote(groupID));

            return ExecuteQuery(sql);
        }


        /// <summary>
        /// 返回重名但ID不重复的项
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<BPrivilege> GetExists(string name, Guid groupId, Guid id)
        {
            var list = this.Load(p =>
            {
                p.AppendItem("Name", SqlTextHelper.SafeQuote(name));
                p.AppendItem("GroupId", groupId);
                p.AppendItem("ID", id, "<>");
                p.AppendItem("ISDELETED", "1", "<>");

            });
            return list;
        }

        /// <summary>
        /// 关联关系的数量
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public int UsageCount(Guid ID)
        {
            int result = 0;

            string sql = string.Format(@"
SELECT COUNT(*) FROM {0} A 
WHERE A.PrivilegeID='{1}' AND A.ISDELETED<1",
                                            ORMapping.GetTableName(typeof(ARolePrivilege)), ID);

            result = (int)DbHelper.RunSqlReturnScalar(sql, ConnectionName);
            return result;
        }

        /// <summary>
        /// 更新权限对应的角色
        /// </summary>
        /// <param name="privilegeID"></param>
        /// <param name="newPrivilegeRoleMappings"></param>
        /// <returns></returns>
        internal int ManagedPrivilegeRoles(Guid privilegeID, List<ARolePrivilege> newPrivilegeRoleMappings)
        {
            int result = base.ManageAssociations<ARolePrivilege>(privilegeID, "privilegeID", newPrivilegeRoleMappings, p => p.PrivilegeID);
            return result;
        }

        /// <summary>
        /// 更新权限直接对应的用户
        /// </summary>
        /// <param name="privilegeID"></param>
        /// <param name="newPrivilegeRoleMappings"></param>
        /// <returns></returns>
        internal int ManagedPrivilegeUsers(Guid privilegeID, List<AUserPrivilege> newPrivilegeUserMappings)
        {
            int result = base.ManageAssociations<AUserPrivilege>(privilegeID, "privilegeID", newPrivilegeUserMappings, p => p.PrivilegeID);
            return result;
        }

        /// <summary>
        /// 根据指定条件获取权限列表  li jing guang 2013-05-13
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        internal List<BPrivilege> GetPrivilegeByGroupID(string groupName)
        {
            string sql = ORMapping.GetSelectSql<BPrivilege>(TSqlBuilder.Instance);

            sql += string.Format(" WHERE {0} and GroupName='{1}' and PrivilegeType='URL'", base.NotDeleted, groupName);

            return ExecuteQuery(sql);
        }
        /// <summary>
        /// 根据ID查询操作权限
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        internal List<BPrivilege> GetPrivilegeByID(string code)
        {
            string sql = ORMapping.GetSelectSql<BPrivilege>(TSqlBuilder.Instance);

            sql += string.Format(" WHERE {0} and Code like'{1}%' and PrivilegeType='action'", base.NotDeleted, code);

            return ExecuteQuery(sql);
        }


        internal List<BPrivilege> GetPrivilegeByCode(string code, PrivilegeType privilegeType)
        {
            string sql = ORMapping.GetSelectSql<BPrivilege>(TSqlBuilder.Instance);

            WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
            where.AppendItem("isdeleted", 0);
            where.AppendItem("PrivilegeType", privilegeType.ToString());
            where.AppendItem("Code", SqlTextHelper.SafeQuote(code), "startwith");
            sql += " where " + where.ToSqlString(TSqlBuilder.Instance);

            return ExecuteQuery(sql);
        }

        /// <summary>
        /// 查询所有有操作权限的页面权限
        /// </summary>
        /// <returns></returns>
        public List<BPrivilege> GetPrivilegeListAction()
        {
            List<BPrivilege> reVal = new List<BPrivilege>();
            string sql = @"SELECT * FROM PB_Privilege WHERE Code IN(
SELECT distinct SUBSTRING(CODE,0,LEN(CODE)) FROM (SELECT LEFT(P.Code,CHARINDEX('#',P.Code,0)) 
AS CODE  FROM PB_Privilege P) T WHERE T.CODE!='') AND PrivilegeType='URL' and IsDeleted=0";

            DataTable dt = ExecuteReturnTable(sql, null);
            if (null != dt && dt.Rows.Count > 0)
            {
                foreach (DataRow item in dt.Rows)
                {
                    BPrivilege privilege = new BPrivilege();
                    privilege.Code = item["Code"].ToString();
                    privilege.GroupID = item["GroupID"].ToString();
                    privilege.GroupName = item["GroupName"].ToString();
                    privilege.ID = Guid.Parse(item["ID"].ToString());
                    privilege.PrivilegeType = item["PrivilegeType"].ToString();
                    privilege.Name = item["Name"].ToString();
                    privilege.CreatorName = item["CreatorName"].ToString();
                    privilege.ModifierName = item["ModifierName"].ToString();
                    privilege.ModifyTime = Convert.ToDateTime(item["ModifyTime"].ToString());
                    privilege.CreateTime = Convert.ToDateTime(item["CreateTime"].ToString());

                    reVal.Add(privilege);
                }
            }

            return reVal;
        }

    }
}

