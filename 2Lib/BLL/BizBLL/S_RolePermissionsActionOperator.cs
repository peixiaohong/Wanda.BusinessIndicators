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
    /// 角色权限
    /// </summary>
    public class S_RolePermissionsActionOperator : BizOperatorBase<S_RolePermissions>
    {
        #region field
        public static readonly S_RolePermissionsActionOperator Instance = PolicyInjection.Create<S_RolePermissionsActionOperator>();

        private static S_RolePermissionsAdapter _s_RolePermissionsAdapter = AdapterFactory.GetAdapter<S_RolePermissionsAdapter>();

        #endregion

        #region construction
        protected override BaseAdapterT<S_RolePermissions> GetAdapter()
        {
            return _s_RolePermissionsAdapter;
        }
        #endregion


        #region methods

        /// <summary>
        /// 修改/保存 角色菜单权限
        /// </summary>
        /// <param name="RoleID">角色ID</param>
        /// <param name="listData">权限表数据</param>
        /// <returns></returns>
        public int SaveListData(Guid RoleID, List<S_RolePermissions> listData)
        {
            List<S_RolePermissions> oldlistData = _s_RolePermissionsAdapter.GetListData(RoleID);
            int dNumber = 1;
            if (oldlistData.Count > 0)
            {
                dNumber = _s_RolePermissionsAdapter.DeleteList(oldlistData);
            }
            if (dNumber > 0)
            {
                if (listData.Count > 0)
                {
                    return _s_RolePermissionsAdapter.InsertList(listData);
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                return 0;
            }
        }

        #endregion 
    }
}
