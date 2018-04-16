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
        ///// <summary>
        ///// 批量插入数据
        ///// </summary>
        ///// <param name="datas"></param>
        ///// <returns></returns>s
        //public int InsertListData(List<S_Role_User> datas)
        //{
        //    return base.InsertList(datas);
        //}

        ///// <summary>
        ///// 批量删除
        ///// </summary>
        ///// <param name="datas"></param>
        ///// <returns></returns>
        //public int DeleteListData(List<S_Role_User> datas)
        //{
        //    return base.DeleteList(datas);
        //}

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
        public int DelteDataByRoleID_LoginName(Guid roleID, string loginName)
        {
            string sql = string.Format(@"Delete [dbo].[S_Role_User] Where [RoleID]='{0}' And [LoginName]='{1}'", roleID, loginName);
            return ExecuteSql(sql);
        }
    }
}
