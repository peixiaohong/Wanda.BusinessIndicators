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
using System.Configuration;


namespace LJTH.BusinessIndicators.BLL
{
    /// <summary>
    /// Targetplan对象的业务逻辑操作
    /// </summary>
    public class A_TargetplanOperator : BizOperatorBase<A_TargetPlan>
    {

        #region Generate Code

        public static readonly A_TargetplanOperator Instance = PolicyInjection.Create<A_TargetplanOperator>();

        private static A_TargetplanAdapter _aTargetplanAdapter = AdapterFactory.GetAdapter<A_TargetplanAdapter>();

        protected override BaseAdapterT<A_TargetPlan> GetAdapter()
        {
            return _aTargetplanAdapter;
        }

        public IList<A_TargetPlan> GetTargetplanList()
        {
            IList<A_TargetPlan> result = _aTargetplanAdapter.GetTargetplanList();
            return result;
        }

        public IList<A_TargetPlan> GetTargetplanList(Guid ID)
        {
            IList<A_TargetPlan> result = _aTargetplanAdapter.GetTargetplanList(ID);
            return result;
        }

        public IList<A_TargetPlan> GetTargetplanList(Guid SystemID, int FinYear)
        {
            IList<A_TargetPlan> result = _aTargetplanAdapter.GetTargetplanList(SystemID, FinYear);
            return result;
        }

        /// <summary>
        /// 获取默认版本
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="FinYear"></param>
        /// <returns></returns>
        public IList<A_TargetPlan> GetDefaultTargetplanList(Guid SystemID, int FinYear)
        {
            IList<A_TargetPlan> result = _aTargetplanAdapter.GetDefaultTargetplanList(SystemID, FinYear);
            return result;
        }
        /// <summary>
        /// 获取多版本混合指标计划
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="FinYear"></param>
        /// <returns></returns>
        public IList<A_TargetPlan> GetTargetplanListForMulitiVersion(Guid SystemID, int FinYear, Guid TargetPlanId)
        {
            IList<A_TargetPlan> result = _aTargetplanAdapter.GetTargetplanListForMulitiVersion(SystemID, FinYear, TargetPlanId);
            return result;
        }
        public Guid AddTargetplan(A_TargetPlan data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.AddNewModel(data);
            return result;
        }

        public A_TargetPlan GetTargetplan(Guid aTargetplanID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(aTargetplanID == null, "Argument aTargetplanID is Empty");
            return base.GetModelObject(aTargetplanID);
        }

        public Guid UpdateTargetplan(A_TargetPlan data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.UpdateModelObject(data);
            return result;
        }

        public Guid RemoveTargetplan(Guid aTargetplanID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(aTargetplanID == null, "Argument aTargetplanID is Empty");
            Guid result = base.RemoveObject(aTargetplanID);
            return result;
        }


        /// <summary>
        /// 物理删除
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public int DeleteModel(A_TargetPlan Model)
        {
            return _aTargetplanAdapter.Delete(Model);
        }

        public IList<A_TargetPlan> GetListByRecalculation(int time)
        {
            time = 0 - time;
            DateTime OpeeratorTime = DateTime.Now.AddMinutes(time);
            IList<A_TargetPlan> result = _aTargetplanAdapter.GetTargetplanListByRecalculation(OpeeratorTime);
            return result;
        }

        #endregion
        /// <summary>
        /// 获取已审批通过的分解指标年份集合
        /// </summary>
        /// <returns></returns>
        public List<A_TargetPlan> GetPlanYearList()
        {
            List<A_TargetPlan> result = _aTargetplanAdapter.GetPlanYearList();
            return result;
        }
        /// <summary>
        /// 获取已审批通过的分解指标版本类型集合
        /// </summary>
        /// <param name="systemID"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public IList<A_TargetPlan> GetTargetVersionType(string systemID, int year, int month)
        {
            return _aTargetplanAdapter.GetTargetVersionType(systemID, year, month);
        }

        /// <summary>
        /// 获取已审批通过的分解指标版本类型集合
        /// </summary>
        /// <param name="systemID"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public IList<A_TargetPlan> GetTargetVersionType(string systemID, int year)
        {
            return _aTargetplanAdapter.GetTargetVersionType(systemID, year);
        }

        public int DeletePlan(Guid PlanID)
        {
            return _aTargetplanAdapter.DeletePlan(PlanID);
        }
    }
}

