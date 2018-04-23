using Lib.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Engine;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.ViewModel;
using Wanda.Platform.Permission.ClientComponent;
using LJTH.BusinessIndicators.BLL.BizBLL;
using LJTH.BusinessIndicators.Model.BizModel;
using Newtonsoft.Json;
using LJTH.BusinessIndicators.Common;

namespace LJTH.BusinessIndicators.Web.BusinessReport
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
            { }
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
                    List<C_System> listSys = sysList.Where(or => or.Category == 1).Distinct().ToList().OrderBy(or => or.Sequence).ToList();
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
                    ddlSystem.DataSource = StaticResource.Instance.SystemList.Where(p => p.Category == 1).OrderBy(x => x.Sequence).ToList();

                ddlSystem.DataTextField = "SystemName";
                ddlSystem.DataValueField = "ID";
                ddlSystem.DataBind();


                lblName.Text = FinYear + "-" + FinMonth + "月度经营报告上报";

                if (string.IsNullOrEmpty(Request.QueryString["BusinessID"])) //如果BusinessID是Null，代表不是从OA进来的
                {
                    HidSystemID.Value = ddlSystem.SelectedValue;
                    HidSystemBatchID.Value = ddlSystemBatchID.Visible ? ddlSystemBatchID.SelectedValue : Guid.Empty.ToString();
                }
                else
                {
                    //从OA进来的
                    hideMonthReportID.Value = Request["BusinessID"];
                    var bmr = B_MonthlyreportOperator.Instance.GetMonthlyreport(Request["BusinessID"].ToGuid());
                    ddlSystem.SelectedValue = bmr.SystemID.ToString();
                    HidSystemID.Value = ddlSystem.SelectedValue;

                    ddlSystemBatchID.SelectedValue = bmr.SystemBatchID.ToString();
                    HidSystemBatchID.Value = ddlSystemBatchID.SelectedValue;
                }

                HideProcessCode.Value = StaticResource.Instance[ddlSystem.SelectedValue.ToGuid(), DateTime.Now].Configuration.Element("ProcessCode").Value;
                //AddMonthlyReport();//如果当前月不存在月度报告数据，添加一条数据
                InitSystemBatch();

            }
        }
        /// <summary>
        /// 转换系统时，如果当前月不存在月度报告数据，添加一条数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlSystem_TextChanged(object sender, EventArgs e)
        {
            InitSystemBatch();
        }
        /// <summary>
        /// 添加月度报告数据
        /// </summary>
        public void AddMonthlyReport()
        {
            HidSystemID.Value = ddlSystem.SelectedValue;
            HidSystemBatchID.Value = ddlSystemBatchID.Visible ? ddlSystemBatchID.SelectedValue : Guid.Empty.ToString();
            HideProcessCode.Value = StaticResource.Instance[ddlSystem.SelectedValue.ToGuid(), DateTime.Now].Configuration.Element("ProcessCode").Value;

            B_SystemBatch BatchModel = null; // 公用的批次 实体

            B_MonthlyReport bmr = null;

            string Gtypt = StaticResource.Instance[ddlSystem.SelectedValue.ToGuid(), DateTime.Now].GroupType;

            if (!string.IsNullOrEmpty(Gtypt))
            {
                #region 包含在批次里的数据的业务系统

                //判断当前URL是否存在BusinessID
                if (string.IsNullOrEmpty(Request["BusinessID"]))
                {
                    B_MonthlyReport _statreRpt = null; //-状态B_MonthlyReport -

                    BatchModel = B_SystemBatchOperator.Instance.GetSystemBatch(Gtypt, FinYear, FinMonth);
                    //判断批次表中是否有数据
                    if (BatchModel == null)
                    {
                        BatchModel = AddSystemBatch(ddlSystem.SelectedValue.ToGuid(), Gtypt);
                    }
                    else
                    {
                        //批次不为Null，且批次还是在审批中的。
                        //批次是草稿状态
                        List<V_SubReport> V_SubReportList = JsonConvert.DeserializeObject<List<V_SubReport>>(BatchModel.SubReport);

                        //获取上报月报的ID
                        var subRpt = V_SubReportList.Find(SR => SR.SystemID == HidSystemBatchID.Value.ToGuid());

                        B_MonthlyReport monthRpt = B_MonthlyreportOperator.Instance.GetMonthlyreport(subRpt.ReportID);
                        #region  批次存在

                        //审批进行中,是要被跳转到审批页面的，但是审批完成就需要重新开启新的流程
                        if (BatchModel.WFBatchStatus == "Progress" || monthRpt.WFStatus == "Progress")
                        {
                            //批次是草稿状态
                            var model = V_SubReportList.Find(p => p.SystemID == HidSystemBatchID.Value.ToGuid());

                            //---------------------------------------------------------------------------------------------------------------------------------------------
                            //这里批次需要控制，每次审批的状态只能是唯一的
                            // 暂时注销掉  BatchModel = AddSystemBatch();

                            var host = new LJTH.BusinessIndicators.Web.AjaxHander.ProProcessController();
                            host.BusinessID = model.ReportID.ToString();

                            if (BPF.Workflow.Client.WFClientSDK.Exist(host.BusinessID))
                            {
                                BPF.Workflow.Object.WorkflowContext wc = BPF.Workflow.Client.WFClientSDK.GetProcess(null, host.BusinessID);
                                if (!wc.CurrentUserHasTodoTask)
                                {
                                    Server.Transfer("~/BusinessReport/TargetApprove.aspx?BusinessID=" + host.BusinessID);
                                }
                                else
                                {
                                    bmr = monthRpt;
                                    hideMonthReportID.Value = monthRpt.ID.ToString();
                                }
                            }
                        }
                        else if (BatchModel.WFBatchStatus == "Approved") //如果审批是完成状态，重新生产
                        {
                            BatchModel = AddSystemBatch(ddlSystem.SelectedValue.ToGuid(),Gtypt);
                        }
                        else
                        {
                            //批次是草稿状态

                            V_SubReportList.ForEach(p =>
                            {   //选择的是那个系统？
                                if (p.SystemID == Guid.Parse(ddlSystemBatchID.SelectedValue))
                                {  //根据选择的系统，将明细数据展示出来

                                    _statreRpt = B_MonthlyreportOperator.Instance.GetMonthlyreport(p.ReportID);

                                    if (_statreRpt.WFStatus == "Cancel") //如果当前月报是作废的状态，则重新生产一个新的ID   ( 这里针对作废后，重新起一个流程)
                                    {

                                        _statreRpt.ID = Guid.NewGuid();//替换实体ID
                                        _statreRpt.WFStatus = "Draft";
                                        _statreRpt.Status = 2;
                                        Guid _BRptID = B_MonthlyreportOperator.Instance.AddMonthlyreport(_statreRpt);

                                        p.ReportID = _BRptID;
                                        HiddenBatch.Value = BatchModel.ID.ToString(); //批次ID不变
                                        hideMonthReportID.Value = p.ReportID.ToString();
                                    }
                                    else
                                    {
                                        //注意这里
                                        hideMonthReportID.Value = p.ReportID.ToString();
                                        HiddenBatch.Value = p.ReportID.ToString();
                                    }

                                    AddBMRD(_statreRpt);
                                    bmr = _statreRpt;
                                    MutipleUpload.LoadByBusinessID(_statreRpt.ID.ToString());
                                }
                            });


                            //这里重新序列，将作废后，重新生成的新ID 序列化到批次里
                            BatchModel.SubReport = JsonConvert.SerializeObject(V_SubReportList);
                            B_SystemBatchOperator.Instance.UpdateSystemBatch(BatchModel);

                        }

                        #endregion
                    }

                }
                else
                {
                    //如果是传过来的BusinessID，就直接去查询，不做操作
                    //通过BusinessID，首先获取批次的实体，根据权限，然后在批次中寻找
                    HiddenBatch.Value = Request["BusinessID"];
                    bmr = B_MonthlyreportOperator.Instance.GetMonthlyreport(Request["BusinessID"].ToGuid());
                    ExceptionHelper.TrueThrow<ArgumentNullException>(bmr == null ? true : false, "Argument B_MonthlyReport is Null");

                    if (bmr.SystemBatchID != Guid.Empty)
                    {
                        BatchModel = B_SystemBatchOperator.Instance.GetSystemBatch(bmr.SystemBatchID);
                        List<V_SubReport> rptLsit = JsonConvert.DeserializeObject<List<V_SubReport>>(BatchModel.SubReport);
                        V_SubReport rptModel = rptLsit.Find(f => f.SystemID == ddlSystem.SelectedValue.ToGuid());
                        hideMonthReportID.Value = rptModel.ReportID.ToString();
                        bmr = B_MonthlyreportOperator.Instance.GetMonthlyreport(rptModel.ReportID);
                        ExceptionHelper.TrueThrow<ArgumentNullException>(bmr == null ? true : false, "Argument B_MonthlyReport is Null");
                        MutipleUpload.LoadByBusinessID(bmr.ID.ToString());
                        UserControl.SetButtonSpanStyle(bmr.Status);
                    }
                }

                #endregion

            }
            else
            {
                //判断当前URL是否存在BusinessID
                if (string.IsNullOrEmpty(Request["BusinessID"]))
                {
                    //添加月报说明
                    bmr = B_MonthlyreportOperator.Instance.GetMonthlyReportDraft(Guid.Parse(ddlSystem.SelectedValue), Guid.Parse(HidSystemBatchID.Value), FinYear, FinMonth);
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
                    ddlSystemBatchID.SelectedValue = bmr.SystemBatchID.ToString();
                    FinYear = bmr.FinYear;
                    FinMonth = bmr.FinMonth;
                    lblName.Text = "";
                    lblName.Text = FinYear + "-" + FinMonth + "月度经营报告上报";
                }
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
            bmr.SystemBatchID = Guid.Parse(ddlSystemBatchID.Visible ? ddlSystemBatchID.SelectedValue : Guid.Empty.ToString());
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

            bool IsExistence = B_MonthlyreportdetailOperator.Instance.GetMonthlyReportDetailCount(bmr.ID);

            List<B_MonthlyReportDetail> B_ReportDetails = new List<B_MonthlyReportDetail>();

            //listBMRD.Count == 0
            Guid SystemBatchID = Guid.Parse(ddlSystemBatchID.Visible ? ddlSystemBatchID.SelectedValue : Guid.Empty.ToString());
            if (IsExistence == false) // 明细里不存在数据
            {
                //A_MonthlyReport AMonthlyReport = A_MonthlyreportOperator.Instance.GetAMonthlyReport(Guid.Parse(ddlSystem.SelectedValue), FinYear, FinMonth);
                A_MonthlyReport AMonthlyReport = A_MonthlyreportOperator.Instance.GetAMonthlyReport(Guid.Parse(ddlSystem.SelectedValue), SystemBatchID, FinYear, FinMonth);
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
                    //B_MonthlyReport BMonthlyReport = B_MonthlyreportOperator.Instance.GetMonthlyReport(Guid.Parse(ddlSystem.SelectedValue), FinYear, FinMonth, bmr.ID);
                    B_MonthlyReport BMonthlyReport = B_MonthlyreportOperator.Instance.GetMonthlyReport(Guid.Parse(ddlSystem.SelectedValue), SystemBatchID, FinYear, FinMonth, bmr.ID);
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
                    var host = new LJTH.BusinessIndicators.Web.AjaxHander.ProcessController();
                    host.BusinessID = Request["BusinessID"];

                    if (BPF.Workflow.Client.WFClientSDK.Exist(host.BusinessID))
                    {
                        BPF.Workflow.Object.WorkflowContext wc = BPF.Workflow.Client.WFClientSDK.GetProcess(null, host.BusinessID);
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

        protected void ddlSystemBatchID_TextChanged(object sender, EventArgs e)
        {
            AddMonthlyReport();
        }

        private void InitSystemBatch()
        {
            if (ddlSystem.SelectedValue != Guid.Empty.ToString())
            {
                var list = StaticResource.Instance.OrgList[ddlSystem.SelectedValue.ToGuid()].Where(x => x.Level == 3).ToList();
               // var list = S_OrganizationalActionOperator.Instance.GetUserRegional(ddlSystem.SelectedValue.ToGuid(),WebHelper.GetCurrentLoginUser());
                if (list != null && list.Count > 0)
                {
                    ddlSystemBatchID.Visible = true;
                    ddlSystemBatchID.DataSource = list;
                    ddlSystemBatchID.DataTextField = "CnName";
                    ddlSystemBatchID.DataValueField = "ID";
                    ddlSystemBatchID.DataBind();
                }
                else
                {
                    ddlSystemBatchID.Visible = false;
                }
            }
            else
            {
                ddlSystemBatchID.Visible = false;
            }
            AddMonthlyReport();
        }

        /// <summary>
        /// 添加批次Model
        /// </summary>
        /// <returns></returns>
        public B_SystemBatch AddSystemBatch(Guid systemId,string groupType)
        {
            B_SystemBatch BatchModel = new B_SystemBatch();
            BatchModel.BatchType = groupType;
            BatchModel.FinMonth = FinMonth;
            BatchModel.FinYear = FinYear;
            BatchModel.WFBatchStatus = "Draft";
            BatchModel.ID = Guid.NewGuid();

            List<V_SubReport> SubReportList = new List<V_SubReport>();
            //获取项目系统的公司
            // List<C_System> ProSysList = StaticResource.Instance.SystemList.Where(p => p.GroupType == "ProSystem").OrderBy(PR => PR.Sequence).ToList();
            List<S_Organizational> ProSysList = StaticResource.Instance.OrgList[systemId].Where(x => x.Level == 3 && !x.IsCompany).ToList();
            //循环添加B_MonthlyReport表数据
            foreach (var item in ProSysList)
            {
                B_MonthlyReport BatchMonthlyReport = new B_MonthlyReport();
                BatchMonthlyReport.SystemID = systemId; //此处存的是大区id============
                BatchMonthlyReport.AreaID = item.ID;
                BatchMonthlyReport.FinMonth = FinMonth;
                BatchMonthlyReport.FinYear = FinYear;
                BatchMonthlyReport.Status = 2;
                BatchMonthlyReport.WFStatus = "Draft";
                BatchMonthlyReport.SystemBatchID = BatchModel.ID;
                BatchMonthlyReport.CreateTime = DateTime.Now;
                BatchMonthlyReport.ID = B_MonthlyreportOperator.Instance.AddMonthlyreport(BatchMonthlyReport);

                //添加数据 ,如果是当前选择的系统，isreaday==true
                if (ddlSystemBatchID.SelectedValue == item.ID.ToString())
                {
                    SubReportList.Add(new V_SubReport(item.ID, item.CnName, BatchMonthlyReport.ID, false));
                    AddBMRD(BatchMonthlyReport); // 添加明细数据
                    hideMonthReportID.Value = BatchMonthlyReport.ID.ToString(); //将新增的月报ID，存在影藏控件中
                    HiddenBatch.Value = BatchMonthlyReport.ID.ToString();
                    MutipleUpload.LoadByBusinessID(BatchMonthlyReport.ID.ToString()); //添加附件ID
                    UserControl.SetButtonSpanStyle(BatchMonthlyReport.Status); //设置第几步的状态
                }
                else
                {
                    //判断其它系统的提交状态
                    SubReportList.Add(new V_SubReport(item.ID, item.CnName, BatchMonthlyReport.ID, false));
                }
            }

            BatchModel.SubReport = JsonConvert.SerializeObject(SubReportList);

            BatchModel.ID = B_SystemBatchOperator.Instance.AddSystemBatch(BatchModel);


            return BatchModel;
        }

    }
}