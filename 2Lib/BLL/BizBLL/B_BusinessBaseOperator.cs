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
    public class B_BusinessBaseOperator : BizOperatorBase<B_BusinessBase>
    {
        #region Generate Code

        public static readonly B_BusinessBaseOperator Instance = PolicyInjection.Create<B_BusinessBaseOperator>();

        private static B_BusinessBaseAdapter _bBusinessBaseAdapter = AdapterFactory.GetAdapter<B_BusinessBaseAdapter>();

        protected override BaseAdapterT<B_BusinessBase> GetAdapter()
        {
            return _bBusinessBaseAdapter;
        }

        public IList<B_BusinessBase> GetBusinessBaseList()
        {
            IList<B_BusinessBase> result = _bBusinessBaseAdapter.GetBusinessBaseList();
            return result;
        }

        public IList<B_BusinessBase> GetBusinessBaseList( Guid MonthlyReportID)
        {
            IList<B_BusinessBase> result = _bBusinessBaseAdapter.GetBusinessBaseList(MonthlyReportID);
            return result;
        }
        

        public Guid AddBusinessBase(B_BusinessBase data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.AddNewModel(data);
            return result;
        }

        public B_BusinessBase GetBusinessBase(Guid businessBaseID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(businessBaseID == null, "Argument businessBaseID is Empty");
            return base.GetModelObject(businessBaseID);
        }

        public Guid UpdateBusinessBase(B_BusinessBase data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.UpdateModelObject(data);
            return result;
        }

        public Guid RemoveBusinessBase(Guid businessBaseID)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(businessBaseID == null, "Argument businessBaseID is Empty");
            Guid result = base.RemoveObject(businessBaseID);
            return result;
        }

        #endregion
    }
}
