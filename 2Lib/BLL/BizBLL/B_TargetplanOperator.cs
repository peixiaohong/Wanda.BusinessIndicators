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
using System.Data;


namespace LJTH.BusinessIndicators.BLL
{
    /// <summary>
    /// Targetplan对象的业务逻辑操作
    /// </summary>
    public class B_TargetplanOperator : BizOperatorBase<B_TargetPlan>
    {

        #region Generate Code

        public static readonly B_TargetplanOperator Instance = PolicyInjection.Create<B_TargetplanOperator>();

        private static B_TargetplanAdapter _bTargetplanAdapter = AdapterFactory.GetAdapter<B_TargetplanAdapter>();

        protected override BaseAdapterT<B_TargetPlan> GetAdapter()
        {
            return _bTargetplanAdapter;
        }

        public B_TargetPlan GetTargetPlanByID(Guid TargetPlanID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(TargetPlanID == null ? true : false, "Argument TargetPlanID is Empty");
            return _bTargetplanAdapter.GetTargetPlanByID(TargetPlanID);
        }

        public B_TargetPlan GetTargetPlanByDraft(Guid SystemID, int Year)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(SystemID == null ? true : false, "Argument SystemID is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(Year <= 0 ? true : false, "Argument Year is Empty");
            return _bTargetplanAdapter.GetTargetPlanByDraft(SystemID, Year);
        }

        public B_TargetPlan GetTargetPlanByProgress(Guid SystemID, int Year)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(SystemID == null ? true : false, "Argument SystemID is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(Year <= 0 ? true : false, "Argument Year is Empty");
            return _bTargetplanAdapter.GetTargetPlanByProgress(SystemID, Year);
        }

        public B_TargetPlan GetTargetPlanByProgressOrApproved(Guid SystemID, int Year)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(SystemID == null ? true : false, "Argument SystemID is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(Year <= 0 ? true : false, "Argument Year is Empty");
            return _bTargetplanAdapter.GetTargetPlanByProgressOrApproved(SystemID, Year);
        }
        public IList<B_TargetPlan> GetTargetPlanByApproved(Guid SystemID, int Year)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(SystemID == null ? true : false, "Argument SystemID is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(Year <= 0 ? true : false, "Argument Year is Empty");
            return _bTargetplanAdapter.GetTargetPlanByApproved(SystemID, Year);
        }
        /// <summary>
        /// 获取所有状态的计划指标的集合/加载全部
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="Year"></param>
        /// <returns></returns>
        public IList<B_TargetPlan> GetTargetPlanByAllList(int Year)
        {
            //ExceptionHelper.TrueThrow<ArgumentNullException>(SystemID == null ? true : false, "Argument SystemID is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(Year <= 0 ? true : false, "Argument Year is Empty");
            return _bTargetplanAdapter.GetTargetPlanByAllList(Year);
        }
        /// <summary>
        /// 获取所有状态的计划指标的集合/分组加载全部
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="Year"></param>
        /// <returns></returns>
        public DataTable GetTargetPlanByGroupList(int Year)
        {
            //ExceptionHelper.TrueThrow<ArgumentNullException>(SystemID == null ? true : false, "Argument SystemID is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(Year <= 0 ? true : false, "Argument Year is Empty");
            return _bTargetplanAdapter.GetTargetPlanByGroupList(Year);
        }
        /// <summary>
        /// 获取所有状态的计划指标的集合
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="Year"></param>
        /// <returns></returns>
        public IList<B_TargetPlan> GetTargetPlanByApprovedList(Guid SystemID, int Year)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(SystemID == null ? true : false, "Argument SystemID is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(Year <= 0 ? true : false, "Argument Year is Empty");
            return _bTargetplanAdapter.GetTargetPlanByApproveList(SystemID, Year);
        }
        /// <summary>
        /// 取出该系统下所有审批中和审批完成的指标
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="Year"></param>
        /// <returns></returns>
        public IList<B_TargetPlan> GetTargetPlanByApprovedAndApproved(Guid SystemID, int Year)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(SystemID == null ? true : false, "Argument SystemID is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(Year <= 0 ? true : false, "Argument Year is Empty");
            return _bTargetplanAdapter.GetTargetPlanByApprovedAndApproved(SystemID, Year);
        }

        /// <summary>
        /// 获取当年 所有的审批中和审批完成的指标
        /// </summary>
        /// <param name="Year"></param>
        /// <returns></returns>
        public IList<B_TargetPlan> GetTargetPlanByApprovedAndApproved(int Year)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(Year <= 0 ? true : false, "Argument Year is Empty");
            return _bTargetplanAdapter.GetTargetPlanByApprovedAndApproved(Year);
        }

        public IList<B_TargetPlan> GetTargetplanList()
        {
            IList<B_TargetPlan> result = _bTargetplanAdapter.GetTargetplanList();
            return result;
        }



        public Guid AddTargetplan(B_TargetPlan data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.AddNewModel(data);
            return result;
        }

        public B_TargetPlan GetTargetplan(Guid bTargetplanID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(bTargetplanID == null, "Argument bTargetplanID is Empty");
            return base.GetModelObject(bTargetplanID);
        }

        public Guid UpdateTargetplan(B_TargetPlan data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.UpdateModelObject(data);
            return result;
        }

        public Guid RemoveTargetplan(Guid bTargetplanID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(bTargetplanID == null, "Argument bTargetplanID is Empty");
            Guid result = base.RemoveObject(bTargetplanID);
            return result;
        }

        #endregion
    }
}

