using Lib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Wanda.BusinessIndicators.BLL;
using Wanda.BusinessIndicators.Model;

namespace Wanda.BusinessIndicators.Web.TargetConfiguration
{
    public partial class TargetList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
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
                if (sysList.Count < 0)
                {

                    ddlSystem.DataSource = sysList.OrderBy(or => or.Sequence).ToList();
                }
                else
                {
                    ddlSystem.DataSource = StaticResource.Instance.SystemList.Where(p => p.Category != 4).ToList();//如果Category=1  为经管系统
                }

                ddlSystem.DataTextField = "SystemName";
                ddlSystem.DataValueField = "ID";
                ddlSystem.DataBind();//绑定前台下拉框


            }

        }
        List<string> PermissionList = null;
        protected override void OnInit(EventArgs e)
        {
            if (!IsPostBack)
                if (!string.IsNullOrEmpty(Request["BusinessID"]))
                {
                    var host = new Wanda.BusinessIndicators.Web.AjaxHander.ProcessController();
                    
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