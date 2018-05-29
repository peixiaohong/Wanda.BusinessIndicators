using Plugin.SSO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication.WebPortal.Public
{
    public partial class Bridge : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string errorMsg = "";
            var result = SSOToolkit.Instance.ValidationWithOAMainSSO(out errorMsg);
            if (!result)
            {
                Response.Write(errorMsg);
                Response.End();
            }
            else
            {
                Response.Redirect("~/BusinessReport/TargetRpt.aspx");
            }
        }
    }
}