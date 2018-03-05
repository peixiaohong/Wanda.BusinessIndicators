using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wanda.Lib.AuthCenter.DAL;
using Wanda.Lib.AuthCenter.Model;
using Wanda.Lib.AuthCenter.ViewModel;
using Wanda.Lib.Data.AppBase;

namespace Wanda.Lib.AuthCenter.BLL
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

