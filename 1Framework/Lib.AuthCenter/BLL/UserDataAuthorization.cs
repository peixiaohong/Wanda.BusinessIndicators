using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LJTH.Lib.AuthCenter.DAL;
using LJTH.Lib.AuthCenter.Model;
using LJTH.Lib.AuthCenter.ViewModel;
using LJTH.Lib.Data.AppBase;

namespace LJTH.Lib.AuthCenter.BLL
{
    public class UserDataAuthorization
    {
        public static UserDataAuthorization Instance = new UserDataAuthorization();
        public IList<VDataAuthorization> GetUserRoleData(int roleid, string userid)
        {
            return VDataAuthorizationAdapter.Instance.GetUserRoleData(roleid, userid);
        }
    }

}

