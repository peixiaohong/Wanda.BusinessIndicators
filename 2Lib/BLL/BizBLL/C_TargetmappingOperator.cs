using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.DAL;
using Lib.Core;
using System;
using Microsoft.Practices.EnterpriseLibrary.PolicyInjection;
using System.Collections.Generic;
using LJTH.Lib.Data.AppBase;
using Lib.Data.AppBase;


namespace LJTH.BusinessIndicators.BLL
{
    /// <summary>
    /// Targetmapping对象的业务逻辑操作
    /// </summary>
    public class C_TargetmappingOperator : BizOperatorBase<C_TargetMapping>
	{
    
        #region Generate Code

        public static readonly C_TargetmappingOperator Instance = PolicyInjection.Create<C_TargetmappingOperator>();

        private static C_TargetmappingAdapter _cTargetmappingAdapter = AdapterFactory.GetAdapter<C_TargetmappingAdapter>();

        protected override BaseAdapterT<C_TargetMapping> GetAdapter()
        {
            return  _cTargetmappingAdapter;
        }

        public IList<C_TargetMapping> GetTargetmappingList()
        {
            IList<C_TargetMapping> result = _cTargetmappingAdapter.GetTargetmappingList();
            return result;
        }

        public Guid AddTargetmapping(C_TargetMapping data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.AddNewModel(data);
            return result;
        }

        public C_TargetMapping GetTargetmapping(Guid cTargetmappingID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(cTargetmappingID==null ,"Argument cTargetmappingID is Empty");
            return base.GetModelObject(cTargetmappingID);
        }

        public Guid UpdateTargetmapping(C_TargetMapping data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.UpdateModelObject(data);
            return result;
        }

        public Guid RemoveTargetmapping(Guid cTargetmappingID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(cTargetmappingID==null ,"Argument cTargetmappingID is Empty");
            Guid result = base.RemoveObject(cTargetmappingID);
            return result;
        }
        
        #endregion
    } 
}

