using Wanda.BusinessIndicators.Model;
using Wanda.BusinessIndicators.DAL;
using Lib.Core;
using Lib.Validation;
using System;
using Microsoft.Practices.EnterpriseLibrary.PolicyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wanda.Lib.Data.AppBase;
using Lib.Data.AppBase;


namespace Wanda.BusinessIndicators.BLL
{
    public class C_ReportTimeOperator : BizOperatorBase<C_ReportTime>
    {
        public static readonly C_ReportTimeOperator Instance = PolicyInjection.Create<C_ReportTimeOperator>();
        private static C_ReportTimeAdapter _cReportTimeAdapter = AdapterFactory.GetAdapter<C_ReportTimeAdapter>();

        protected override BaseAdapterT<C_ReportTime> GetAdapter()
        {
            return _cReportTimeAdapter;
        }
        internal IList<C_ReportTime> GetReportTimeList()
        {
            IList<C_ReportTime> result = _cReportTimeAdapter.GetReportTimeList();
            return result;
        }

        public Guid AddReportTime(C_ReportTime data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.AddNewModel(data);
            return result;
        }

        public C_ReportTime GetReportTime(Guid cReportTimeID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(cReportTimeID == null, "Argument cReportTimeID is Empty");
            return base.GetModelObject(cReportTimeID);
        }

        public Guid UpdateReportTime(C_ReportTime data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.UpdateModelObject(data);
            return result;
        }

        public Guid RemoveReportTime(Guid cReportTime)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(cReportTime == null, "Argument cReportTimeID is Empty");
            Guid result = base.RemoveObject(cReportTime);
            return result;
        }


        public C_ReportTime GetReportTime() 
        {
            List<C_ReportTime> result = _cReportTimeAdapter.GetReportTimeList().ToList();
             C_ReportTime model=new C_ReportTime();
            if (result.Count > 0)
            {
                model = result.FirstOrDefault();
            }
            else
            {
                model.ID = Guid.NewGuid();
                model.IsDeleted = false;
                model.ReportTime = DateTime.MinValue;
                model.OpenStatus = "Waiting";
                AddReportTime(model);
            }
            return model;
        }
     
    }
}
