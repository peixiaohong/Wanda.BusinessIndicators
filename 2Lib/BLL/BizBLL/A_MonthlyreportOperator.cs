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
    /// Monthlyreport对象的业务逻辑操作
    /// </summary>
    public class A_MonthlyreportOperator : BizOperatorBase<A_MonthlyReport>
	{
    
        #region Generate Code

        public static readonly A_MonthlyreportOperator Instance = PolicyInjection.Create<A_MonthlyreportOperator>();

        private static A_MonthlyreportAdapter _aMonthlyreportAdapter = AdapterFactory.GetAdapter<A_MonthlyreportAdapter>();

        protected override BaseAdapterT<A_MonthlyReport> GetAdapter()
        {
            return  _aMonthlyreportAdapter;
        }

        public IList<A_MonthlyReport> GetMonthlyreportList()
        {
            IList<A_MonthlyReport> result = _aMonthlyreportAdapter.GetMonthlyreportList();
            return result;
        }

        public Guid AddMonthlyreport(A_MonthlyReport data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.AddNewModel(data);
            return result;
        }

        public A_MonthlyReport GetMonthlyreport(Guid aMonthlyreportID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(aMonthlyreportID==null ,"Argument aMonthlyreportID is Empty");
            return base.GetModelObject(aMonthlyreportID);
        }

        public Guid UpdateMonthlyreport(A_MonthlyReport data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.UpdateModelObject(data);
            return result;
        }
        public List<A_MonthlyReport> GetMonthlyreportListID(Guid aMonthlyreportID) 
        {
            List<A_MonthlyReport> result = _aMonthlyreportAdapter.GetMonthlyreportListID(aMonthlyreportID).ToList();
            return result;
        }

        public Guid RemoveMonthlyreport(Guid aMonthlyreportID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(aMonthlyreportID==null ,"Argument aMonthlyreportID is Empty");
            Guid result = base.RemoveObject(aMonthlyreportID);
            return result;
        }
        public A_MonthlyReport GetAMonthlyReport(Guid SystemID, int Year, int Month)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(SystemID == null ? true : false, "Argument SystemID is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(Year <= 0 ? true : false, "Argument Year is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(Month <= 0 ? true : false, "Argument Month is Empty");
            return _aMonthlyreportAdapter.GetLatestAMonthlyReport(SystemID, Year, Month);
        }


        public A_MonthlyReport GetAMonthlyReport(Guid SystemID,Guid AreaID, int Year, int Month,Guid TargetPlanId)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(SystemID == null ? true : false, "Argument SystemID is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(AreaID == null ? true : false, "Argument SystemID is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(Year <= 0 ? true : false, "Argument Year is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(Month <= 0 ? true : false, "Argument Month is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(TargetPlanId == null ? true : false, "Argument TargetPlanId is Empty");
            return _aMonthlyreportAdapter.GetLatestAMonthlyReport(SystemID, AreaID, Year, Month,TargetPlanId);
        }


        public A_MonthlyReport GetAMonthlyReport(Guid SystemID,Guid AreaID, int Year, int Month)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(SystemID == null ? true : false, "Argument SystemID is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(AreaID == null ? true : false, "Argument SystemID is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(Year <= 0 ? true : false, "Argument Year is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(Month <= 0 ? true : false, "Argument Month is Empty");
            return _aMonthlyreportAdapter.GetLatestAMonthlyReport(SystemID, AreaID, Year, Month);
        }
        /// <summary>
        /// 物理删除
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public int DeleteModel(A_MonthlyReport Model)
        {
            return _aMonthlyreportAdapter.Delete(Model);
        }

        public void InsertAllFromB(Guid monthlyReportID)
        {
            _aMonthlyreportAdapter.InsertAllFromB(monthlyReportID);
        }
        #endregion

    } 
}

