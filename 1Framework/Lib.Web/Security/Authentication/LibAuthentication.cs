using System.Web;

namespace Lib.Web.Security
{
    public sealed class LibAuthentication
    {
        private static bool authRequired = true;

        private static string cookiePath;

        private static string cookieName;

        private static int cookieTimeout = 1;

        private static bool slidingExpiration;

        private static bool requireSSL;

        private static string loginUrl;

        private static string cookieDomain;

        public static bool AuthRequired
        {
            get { return authRequired; }
        }

        public static string CookiePath
        {
            get { return cookiePath; }
        }

        public static string CookieName
        {
            get { return cookieName; }
        }

        /// <summary>
        /// 超时时长，以分钟为单位
        /// </summary>
        public static int CookieTimeout
        {
            get { return cookieTimeout; }
        }

        public static bool SlidingExpiration
        {
            get { return slidingExpiration; }
        }

        public static bool RequireSSL
        {
            get { return requireSSL; }
        }

        public static string CookieDomain
        {
            get { return cookieDomain; }
        }

        public static string LoginUrl
        {
            get { return loginUrl; }
        }




        public static void Initialize()
        {
            cookiePath = "/";
            cookieName = "lib.auth";
            loginUrl = "/Web/Login.aspx";
            slidingExpiration = true;
            requireSSL = false;
        }

        public static void RenewTicket(ILibAuthenticationTicket ticket)
        {
            ticket = new LibAuthenticationTicket(ticket);
        }

        public static string Encrypt(ILibAuthenticationTicket ticket)
        {
            return ticket.Serialize();
        }

        public static ILibAuthenticationTicket Decrypt(string encryptedTicket)
        {
            ILibAuthenticationTicket ticket = new LibAuthenticationTicket();
            return ticket.Deserialize(encryptedTicket);
        }

        public static ILibAuthenticationTicket ExtractTicketFromCookie(ref bool fromCookie)
        {
            ILibAuthenticationTicket ticket = null;

            if (fromCookie)
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies[LibAuthentication.CookieName];
                if (cookie != null)
                {
                    ticket = LibAuthentication.Decrypt(cookie.Value);
                    fromCookie = true;
                }
                else
                    fromCookie = false;
            }

            return ticket;
        }

        public static ILibAuthenticationTicket PrepareTicket(ILibAuthenticationTicket ticket)
        {
            if (ticket != null && !ticket.Expired)
            {
                if (LibAuthentication.SlidingExpiration)
                {
                    LibAuthentication.RenewTicket(ticket);
                }
            }

            return ticket;
        }

        public static LibAuthenticationTicket CreateTicket(ILoginUser user)
        {
            return new LibAuthenticationTicket(user, true);
        }

        public static HttpCookie PrepareCookie(ILibAuthenticationTicket ticket, bool fromCookie)
        {
            HttpCookie cookie = null;

            if (fromCookie && !LibAuthentication.CookiePath.Equals("/"))
            {
                cookie = HttpContext.Current.Request.Cookies[LibAuthentication.CookieName];
                if (cookie != null)
                {
                    cookie.Path = LibAuthentication.CookiePath;
                }
            }

            if (cookie == null)
            {
                cookie = new HttpCookie(LibAuthentication.CookieName);
                cookie.Path = LibAuthentication.CookiePath;
            }
            if (ticket.IsPersistent)
            {
                cookie.Expires = ticket.LoginTimeout;
            }
            cookie.Value = LibAuthentication.Encrypt(ticket);
            cookie.Secure = LibAuthentication.RequireSSL;
            cookie.HttpOnly = true;

            if (LibAuthentication.CookieDomain != null)
            {
                cookie.Domain = LibAuthentication.CookieDomain;
            }

            return cookie;
        }

        public static void RedirectLogin(HttpContext context)
        {
            string login = string.Format("{0}?ReturnUrl={1}", loginUrl, HttpUtility.UrlEncode(context.Request.RawUrl));
            context.Response.Redirect(login);
        }

        public static void SetAuthCookie(ILoginUser user)
        {
            Initialize();

            if (!HttpContext.Current.Request.IsSecureConnection && RequireSSL)
            {
                throw new HttpException("Connection_not_secure_creating_secure_cookie");
            }

            bool fromCookie = false;

            ILibAuthenticationTicket ticket = LibAuthentication.ExtractTicketFromCookie(ref fromCookie);
            if (ticket == null)
                ticket = LibAuthentication.CreateTicket(user);
            HttpCookie cookie = LibAuthentication.PrepareCookie(ticket, false);
            if (fromCookie)
            {
                HttpContext.Current.Response.Cookies.Remove(cookie.Name);

                HttpContext.Current.Response.Cookies.Add(cookie);
            }
            else
            {
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }
    }
}
