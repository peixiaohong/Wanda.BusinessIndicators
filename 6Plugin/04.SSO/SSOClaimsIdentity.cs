using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.SSO
{
    public class SSOClaimsIdentity : IIdentity
    {

        public string AuthenticationType
        {
            get
            {
                return "SSO Authentication";
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                return true;
            }
        }

        public string Name
        {
            get
            {
                return this.UserName;
            }
        }

        public string UserName
        {
            get;
            set;
        }
      

        public SSOClaimsIdentity()
        {
        }
    }
}
