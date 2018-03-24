using Lib.Data;
using System;
using System.Collections.Generic;
using LJTH.Lib.AuthCenter.Model;
using LJTH.Lib.Data.AppBase;

namespace LJTH.Lib.AuthCenter.DAL
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
