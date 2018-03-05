using Lib.Core;
using Lib.Data.AppBase;
using Microsoft.Practices.EnterpriseLibrary.PolicyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wanda.BusinessIndicators.DAL;
using Wanda.BusinessIndicators.Model;
using Wanda.Lib.Data.AppBase;

namespace Wanda.BusinessIndicators.BLL
{
    public class B_MonthlyReportActionOperator : BizOperatorBase<B_MonthlyReportAction>
    {
        #region Generate Code

        public static readonly B_MonthlyReportActionOperator Instance = PolicyInjection.Create<B_MonthlyReportActionOperator>();

        private static B_MonthlyReportActionAdapter _bMonthlyReportActionAdapter = AdapterFactory.GetAdapter<B_MonthlyReportActionAdapter>();

        protected override BaseAdapterT<B_MonthlyReportAction> GetAdapter()
        {
            return _bMonthlyReportActionAdapter;
        }

        public IList<B_MonthlyReportAction> GetMonthlyReportActionList()
        {
            IList<B_MonthlyReportAction> result = _bMonthlyReportActionAdapter.GetMonthlyReportActionList();
            return result;
        }

        public IList<B_MonthlyReportAction> GetMonthlyReportActionList(Guid businessID)
        {
            IList<B_MonthlyReportAction> result = _bMonthlyReportActionAdapter.GetMonthlyReportActionList(businessID);
            return result;
        }
        public IList<B_MonthlyReportAction> GetsystemctionList(Guid SystemID, int FinYear, int FinMonth)
        {
            IList<B_MonthlyReportAction> result = _bMonthlyReportActionAdapter.GetsystemActionList(SystemID, FinYear, FinMonth);
            return result;
        }

        public Guid AddMonthlyReportAction(B_MonthlyReportAction data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.AddNewModel(data);
            return result;
        }

        public B_MonthlyReportAction GetMonthlyReportAction(Guid bMonthlyReportActionID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(bMonthlyReportActionID == null, "Argument aTargetplanID is Empty");
            return base.GetModelObject(bMonthlyReportActionID);
        }

        public Guid UpdateMonthlyReportAction(B_MonthlyReportAction data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.UpdateModelObject(data);
            return result;
        }

        public Guid RemoveMonthlyReportAction(Guid bMonthlyReportActionID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(bMonthlyReportActionID == null, "Argument aTargetplanID is Empty");
            Guid result = base.RemoveObject(bMonthlyReportActionID);
            return result;
        }


        public List<B_MonthlyReportAction> GetMonthlyReportActionOneHour(Guid sysid, int finmonth, int year,int time)
        {
           
             List<B_MonthlyReportAction> list = _bMonthlyReportActionAdapter.GetMonthlyReportActionOneHour(sysid, finmonth, year,time).ToList();

             return list;
        }
        #endregion
    }
}
