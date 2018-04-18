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
    /// 菜单
    /// </summary>
    public class S_MenuActionOperator : BizOperatorBase<S_Menu>
    {
        #region field
        public static readonly S_MenuActionOperator Instance = PolicyInjection.Create<S_MenuActionOperator>();

        private static S_MenuAdapter _sMenuAdapter = AdapterFactory.GetAdapter<S_MenuAdapter>();

        #endregion

        #region construction
        protected override BaseAdapterT<S_Menu> GetAdapter()
        {
            return _sMenuAdapter;
        }
        #endregion


        #region methods

        /// <summary>
        /// 获取单个角色的菜单
        /// </summary>
        /// <param name="ID">角色ID</param>
        /// <returns></returns>
        public List<S_Menu> GetRoleMenus(Guid ID)
        {
            return _sMenuAdapter.GetRoleMenus(ID);
        }

        /// <summary>
        /// 获取人已经授予的菜单
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public List<S_Menu> GetLoinNameMenu(string loginName)
        {
            return _sMenuAdapter.GetLoinNameMenu(loginName);
        }

        #endregion
    }
}
