using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.SSO
{
    public class SSOClaimsPrincipal : IPrincipal
    {
        private SSOClaimsIdentity _identity;

        public SSOClaimsIdentity Identity
        {
            get
            {
                return this._identity;
            }
        }

        IIdentity IPrincipal.Identity
        {
            get
            {
                return this._identity;
            }
        }

        public string SystemCode
        {
            get;
            set;
        }

        public string ChallengeNumer
        {
            get;
            set;
        }

        public DateTime ClientDate
        {
            get;
            set;
        }
        

        public SSOClaimsPrincipal(SSOClaimsIdentity identity)
        {
            this._identity = identity;
        }

        public bool IsInRole(string role)
        {
            return false;
        }
    }
}
