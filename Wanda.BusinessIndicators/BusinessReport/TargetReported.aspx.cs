using Lib.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.BLL.BizBLL;
using LJTH.BusinessIndicators.Common;
using LJTH.BusinessIndicators.Model.BizModel;
using System.Linq;
using LJTH.BusinessIndicators.ViewModel;
using Newtonsoft.Json;

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

                #region 原来的获取方法
                //List<C_System> sysList = new List<C_System>();
                //if (PermissionList != null && PermissionList.Count > 0)
                //{
                //    foreach (var item in PermissionList)
                //    {
                //        sysList.AddRange(StaticResource.Instance.SystemList.Where(p => p.SystemName == item.ToString()).Distinct().ToList());
                //    }
                //}


                //if (sysList.Count > 0)
                //{
                //    //Category ==1 代表的是经营系统
                //    List<C_System> listSys = sysList.Where(or => or.Category == 1).Distinct().ToList().OrderBy(or => or.Sequence).ToList();
                //    ddlSystem.DataSource = listSys;
                //    if (string.IsNullOrEmpty(Request.QueryString["BusinessID"]))
                //    {
                //        if (listSys.Count == 0 && sysList.Count > 0)
                //        {
                //            C_System cs = sysList.FirstOrDefault();
                //            if (cs.Category == 2)
                //            {
                //                Server.Transfer("~/BusinessReport/TargetProReported.aspx");
                //            }
                //            else if (cs.Category == 3)
                //            {
                //                Server.Transfer("~/BusinessReport/TargetGroupReported.aspx");
                //            }
                //            else if (cs.Category == 4)
                //            {
                //                Server.Transfer("~/BusinessReport/TargetDirectlyReported.aspx");
                //            }
                //        }
                //    }
                //}
                //else
                //    ddlSystem.DataSource = StaticResource.Instance.SystemList.Where(p => p.Category == 1).OrderBy(x => x.Sequence).ToList();
                #endregion


                var _SystemIds = S_OrganizationalActionOperator.Instance.GetUserSystemData(WebHelper.GetCurrentLoginUser()).Select(v => v.SystemID).ToList();

                if (_SystemIds == null || _SystemIds.Count == 0)
                {
                    Response.Redirect("~/NoPermission.aspx");
                    return;
                }
                //获取当前人拥有的系统板块
                List<C_System> c_SystemList = StaticResource.Instance.SystemList.Where(p => _SystemIds.Contains(p.ID)).OrderBy(x => x.Sequence).ToList();

                ddlSystem.DataSource = c_SystemList;
                ddlSystem.DataTextField = "SystemName";
                ddlSystem.DataValueField = "ID";
                ddlSystem.DataBind();
                if (string.IsNullOrEmpty(Request.QueryString["BusinessID"]))
                {
                    C_System cs;
                    if (!string.IsNullOrEmpty(Request["SystemId"]) && Guid.Parse(Request["SystemId"]) != Guid.Empty)
                    {
                        cs = c_SystemList.Where(v => v.ID == Guid.Parse(Request["SystemId"])).FirstOrDefault();
                        ddlSystem.SelectedIndex = ddlSystem.Items.IndexOf(ddlSystem.Items.FindByValue(cs.ID.ToString()));
                    }
                    else
                    {
                        cs = c_SystemList.FirstOrDefault();
                    }
                    if (cs.Category == 2)
                    {
                        //Server.Transfer("~/BusinessReport/TargetProReported.aspx?SystemId=" + cs.ID);
                        Response.Redirect("~/BusinessReport/TargetProReported.aspx?SystemId=" + cs.ID);
                    }
                    else if (cs.Category == 3)
                    {
                        //Server.Transfer("~/BusinessReport/TargetGroupReported.aspx?SystemId=" + cs.ID);
                        Response.Redirect("~/BusinessReport/TargetGroupReported.aspx?SystemId=" + cs.ID);
                    }
                    else if (cs.Category == 4)
                    {
                        //Server.Transfer("~/BusinessReport/TargetDirectlyReported.aspx?SystemId=" + cs.ID);
                        Response.Redirect("~/BusinessReport/TargetDirectlyReported.aspx?SystemId=" + cs.ID);
                    }
                }

                lblName.Text = FinYear + "-" + FinMonth + "月度经营报告上报";
                //加载区域
                LoadAreaData();
                if (string.IsNullOrEmpty(Request.QueryString["BusinessID"])) //如果BusinessID是Null，代表不是从OA进来的
                {
                    HidSystemID.Value = ddlSystem.SelectedValue;
                    HidAreaID.Value = ddlAreaID.Visible ? ddlAreaID.SelectedValue : Guid.Empty.ToString();
                }
                else
                {
                    //从OA进来的
                    hideMonthReportID.Value = Request["BusinessID"];
                    var bmr = B_MonthlyreportOperator.Instance.GetMonthlyreport(Request["BusinessID"].ToGuid());
                    //ddlSystem.SelectedValue = bmr.SystemID.ToString();
                    ddlSystem.SelectedIndex = ddlSystem.Items.IndexOf(ddlSystem.Items.FindByValue(bmr.SystemID.ToString()));
                    HidSystemID.Value = ddlSystem.SelectedValue;

                    //加载区域
                    LoadAreaData();
                    //ddlAreaID.SelectedValue = bmr.AreaID.ToString();
                    ddlAreaID.SelectedIndex = ddlAreaID.Items.IndexOf(ddlAreaID.Items.FindByValue(bmr.AreaID.ToString()));
                    HidAreaID.Value = ddlAreaID.SelectedValue;

                    ddlSystem.Enabled = false;
                    ddlAreaID.Enabled = false;
                    //判断是否是是该类型下的板块
                    //ChangeSystem();
                }

                HideProcessCode.Value = StaticResource.Instance[ddlSystem.SelectedValue.ToGuid(), DateTime.Now].Configuration.Element("ProcessCode").Value;
                //AddMonthlyReport();//如果当前月不存在月度报告数据，添加一条数据
                AddMonthlyReport();
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
            InitAreaData();
        }
        /// <summary>
        /// 添加月度报告数据
        /// </summary>
        public void AddMonthlyReport()
        {
            HidSystemID.Value = ddlSystem.SelectedValue;
            HidAreaID.Value = ddlAreaID.Visible ? ddlAreaID.SelectedValue : Guid.Empty.ToString();

            hiddenDis.Value = "2";
            //如果存在审批中的数据，不让重复提交
            var bmrProgress = B_MonthlyreportOperator.Instance.GetMonthlyReportModel(Guid.Parse(ddlSystem.SelectedValue), Guid.Parse(HidAreaID.Value), FinYear, FinMonth, 1, "Progress");
            if (bmrProgress != null && bmrProgress.WFStatus == "Progress")
            {
                hiddenDis.Value = "1";
                hideMonthReportID.Value = bmrProgress.ID.ToString();
                return;
            }


            HideProcessCode.Value = StaticResource.Instance[ddlSystem.SelectedValue.ToGuid(), DateTime.Now].Configuration.Element("ProcessCode").Value;

            hiddenDis.Value = "0";

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
                        BatchModel = AddSystemBatch(Gtypt);
                        if (BatchModel == null)
                        {
                            UserControl.SetButtonSpanStyle(2);
                            return;
                        }
                    }
                    else
                    {
                        //批次不为Null，且批次还是在审批中的。
                        //批次是草稿状态
                        List<V_SubReport> V_SubReportList = JsonConvert.DeserializeObject<List<V_SubReport>>(BatchModel.SubReport);

                        //获取上报月报的ID
                        var subRpt = V_SubReportList.Find(SR => SR.SystemID == ddlAreaID.SelectedValue.ToGuid());
                        B_MonthlyReport monthRpt = B_MonthlyreportOperator.Instance.GetMonthlyreport(subRpt.ReportID);
                        #region  批次存在

                        //审批进行中,是要被跳转到审批页面的，但是审批完成就需要重新开启新的流程
                        if (BatchModel.WFBatchStatus == "Progress" || monthRpt.WFStatus == "Progress")
                        {
                            //批次是草稿状态
                            //var model = V_SubReportList.Find(p => p.SystemID == ddlAreaID.SelectedValue.ToGuid());

                            //---------------------------------------------------------------------------------------------------------------------------------------------
                            //这里批次需要控制，每次审批的状态只能是唯一的
                            // 暂时注销掉  BatchModel = AddSystemBatch();

                            if (BPF.Workflow.Client.WFClientSDK.Exist(monthRpt.ID.ToString()))
                            {
                                BPF.Workflow.Object.WorkflowContext wc = BPF.Workflow.Client.WFClientSDK.GetProcess(null, monthRpt.ID.ToString());
                                if (!wc.CurrentUserHasTodoTask)
                                {
                                    hiddenDis.Value = "1";
                                    hideMonthReportID.Value = monthRpt.ID.ToString();
                                    return;
                                    //Server.Transfer("~/BusinessReport/TargetApprove.aspx?BusinessID=" + host.BusinessID);
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
                            BatchModel = AddSystemBatch(Gtypt);
                        }
                        else
                        {
                            //批次是草稿状态

                            V_SubReportList.ForEach(p =>
                            {   //选择的是那个系统？
                                if (p.SystemID == Guid.Parse(ddlAreaID.SelectedValue))
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
                    UserControl.SetButtonSpanStyle(bmr == null ? -1 : bmr.Status);
                }
                else
                {
                    //如果是传过来的BusinessID，就直接去查询，不做操作
                    //通过BusinessID，首先获取批次的实体，根据权限，然后在批次中寻找
                    bmr = B_MonthlyreportOperator.Instance.GetMonthlyreport(Request["BusinessID"].ToGuid());
                    ExceptionHelper.TrueThrow<ArgumentNullException>(bmr == null ? true : false, "Argument B_MonthlyReport is Null");
                    HiddenBatch.Value = bmr.SystemBatchID.ToString();
                    hideMonthReportID.Value = bmr.ID.ToString();

                    if (bmr.SystemBatchID != Guid.Empty)
                    {
                        //BatchModel = B_SystemBatchOperator.Instance.GetSystemBatch(bmr.SystemBatchID);
                        //List<V_SubReport> rptLsit = JsonConvert.DeserializeObject<List<V_SubReport>>(BatchModel.SubReport);
                        //V_SubReport rptModel = rptLsit.Find(f => f.SystemID == ddlSystem.SelectedValue.ToGuid());
                        //hideMonthReportID.Value = rptModel.ReportID.ToString();
                        //bmr = B_MonthlyreportOperator.Instance.GetMonthlyreport(rptModel.ReportID);
                        //ExceptionHelper.TrueThrow<ArgumentNullException>(bmr == null ? true : false, "Argument B_MonthlyReport is Null");
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
                    // bmr = B_MonthlyreportOperator.Instance.GetMonthlyReportDraft(Guid.Parse(ddlSystem.SelectedValue), FinYear, FinMonth);
                    bmr = B_MonthlyreportOperator.Instance.GetMonthlyReportDraft(Guid.Parse(ddlSystem.SelectedValue), Guid.Parse(HidAreaID.Value), FinYear, FinMonth);
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
                }
                else
                {
                    hideMonthReportID.Value = Request["BusinessID"];
                    bmr = B_MonthlyreportOperator.Instance.GetMonthlyreport(Request["BusinessID"].ToGuid());
                    ddlSystem.SelectedValue = bmr.SystemID.ToString();
                    ddlAreaID.SelectedValue = bmr.AreaID.ToString();
                    FinYear = bmr.FinYear;
                    FinMonth = bmr.FinMonth;
                    lblName.Text = "";
                    lblName.Text = FinYear + "-" + FinMonth + "月度经营报告上报";
                }

                if (bmr != null)
                {
                    hideMonthReportID.Value = bmr.ID.ToString();
                    MutipleUpload.LoadByBusinessID(bmr.ID.ToString());
                    UserControl.SetButtonSpanStyle(bmr.Status);
                }
                else
                {
                    hiddenDis.Value = "2";
                    UserControl.SetButtonSpanStyle(-1);
                }

            }
        }
        /// <summary>
        /// 添加批次Model
        /// </summary>
        /// <returns></returns>
        public B_SystemBatch AddSystemBatch(string groupType)
        {
            B_SystemBatch BatchModel = new B_SystemBatch();
            BatchModel.BatchType = groupType;
            BatchModel.FinMonth = FinMonth;
            BatchModel.FinYear = FinYear;
            BatchModel.WFBatchStatus = "Draft";
            BatchModel.ID = Guid.NewGuid();
            var defaultTarget = A_TargetplanOperator.Instance.GetDefaultTargetplanList(ddlSystem.SelectedValue.ToGuid(), DateTime.Now.Year);
            if (defaultTarget == null || defaultTarget.Count < 1)
                return null;
            BatchModel.TargetPlanID = defaultTarget.FirstOrDefault().ID;
            List<V_SubReport> SubReportList = new List<V_SubReport>();
            //获取项目系统的公司
            //List<C_System> ProSysList = StaticResource.Instance.SystemList.Where(p => p.GroupType == groupType).OrderBy(PR => PR.Sequence).ToList();
            var orgList = StaticResource.Instance.OrgList[ddlSystem.SelectedValue.ToGuid()].Where(x => x.Level == 3);
            //循环添加B_MonthlyReport表数据
            foreach (var item in orgList)
            {
                B_MonthlyReport BatchMonthlyReport = new B_MonthlyReport();
                BatchMonthlyReport.SystemID = ddlSystem.SelectedValue.ToGuid();
                BatchMonthlyReport.AreaID = item.ID;
                BatchMonthlyReport.FinMonth = FinMonth;
                BatchMonthlyReport.FinYear = FinYear;
                BatchMonthlyReport.Status = 2;
                BatchMonthlyReport.DefaultVersionStatus = 1;
                BatchMonthlyReport.WFStatus = "Draft";
                BatchMonthlyReport.SystemBatchID = BatchModel.ID;
                BatchMonthlyReport.CreateTime = DateTime.Now;
                BatchMonthlyReport.DefaultVersionStatus = 1;
                BatchMonthlyReport.TargetPlanID = BatchModel.TargetPlanID;

                BatchMonthlyReport.ID = B_MonthlyreportOperator.Instance.AddMonthlyreport(BatchMonthlyReport);

                //添加数据 ,如果是当前选择的系统，isreaday==true
                if (ddlAreaID.SelectedValue == item.ID.ToString())
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

        public B_MonthlyReport AddMR()
        {
            B_MonthlyReport bmr = new B_MonthlyReport();
            bmr.SystemID = Guid.Parse(ddlSystem.SelectedValue);
            bmr.AreaID = Guid.Parse(ddlAreaID.Visible ? ddlAreaID.SelectedValue : Guid.Empty.ToString());
            bmr.FinMonth = FinMonth;
            bmr.FinYear = FinYear;
            bmr.Status = 2;
            bmr.WFStatus = "Draft";
            bmr.DefaultVersionStatus = 1;
            bmr.CreateTime = DateTime.Now;
            var targetPlanDetail = StaticResource.Instance.GetDefaultTargetPlanList(bmr.SystemID, bmr.FinYear).FirstOrDefault();
            if (targetPlanDetail != null && targetPlanDetail.TargetPlanID != null)
                bmr.TargetPlanID = targetPlanDetail.TargetPlanID;
            else
                return null;
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

            var targetPlan = StaticResource.Instance.GetDefaultTargetPlanList(Guid.Parse(ddlSystem.SelectedValue), FinYear).FirstOrDefault();
            if (targetPlan == null || targetPlan.TargetPlanID == null || targetPlan.TargetPlanID == Guid.Empty)
                return;
            var targetPlanId = targetPlan.TargetPlanID;

            bool IsExistence = B_MonthlyreportdetailOperator.Instance.GetMonthlyReportDetailCount(bmr.ID);

            List<B_MonthlyReportDetail> B_ReportDetails = new List<B_MonthlyReportDetail>();

            //listBMRD.Count == 0
            Guid AreaID = Guid.Parse(ddlAreaID.Visible ? ddlAreaID.SelectedValue : Guid.Empty.ToString());
            if (IsExistence == false) // 明细里不存在数据
            {
                //A_MonthlyReport AMonthlyReport = A_MonthlyreportOperator.Instance.GetAMonthlyReport(Guid.Parse(ddlSystem.SelectedValue), FinYear, FinMonth);
                A_MonthlyReport AMonthlyReport = A_MonthlyreportOperator.Instance.GetAMonthlyReport(Guid.Parse(ddlSystem.SelectedValue), AreaID, FinYear, FinMonth, targetPlanId);
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
                    //B_MonthlyReport BMonthlyReport = B_MonthlyreportOperator.Instance.GetMonthlyReport(Guid.Parse(ddlSystem.SelectedValue), FinYear, FinMonth, bmr.ID);
                    B_MonthlyReport BMonthlyReport = B_MonthlyreportOperator.Instance.GetMonthlyReport(Guid.Parse(ddlSystem.SelectedValue), AreaID, FinYear, FinMonth, bmr.ID, targetPlanId);
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


        /// <summary>
        /// 更改区域
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlAreaID_TextChanged(object sender, EventArgs e)
        {
            AddMonthlyReport();
        }

        /// <summary>
        /// 加载区域并执行AddMonthlyReport
        /// </summary>
        private void InitAreaData()
        {
            LoadAreaData();
            AddMonthlyReport();
        }


        /// <summary>
        /// 加载区域
        /// </summary>
        private void LoadAreaData()
        {
            if (ddlSystem.SelectedValue != Guid.Empty.ToString())
            {
                var list = S_OrganizationalActionOperator.Instance.GetUserRegional(ddlSystem.SelectedValue.ToGuid(), WebHelper.GetCurrentLoginUser());
                //list.Add(new Model.BizModel.S_Organizational {
                //    ID = Guid.Parse("D8483CEA-1C7C-4C22-9969-BD7051D79E86"),
                //    CnName="东北"
                //});
                //list.Add(new Model.BizModel.S_Organizational
                //{
                //    ID = Guid.Parse("228C5228-3495-444C-AC82-B0E08716D0B0"),
                //    CnName = "西南"
                //});
                if (list != null && list.Count > 0)
                {
                    ddlAreaID.Visible = true;
                    ddlAreaID.DataSource = list;
                    ddlAreaID.DataTextField = "CnName";
                    ddlAreaID.DataValueField = "ID";
                    ddlAreaID.DataBind();

                }
                else
                {
                    ddlAreaID.Visible = false;
                }
            }
            else
            {
                ddlAreaID.Visible = false;
            }
        }

        /// <summary>
        /// 根据更改的板块加载不通页面
        /// </summary>
        private void ChangeSystem()
        {
            C_System cs = C_SystemOperator.Instance.GetSystem(Guid.Parse(ddlSystem.SelectedValue));

            if (cs.Category == 1)
            {
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
            else if (cs.Category == 4)
            {
                //Server.Transfer("~/BusinessReport/TargetDirectlyReported.aspx?SystemId=" + cs.ID);
                Response.Redirect("~/BusinessReport/TargetDirectlyReported.aspx?SystemId=" + cs.ID);
            }
        }
    }
}