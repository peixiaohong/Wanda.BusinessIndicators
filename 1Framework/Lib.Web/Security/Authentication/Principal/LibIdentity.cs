using System;
using System.Security.Principal;

namespace Lib.Web.Security
{
    public class LibIdentity : IIdentity
    {
        public static LibIdentity Current
        {
            get
            {
                return LibPrincipal.Current.Identity as LibIdentity;
            }
        }

        public static ILoginUser CurrentUser
        {
            get
            {
                return Current.User;
            }
        }


        private ILibAuthenticationTicket ticket = null;

        public LibIdentity(ILibAuthenticationTicket ticket)
        {
            if (ticket == null)
                throw new ArgumentNullException("ticket");
            this.ticket = ticket;
        }

        public string AuthenticationType
        {
            get { return "LibAuth"; }
        }

        public bool IsAuthenticated
        {
            get { return true; }
        }

        public string Name
        {
            get { return this.ticket.Name; }
        }

        public ILoginUser User
        {
            get { return this.ticket.LoginUser; }
        }
    }
}
