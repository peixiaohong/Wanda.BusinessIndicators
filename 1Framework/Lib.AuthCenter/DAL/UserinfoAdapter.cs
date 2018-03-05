using Lib.Core;
/*
 * Create by Hu Wei Zheng

 */
using Lib.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Wanda.Lib.AuthCenter.Model;
using Wanda.Lib.Data.AppBase;


namespace Wanda.Lib.AuthCenter.DAL
{
    /// <summary>
    /// Userinfo对象的数据访问适配器
    /// </summary>
    internal sealed class UserinfoAdapter : AuthBaseAdapterT<BUserinfo>
    {
        /// <summary>
        /// 获得所有的用户列表
        /// </summary>
        /// <returns></returns>
        public IList<BUserinfo> GetUserinfoList()
        {
            string sql = ORMapping.GetSelectSql<BUserinfo>(TSqlBuilder.Instance);

            sql += "WHERE " + base.NotDeleted;

            return ExecuteQuery(sql);
        }


        ///// <summary>
        ///// 检测UserInfo 表中 对应的 WD_User表中的用户状态 返回离职状态的用户信息
        ///// </summary>
        ///// <returns></returns>
        //public IList<BUserinfo> GetResignedEmployees()
        //{
        //    StringBuilder sql = new StringBuilder();
        //    sql.AppendFormat(" SELECT  A.* from {0} as A ", ORMapping.GetTableName(typeof(BUserinfo)));
        //    sql.AppendFormat(" INNER JOIN  {0} as B on A.WD_UserID=B.employeeCode", "WD_User");
        //    sql.AppendFormat(" WHERE  B.employeeStatus='{0}'", "3");
        //    sql.Append(" AND  A.IsDeleted<1 and A.IsForbidden=0");

        //    return ExecuteQuery(sql.ToString());
        //}

        /// <summary>
        /// 根据万达用户Id查询用户是否存在UserInfo表中
        /// </summary>
        /// <param name="WdUserId"></param>
        /// <returns></returns>
        public BUserinfo GetUserInfoByWDUserID(int WdUserId)
        {

            WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();

            string idField = "Wd_UserId"; //TODO , 通过ORM 反射找到对应的Field; 

            //ORMapping.GetMappingInfo<T>()[]

            where.AppendItem(idField, WdUserId);

            string sqlString = ORMapping.GetSelectSql<BUserinfo>(TSqlBuilder.Instance)
                + " where "
                + where.ToSqlString(TSqlBuilder.Instance)
                + " AND " + NotDeleted;

            var listResult = this.ExecuteQuery(sqlString, null);

            return listResult.FirstOrDefault();
        }
 



        /// <summary>
        /// 找到指定角色下的用户
        /// </summary>
        /// <param name="roleID"></param>
        /// <returns></returns>
        public IList<BUserinfo> GetUsersOfSpecificRole(Guid roleID)
        {
            //StringBuilder sql = new StringBuilder(ORMapping.GetSelectSql<Userinfo>(TSqlBuilder.Instance));
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(" SELECT  A.* from {0} as A ", ORMapping.GetTableName(typeof(BUserinfo)));
            sql.AppendFormat(" INNER JOIN  {0} as B on A.ID=B.UserID", ORMapping.GetTableName(typeof(AUserRole)));
            sql.AppendFormat(" WHERE  B.RoleID={0}", roleID);
            sql.Append(" AND  A.IsDeleted<1 AND B.IsDeleted<1");

            return ExecuteQuery(sql.ToString());
        }




        /// <summary>
        /// 找到指定权限直接分配给的用户
        /// </summary>
        /// <param name="roleID"></param>
        /// <returns></returns>
        public IList<BUserinfo> GetUsersOfSpecificPrivilege(Guid privilegeID)
        {
            //StringBuilder sql = new StringBuilder(ORMapping.GetSelectSql<Userinfo>(TSqlBuilder.Instance));
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(" SELECT  A.* from {0} as A ", ORMapping.GetTableName(typeof(BUserinfo)));
            sql.AppendFormat(" INNER JOIN  {0} as B on A.ID=B.UserID", ORMapping.GetTableName(typeof(AUserPrivilege)));
            sql.AppendFormat(" WHERE  B.PrivilegeID='{0}'", privilegeID);
            sql.Append(" AND  A.IsDeleted<1 AND B.IsDeleted<1");

            return ExecuteQuery(sql.ToString());
        }


