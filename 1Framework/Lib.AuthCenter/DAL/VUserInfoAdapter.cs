using Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LJTH.Lib.Data.AppBase;
using LJTH.Lib.AuthCenter.Model;

namespace LJTH.Lib.AuthCenter.DAL
{
    class VUserInfoAdapter : AuthBaseCompositionAdapterT<VUserInfo>
    {
        public static VUserInfoAdapter Instance = new VUserInfoAdapter();

 
    }
}
