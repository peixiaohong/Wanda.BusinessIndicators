using LJTH.BusinessIndicators.Model.BizModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJTH.BusinessIndicators.DAL.BizDal
{
    /// <summary>
    /// 角色-用户关系
    /// </summary>
    sealed class S_Role_UserAdapter : AppBaseAdapterT<S_Role_User>
    {
        /// <summary>
        /// 根据角色ID, 查询已经配置的所有账号
        /// </summary>
        /// <param name="RoleID"></param>
        /// <returns></returns>
        public List<string> GetDataByRoleID(Guid RoleID)
        {
            string sql = "Select * From [dbo].[S_Role_User] Where [IsDeleted]=0 And [RoleID]=@RoleID";
            DbParameter[] parameters = new DbParameter[]
            {
                 CreateSqlParameter("@RoleID",DbType.Guid,RoleID),
            };
            return ExecuteQuery(sql, parameters).Select(o => o.LoginName).ToList<string>();
        }

        /// <summary>
        /// 根据角色ID,用户账号删除对应的关系
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public int DeleteDataByRoleID_LoginName(Guid roleID, string loginName)
        {
            string sql = string.Format(@"Delete [dbo].[S_Role_User] Where [RoleID]='{0}' And [LoginName]=@LoginName", roleID, loginName);
            DbParameter[] parameters = new DbParameter[]
            {
                 CreateSqlParameter("@RoleID",DbType.Guid,roleID),
                 CreateSqlParameter("@LoginName",DbType.String,loginName)
            };
            return ExecuteSql(sql, parameters);
        }

        /// <summary>
        /// 根据用户账号，删除所有的 用户-角色 关系
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public int DeleteDatasByLoginName(string loginName)
        {
            string sql = "Delete [dbo].[S_Role_User] Where [LoginName]=@LoginName";
            DbParameter[] parameters = new DbParameter[]
            {
                 CreateSqlParameter("@LoginName",DbType.String,loginName)
            };
            return ExecuteSql(sql, parameters);
        }

        /// <summary>
        /// 根据角色ID,登陆账号删除对应的权限
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public int DeleteDAtaByLoginNameAndRoleID(Guid roleID, string loginName)
        {
            string sql = "Delete [dbo].[S_Role_User] Where [RoleID]=@RoleID And [LoginName]=@LoginName";
            DbParameter[] parameters = new DbParameter[]
            {
                 CreateSqlParameter("@RoleID",DbType.Guid,roleID),
                 CreateSqlParameter("@LoginName",DbType.String,loginName)
            };
            return ExecuteSql(sql, parameters);
        }
    }
}
