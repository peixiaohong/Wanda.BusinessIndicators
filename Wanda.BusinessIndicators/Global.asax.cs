using LJTH.BusinessIndicators.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using LJTH.Lib.AuthCenter.BLL;

namespace LJTH.BusinessIndicators.Web
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            WebHelper.Instance.GetUser += UserinfoOperator.Instance.GetUserInfoByName;

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            if (HttpContext.Current.CurrentHandler is System.Web.UI.Page)
            {
                HttpContext.Current.Items[ErrorForContextKey] = Server.GetLastError();
                Server.ClearError();
                Server.Execute("~/Error.aspx"); 
            }
            //Ajax请求不进行错误页转发，否则HttpStatus就不对了
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }

        internal const string ErrorForContextKey = "HTTPRUNTIMEERROR_CACHE";
    }
}