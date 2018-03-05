using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Wanda.BusinessIndicators.Engine;
using Wanda.BusinessIndicators.BLL;
namespace Wanda.BusinessIndicators.Web.ProtoType
{
    public partial class TargetSimpleReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Syscategory.DataSource = SysConfigOperator.Instance.GetSysConfigListByType("SysCategory");
                Syscategory.DataBind();
                int year = DateTime.Now.Year;
                CheckYear.Items.Add(new ListItem((year - 1).ToString(), (year - 1).ToString()));
                CheckYear.Items.Add(new ListItem((year).ToString(), (year).ToString()));
                CheckYear.Items.Add(new ListItem((year + 1).ToString(), (year + 1).ToString()));
                CheckYear.Items.Add(new ListItem((year + 2).ToString(), (year + 2).ToString()));
                for (int i = 1; i <= 12; i++)
                {
                    CheckMonth.Items.Add(new ListItem(i + "月", i.ToString()));
                }
            }
            CheckMonth.SelectedValue = DateTime.Now.Month == 1 ? "12" : (DateTime.Now.Month - 1).ToString();
            CheckYear.SelectedValue = DateTime.Now.Month == 1 ? (DateTime.Now.Year - 1).ToString() : DateTime.Now.Year.ToString();
        }
    }
}