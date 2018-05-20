using Lib.Data.AppBase;
using LJTH.BusinessIndicators.DAL.BizDal;
using LJTH.BusinessIndicators.Model.BizModel;
using LJTH.Lib.Data.AppBase;
using Microsoft.Practices.EnterpriseLibrary.PolicyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJTH.BusinessIndicators.BLL.BizBLL
{
    /// <summary>
    /// 角色-用户
    /// </summary>
    public class S_Role_UserActionOperator : BizOperatorBase<S_Role_User>
    {
        #region field
        public static readonly S_Role_UserActionOperator Instance = PolicyInjection.Create<S_Role_UserActionOperator>();

        private static S_Role_UserAdapter _s_Role_UserAdapter = AdapterFactory.GetAdapter<S_Role_UserAdapter>();

        #endregion

        #region construction
        protected override BaseAdapterT<S_Role_User> GetAdapter()
        {
            return _s_Role_UserAdapter;
        }
        #endregion


        #region methods

        /// <summary>
        /// 根据角色ID, 查询已经配置的所有账号
        /// </summary>
        /// <param name="RoleID"></param>
        /// <returns></returns>
        public List<string> GetDataByRoleID(Guid RoleID)
        {
            return _s_Role_UserAdapter.GetDataByRoleID(RoleID);
        }

        /// <summary>
        /// 批量新增数据
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        public int InsertListData(List<S_Role_User> datas)
        {
            return _s_Role_UserAdapter.InsertList(datas);
        }

        /// <summary>
        /// 根据角色ID,用户账号删除对应的关系
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public int DelteDataByRoleID_LoginName(Guid roleID, string loginName)
        {
            return _s_Role_UserAdapter.DeleteDataByRoleID_LoginName(roleID, loginName);
        }

        /// <summary>
        /// 根据用户账号，删除所有的 用户-角色 关系
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public int DeleteDatasByLoginName(string loginName)
        {
            return _s_Role_UserAdapter.DeleteDatasByLoginName(loginName);
        }

        /// <summary>
        /// 根据角色ID,登陆账号删除对应的权限
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public int DeleteDAtaByLoginNameAndRoleID(Guid roleID, string loginName)
        {
            return _s_Role_UserAdapter.DeleteDAtaByLoginNameAndRoleID(roleID, loginName);
        }

        #endregion
    }
}

