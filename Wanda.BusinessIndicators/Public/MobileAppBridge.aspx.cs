using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LJTH.BusinessIndicators.Web.Public
{
    public partial class MobileAppBridge : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string errorMsg = "";
            var result = Plugin.SSO.SSOToolkit.Instance.ValidationWithMobileAppMainSSO(out errorMsg);
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