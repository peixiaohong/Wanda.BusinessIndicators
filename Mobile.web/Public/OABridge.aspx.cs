using Plugin.SSO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication.WebPortal.Public
{
    public partial class OABridge : System.Web.UI.Page
    {
        protected override void OnPreLoad(EventArgs e)
        {
            base.OnPreLoad(e);
            
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            string errorMsg = "";
            string returnUrl = "";
            var result = SSOToolkit.Instance.ValidationWithOASSO(out errorMsg, out returnUrl);
            if (!result)
            {
                Response.Write(errorMsg);
                Response.End();
            }
            else
            {
                Response.Redirect(returnUrl);
            }
        }
    }
}