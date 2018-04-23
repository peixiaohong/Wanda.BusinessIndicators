using Lib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.ViewModel;
using Newtonsoft.Json;
using BPF.Workflow.Object;
namespace LJTH.BusinessIndicators.Web.BusinessReport
{
    public partial class ProTargetApprove : System.Web.UI.Page//,IActivityHost
    {
        /// <summary>
        /// 批次Model
        /// </summary>
        B_SystemBatch _BatchModel = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (!IsPostBack)
            {
                //判断工作流业务ID
                if (!string.IsNullOrEmpty(Request.QueryString["BusinessID"]))
                {
                    //判断是正常的月报类型还是批次类型
                    if (string.IsNullOrEmpty(Request.QueryString["ProType"]))
                        LoadData(true);
                    else
                        LoadBatchData(true);
                }
            }
        }



        /// <summary>
        /// 正常获取月报的审批
        /// </summary>
        /// <param name="isFirst"></param>
        private void LoadData(bool isFirst)
        {

            HiddBusinessID.Value = Request.QueryString["BusinessID"];
            HideProType.Value = Request.QueryString["ProType"];

            //审批页面，判断该月报ID，是否含有批次ID
            B_MonthlyReport bmonthReport = B_MonthlyreportOperator.Instance.GetMonthlyreport(Guid.Parse(Request.QueryString["BusinessID"]));
            B_SystemBatch bs = null;
            List<ProcessLog> lstBatchProcess = new List<ProcessLog>();
            if (bmonthReport.SystemBatchID != Guid.Empty)
            {
                bs = B_SystemBatchOperator.Instance.GetSystemBatch(bmonthReport.SystemBatchID);
                if (bs != null) 
                {
                    lstBatchProcess = BPF.Workflow.Client.WFClientSDK.GetProcessLogList(bs.ID.ToString()); //JsonConvert.DeserializeObject<List<ProcessLog>>(bs.Batch_Opinions);
                }
            }
            List<ProcessLog> lstProcess = new List<ProcessLog>();
            lstProcess = BPF.Workflow.Client.WFClientSDK.GetProcessLogList(bmonthReport.ID.ToString());


            if (lstBatchProcess != null)
            {
                lstProcess.AddRange(lstBatchProcess);
            }
            HideOpinions.Value = JsonConvert.SerializeObject(lstProcess.Where(p => p.NodeType != 7).OrderByDescending(p => p.FinishDateTime));
            List<C_System> sysList = null;

            if (isFirst)
            {
                if (bmonthReport != null)
                {
                    hideSystemID.Value = bmonthReport.SystemID.ToString();
                    HideProcessCode.Value = StaticResource.Instance[bmonthReport.SystemID,DateTime.Now].Configuration.Element("ProcessCode").Value;
                    //判断批次号
                    //if (bmonthReport.SystemBatchID != Guid.Empty)
                    //{
                        //通过权限，加载可以看到的系统
                        sysList = new List<C_System>();

                        if (PermissionList != null && PermissionList.Count > 0)
                        {
                            foreach (var item in PermissionList)
                            {
                                sysList.AddRange(StaticResource.Instance.SystemList.Where(p => p.SystemName == item.ToString()).ToList());
                            }
                        }

                        /* 暂时放开权限*/
                        if (sysList.Count > 0)
                        {
                            //Category ==1 代表的是经营系统
                            ddlSystem.DataSource = sysList.Distinct().OrderBy(or => or.Sequence).ToList();
                        }
                        else
                            ddlSystem.DataSource = StaticResource.Instance.SystemList.Where(p => p.Category == 2).ToList();
                        /* 暂时放开权限*/

                        ddlSystem.DataTextField = "SystemName";
                        ddlSystem.DataValueField = "ID";
                        ddlSystem.DataBind();

                    //}
                }

                ddlSystem.SelectedValue = bmonthReport.SystemID.ToString();
            }

            lblName.Text = bmonthReport.FinYear + "-" + bmonthReport.FinMonth + "月度经营报告";
            
            HiddenMonth.Value = bmonthReport.FinMonth.ToString();
            hideMonthReportID.Value = bmonthReport.ID.ToString();

            

            //获取批次实体
            //_BatchModel = B_SystemBatchOperator.Instance.GetSystemBatch(bmonthReport.SystemBatchID);


            //if (isFirst)
            //{
            //    if (_BatchModel != null)
            //    {
            //        List<V_SubReport> rptList = JsonSerializer.Deserialize<List<V_SubReport>>(_BatchModel.SubReport);
            //        if (sysList.Count == 1)
            //        {
            //            V_SubReport rptModel = rptList.Find(p => p.SystemID == sysList[0].ID);
            //            //在当前批次中，的项目系统
            //            hideMonthReportID.Value = rptModel.ReportID.ToString();
            //        }
            //        else
            //        {
            //            ddlSystem.DataSource = rptList;
            //            ddlSystem.DataTextField = "SystemName";
            //            ddlSystem.DataValueField = "SystemID";
            //            ddlSystem.DataBind();

            //            V_SubReport rptModel = rptList.Find(p => p.SystemID == ddlSystem.SelectedValue.ToGuid());
            //            //在当前批次中，的项目系统
            //            hideMonthReportID.Value = rptModel.ReportID.ToString();
            //        }
            //    }
            //}
            //else
            //{
            //    if (_BatchModel != null)
            //    {
            //        List<V_SubReport> rptList = JsonSerializer.Deserialize<List<V_SubReport>>(_BatchModel.SubReport);

            //        V_SubReport rptModel = rptList.Find(p => p.SystemID == ddlSystem.SelectedValue.ToGuid());
            //        //在当前批次中，的项目系统
            //        hideMonthReportID.Value = rptModel.ReportID.ToString();
            //    }
            //}

        }



