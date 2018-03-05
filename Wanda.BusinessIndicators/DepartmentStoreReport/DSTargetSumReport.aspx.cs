using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Wanda.BusinessIndicators.BLL;
using Wanda.BusinessIndicators.Model;

namespace Wanda.BusinessIndicators.Web.DepartmentStoreReport
{
    public partial class DSTargetSumReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                List<C_System> SystemList = StaticResource.Instance.SystemList.ToList();
                ddlSystem.DataSource = SystemList;
                ddlSystem.DataTextField = "SystemName";
                ddlSystem.DataValueField = "ID";
                ddlSystem.DataBind();
                C_System SystemModel = SystemList.SingleOrDefault(t => t.SystemName == "百货系统");
                if (SystemModel != null)
                {
                    ddlSystem.SelectedValue = SystemModel.ID.ToString();
                }
                ddlSystem.Enabled = false;
                List<int> Year = new List<int>();
                for (int i = -5; i < 5; i++)
                {
                    Year.Add(DateTime.Now.Year + i);
                }
                ddlYear.DataSource = Year;
                ddlYear.DataBind();
                ddlYear.SelectedValue = DateTime.Now.Year.ToString();
                List<int> Month = new List<int>();
                for (int i = 1; i <= 12; i++)
                {
                    Month.Add(i);
                }
                ddlMonth.DataSource = Month;
                ddlMonth.DataBind();
                if (DateTime.Now.Month == 1)
                {
                    ddlYear.SelectedValue = (DateTime.Now.Year - 1).ToString();
                    ddlMonth.SelectedValue = "12";
                }
                else
                {
                    ddlMonth.SelectedValue = (DateTime.Now.Month - 1).ToString();
                }
            }
        }
    }
}