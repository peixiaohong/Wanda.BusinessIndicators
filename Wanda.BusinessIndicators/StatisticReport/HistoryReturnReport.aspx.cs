using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Wanda.BusinessIndicators.BLL;
using Wanda.BusinessIndicators.Model;

namespace Wanda.BusinessIndicators.Web.StatisticReport
{
    public partial class HistoryReturnReport : System.Web.UI.Page
    {

        private string sysID;
        private int finYear;

        List<string> list = null;
        List<string> list1 = null;

        protected void Page_Load(object sender, EventArgs e)
        {


            if (!string.IsNullOrEmpty(Request.QueryString["_sysid"]))
            {
                sysID = HttpUtility.UrlDecode(Request.QueryString["_sysid"]);
            }

      

            if (!string.IsNullOrEmpty(Request.QueryString["_finYear"]))
            {
                finYear = int.Parse(HttpUtility.UrlDecode(Request.QueryString["_finYear"]));
            }
            if ( finYear == 0)
            {
                DateTime datetime = StaticResource.Instance.GetReportDateTime();
                finYear = datetime.Year;
            }


            List<C_System> sysList = new List<C_System>();
            if (!IsPostBack)
            {
                if (list != null && list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        sysList.AddRange(StaticResource.Instance.SystemList.Where(p => p.SystemName == item.ToString()).ToList());
                    }
                }
                if (sysList.Count > 0)
                {
                    ddlSystem.DataSource = sysList.Distinct().ToList().OrderBy(or => or.Sequence).ToList();
                }
                else
                {
                    ddlSystem.DataSource = StaticResource.Instance.SystemList.ToList();
                }

                //ddlSystem.DataSource = StaticResource.Instance.SystemList.ToList();

                ddlSystem.DataTextField = "SystemName";
                ddlSystem.DataValueField = "ID";
                ddlSystem.DataBind();

                if (!string.IsNullOrEmpty(sysID))
                {
                    ddlSystem.SelectedValue = sysID;
                }

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

            }
        }

        protected void ddlSystem_SelectedIndexChanged(object sender, EventArgs e)
        {

        }




    }
}