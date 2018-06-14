using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.DAL;
using Lib.Core;
using Lib.Validation;
using System;
using Microsoft.Practices.EnterpriseLibrary.PolicyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LJTH.Lib.Data.AppBase;
using Lib.Data.AppBase;


namespace LJTH.BusinessIndicators.BLL
{
    /// <summary>
    /// Company对象的业务逻辑操作
    /// </summary>
    public class C_CompanyOperator : BizOperatorBase<C_Company>
    {

        #region Generate Code

        public static readonly C_CompanyOperator Instance = PolicyInjection.Create<C_CompanyOperator>();

        private static C_CompanyAdapter _cCompanyAdapter = AdapterFactory.GetAdapter<C_CompanyAdapter>();

        protected override BaseAdapterT<C_Company> GetAdapter()
        {
            return _cCompanyAdapter;
        }

        public IList<C_Company> GetCompanyList()
        {
            IList<C_Company> result = _cCompanyAdapter.GetCompanyList();
            return result;
        }

        /// <summary>
        /// 根据系统ID获取公司
        /// </summary>
        /// <param name="systemID">系统ID</param>
        /// <returns>公司集合(IList<C_Company>)</returns>
        public IList<C_Company> GetCompanyList(Guid SystemID)
        {
            IList<C_Company> result = _cCompanyAdapter.GetCompanyList(SystemID);
            return result;
        }

        public Guid AddCompany(C_Company data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.AddNewModel(data);
            return result;
        }

        public C_Company GetCompany(Guid cCompanyID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(cCompanyID == null, "Argument cCompanyID is Empty");
            return base.GetModelObject(cCompanyID);
        }


        /// <summary>
        /// 获取版本数据
        /// </summary>
        /// <param name="cCompanyID"></param>
        /// <param name="VersionTime"></param>
        /// <returns></returns>
        public C_Company GetCompany(Guid cCompanyID, DateTime? VersionTime)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(cCompanyID == null, "Argument cCompanyID is Empty");

            ExceptionHelper.TrueThrow<ArgumentNullException>(VersionTime == null, "Argument VersionTime is Empty");

            List<C_Company> _CompanyList = _cCompanyAdapter.GetCompanyListByVersionTime(cCompanyID, VersionTime).ToList();

            return _CompanyList.FirstOrDefault();
        }



        public Guid UpdateCompany(C_Company data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.UpdateModelObject(data);
            return result;
        }

        public Guid RemoveCompany(Guid cCompanyID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(cCompanyID == null, "Argument cCompanyID is Empty");
            Guid result = base.RemoveObject(cCompanyID);
            return result;
        }

        public List<C_Company> GetCompanyTargetList(Guid TargetID, Guid SystemID)
        {
            return _cCompanyAdapter.GetCompanyTargetList(TargetID, SystemID);
        }


        /// <summary>
        /// 批量修改数据
        /// </summary>
        /// <param name="CompanyList"></param>
        /// <returns></returns>
        public int UpdateCompanyList(List<C_Company> CompanyList)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(CompanyList == null, "CompanyList data is Empty");

            if (CompanyList.Count > 0)
                return _cCompanyAdapter.UpdateCompanylLisr(CompanyList);
            else
                return 0;

        }

        public int AddCompanyList(List<C_Company> CompanyList)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(CompanyList == null, "CompanyList data is Empty");
            int n = 0;
            if (CompanyList.Count > 0)
            {

                foreach (C_Company item in CompanyList)
                {
                    item.ID = Guid.NewGuid();
                    item.IsDeleted = false;
                    item.NeedEvaluation = 0;
                    item.VersionStart = DateTime.Now;
                    item.VersionEnd = DateTime.Parse("9999-12-31");
                    n++;
                    AddCompany(item);
                }

            }
            if (n == CompanyList.Count)
            {
                return n;
            }
            else return 0;
        }


        /// <summary>
        /// 百货系统中获取上一个月的累计未达标，需要当月补回的公司信息
        /// </summary>
        /// <param name="FinYear"></param>
        /// <param name="FinMonth"></param>
        /// <param name="SystemID"></param>
        /// <param name="IsMissTarget"></param>
        /// <returns></returns>
        public List<C_Company> GetCompanyListBySystemID(int FinYear, int FinMonth, Guid SystemID, int IsMissTarget, bool IsLatestVersion)
        {
            return _cCompanyAdapter.GetCompanyListBySystemID(FinYear, FinMonth, SystemID, IsMissTarget, IsLatestVersion);
        }

        /// <summary>
        /// 百货系统中获取上一个月的累计其中的而一个指标是否达标，需要当月补回的公司信息
        /// </summary>
        /// <param name="FinYear"></param>
        /// <param name="FinMonth"></param>
        /// <param name="SystemID"></param>
        /// <param name="IsMissTarget"></param>
        /// <param name="TargetID"></param>
        /// <returns></returns>
        public List<C_Company> GetCompanyListBySystemIDAndTargetID(int FinYear, int FinMonth, Guid SystemID, int IsMissTarget, Guid TargetID, bool IsLatestVersion)
        {
            return _cCompanyAdapter.GetCompanyListBySystemIDAndTargetID(FinYear, FinMonth, SystemID, IsMissTarget, TargetID, IsLatestVersion);
        }

        /// <summary>
        /// 获取所有可比公司
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="TargetID"></param>
        /// <returns></returns>
        public List<C_Company> ContrastCompany(Guid SystemID, Guid TargetID)
        {
            return _cCompanyAdapter.ContrastCompany(SystemID, TargetID).ToList();
        }
        /// <summary>
        /// 获取所有不可比公司
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="TargetID"></param>
        /// <returns></returns>
        public List<C_Company> NotContrastCompany(Guid SystemID, Guid TargetID)
        {
            return _cCompanyAdapter.NotContrastCompany(SystemID, TargetID).ToList();
        }


        /// <summary>
        /// 获取上报切考核的公司
        /// </summary>
        /// <param name="TargetID"></param>
        /// <param name="SystemID"></param>
        /// <returns></returns>
        public List<C_Company> GetCompanyListByNeedEvaluation(Guid TargetID, Guid SystemID)
        {
            return _cCompanyAdapter.GetCompanyByNeedEvaluation(TargetID, SystemID);
        }

        public C_Company ProCompanyAll(Guid SystemID)
        {
            return _cCompanyAdapter.ProCompanyAll(SystemID).ToList().FirstOrDefault();
        }

        #endregion

        #region  新增方法
        /// <summary>
        /// 根据项目公司名称查询项目 注意不是模糊查询
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public List<C_Company> GetCompanyInfoByName(string name, Guid systemID)
        {
            return _cCompanyAdapter.GetCompanyInfoByName(name,systemID);
        }

        /// <summary>
        /// 根据板块ID获取板块下没有的项目
        /// </summary>
        /// <param name="systemID">板块ID</param>
        /// <param name="keyWord">模糊查询条件</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">显示行数</param>
        /// <param name="TotalCount"></param>
        /// <returns></returns>
        public List<C_Company> GetCompanyInfoBySystem(Guid systemID, string keyWord, int pageIndex, int pageSize, out int TotalCount)
        {
            return _cCompanyAdapter.GetCompanyInfoBySystem(systemID, keyWord, pageIndex, pageSize, out TotalCount);
        }
        #endregion
    }
}

