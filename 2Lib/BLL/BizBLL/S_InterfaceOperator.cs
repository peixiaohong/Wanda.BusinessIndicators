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
    /// Interface对象的业务逻辑操作
    /// </summary>
    public class S_InterfaceOperator : BizOperatorBase<S_Interface>
	{
    
        #region Generate Code

        public static readonly S_InterfaceOperator Instance = PolicyInjection.Create<S_InterfaceOperator>();

        private static S_InterfaceAdapter _sInterfaceAdapter = AdapterFactory.GetAdapter<S_InterfaceAdapter>();

        protected override BaseAdapterT<S_Interface> GetAdapter()
        {
            return  _sInterfaceAdapter;
        }

        public IList<S_Interface> GetInterfaceList()
        {
            IList<S_Interface> result = _sInterfaceAdapter.GetInterfaceList();
            return result;
        }

        public Guid AddInterface(S_Interface data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.AddNewModel(data);
            return result;
        }

        public S_Interface GetInterface(Guid sInterfaceID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(sInterfaceID==null ,"Argument sInterfaceID is Empty");
            return base.GetModelObject(sInterfaceID);
        }

        public Guid UpdateInterface(S_Interface data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.UpdateModelObject(data);
            return result;
        }

        public Guid RemoveInterface(Guid sInterfaceID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(sInterfaceID==null ,"Argument sInterfaceID is Empty");
            Guid result = base.RemoveObject(sInterfaceID);
            return result;
        }
        
        #endregion
    } 
}

