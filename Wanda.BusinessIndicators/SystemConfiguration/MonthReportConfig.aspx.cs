using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Model;

namespace LJTH.BusinessIndicators.Web.SystemConfiguration
{
    public partial class MonthReportConfig : System.Web.UI.Page
    {

        List<string> list = null;

        protected void Page_Load(object sender, EventArgs e)
        {

            List<C_System> sysList = new List<C_System>();
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["IsLastestVersion"]))
                {
                    chkIsLastestVersion.Checked = bool.Parse(HttpUtility.UrlDecode(Request.QueryString["IsLastestVersion"]));
                }

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

                //if (!string.IsNullOrEmpty(sysID))
                //{
                //    ddlSystem.SelectedValue = sysID;
                //}

                List<int> Year = new List<int>();
                for (int i = -5; i < 5; i++)
                {
                    Year.Add(DateTime.Now.Year + i);
                }
                ddlYear.DataSource = Year;
                ddlYear.DataBind();

                ddlYear.SelectedValue =DateTime.Now.Year.ToString();


                List<int> Month = new List<int>();
                for (int i = 1; i <= 12; i++)
                {
                    Month.Add(i);
                }
                ddlMonth.DataSource = Month;
                ddlMonth.DataBind();

                ddlMonth.SelectedValue = DateTime.Now.Month.ToString();

            }

        }
    }
}