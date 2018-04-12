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
    /// 组织架构
    /// </summary>
    public class S_OrganizationalActionOperator : BizOperatorBase<S_Organizational>
    {
        #region field
        public static readonly S_OrganizationalActionOperator Instance = PolicyInjection.Create<S_OrganizationalActionOperator>();

        private static S_OrganizationalAdapter _s_OrganizationalAdapter = AdapterFactory.GetAdapter<S_OrganizationalAdapter>();

        #endregion

        #region construction
        protected override BaseAdapterT<S_Organizational> GetAdapter()
        {
            return _s_OrganizationalAdapter;
        }
        #endregion

        #region methods
        #endregion 
    }
}
