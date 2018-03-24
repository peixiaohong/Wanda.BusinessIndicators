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
    /// Misstargetcategory对象的业务逻辑操作
    /// </summary>
    public class C_MisstargetcategoryOperator : BizOperatorBase<C_MissTargetCategory>
	{
    
        #region Generate Code

        public static readonly C_MisstargetcategoryOperator Instance = PolicyInjection.Create<C_MisstargetcategoryOperator>();

        private static C_MisstargetcategoryAdapter _cMisstargetcategoryAdapter = AdapterFactory.GetAdapter<C_MisstargetcategoryAdapter>();

        protected override BaseAdapterT<C_MissTargetCategory> GetAdapter()
        {
            return  _cMisstargetcategoryAdapter;
        }

        public IList<C_MissTargetCategory> GetMisstargetcategoryList()
        {
            IList<C_MissTargetCategory> result = _cMisstargetcategoryAdapter.GetMisstargetcategoryList();
            return result;
        }

        public Guid AddMisstargetcategory(C_MissTargetCategory data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.AddNewModel(data);
            return result;
        }

        public C_MissTargetCategory GetMisstargetcategory(Guid cMisstargetcategoryID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(cMisstargetcategoryID==null ,"Argument cMisstargetcategoryID is Empty");
            return base.GetModelObject(cMisstargetcategoryID);
        }

        public Guid UpdateMisstargetcategory(C_MissTargetCategory data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.UpdateModelObject(data);
            return result;
        }

        public Guid RemoveMisstargetcategory(Guid cMisstargetcategoryID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(cMisstargetcategoryID==null ,"Argument cMisstargetcategoryID is Empty");
            Guid result = base.RemoveObject(cMisstargetcategoryID);
            return result;
        }
        
        #endregion
    } 
}

