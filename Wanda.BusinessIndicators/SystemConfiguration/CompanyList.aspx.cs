﻿using Lib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Model;
namespace LJTH.BusinessIndicators.Web
{
    public partial class CompanyList : System.Web.UI.Page
    {
     
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string ID = Request.QueryString["ID"];
                SysID.Value = ID;
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
                    if (BPF.Workflow.Client.WFClientSDK.Exist(host.BusinessID))
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