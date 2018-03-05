using System;
using System.Web.Configuration;
using System.Web;

namespace Lib.Web.Security
{
    public class LibAuthenticationConfig
    {
        public static bool AccessingLoginPage(HttpContext context, string loginUrl)
        {
            if (context.Request.Url.AbsolutePath.CompareTo(loginUrl) == 0)
                return true;

            return false;
        }
    }
}
