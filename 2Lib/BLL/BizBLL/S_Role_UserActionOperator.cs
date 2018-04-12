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
        #endregion 
    }
}
