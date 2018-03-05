using Lib.Data;
using System;
using System.Collections.Generic;
using Wanda.Lib.AuthCenter.Model;
using Wanda.Lib.Data.AppBase;

namespace Wanda.Lib.AuthCenter.DAL
{
    class VSimleWdUserInfoAdapter : AuthBaseCompositionAdapterT<VWdSimpleUserInfo>
    {

        public static readonly VSimleWdUserInfoAdapter Instance = new VSimleWdUserInfoAdapter();

        private VSimleWdUserInfoAdapter()
        {

        }


        internal PartlyCollection<VWdSimpleUserInfo> GetList(WdSimpleUserDataFilter filter)
        {
            return base.GetList(filter, "username");
        }
         
    }
}
