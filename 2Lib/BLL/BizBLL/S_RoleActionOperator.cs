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

        /// <summary>
        /// 获取全部有效的角色
        /// </summary>
        /// <returns></returns>
        public List<S_Role> GetDatas(string CnName)
        {
            return _s_RoleAdapter.GetDatas(CnName);
        }

        /// <summary>
        /// 获取名称是一样的角色
        /// </summary>
        /// <param name="cnName"></param>
        /// <returns></returns>
        public List<S_Role> GetDatasByCnName(string cnName)
        {
            return _s_RoleAdapter.GetDatasByCnName(cnName);
        }

        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public int InsertData(S_Role data)
        {
            return _s_RoleAdapter.InsertData(data);
        }

        /// <summary>
        /// 根据ID获取一条数据
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public S_Role GetRoleByID(Guid ID)
        {
            return base.GetModelObject(ID);
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Guid UpdateData(S_Role data)
        {
            return base.UpdateModelObject(data);
        }

        /// <summary>
        /// 获取全部有效角色 【用于用户授权角色页面】
        /// </summary>
        /// <param name="cnName"></param>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public List<S_Role> GetDatasByCnName(string cnName, string loginName)
        {
            return _s_RoleAdapter.GetDatasByCnName(cnName, loginName);
        }
        #endregion
    }
}