        /// <summary>
        /// 根据用户名称找到用户 li jing guang 2013-06-04
        /// </summary> 
        /// <param name="userName"></param>
        /// <returns></returns>
        public BUserinfo GetUserInfoByName(string userName)
        {
            string sql = string.Format(@"
select * from {0} 
where LoginName='{1}' 
    and {2}",
            ORMapping.GetTableName(typeof(BUserinfo)),
             SqlTextHelper.SafeQuote(userName),
            base.NotDeleted);

            IList<BUserinfo> list = ExecuteQuery(sql);

            return list.FirstOrDefault();


        }

        //internal BUserinfo GetUserinfoByWdUid(int wdUserID)
        //{
        //    string SQL = string.Format("SELECT * FROM {0} WHERE  WD_UserID={1} AND {2}", ORMapping.GetTableName<BUserinfo>(), wdUserID, NotDeleted);
        //    List<BUserinfo> users = ExecuteQuery(SQL);
        //    return users.FirstOrDefault();
        //} //代码功能重复




        /// <summary>
        /// 返回重名但ID不重复的项
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        internal IList<BUserinfo> GetExists(string name, string jobTitle, Guid id)
        {
            var result = this.Load(p =>
            {
                p.AppendItem("Name", SqlTextHelper.SafeQuote(name));
                p.AppendItem("JobTitle", SqlTextHelper.SafeQuote(jobTitle));
                if (id != null && id!=Guid.Empty)
                {
                    p.AppendItem("ID", id, "<>");
                }
                p.AppendItem("ISDELETED", "1", "<>");

            });
            return result;
        }

        /// <summary>
        /// 维护用户直接获得的授权
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="rolePermissions"></param>
        /// <returns></returns>
        internal int ManagedUserPermissions(Guid userID, List<AUserPrivilege> userPermissions)
        {

            int result = base.ManageAssociations<AUserPrivilege>(userID, "UserID", userPermissions, p => p.UserID);
            return result;
        }

        /// <summary>
        /// 维护用户所属的角色
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="userRoles"></param>
        /// <returns></returns>
        internal int ManagedUserRoles(Guid userID, List<AUserRole> userRoles)
        {
            int result = base.ManageAssociations<AUserRole>(userID, "UserID", userRoles, p => p.UserID);
            return result;
        }
        /// <summary>
        /// 验证用户,密码暂时未使用
        /// </summary>
        /// <param name="strUserName">用户名</param>
        /// <param name="strPassword"></param>
        /// <returns></returns>
        internal BUserinfo ValidateUser(string strUserName, string strPassword)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(" SELECT * from {0}  ", ORMapping.GetTableName(typeof(BUserinfo)));
            sql.AppendFormat(" WHERE  LoginName='{0}'", SqlTextHelper.SafeQuote(strUserName));
            sql.Append(" AND  IsDeleted<1");
            List<BUserinfo> result = ExecuteQuery(sql.ToString());
            return result.FirstOrDefault();
        }

        internal List<BUserinfo> GetBatchModelObjects(List<Guid> userIDs)
        {
            return base.Load(p => p.AppendItem("IsDeleted", "0")).FindAll(p => userIDs.Contains(p.ID));
        }
        /// <summary>
        /// 更新userinfo的jobTitle
        /// </summary>
        /// <returns></returns>
        public int UpdateUserInfoJob()
        {
            string sql = @"update PB_UserInfo set JobTitle = (select jobName from Wd_User where employeeCode=WD_UserID) 
                                  , Department=(select unitName from Wd_User where employeeCode=WD_UserID)";
            return base.ExecuteSql(sql);
        }
    }
}

