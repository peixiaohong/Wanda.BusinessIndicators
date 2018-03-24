using Lib.Core;
using Lib.Data.AppBase;
using Microsoft.Practices.EnterpriseLibrary.PolicyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LJTH.BusinessIndicators.DAL;
using LJTH.BusinessIndicators.Model;
using LJTH.Lib.Data.AppBase;

namespace LJTH.BusinessIndicators.BLL
{
    public  class R_MissTargetEvaluationScopeOperator : BizOperatorBase<R_MissTargetEvaluationScope>
    {
        public static readonly R_MissTargetEvaluationScopeOperator Instance = PolicyInjection.Create<R_MissTargetEvaluationScopeOperator>();

        private static R_MissTargetEvaluationScopeAdapter _rMissTargetEvaluationScopeAdapter = AdapterFactory.GetAdapter<R_MissTargetEvaluationScopeAdapter>();


        protected override BaseAdapterT<R_MissTargetEvaluationScope> GetAdapter()
        {
            return _rMissTargetEvaluationScopeAdapter;
        }

        public IList<R_MissTargetEvaluationScope> GetTargetmappingList()
        {
            IList<R_MissTargetEvaluationScope> result = _rMissTargetEvaluationScopeAdapter.GetTargetmappingList();
            return result;
        }

        public Guid AddMissTargetEvaluationScope(R_MissTargetEvaluationScope data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.AddNewModel(data);
            return result;
        }

        public R_MissTargetEvaluationScope GetMissTargetEvaluationScope(Guid missTargetEvaluationScopeID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(missTargetEvaluationScopeID == null, "Argument missTargetEvaluationScopeID is Empty");
            return base.GetModelObject(missTargetEvaluationScopeID);
        }

        /// <summary>
        /// 通过条件获取当前实体
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="TargetID"></param>
        /// <param name="FinYear"></param>
        /// <param name="FinMonth"></param>
        /// <returns></returns>
        public R_MissTargetEvaluationScope GetMissTargetEvaluationScope(Guid SystemID , Guid TargetID , int FinYear , int FinMonth  )
        {

            if (_rMissTargetEvaluationScopeAdapter.GetTargetmappingList(SystemID, TargetID, FinYear, FinMonth).ToList().Count > 0)
                return _rMissTargetEvaluationScopeAdapter.GetTargetmappingList(SystemID, TargetID, FinYear, FinMonth).FirstOrDefault();
            else
                return null;
        }


        public Guid UpdateMissTargetEvaluationScope(R_MissTargetEvaluationScope data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.UpdateModelObject(data);
            return result;
        }

        public Guid RemoveMissTargetEvaluationScope(Guid missTargetEvaluationScopeID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(missTargetEvaluationScopeID == null, "Argument missTargetEvaluationScopeID is Empty");
            Guid result = base.RemoveObject(missTargetEvaluationScopeID);
            return result;
        }
        /// <summary>
        /// 根据Type取出数据
        /// </summary>
        /// <param name="SystemID"></param>
        /// <param name="TargetID"></param>
        /// <param name="FinYear"></param>
        /// <param name="FinMonth"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public R_MissTargetEvaluationScope GetEvaluationDetailByType(Guid SystemID, Guid TargetID, int FinYear, int FinMonth, string Type)
        {
            if (_rMissTargetEvaluationScopeAdapter.GetEvaluationDetailByType(SystemID, TargetID, FinYear, FinMonth, Type).ToList().Count > 0)
                return _rMissTargetEvaluationScopeAdapter.GetEvaluationDetailByType(SystemID, TargetID, FinYear, FinMonth, Type).ToList().FirstOrDefault();
            else
                return null; ;
        }

    }
}
