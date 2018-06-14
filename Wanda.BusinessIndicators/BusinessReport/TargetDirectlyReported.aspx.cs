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
                DateTime datetime = new DateTime();
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
                    if (!string.IsNullOrEmpty(Request["SystemId"]) && Guid.Parse(Request["SystemId"]) != Guid.Empty)
                    {
                        ddlSystem.SelectedIndex = ddlSystem.Items.IndexOf(ddlSystem.Items.FindByValue(Request["SystemId"]));
                    }
                    ChangeSystem();
                }
                var targetPlanDetail = StaticResource.Instance.GetDefaultTargetPlanList(ddlSystem.SelectedValue.ToGuid(), FinYear).FirstOrDefault();
                if (targetPlanDetail == null || targetPlanDetail.TargetPlanID == Guid.Empty)
                {
                    hiddenDis.Value = "2";
                    return;
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
            var targetPlanDetail = StaticResource.Instance.GetDefaultTargetPlanList(ddlSystem.SelectedValue.ToGuid(), FinYear).FirstOrDefault();
            if (targetPlanDetail == null || targetPlanDetail.TargetPlanID == Guid.Empty)
            {
                hiddenDis.Value = "2";
                return;
            }
            AddMonthlyReport();
        }

        /// <summary>
        /// 添加月度报告数据
        /// </summary>
        public void AddMonthlyReport()
        {
            //如果存在审批中的数据，不让重复提交
            var bmrProgress = B_MonthlyreportOperator.Instance.GetMonthlyReportModel(Guid.Parse(ddlSystem.SelectedValue), Guid.Empty, FinYear, FinMonth, 1, "Progress");
            if (bmrProgress != null && bmrProgress.WFStatus == "Progress")
            {
                hiddenDis.Value = "1";
                hideMonthReportID.Value = bmrProgress.ID.ToString();
                return;
            }

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
            else
            {
                UserControl.SetButtonSpanStyle(-1);
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
            else
            {
                return null;
            }
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

            var targetPlan = StaticResource.Instance.GetDefaultTargetPlanList(Guid.Parse(ddlSystem.SelectedValue), FinYear).FirstOrDefault();
            if (targetPlan == null || targetPlan.TargetPlanID == null || targetPlan.TargetPlanID == Guid.Empty)
                return;
            var targetPlanId = targetPlan.TargetPlanID;
            List<B_MonthlyReportDetail> B_ReportDetails = new List<B_MonthlyReportDetail>();
            if (IsExistence == false)
            {
                A_MonthlyReport AMonthlyReport = A_MonthlyreportOperator.Instance.GetAMonthlyReport(Guid.Parse(ddlSystem.SelectedValue), Guid.Empty, FinYear, FinMonth, targetPlanId);
                //如果A表有数据从A表取数据，否则取B表上一版本的数据。
                if (AMonthlyReport != null)
                {
                    //从A表获取数据插入B表
                    bmr.Description = AMonthlyReport.Description;//月报说明
                    B_MonthlyreportOperator.Instance.UpdateMonthlyreport(bmr);//更新月报说明

                    B_ReportDetails = B_MonthlyreportdetailOperator.Instance.GetMonthlyReportDetail_ByAToB(FinYear, FinMonth, AMonthlyReport.SystemID, bmr.AreaID, bmr.ID, targetPlanId);
                }
                else
                {
                    B_MonthlyReport BMonthlyReport = B_MonthlyreportOperator.Instance.GetMonthlyReport(Guid.Parse(ddlSystem.SelectedValue), bmr.AreaID, FinYear, FinMonth, bmr.ID, targetPlanId);
                    if (BMonthlyReport != null)
                    {
                        //从B表获取上一版本数据插入B表
                        B_ReportDetails = B_MonthlyreportdetailOperator.Instance.GetMonthlyReportDetail_ByBToB(FinYear, FinMonth, BMonthlyReport.SystemID, BMonthlyReport.AreaID, bmr.ID);
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