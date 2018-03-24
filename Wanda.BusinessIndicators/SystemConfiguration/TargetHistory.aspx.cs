using Lib.Core;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Engine;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.ViewModel;
using Wanda.Platform.Permission.ClientComponent;

namespace LJTH.BusinessIndicators.Web
{
    public partial class TargetHistory : System.Web.UI.Page
    {
        private string sysID;
        private int finMonth;
        private int finYear;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (finMonth == 0 && finYear == 0)
            {
                DateTime datetime = StaticResource.Instance.GetReportDateTime();
                finMonth = datetime.Month;
                finYear = datetime.Year;
            }

            if (!IsPostBack)
            {

                List<C_System> sysList = new List<C_System>();
                if (PermissionList != null && PermissionList.Count > 0)
                {
                    foreach (var item in PermissionList)
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

                //sysList = StaticResource.Instance.SystemList.ToList();
                //ddlSystem.DataSource = sysList.OrderBy(or => or.Sequence).ToList();
                ddlSystem.DataTextField = "SystemName";
                ddlSystem.DataValueField = "ID";
                ddlSystem.DataBind();//绑定前台下拉框
                //ddlSystem_TextChanged(ddlSystem.SelectedValue, null);
                List<int> Year = new List<int>();
                for (int i = -5; i < 5; i++)
                {
                    Year.Add(DateTime.Now.Year + i);
                }
                ddlYear.DataSource = Year;
                ddlYear.DataBind();

                ddlYear.SelectedValue = finYear.ToString();
            }
        }


        List<string> PermissionList = null;
        protected override void OnInit(EventArgs e)
        {
            if (!IsPostBack)
                if (!string.IsNullOrEmpty(Request["BusinessID"]))
                {
                    var host = new LJTH.BusinessIndicators.Web.AjaxHander.ProcessController();
                    host.BusinessID = Request["BusinessID"];


                    if (Wanda.Workflow.Client.WFClientSDK.Exist(host.BusinessID))
                    {
                        HttpContext.Current.Response.Clear();
                        Server.Execute("~/BusinessReport/TargetApprove.aspx");
                    }

                    //WfClientListener.Listen(host, null);
                    //if (WfClientContext.Current.ProcessResponse.FromReadOnly)
                    //{
                    //    HttpContext.Current.Response.Clear();
                    //    Server.Execute("~/BusinessReport/TargetApprove.aspx");
                    //}
                }
            PermissionHelper.GetPermission();
            if (PermissionHelper.EnablePermission)
            {
                PermissionList = PermissionHelper.GetStartProcessList();
                ExceptionHelper.TrueThrow<ArgumentNullException>(PermissionList == null, "Argument GetStartProcessList is Empty");
            }
        }
    }
}
