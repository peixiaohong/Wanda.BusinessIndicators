using Lib.Data.AppBase;
using LJTH.BusinessIndicators.DAL.BizDal;
using LJTH.BusinessIndicators.Model.BizModel;
using LJTH.BusinessIndicators.ViewModel.Common;
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

        /// <summary>
        /// 根据板块拿到所有的大区
        /// </summary>
        /// <param name="systemID"></param>
        /// <returns></returns>
        public List<S_Organizational> GetSystem_Regional(Guid systemID)
        {
            return _s_OrganizationalAdapter.GetSystem_Regional(systemID);
        }

        /// <summary>
        /// 根据板块ID,大区Id获取项目，如果大区下没有，就拿板块下的项目
        /// </summary>
        /// <param name="systemID"></param>
        /// <param name="regionalID"></param>
        /// <returns></returns>
        public List<S_Organizational> GetCompanyByRegionalID(Guid systemID, Guid regionalID)
        {
            return _s_OrganizationalAdapter.GetCompanyByRegionalID(systemID, regionalID);
        }


        /// <summary>
        /// 根据板块ID,名称查询已有数据
        /// </summary>
        /// <param name="systemID"></param>
        /// <param name="cnName"></param>
        /// <returns></returns>
        public List<S_Organizational> GetSystemsubsetCnName(Guid systemID, string cnName)
        {
            return _s_OrganizationalAdapter.GetSystemsubsetCnName(systemID, cnName);
        }

        /// <summary>
        /// 获取组织架构【用户设置组织】
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public List<S_Organizational> GetDataByLoginName(string loginName)
        {
            return _s_OrganizationalAdapter.GetDataByLoginName(loginName);
        }

        /// <summary>
        /// 根据ID拿到实体类
        /// </summary>
        /// <param name="ID">主键ID</param>
        /// <returns></returns>
        public S_Organizational GetDataByID(Guid ID)
        {
            return base.GetModelObject(ID);
        }

        #endregion


        #region 数据权限

        #region 有判断项目 IsDelete=0
        /// <summary>
        /// 根据板块拿授权的区域
        /// </summary>
        /// <param name="systemID"></param>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public List<DataPermissions> GetUserAuthorizationArea(Guid systemID, string loginName)
        {
            return _s_OrganizationalAdapter.GetUserAuthorizationArea(systemID, loginName);
        }

        /// <summary>
        /// 根据登陆人拿所有授权的组织架构数据
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public List<S_Organizational> GetUserAuthorizationOrg(string loginName)
        {
            return _s_OrganizationalAdapter.GetUserAuthorizationOrg(loginName);
        }

        /// <summary>
        /// 根据登陆人拿到所有授权的板块
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public List<S_Organizational> GetUserSystemData(string loginName)
        {
            return _s_OrganizationalAdapter.GetUserSystemData(loginName);
        }

        /// <summary>
        /// 根据登陆人，板块ID拿到所有授权的项目
        /// </summary>
        /// <param name="systemID"></param>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public List<S_Organizational> GetUserCompanyData(Guid systemID, string loginName)
        {
            return _s_OrganizationalAdapter.GetUserCompanyData(systemID, loginName);
        }

        /// <summary>
        /// 根据登陆人，获取板块下第一层授权的大区
        /// </summary>
        /// <param name="systemID"></param>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public List<S_Organizational> GetUserRegional(Guid systemID, string loginName)
        {
            return _s_OrganizationalAdapter.GetUserRegional(systemID, loginName);
        }

        /// <summary>
        /// 根据板块ID，获取所有有效的组织架构信息
        /// </summary>
        /// <param name="systemID"></param>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public List<S_Organizational> GetAllDataBySystemID(Guid systemID)
        {
            return _s_OrganizationalAdapter.GetAllDataBySystemID(systemID);
        }
        #endregion

        #region 没有判断项目 IsDelete=0
        /// <summary>
        /// 根据板块拿授权的区域
        /// </summary>
        /// <param name="systemID"></param>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public List<DataPermissions> GetUserAuthorizationAreaNoIsDelete(Guid systemID, string loginName)
        {
            return _s_OrganizationalAdapter.GetUserAuthorizationAreaNoIsDelete(systemID, loginName);
        }

        /// <summary>
        /// 根据登陆人拿所有授权的组织架构数据
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public List<S_Organizational> GetUserAuthorizationOrgNoIsDelete(string loginName)
        {
            return _s_OrganizationalAdapter.GetUserAuthorizationOrgNoIsDelete(loginName);
        }

        /// <summary>
        /// 根据登陆人拿到所有授权的板块
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public List<S_Organizational> GetUserSystemDataNoIsDelete(string loginName)
        {
            return _s_OrganizationalAdapter.GetUserSystemDataNoIsDelete(loginName);
        }

        /// <summary>
        /// 根据登陆人，板块ID拿到所有授权的项目
        /// </summary>
        /// <param name="systemID"></param>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public List<S_Organizational> GetUserCompanyDataNoIsDelete(Guid systemID, string loginName)
        {
            return _s_OrganizationalAdapter.GetUserCompanyDataNoIsDelete(systemID, loginName);
        }

        /// <summary>
        /// 根据登陆人，获取板块下第一层授权的大区
        /// </summary>
        /// <param name="systemID"></param>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public List<S_Organizational> GetUserRegionalNoIsDelete(Guid systemID, string loginName)
        {
            return _s_OrganizationalAdapter.GetUserRegionalNoIsDelete(systemID, loginName);
        }

        #endregion 

        #endregion
    }
}

