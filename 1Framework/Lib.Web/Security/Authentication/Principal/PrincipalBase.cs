using System;
using System.Security.Principal;

namespace Lib.Web.Security
{
    public abstract class LibPrincipal<TUser> : IPrincipal where TUser : ILoginUser
    {
        private LibIdentity identity;
        private TUser user;

        public LibPrincipal(LibIdentity identity)
        {
            this.identity = identity;
            this.user = (TUser)identity.User;
        }

        public IIdentity Identity
        {
            get { return this.identity; }
        }

        public TUser User
        {
            get { return this.user; }
        }

        public bool IsInRole(string role)
        {
            return this.user.IsInRole(role);
        }
    }
}
