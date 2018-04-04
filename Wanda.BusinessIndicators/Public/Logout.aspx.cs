using Plugin.SSO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication.WebPortal.Public
{
    public partial class Logout : System.Web.UI.Page
    {
        protected override void OnPreLoad(EventArgs e)
        {
            base.OnPreLoad(e);
            

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            SSOToolkit.Instance.Logout();
        }
    }
}