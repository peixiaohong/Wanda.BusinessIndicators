using Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wanda.Lib.Data.AppBase;
using Wanda.Lib.AuthCenter.Model;
using Wanda.Lib.Data.AppBase;

namespace Wanda.Lib.AuthCenter.DAL
{
    class VUserInfoAdapter : AuthBaseCompositionAdapterT<VUserInfo>
    {
        public static VUserInfoAdapter Instance = new VUserInfoAdapter();

 
    }
}
