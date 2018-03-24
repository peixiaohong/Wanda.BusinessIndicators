using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LJTH.BusinessIndicators.BLL;

namespace LJTH.BusinessIndicators.Web
{
    public partial class ContrastCompleteDetail : System.Web.UI.Page
    {
        private int finMonth;
        private int finYear;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DateTime datetime = StaticResource.Instance.GetReportDateTime();
                finMonth = datetime.Month;
                finYear = datetime.Year;
                 List<int> Year = new List<int>();
                for (int i = -5; i < 5; i++)
                {
                    Year.Add(DateTime.Now.Year + i);
                }
                ddlYear.DataSource = Year;
                ddlYear.DataBind();

                ddlYear.SelectedValue = finYear.ToString();



                List<int> Month = new List<int>();
                for (int i = 1; i <= 12; i++)
                {
                    Month.Add(i);
                }
                ddlMonth.DataSource = Month;
                ddlMonth.DataBind();
        
                ddlMonth.SelectedValue = finMonth.ToString();
            }
        }
    }
}