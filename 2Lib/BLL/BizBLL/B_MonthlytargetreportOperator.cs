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
    /// Monthlytargetreport对象的业务逻辑操作
    /// </summary>
    public class B_MonthlytargetreportOperator : BizOperatorBase<B_MonthlyTargetReport>
	{
    
        #region Generate Code

        public static readonly B_MonthlytargetreportOperator Instance = PolicyInjection.Create<B_MonthlytargetreportOperator>();

        private static B_MonthlytargetreportAdapter _bMonthlytargetreportAdapter = AdapterFactory.GetAdapter<B_MonthlytargetreportAdapter>();

        protected override BaseAdapterT<B_MonthlyTargetReport> GetAdapter()
        {
            return  _bMonthlytargetreportAdapter;
        }

        public IList<B_MonthlyTargetReport> GetMonthlytargetreportList()
        {
            IList<B_MonthlyTargetReport> result = _bMonthlytargetreportAdapter.GetMonthlytargetreportList();
            return result;
        }

        public Guid AddMonthlytargetreport(B_MonthlyTargetReport data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.AddNewModel(data);
            return result;
        }

        public B_MonthlyTargetReport GetMonthlytargetreport(Guid bMonthlytargetreportID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(bMonthlytargetreportID==null ,"Argument bMonthlytargetreportID is Empty");
            return base.GetModelObject(bMonthlytargetreportID);
        }

        public Guid UpdateMonthlytargetreport(B_MonthlyTargetReport data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.UpdateModelObject(data);
            return result;
        }

        public Guid RemoveMonthlytargetreport(Guid bMonthlytargetreportID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(bMonthlytargetreportID==null ,"Argument bMonthlytargetreportID is Empty");
            Guid result = base.RemoveObject(bMonthlytargetreportID);
            return result;
        }
        
        #endregion
    } 
}

