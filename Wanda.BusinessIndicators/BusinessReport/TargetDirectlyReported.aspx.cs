using Lib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.BLL.BizBLL;
using LJTH.BusinessIndicators.Common;

namespace LJTH.BusinessIndicators.Web.BusinessReport
{
    public partial class TargetDirectlyReported : System.Web.UI.Page
    {
        int FinYear;
        int FinMonth;
        protected void Page_Load(object sender, EventArgs e)
        {
            //JS_WF_JY_Starter
            try
            {
                //if (FinMonth == 12)
                //    FinYear = FinYear - 1;

                DateTime datetime = new DateTime();
                //if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["TestReportDateTime"]))
                //{
                //    datetime = DateTime.Parse(ConfigurationManager.AppSettings["TestReportDateTime"]);
                //    FinYear = datetime.Year;
                //    FinMonth = datetime.Month;
                //}
                datetime = StaticResource.Instance.GetReportDateTime();
                FinYear = datetime.Year;
                FinMonth = datetime.Month;
            }
            catch (System.FormatException)
            { }
            hideFinYear.Value = FinYear.ToString();
            hideFinMonth.Value = FinMonth.ToString();

            if (!IsPostBack)
            {
                #region 原来的权限控制

                //List<C_System> sysList = new List<C_System>();
                //if (PermissionList != null && PermissionList.Count > 0)
                //{
                //    foreach (var item in PermissionList)
                //    {
                //        sysList.AddRange(StaticResource.Instance.SystemList.Where(p => p.SystemName == item.ToString()).ToList());
                //    }
                //}


                //if (sysList.Count > 0)
                //{
                //    //Category ==4 代表的是直管系统
                //    //ddlSystem.DataSource = sysList.Where(p => p.Category == 4).Distinct().ToList().OrderBy(or => or.Sequence).ToList();
                //    List<C_System> listSys = sysList.Where(or => or.Category == 4).Distinct().ToList().OrderBy(or => or.Sequence).ToList();
                //    ddlSystem.DataSource = listSys;
                //    if(string.IsNullOrEmpty(Request.QueryString["BusinessID"]))
                //    {
                //        if (listSys.Count == 0 && sysList.Count > 0)
                //        {
                //            C_System cs = sysList.FirstOrDefault();
                //            if (cs.Category == 1)
                //            {
                //                Server.Transfer("~/BusinessReport/TargetReported.aspx");
                //            }
                //            else if (cs.Category == 2)
                //            {
                //                Server.Transfer("~/BusinessReport/TargetProReported.aspx");
                //            }
                //            else if (cs.Category == 3)
                //            {
                //                Server.Transfer("~/BusinessReport/TargetGroupReported.aspx");
                //            }
                //        }
                //    }
                //}
                //else
                //{
                //    ddlSystem.DataSource = StaticResource.Instance.SystemList.Where(p => p.Category == 4).ToList();
                //}
                #endregion

                var _SystemIds = S_OrganizationalActionOperator.Instance.GetUserSystemData(WebHelper.GetCurrentLoginUser()).Select(v => v.SystemID).ToList();

                if (_SystemIds == null || _SystemIds.Count == 0)
                {
                    Response.Redirect("~/NoPermission.aspx");
                    return;
                }
                List<C_System> sysList = StaticResource.Instance.SystemList.Where(p => _SystemIds.Contains(p.ID)).OrderBy(x => x.Sequence).ToList();

                //获取当前人拥有的系统板块

                ddlSystem.DataSource = sysList;
                ddlSystem.DataTextField = "SystemName";
                ddlSystem.DataValueField = "ID";
                ddlSystem.DataBind();
                if (string.IsNullOrEmpty(Request.QueryString["BusinessID"]))
                {
                    C_System cs;
                    if (!string.IsNullOrEmpty(Request["SystemId"]) && Guid.Parse(Request["SystemId"]) != Guid.Empty)
                    {
                        cs = sysList.Where(v => v.ID == Guid.Parse(Request["SystemId"])).FirstOrDefault();
                        ddlSystem.SelectedIndex = ddlSystem.Items.IndexOf(ddlSystem.Items.FindByValue(cs.ID.ToString()));
                    }
                    else
                    {
                        cs = sysList.FirstOrDefault();
                    }
                    if (cs.Category == 1)
                    {
                        //Server.Transfer("~/BusinessReport/TargetReported.aspx?SystemId=" + cs.ID);
                        Response.Redirect("~/BusinessReport/TargetReported.aspx?SystemId=" + cs.ID);
                    }
                    else if (cs.Category == 2)
                    {
                        //Server.Transfer("~/BusinessReport/TargetProReported.aspx?SystemId=" + cs.ID);
                        Response.Redirect("~/BusinessReport/TargetProReported.aspx?SystemId=" + cs.ID);
                    }
                    else if (cs.Category == 3)
                    {
                        //Server.Transfer("~/BusinessReport/TargetGroupReported.aspx?SystemId=" + cs.ID);
                        Response.Redirect("~/BusinessReport/TargetGroupReported.aspx?SystemId=" + cs.ID);
                    }
                }

                lblName.Text = FinYear + "-" + FinMonth + "月度经营报告上报";
                if (string.IsNullOrEmpty(Request.QueryString["BusinessID"])) //如果BusinessID是Null，代表不是从OA进来的
                {
                    HidSystemID.Value = ddlSystem.SelectedValue;
                }
                else
                {
                    //从OA进来的
                    hideMonthReportID.Value = Request["BusinessID"];
                    var bmr = B_MonthlyreportOperator.Instance.GetMonthlyreport(Request["BusinessID"].ToGuid());
                    ddlSystem.SelectedValue = bmr.SystemID.ToString();
                    HidSystemID.Value = ddlSystem.SelectedValue;
                    //判断是否是是该类型下的板块
                    ChangeSystem();
                }

                HideProcessCode.Value = StaticResource.Instance[ddlSystem.SelectedValue.ToGuid(), DateTime.Now].Configuration.Element("ProcessCode").Value;
                AddMonthlyReport();//如果当前月不存在月度报告数据，添加一条数据
            }
        }
        /// <summary>
        /// 转换系统时，如果当前月不存在月度报告数据，添加一条数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlSystem_TextChanged(object sender, EventArgs e)
        {
            ChangeSystem();
            AddMonthlyReport();
        }

