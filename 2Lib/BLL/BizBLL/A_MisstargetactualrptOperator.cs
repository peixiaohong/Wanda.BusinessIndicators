
using Lib.Core;
using Lib.Validation;
using System;
using Microsoft.Practices.EnterpriseLibrary.PolicyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wanda.Lib.Data.AppBase;
using Wanda.BusinessIndicators.Model;
using Wanda.BusinessIndicators.DAL;
using Lib.Data.AppBase;


namespace Wanda.BusinessIndicators.BLL
{
    /// <summary>
    /// Misstargetactualrpt对象的业务逻辑操作
    /// </summary>
    public class A_MisstargetactualrptOperator : BizOperatorBase<A_MissTargetActualRpt>
	{
    
        #region Generate Code

        public static readonly A_MisstargetactualrptOperator Instance = PolicyInjection.Create<A_MisstargetactualrptOperator>();

        private static A_MisstargetactualrptAdapter _aMisstargetactualrptAdapter = AdapterFactory.GetAdapter<A_MisstargetactualrptAdapter>();

        protected override BaseAdapterT<A_MissTargetActualRpt> GetAdapter()
        {
            return  _aMisstargetactualrptAdapter;
        }

        public IList<A_MissTargetActualRpt> GetAMisstargetactualrptList(  )
        {
            IList<A_MissTargetActualRpt> result = _aMisstargetactualrptAdapter.GetMisstargetactualrptList();
            return result;
        }

        public Guid AddMisstargetactualrpt(A_MissTargetActualRpt data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.AddNewModel(data);
            return result;
        }

        public A_MissTargetActualRpt GetAMisstargetactualrpt(Guid aMisstargetactualrptID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(aMisstargetactualrptID ==null , "Argument aMisstargetactualrptID is Empty");
            return base.GetModelObject(aMisstargetactualrptID);
        }

        public Guid UpdateAMisstargetactualrpt(A_MissTargetActualRpt data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.UpdateModelObject(data);
            return result;
        }

        public Guid RemoveAMisstargetactualrpt(Guid aMisstargetactualrptID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(aMisstargetactualrptID == null ? true : false, "Argument aMisstargetactualrptID is Empty");
            Guid result = base.RemoveObject(aMisstargetactualrptID);
            return result;
        }
        
        #endregion
    } 
}

