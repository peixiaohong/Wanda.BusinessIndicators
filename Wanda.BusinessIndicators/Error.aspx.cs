using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LJTH.BusinessIndicators.Web
{
    public partial class Error : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Exception erroy = Context.Items[Global.ErrorForContextKey] as Exception;

                Exception exp = erroy;
                while (exp.InnerException != null)
                {
                    exp = exp.InnerException;
                    ErrorTitle.Text = exp.Message;
                }
                ErrorInfo.Text = erroy.ToString().Replace(@"
", "</br>");
            }
        }

    }
}