        /// <summary>
        /// 添加月度报告数据
        /// </summary>
        public void AddMonthlyReport()
        {
            HidSystemID.Value = ddlSystem.SelectedValue;
            HideProcessCode.Value = StaticResource.Instance[ddlSystem.SelectedValue.ToGuid(), DateTime.Now].Configuration.Element("ProcessCode").Value;

            B_MonthlyReport bmr = null;
            //判断当前URL是否存在BusinessID
            if (string.IsNullOrEmpty(Request["BusinessID"]))
            {
                //添加月报说明
                bmr = B_MonthlyreportOperator.Instance.GetMonthlyReportDraft(Guid.Parse(ddlSystem.SelectedValue), FinYear, FinMonth);
                if (bmr == null)
                {
                    bmr = AddMR();
                }
                else
                {
                    if (bmr.WFStatus == "Progress" || bmr.WFStatus == "Approved")
                    {
                        bmr = AddMR();

                    }
                }
                //为当前月度经营报告（bmr），插入月度经营明细数据
                AddBMRD(bmr);
                hideMonthReportID.Value = bmr.ID.ToString();
            }
            else
            {
                hideMonthReportID.Value = Request["BusinessID"];
                bmr = B_MonthlyreportOperator.Instance.GetMonthlyreport(Request["BusinessID"].ToGuid());
                ddlSystem.SelectedValue = bmr.SystemID.ToString();
                FinYear = bmr.FinYear;
                FinMonth = bmr.FinMonth;
                lblName.Text = "";
                lblName.Text = FinYear + "-" + FinMonth + "月度经营报告上报";
            }

            if (bmr != null)
            {
                MutipleUpload.LoadByBusinessID(bmr.ID.ToString());
                UserControl.SetButtonSpanStyle(bmr.Status);
            }
        }

        public B_MonthlyReport AddMR()
        {
            B_MonthlyReport bmr = new B_MonthlyReport();
            bmr.SystemID = Guid.Parse(ddlSystem.SelectedValue);
            bmr.FinMonth = FinMonth;
            bmr.FinYear = FinYear;
            bmr.Status = 2;
            bmr.DefaultVersionStatus = 1;
            bmr.WFStatus = "Draft";
            bmr.CreateTime = DateTime.Now;
            var targetPlanDetail = StaticResource.Instance.GetDefaultTargetPlanList(bmr.SystemID, bmr.FinYear).FirstOrDefault();
            if (targetPlanDetail != null && targetPlanDetail.TargetPlanID != null)
                bmr.TargetPlanID = targetPlanDetail.TargetPlanID;
            bmr.ID = B_MonthlyreportOperator.Instance.AddMonthlyreport(bmr);
            return bmr;
        }


