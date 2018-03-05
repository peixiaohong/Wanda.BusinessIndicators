using System;
using System.Web;
using System.Web.Configuration;
using Lib.Web.Security;

namespace Lib.Web.Security
{
    public class LibAuthenticationModule : IHttpModule
    {
        private bool IsLibAuth = false;

        public event LibAuthenticationEventHandler OnAuthenticate;

        #region IHttpModule

        public void Init(HttpApplication context)
        {
            if (LibAuthentication.AuthRequired)
            {
                LibAuthentication.Initialize();
                context.AuthenticateRequest += new EventHandler(context_AuthenticateRequest);
                context.EndRequest += new EventHandler(context_EndRequest);
            }
        }

        public void Dispose()
        {
            
        }

        #endregion

        #region Protected

        protected virtual void context_AuthenticateRequest(object sender, EventArgs e)
        {
            this.IsLibAuth = true;

            HttpApplication application = sender as HttpApplication;
            if (application == null)
                return;

            HttpContext context = application.Context;
            if (context == null)
                return;

            if (LibAuthenticationConfig.AccessingLoginPage(context, LibAuthentication.LoginUrl))
                return;

            this.Authenticate(new LibAuthenticationEventArgs(context));
        }

        protected virtual void context_EndRequest(object sender, EventArgs e)
        {
            if (!this.IsLibAuth)
                return;



            this.IsLibAuth = false;
        }

        protected virtual void Authenticate(LibAuthenticationEventArgs e)
        {
            if (this.OnAuthenticate != null)
                this.OnAuthenticate(this, e);

            if (e.Context.User == null)
            {
                if (e.User != null)
                {
                    e.Context.User = e.User;
                }
                else
                {
                    bool fromCookie = true;
                    ILibAuthenticationTicket ticket = LibAuthentication.ExtractTicketFromCookie(ref fromCookie);
                    if (ticket == null)
                    {
                        LibAuthentication.RedirectLogin(e.Context);
                        return;
                    }
                    else
                    {
                        LibAuthentication.PrepareTicket(ticket);
                        e.Context.User = new LibPrincipal(new LibIdentity(ticket));
                        HttpCookie cookie = LibAuthentication.PrepareCookie(ticket, fromCookie);

                        e.Context.Response.Cookies.Remove(cookie.Name);
                        e.Context.Response.Cookies.Add(cookie);
                    }
                }
            }
        }

        #endregion
    }
}
