using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Plugin.SSO;

namespace Mobile.web.Public
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var flag = Request["flag"];
            if (!string.IsNullOrEmpty(flag) && int.Parse(flag) == 1)
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "", "alert('该用户组织架构不存在，请联系管理员！');", true);
            }
        }
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            var userName = this.txtUserName.Text;
            var userPwd = this.txtUserPassword.Text;
            var result = SSOBuilder.Validation(userName, userPwd);
            if (!result)
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "", "alert('用户名或密码错误');", true);
                return;
            }
            SSOBuilder.Authorization(userName, userPwd);

            var returnUrl = Request["returnUrl"];
            if (string.IsNullOrEmpty(returnUrl) || "/".Equals(returnUrl))
            {
                Response.Redirect("~/Report/ReportMonth.aspx");
            }
            else
            {
                Response.Redirect(Server.UrlDecode(returnUrl));
            }
        }

    }
}