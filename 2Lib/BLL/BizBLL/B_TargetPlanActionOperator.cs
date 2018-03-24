using Lib.Core;
using Lib.Data.AppBase;
using Microsoft.Practices.EnterpriseLibrary.PolicyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using LJTH.BusinessIndicators.DAL;
using LJTH.BusinessIndicators.Model;
using LJTH.Lib.Data.AppBase;
using System.Web;


namespace LJTH.BusinessIndicators.BLL
{
    public class B_TargetPlanActionOperator : BizOperatorBase<B_TargetPlanAction>
    {
        #region Generate Code   
        public static readonly B_TargetPlanActionOperator Instance = PolicyInjection.Create<B_TargetPlanActionOperator>();

        private static B_TargetPlanActionAdapter _aTargetPlanActionAdapter = AdapterFactory.GetAdapter<B_TargetPlanActionAdapter>();

        protected override BaseAdapterT<B_TargetPlanAction> GetAdapter()
        {
            return _aTargetPlanActionAdapter;
        }

        public IList<B_TargetPlanAction> GetTargetPlanActionList()
        {
            IList<B_TargetPlanAction> result = _aTargetPlanActionAdapter.GetTargetPlanActionList();
            return result;
        }

        public Guid AddTargetPlanAction(B_TargetPlanAction data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.AddNewModel(data);
            return result;
        }

        public B_TargetPlanAction GetTargetPlanAction(Guid bTargetPlanActionID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(bTargetPlanActionID == null, "Argument bTargetPlanActionID is Empty");
            return base.GetModelObject(bTargetPlanActionID);
        }

        public Guid RemoveTargetPlanAction(Guid bTargetPlanActionID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(bTargetPlanActionID == null ? true : false, "Argument bTargetPlanActionID is Empty");
            Guid result = base.RemoveObject(bTargetPlanActionID);
            return result;
        }


        public IList<B_TargetPlanAction> GetActionByTargetplanID(Guid TargetPlanID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(TargetPlanID == null ? true : false, "Argument bTargetPlanActionID is Empty");
            IList<B_TargetPlanAction> result = _aTargetPlanActionAdapter.GetActionByTargetplanID(TargetPlanID);
            return result;

        }

        #endregion
    }
}
