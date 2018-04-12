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
        #endregion 
    }
}
