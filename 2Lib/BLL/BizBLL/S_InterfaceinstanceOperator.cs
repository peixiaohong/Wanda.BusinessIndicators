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
    /// Interfaceinstance对象的业务逻辑操作
    /// </summary>
    public class S_InterfaceinstanceOperator : BizOperatorBase<S_InterfaceInstance>
	{
    
        #region Generate Code

        public static readonly S_InterfaceinstanceOperator Instance = PolicyInjection.Create<S_InterfaceinstanceOperator>();

        private static S_InterfaceinstanceAdapter _sInterfaceinstanceAdapter = AdapterFactory.GetAdapter<S_InterfaceinstanceAdapter>();

        protected override BaseAdapterT<S_InterfaceInstance> GetAdapter()
        {
            return  _sInterfaceinstanceAdapter;
        }

        public IList<S_InterfaceInstance> GetInterfaceinstanceList()
        {
            IList<S_InterfaceInstance> result = _sInterfaceinstanceAdapter.GetInterfaceinstanceList();
            return result;
        }

        public Guid AddInterfaceinstance(S_InterfaceInstance data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.AddNewModel(data);
            return result;
        }

        public S_InterfaceInstance GetInterfaceinstance(Guid sInterfaceinstanceID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(sInterfaceinstanceID==null ,"Argument sInterfaceinstanceID is Empty");
            return base.GetModelObject(sInterfaceinstanceID);
        }

        public Guid UpdateInterfaceinstance(S_InterfaceInstance data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.UpdateModelObject(data);
            return result;
        }

        public Guid RemoveInterfaceinstance(Guid sInterfaceinstanceID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(sInterfaceinstanceID==null ,"Argument sInterfaceinstanceID is Empty");
            Guid result = base.RemoveObject(sInterfaceinstanceID);
            return result;
        }
        
        #endregion
    } 
}

