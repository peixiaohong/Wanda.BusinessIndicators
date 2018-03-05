using Lib.Core;
using Lib.Data;
using Lib.Data.AppBase;
using Lib.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using Wanda.Lib.AuthCenter.DAL;
using Wanda.Lib.AuthCenter.Model;
using Wanda.Lib.AuthCenter.ViewModel;
using Wanda.Lib.Data.AppBase;
namespace Wanda.Lib.AuthCenter.BLL
{
    /// <summary>
    /// AUserRole对象的业务逻辑操作
    /// </summary>
    public class AUserRoleOperator : BizOperatorBase<AUserRole>
    {
        public static AUserRoleOperator Instance = BizOperatorFactory.Create<AUserRoleOperator>(WebHelper.GetCurrentLoginUser, WebHelper.GetTimeNow);

        private static AUserRoleAdapter _userinfoAdapter = AdapterFactory.GetAdapter<AUserRoleAdapter>();

        protected override BaseAdapterT<AUserRole> GetAdapter()
        {
            return _userinfoAdapter;
        }
        public Guid AddDefaultRoleForUser(AUserRole info)
        {
            Guid result = Guid.Empty;
            result = base.AddNewModel(info);
            return result;
        }
    }
}

