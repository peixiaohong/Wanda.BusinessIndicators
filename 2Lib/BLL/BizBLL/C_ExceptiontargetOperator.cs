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
using LJTH.BusinessIndicators.ViewModel;


namespace LJTH.BusinessIndicators.BLL
{
    /// <summary>
    /// Exceptiontarget对象的业务逻辑操作
    /// </summary>
    public class C_ExceptiontargetOperator : BizOperatorBase<C_ExceptionTarget>
    {

        #region Generate Code

        public static readonly C_ExceptiontargetOperator Instance = PolicyInjection.Create<C_ExceptiontargetOperator>();

        private static C_ExceptiontargetAdapter _cExceptiontargetAdapter = AdapterFactory.GetAdapter<C_ExceptiontargetAdapter>();

        protected override BaseAdapterT<C_ExceptionTarget> GetAdapter()
        {
            return _cExceptiontargetAdapter;
        }

        public IList<C_ExceptionTarget> GetExceptiontargetList()
        {
            IList<C_ExceptionTarget> result = _cExceptiontargetAdapter.GetExceptiontargetList();
            return result;
        }
        /// <summary>
        /// 获取ExceptionType小于3的数据
        /// </summary>
        /// <returns></returns>
        public IList<C_ExceptionTarget> GetExceptionTList()
        {

            IList<C_ExceptionTarget> result = _cExceptiontargetAdapter.GetExceptionTList();
            return result;
        }
        public IList<C_ExceptionTarget> GetNotContrastList(Guid TargetID)
        {
            IList<C_ExceptionTarget> result = _cExceptiontargetAdapter.GetNotContrastList(TargetID);
            return result;
        }


        public IList<C_ExceptionTarget> GetExceptiontargetList(Guid CompanyID, Guid TargetID)
        {
            IList<C_ExceptionTarget> result = _cExceptiontargetAdapter.GetExceptiontargetList(CompanyID, TargetID);
            return result;
        }

        public IList<C_ExceptionTarget> GetExcTargetListBytargetID(Guid TargetID)
        {
            IList<C_ExceptionTarget> result = _cExceptiontargetAdapter.GetExcTargetListBytargetID(TargetID);
            return result;
        }

        /// <summary>
        /// 获取不上报 不考核数据
        /// </summary>
        /// <param name="TargetID"></param>
        /// <returns></returns>
        public IList<C_ExceptionTarget> GetNotUpdateListBytargetID(Guid TargetID)
        {
            IList<C_ExceptionTarget> result = _cExceptiontargetAdapter.GetNotUpdateListBytargetID(TargetID);
            return result;
        }
        public Guid AddExceptiontarget(C_ExceptionTarget data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.AddNewModel(data);
            return result;
        }

        public C_ExceptionTarget GetExceptiontarget(Guid cExceptiontargetID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(cExceptiontargetID == null, "Argument cExceptiontargetID is Empty");
            return base.GetModelObject(cExceptiontargetID);
        }

        public Guid UpdateExceptiontarget(C_ExceptionTarget data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.UpdateModelObject(data);
            return result;
        }

        public Guid RemoveExceptiontarget(Guid cExceptiontargetID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(cExceptiontargetID == null, "Argument cExceptiontargetID is Empty");
            Guid result = base.RemoveObject(cExceptiontargetID);
            return result;
        }


        public IList<ExceptionTargetVModel> GetExctargetListByComList(Guid CompanyID)
        {
            IList<ExceptionTargetVModel> result = _cExceptiontargetAdapter.GetExctargetListByComList(CompanyID);
            return result;
        }

        public IList<ExceptionCompanyVModel> GetExctargetListByTarList(Guid TargetID, int ExceptionType)
        {
            IList<ExceptionCompanyVModel> result = _cExceptiontargetAdapter.GetExctargetListByTarList(TargetID, ExceptionType);
            return result;
        }
        public IList<LastExceptionCompanyVModel> GetLastExctargetListByTarList(Guid TargetID, Guid SystemID)
        {
            IList<LastExceptionCompanyVModel> result = _cExceptiontargetAdapter.GetLastExctargetListByTarList(TargetID, SystemID);
            return result;
        }
        public IList<LastExceptionCompanyVModel> GetNoExceptionReplaceList(Guid TargetID, Guid SystemID)
        {
            IList<LastExceptionCompanyVModel> result = _cExceptiontargetAdapter.GetNoExceptionReplaceList(TargetID, SystemID);
            return result;
        }




        public IList<ContrastAllCompanyVM> GetContrastAllCompany(int FinYear, int FinMonth, Guid ID, Guid TargetID, bool IfNow)
        {
            IList<ContrastAllCompanyVM> result = _cExceptiontargetAdapter.GetContrastAllCompany(FinYear, FinMonth, ID, TargetID, IfNow);
            return result;
        }
        /// <summary>
        /// 获取可比公司数量
        /// </summary>
        /// <param name="FinYear"></param>
        /// <param name="FinMonth"></param>
        /// <param name="SystemID"></param>
        /// <returns></returns>
        public IList<ContrastAllCompanyVM> GetContrastCompanyTotal(int FinYear, int FinMonth, Guid ID, Guid TargetID, bool IfNow)
        {
            IList<ContrastAllCompanyVM> result = _cExceptiontargetAdapter.GetContrastCompanyTotal(FinYear, FinMonth, ID, TargetID, IfNow);
            return result;
        }
        /// <summary>
        /// 获取不可比公司数量
        /// </summary>
        /// <param name="FinYear"></param>
        /// <param name="FinMonth"></param>
        /// <param name="SystemID"></param>
        /// <returns></returns>
        public IList<ContrastAllCompanyVM> GetNoContrastCompanyTotal(int FinYear, int FinMonth, Guid ID, Guid TargetID, bool IfNow)
        {
            IList<ContrastAllCompanyVM> result = _cExceptiontargetAdapter.GetNoContrastCompanyTotal(FinYear, FinMonth, ID, TargetID, IfNow);
            return result;
        }

        public List<Guid> AddExceptiontargetList(List<string> IDlist, Guid CompanyID)
        {
            List<Guid> result = new List<Guid>();
            for (int i = 0; i < IDlist.Count; i++)
            {
                C_ExceptionTarget date = new C_ExceptionTarget();
                date.CompanyID = CompanyID;
                date.ExceptionType = 2;
                date.TargetID = Guid.Parse(IDlist[i]);
                Guid ID = AddExceptiontarget(date);
                result.Add(ID);
            }
            return result;
        }




        #endregion
    }
}

