using Lib.Core;
using Lib.Data.AppBase;
using Microsoft.Practices.EnterpriseLibrary.PolicyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wanda.BusinessIndicators.DAL;
using Wanda.BusinessIndicators.Model;
using Wanda.Lib.Data.AppBase;

namespace Wanda.BusinessIndicators.BLL
{
  public  class B_MonthlyReportJsonDataOperator : BizOperatorBase<B_MonthlyReportJsonData>
    {
        public static readonly B_MonthlyReportJsonDataOperator Instance = PolicyInjection.Create<B_MonthlyReportJsonDataOperator>();

        private static B_MonthlyReportJsonDataAdapter _bMonthlyReportJsonDataAdapter = AdapterFactory.GetAdapter<B_MonthlyReportJsonDataAdapter>();

        protected override BaseAdapterT<B_MonthlyReportJsonData> GetAdapter()
        {
            return _bMonthlyReportJsonDataAdapter;
        }

        public IList<B_MonthlyReportJsonData> GetMonthlyReportJsonDataList()
        {
            IList<B_MonthlyReportJsonData> result = _bMonthlyReportJsonDataAdapter.GetMonthlyReportJsonDataList();
            return result;
        }

        public Guid AddMonthlyReportJsonData(B_MonthlyReportJsonData data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.AddNewModel(data);
            return result;
        }

        public B_MonthlyReportJsonData GetMonthlyReportJsonData(Guid bMonthlyreportID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(bMonthlyreportID == null ? true : false, "Argument bMonthlyreportID is Empty");
            return base.GetModelObject(bMonthlyreportID);
        }


        public Guid UpdateMonthlyReportJsonData(B_MonthlyReportJsonData data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.UpdateModelObject(data);
            return result;
        }

        public Guid RemoveMonthlyReportJsonData(Guid bMonthlyreportID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(bMonthlyreportID == null ? true : false, "Argument bMonthlyreportID is Empty");
            Guid result = base.RemoveObject(bMonthlyreportID);
            return result;
        }


    }
}
