using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LJTH.BusinessIndicators.Web;

namespace LJTH.BusinessIndicators.Web.UserControl
{
    public partial class MonthReportedAction :System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            List<string> funcPerission = PermissionHelper.GetMonthReportLogList();
            if (funcPerission != null)
            {
                if (funcPerission.Count > 0)
                {
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "MyFunction", "<script>document.getElementById('MonthReportLog').style.display = 'block';</script>");
                }
              
            }
            if (ConfigurationManager.AppSettings["MonthReportLog"] != null)
            {
                if (ConfigurationManager.AppSettings["MonthReportLog"].ToLower() == "true")
                {
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "MyFunction", "<script>document.getElementById('MonthReportLog').style.display = 'block';</script>");
                }
            }
        }
    }
}