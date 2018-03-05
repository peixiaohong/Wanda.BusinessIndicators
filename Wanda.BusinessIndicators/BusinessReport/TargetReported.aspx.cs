using Lib.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Wanda.BusinessIndicators.BLL;
using Wanda.BusinessIndicators.Engine;
using Wanda.BusinessIndicators.Model;
using Wanda.BusinessIndicators.ViewModel;
using Wanda.Platform.Permission.ClientComponent;


namespace Wanda.BusinessIndicators.Web.BusinessReport
{
    public partial class TargetReported : System.Web.UI.Page
    {

        int FinYear;
        int FinMonth;
        protected void Page_Load(object sender, EventArgs e)
        {
            //JS_WF_JY_Starter
            try
            {
               

                DateTime datetime = new DateTime();
                //if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["TestReportDateTime"]))
                //{
                //    datetime = DateTime.Parse(ConfigurationManager.AppSettings["TestReportDateTime"]);
                //    FinYear = datetime.Year;
                //    FinMonth = datetime.Month;
                //}

                Hide_sg.Value = ConfigurationManager.AppSettings["MonthSG"].ToLower();
                Hide_sgrent.Value = ConfigurationManager.AppSettings["MonthSGRent"].ToLower();

                datetime = StaticResource.Instance.GetReportDateTime();
                FinYear = datetime.Year;
                FinMonth = datetime.Month;
            }
            catch (System.FormatException)
            {}
            hideFinYear.Value = FinYear.ToString();
            hideFinMonth.Value = FinMonth.ToString();

            if (!IsPostBack)
            {
                
                List<C_System> sysList = new List<C_System>();
                if (PermissionList != null && PermissionList.Count > 0)
                {
                    foreach (var item in PermissionList)
                    {
                        sysList.AddRange(StaticResource.Instance.SystemList.Where(p => p.SystemName == item.ToString()).Distinct().ToList());
                    }
                }


                if (sysList.Count > 0)
                {
                    //Category ==1 代表的是经营系统
                    List<C_System> listSys=sysList.Where(or => or.Category == 1).Distinct().ToList().OrderBy(or => or.Sequence).ToList();
                    ddlSystem.DataSource = listSys;
                    if (string.IsNullOrEmpty(Request.QueryString["BusinessID"]))
                    {
                        if (listSys.Count == 0 && sysList.Count > 0)
                        {
                            C_System cs = sysList.FirstOrDefault();
                            if (cs.Category == 2)
                            {
                                Server.Transfer("~/BusinessReport/TargetProReported.aspx");
                            }
                            else if (cs.Category == 3)
                            {
                                Server.Transfer("~/BusinessReport/TargetGroupReported.aspx");
                            }
                            else if (cs.Category == 4)
                            {
                                Server.Transfer("~/BusinessReport/TargetDirectlyReported.aspx");
                            }
                        }
                    }
                }
                else
                    ddlSystem.DataSource = StaticResource.Instance.SystemList.Where(p => p.Category == 1).ToList();
                
                ddlSystem.DataTextField = "SystemName";
                ddlSystem.DataValueField = "ID";
                ddlSystem.DataBind();

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
                }


                HideProcessCode.Value = StaticResource.Instance[ddlSystem.SelectedValue.ToGuid(),DateTime.Now].Configuration.Element("ProcessCode").Value;
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
            AddMonthlyReport();

        }
        /// <summary>
        /// 添加月度报告数据
        /// </summary>
        public void AddMonthlyReport()
        {
            HidSystemID.Value = ddlSystem.SelectedValue;
            HideProcessCode.Value = StaticResource.Instance[ddlSystem.SelectedValue.ToGuid(),DateTime.Now].Configuration.Element("ProcessCode").Value;

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
            bmr.WFStatus = "Draft";
            bmr.CreateTime = DateTime.Now;
            bmr.ID = B_MonthlyreportOperator.Instance.AddMonthlyreport(bmr);
            return bmr;
        }


        /// <summary>
        /// 获取A表的数据插入B表
        /// </summary>
        /// <param name="bmr"></param>
        public void AddBMRD(B_MonthlyReport bmr)
        {
            //IList<B_MonthlyReportDetail> listBMRD = B_MonthlyreportdetailOperator.Instance.GetMonthlyreportdetailList(bmr.ID);

            bool IsExistence  = B_MonthlyreportdetailOperator.Instance.GetMonthlyReportDetailCount(bmr.ID);
            
            List<B_MonthlyReportDetail> B_ReportDetails = new List<B_MonthlyReportDetail>();

            //listBMRD.Count == 0

            if (IsExistence==false) // 明细里不存在数据
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
                   // B_MonthlyreportdetailOperator.Instance.AddOrUpdateTargetDetail(B_ReportDetails, "Insert");//更新月度经营明细表
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
                    var host = new Wanda.BusinessIndicators.Web.AjaxHander.ProcessController();
                    host.BusinessID = Request["BusinessID"];

                      if (Wanda.Workflow.Client.WFClientSDK.Exist(host.BusinessID))
                    {
                        Wanda.Workflow.Object.WorkflowContext wc = Wanda.Workflow.Client.WFClientSDK.GetProcess(null, host.BusinessID);
                        if (!wc.CurrentUserHasTodoTask)
                        {
                            Server.Transfer("~/BusinessReport/TargetApprove.aspx?BusinessID=" + host.BusinessID);
                        }
                    }

                    //WfClientListener.Listen(host, null);
                    //if (WfClientContext.Current.ProcessResponse.FromReadOnly)
                    //{
                    //    HttpContext.Current.Response.Clear();
                    //    Server.Transfer("~/BusinessReport/TargetApprove.aspx?BusinessID=" + host.BusinessID);
                    //}
                }
            PermissionHelper.GetPermission();
            if (PermissionHelper.EnablePermission)
            {
                PermissionList = PermissionHelper.GetStartProcessList();
                ExceptionHelper.TrueThrow<ArgumentNullException>(PermissionList == null, "Argument GetStartProcessList is Empty");

            }// ExceptionHelper.TrueThrow<ArgumentNullException>(PermissionList.Count == 0, "Argument GetStartProcessList is Count = 0");
        }

    }
}