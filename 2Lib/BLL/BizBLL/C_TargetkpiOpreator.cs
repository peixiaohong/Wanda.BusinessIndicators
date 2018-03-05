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
    /// <summary>
    /// Targetkpi对象的业务逻辑操作
    /// </summary>
    public class C_TargetkpiOperator : BizOperatorBase<C_TargetKpi>
	{
    
        #region Generate Code

        public static readonly C_TargetkpiOperator Instance = PolicyInjection.Create<C_TargetkpiOperator>();

        private static C_TargetkpiAdapter _cTargetkpiAdapter = AdapterFactory.GetAdapter<C_TargetkpiAdapter>();

        protected override BaseAdapterT<C_TargetKpi> GetAdapter()
        {
            return  _cTargetkpiAdapter;
        }

        internal IList<C_TargetKpi> GetTargetkpiList()
        {
            IList<C_TargetKpi> result = _cTargetkpiAdapter.GetTargetkpiList();
            return result;
        }
        /// <summary>
        /// 根据systemID和年度得到年度指标
        /// </summary>
        /// <param name="systemID"></param>
        /// <returns></returns>
        internal IList<C_TargetKpi> GetTargetkpiList(Guid systemID,int FinYear)
        {
            IList<C_TargetKpi> result = _cTargetkpiAdapter.GetTargetkpiList(systemID, FinYear);
            return result;
        }

        public Guid AddTargetkpi(C_TargetKpi data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.AddNewModel(data);
            return result;
        }

        public C_TargetKpi GetTargetkpi(Guid cTargetkpiID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(cTargetkpiID==null,"Argument cTargetkpiID is Empty");
            return base.GetModelObject(cTargetkpiID);
        }



        /// <summary>
        /// 获取版本实体
        /// </summary>
        /// <param name="cTargetkpiID"></param>
        /// <param name="_VersionTime"></param>
        /// <returns></returns>
        public C_TargetKpi GetTargetkpi(Guid cTargetkpiID, DateTime? _VersionTime)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(cTargetkpiID == null, "Argument cTargetkpiID is Empty");
            ExceptionHelper.TrueThrow<ArgumentNullException>(_VersionTime == null, "Argument _VersionTime is Empty");

            List<C_TargetKpi> TargetkpiList = _cTargetkpiAdapter.GetTargetKpiListByVersionTime(cTargetkpiID, _VersionTime).ToList();

            return TargetkpiList.FirstOrDefault();
        }



        public Guid UpdateTargetkpi(C_TargetKpi data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.UpdateModelObject(data);
            return result;
        }

        public Guid RemoveTargetkpi(Guid cTargetkpiID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(cTargetkpiID==null,"Argument cTargetkpiID is Empty");
            Guid result = base.RemoveObject(cTargetkpiID);
            return result;
        }
        
        #endregion
    } 
}

