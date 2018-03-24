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


namespace LJTH.BusinessIndicators.BLL.BizBLL
{
    public class TSM_MessagesOperator : BizOperatorBase<TSM_Messages>
    {
        public static readonly TSM_MessagesOperator Instance = PolicyInjection.Create<TSM_MessagesOperator>();
        private static TSM_MessagesAdapter _TSM_MessagesAdapter = AdapterFactory.GetAdapter<TSM_MessagesAdapter>();

        protected override BaseAdapterT<TSM_Messages> GetAdapter()
        {
            return _TSM_MessagesAdapter;
        }
        internal IList<TSM_Messages> GetTSM_MessagesList()
        {
            IList<TSM_Messages> result = _TSM_MessagesAdapter.GetMessagesList();
            return result;
        }
        public TSM_Messages GetTSM_Messages(Guid Mid)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(Mid == null, "Argument cSystemID is Empty");
            return base.GetModelObject(Mid);
        }
        public Guid AddTSM_Messages(TSM_Messages data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.AddNewModel(data);
            return result;
        }
        public Guid UpdateTSM_Messages(TSM_Messages data)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(data == null, "Argument data is Empty");
            Guid result = base.UpdateModelObject(data);
            return result;
        }
        public int RemoveTSM_Messages(TSM_Messages data)
        {
            int i = _TSM_MessagesAdapter.Remove(data);
            return i;
        }
    }
}
