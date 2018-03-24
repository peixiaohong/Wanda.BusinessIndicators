using Lib.Core;
using Lib.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Engine;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.ViewModel;
using Newtonsoft.Json;
using LJTH.BusinessIndicators.Common;
using System.Configuration;

namespace LJTH.BusinessIndicators.Web.BusinessReport
{
    public partial class TargetReportApproveTime : System.Web.UI.Page
    {
       public string MonthSG;
       public string MonthSGRent;
        protected void Page_Load(object sender, EventArgs e)
        {
            MonthSG = ConfigurationManager.AppSettings["MonthSG"];
            MonthSGRent = ConfigurationManager.AppSettings["MonthSGRent"];
            List<C_System> sysList = new List<C_System>();

            if (!IsPostBack)
            {
                if (list != null && list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        sysList.AddRange(StaticResource.Instance.SystemList.Where(p => p.SystemName == item.ToString() && p.Category != 3).ToList());
                    }
                }
                if (sysList.Count > 0)
                {
                    ddlSystem.DataSource = sysList.Where(p => p.Category != 3).Distinct().ToList().OrderBy(or => or.Sequence).ToList();
                }
                else
                {
                    ddlSystem.DataSource = StaticResource.Instance.SystemList.Where(p => p.Category != 3).ToList();
                }

               // ddlSystem.DataSource = StaticResource.Instance.SystemList;

                ddlSystem.DataTextField = "SystemName";
                ddlSystem.DataValueField = "ID";
                ddlSystem.DataBind();
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


        List<string> list = null;

        protected override void OnInit(EventArgs e)
        {
            PermissionHelper.GetPermission();
            if (PermissionHelper.EnablePermission)
            {
                list = PermissionHelper.GetStartProcessList();
                ExceptionHelper.TrueThrow<ArgumentNullException>(list == null, "Argument GetStartProcessList is Empty");
            }
        }

    }
}