        /// <summary>
        /// 获取A表的数据插入B表
        /// </summary>
        /// <param name="bmr"></param>
        public void AddBMRD(B_MonthlyReport bmr)
        {
            bool IsExistence = B_MonthlyreportdetailOperator.Instance.GetMonthlyReportDetailCount(bmr.ID);

            List<B_MonthlyReportDetail> B_ReportDetails = new List<B_MonthlyReportDetail>();
            if (IsExistence == false)
            {
                A_MonthlyReport AMonthlyReport = A_MonthlyreportOperator.Instance.GetAMonthlyReport(Guid.Parse(ddlSystem.SelectedValue), FinYear, FinMonth);
                //如果A表有数据从A表取数据，否则取B表上一版本的数据。
                if (AMonthlyReport != null)
                {
                    //从A表获取数据插入B表
                    bmr.Description = AMonthlyReport.Description;//月报说明
                    B_MonthlyreportOperator.Instance.UpdateMonthlyreport(bmr);//更新月报说明

                    B_ReportDetails = B_MonthlyreportdetailOperator.Instance.GetMonthlyReportDetail_ByAToB(FinYear, FinMonth, AMonthlyReport.SystemID, bmr.ID);
                }
                else
                {
                    B_MonthlyReport BMonthlyReport = B_MonthlyreportOperator.Instance.GetMonthlyReport(Guid.Parse(ddlSystem.SelectedValue), FinYear, FinMonth, bmr.ID);
                    if (BMonthlyReport != null)
                    {
                        //从B表获取上一版本数据插入B表
                        B_ReportDetails = B_MonthlyreportdetailOperator.Instance.GetMonthlyReportDetail_ByBToB(FinYear, FinMonth, BMonthlyReport.SystemID, BMonthlyReport.ID, bmr.ID);
                    }
                }

                //判断当前B_MonthlyReportID在明细表中是否有数据。
                if (B_ReportDetails.Count > 0)
                {
                    bmr.Status = 5;
                    B_MonthlyreportOperator.Instance.UpdateMonthlyreport(bmr);//更新月度经营表
                    //B_MonthlyreportdetailOperator.Instance.AddOrUpdateTargetDetail(B_ReportDetails, "Insert");//更新月度经营明细表
                }
            }
        }

        //权限list
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
                        BPF.Workflow.Object.WorkflowContext wc = BPF.Workflow.Client.WFClientSDK.GetProcess(null, host.BusinessID);
                        if (!wc.CurrentUserHasTodoTask)
                        {
                            Server.Transfer("~/BusinessReport/DirectlyTargetApprove.aspx?BusinessID=" + host.BusinessID);
                        }
                    }

                    //WfClientListener.Listen(host, null);
                    //if (WfClientContext.Current.ProcessResponse.FromReadOnly)
                    //{
                    //    HttpContext.Current.Response.Clear();
                    //    Server.Transfer("~/BusinessReport/DirectlyTargetApprove.aspx?BusinessID=" + host.BusinessID);
                    //}
                }
            PermissionHelper.GetPermission();
            if (PermissionHelper.EnablePermission)
            {
                PermissionList = PermissionHelper.GetStartProcessList();
                ExceptionHelper.TrueThrow<ArgumentNullException>(PermissionList == null, "Argument GetStartProcessList is Empty");

            }// ExceptionHelper.TrueThrow<ArgumentNullException>(PermissionList.Count == 0, "Argument GetStartProcessList is Count = 0");
        }


        /// <summary>
        /// 根据更改的板块加载不通页面
        /// </summary>
        private void ChangeSystem()
        {
            C_System cs = C_SystemOperator.Instance.GetSystem(Guid.Parse(ddlSystem.SelectedValue));
            if (cs.Category == 1)
            {
                //Server.Transfer("~/BusinessReport/TargetReported.aspx?SystemId=" + cs.ID);
                Response.Redirect("~/BusinessReport/TargetReported.aspx?SystemId=" + cs.ID);
            }
            else if (cs.Category == 2)
            {
                //Server.Transfer("~/BusinessReport/TargetProReported.aspx?SystemId=" + cs.ID);
                Response.Redirect("~/BusinessReport/TargetProReported.aspx?SystemId=" + cs.ID);
            }
            else if (cs.Category == 3)
            {
                //Server.Transfer("~/BusinessReport/TargetGroupReported.aspx?SystemId=" + cs.ID);
                Response.Redirect("~/BusinessReport/TargetGroupReported.aspx?SystemId=" + cs.ID);
            }
        }
    }
}