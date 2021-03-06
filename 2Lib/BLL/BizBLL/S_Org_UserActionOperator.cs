﻿using Lib.Data.AppBase;
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
    /// 组织架构-用户
    /// </summary>
    public class S_Org_UserActionOperator : BizOperatorBase<S_Org_User>
    {
        #region field
        public static readonly S_Org_UserActionOperator Instance = PolicyInjection.Create<S_Org_UserActionOperator>();

        private static S_Org_UserAdapter _s_Org_UserAdapter = AdapterFactory.GetAdapter<S_Org_UserAdapter>();

        #endregion

        #region construction
        protected override BaseAdapterT<S_Org_User> GetAdapter()
        {
            return _s_Org_UserAdapter;
        }
        #endregion

        #region methods
        /// <summary>
        /// 根据组织架构ID查询授权数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<S_Org_User> GetDataByOrgID(Guid id)
        {
            return _s_Org_UserAdapter.GetDataByOrgID(id);
        }

        /// <summary>
        /// 删除数据，根据账号
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public int DeleteDataByLoginName(string loginName)
        {
            return _s_Org_UserAdapter.DeleteDataByLoginName(loginName);
        }

        /// <summary>
        /// 批量添加数据
        /// </summary>
        /// <param name="listData"></param>
        /// <returns></returns>
        public int InsertListData(List<S_Org_User> listData)
        {
            return _s_Org_UserAdapter.InsertList(listData);
        }

        /// <summary>
        /// 根据当前是项目的父级ID,得到 当前同批项目已经授权的人
        /// </summary>
        /// <param name="orgParentID">项目的父级ID</param>
        /// <returns></returns>
        public List<S_Org_User> GetRegionalPermissions(Guid orgParentID)
        {
            return _s_Org_UserAdapter.GetRegionalPermissions(orgParentID);
        }

        #endregion
    }
}
