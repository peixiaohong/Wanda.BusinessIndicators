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
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int InsertData(S_Organizational data)
        {
            return _s_OrganizationalAdapter.Insert(data);
        }

        /// <summary>
        /// 获取全部有效的数据
        /// </summary>
        /// <returns></returns>
        public List<S_Organizational> GetAllData()
        {
            return _s_OrganizationalAdapter.GetAllData();
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int UpdateData(S_Organizational data)
        {
            var olddata = _s_OrganizationalAdapter.GetModelByID(data.ID);
            data.CreateTime = olddata.CreateTime;
            data.CreatorName = olddata.CreatorName;
            return _s_OrganizationalAdapter.Update(data);
        }

        /// <summary>
        /// 逻辑删除单条数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeleteData(Guid id)
        {
            var data = _s_OrganizationalAdapter.GetModelByID(id);
            return _s_OrganizationalAdapter.DeleteData(data);
        }

        /// <summary>
        /// 获取下层子节点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<S_Organizational> GetChildDataByID(Guid id)
        {
            return _s_OrganizationalAdapter.GetChildDataByID(id);
        }

        #endregion
    }
}

