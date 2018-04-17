using LJTH.BusinessIndicators.Model.BizModel;
using System;
using System.Collections.Generic;
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
            string sql = string.Format(@"Select * From [dbo].[S_Role_User] Where [IsDeleted]=0 And [RoleID]='{0}'", RoleID);

            return ExecuteQuery(sql).Select(o => o.LoginName).ToList<string>();
        }

        /// <summary>
        /// 根据角色ID,用户账号删除对应的关系
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public int DeleteDataByRoleID_LoginName(Guid roleID, string loginName)
        {
            string sql = string.Format(@"Delete [dbo].[S_Role_User] Where [RoleID]='{0}' And [LoginName]='{1}'", roleID, loginName);
            return ExecuteSql(sql);
        }

        /// <summary>
        /// 根据用户账号，删除所有的 用户-角色 关系
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public int DeleteDatasByLoginName(string loginName)
        {
            string sql = string.Format(@"Delete [dbo].[S_Role_User] Where [LoginName]='{0}'",loginName);
            return ExecuteSql(sql);
        }
    }
}
