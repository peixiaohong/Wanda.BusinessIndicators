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
   public class S_RoleActionOperator : BizOperatorBase<S_Role>
    {
        #region field
        public static readonly S_RoleActionOperator Instance = PolicyInjection.Create<S_RoleActionOperator>();

        private static S_RoleAdapter _s_RoleAdapter = AdapterFactory.GetAdapter<S_RoleAdapter>();

        #endregion

        #region construction
        protected override BaseAdapterT<S_Role> GetAdapter()
        {
            return _s_RoleAdapter;
        }
        #endregion

        #region methods
        #endregion 
    }
}
