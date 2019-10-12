using LJTH.SDK.SSO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LJTH.BusinessIndicators.Web.Public
{
    public partial class LogoutSSO : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SSOToolkit.Instance.Logout();
            Session.RemoveAll();
            // HttpContext.Current.User.Identity.IsAuthenticated
            Response.ContentType = "image/gif";
            try
            {
                System.IO.FileStream fs = new System.IO.FileStream(Server.MapPath("~/images/ajax-loader.gif"), System.IO.FileMode.Open, System.IO.FileAccess.Read);
                byte[] datas = new byte[fs.Length];
                fs.Read(datas, 0, Convert.ToInt32(fs.Length));
                Response.OutputStream.Write(datas, 0, Convert.ToInt32(fs.Length));
                fs.Close();
            }
            catch (System.Exception ex)
            {

            }
            finally
            {
                Response.Flush();
                Response.End();
            }
        }
    }
}