        /// <summary>
        /// 通过批次获取月报的审批
        /// </summary>
        private void LoadBatchData(bool isFirst)
        {
            HiddBusinessID.Value = Request.QueryString["BusinessID"];
            HideProType.Value = Request.QueryString["ProType"];

            #region  获取子流程和主流程的审批日志
            _BatchModel = B_SystemBatchOperator.Instance.GetSystemBatch(Guid.Parse(Request.QueryString["BusinessID"]));
            List<ProcessLog> listALLProLog = new List<ProcessLog>();
            List<ProcessLog> listProLog = new List<ProcessLog>(); //JsonConvert.DeserializeObject<List<ProcessLog>>(_BatchModel.Opinions);
            List<V_SubReport> BatchRptList = JsonConvert.DeserializeObject<List<V_SubReport>>(_BatchModel.SubReport);
            for (int i = 0; i < BatchRptList.Count; i++)
            {
                if (BPF.Workflow.Client.WFClientSDK.Exist(BatchRptList[i].ReportID.ToString()))
                {
                    listProLog.AddRange(BPF.Workflow.Client.WFClientSDK.GetProcessLogList(BatchRptList[i].ReportID.ToString()));
                }
            }
            List<ProcessLog> listBatchProLog = null;
            if (BPF.Workflow.Client.WFClientSDK.Exist(_BatchModel.ID.ToString()))
            {
                listBatchProLog = BPF.Workflow.Client.WFClientSDK.GetProcessLogList(_BatchModel.ID.ToString()); //JsonConvert.DeserializeObject<List<ProcessLog>>(_BatchModel.Batch_Opinions);
            }
            if (listBatchProLog != null)
                listALLProLog.AddRange(listBatchProLog);
            if(listProLog!=null)
                listALLProLog.AddRange(listProLog);
            if (listALLProLog != null)
            {
                //这里去掉汇总审批时的虚拟节点同意日志
                HideOpinions.Value = JsonConvert.SerializeObject(listALLProLog.Where(p=>p.NodeType !=7).OrderByDescending(p => p.FinishDateTime));
            }
            #endregion

            List<C_System> sysList = null;

            if (isFirst)
            {
                //判断批次号
                if (_BatchModel != null)
                {
                    List<V_SubReport> rptList = JsonConvert.DeserializeObject<List<V_SubReport>>(_BatchModel.SubReport);
                    if (sysList != null && sysList.Count == 1)
                    {
                        V_SubReport rptModel = rptList.Find(p => p.SystemID == sysList[0].ID);
                        //在当前批次中，的项目系统
                        hideMonthReportID.Value = rptModel.ReportID.ToString();
                    }
                    else
                    {
                        ddlSystem.DataSource = rptList;
                        ddlSystem.DataTextField = "SystemName";
                        ddlSystem.DataValueField = "SystemID";
                        ddlSystem.DataBind();

                        V_SubReport rptModel = rptList.Find(p => p.SystemID == ddlSystem.SelectedValue.ToGuid());

                        lblName.Text = _BatchModel.FinYear + "-" + _BatchModel.FinMonth + "月度经营报告";

                        HiddenMonth.Value = _BatchModel.FinMonth.ToString();

                        //在当前批次中，的项目系统
                        hideMonthReportID.Value = rptModel.ReportID.ToString();
                        hideSystemID.Value = ddlSystem.SelectedValue;
                        HideProcessCode.Value = StaticResource.Instance[ddlSystem.SelectedValue.ToGuid(),DateTime.Now].Configuration.Element("ProcessCode").Value;
                    }
                }
            }
            else
            { 
                  //判断批次号
                if (_BatchModel != null)
                {
                    List<V_SubReport> rptList =JsonConvert.DeserializeObject<List<V_SubReport>>(_BatchModel.SubReport);

                    V_SubReport rptModel = rptList.Find(p => p.SystemID == ddlSystem.SelectedValue.ToGuid());
                    //在当前批次中，的项目系统
                    hideMonthReportID.Value = rptModel.ReportID.ToString();
                    hideSystemID.Value = ddlSystem.SelectedValue;
                    HideProcessCode.Value = StaticResource.Instance[ddlSystem.SelectedValue.ToGuid(),DateTime.Now].Configuration.Element("ProcessCode").Value;
                    lblName.Text = _BatchModel.FinYear + "-" + _BatchModel.FinMonth + "月度经营报告";
                    HiddenMonth.Value = _BatchModel.FinMonth.ToString();
                }
            }


        }





        #region 获取权限List

        //权限list
        List<string> PermissionList = null;

        protected override void OnInit(EventArgs e)
        {
            PermissionHelper.GetPermission();
            if (PermissionHelper.EnablePermission)
            {
                PermissionList = PermissionHelper.GetStartProcessList();

                ExceptionHelper.TrueThrow<ArgumentNullException>(PermissionList == null, "Argument GetStartProcessList is Empty");
            }
        }

        #endregion

        protected void ddlSystem_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["BusinessID"]))
            {
                if (string.IsNullOrEmpty(Request.QueryString["ProType"]))
                    LoadData(false);
                else
                    LoadBatchData(false);
            }
        }

    }
}