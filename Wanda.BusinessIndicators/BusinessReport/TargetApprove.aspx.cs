using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BPF.Workflow.Object;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.ViewModel;
using Newtonsoft.Json;

namespace LJTH.BusinessIndicators.Web.BusinessReport
{
    public partial class TargetApprove : System.Web.UI.Page//,IActivityHost
    {
        /// <summary>
        /// 批次Model
        /// </summary>
        B_SystemBatch _BatchModel = null;

        protected void Page_Load(object sender, EventArgs e)
        {

            if (Request.QueryString["BusinessID"] != null)
            {
                if (string.IsNullOrEmpty(Request.QueryString["ProType"]))
                {
                    LoadData();
                }
                else
                    LoadBatchData();
            }
        }

        private void LoadData()
        {
            if (!string.IsNullOrEmpty(Request.QueryString["BusinessID"].ToString()))
            {
                hideMonthReportID.Value = Request.QueryString["BusinessID"].ToString();
                B_MonthlyReport bmonthReport = B_MonthlyreportOperator.Instance.GetMonthlyreport(Guid.Parse(hideMonthReportID.Value));
                if (bmonthReport != null)
                {
                    hideSystemID.Value = bmonthReport.SystemID.ToString();
                    hideFinYear.Value = bmonthReport.FinYear.ToString();
                    hideFinMonth.Value = bmonthReport.FinMonth.ToString();
                    hideProcessCode.Value = StaticResource.Instance[bmonthReport.SystemID, DateTime.Now].Configuration.Element("ProcessCode").Value;
                    C_System system = StaticResource.Instance.SystemList.Where(p => p.ID == bmonthReport.SystemID).FirstOrDefault();
                    if (system != null)
                    {
                        lblName.Text = system.SystemName + bmonthReport.FinYear + "-" + bmonthReport.FinMonth + lblName.Text;
                    }
                }
            }
        }
        /// <summary>
        /// 通过批次获取月报的审批
        /// </summary>
        private void LoadBatchData()
        {
            hideBusinessID.Value = Request.QueryString["BusinessID"];
            hideProType.Value = Request.QueryString["ProType"];

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
            if (listProLog != null)
                listALLProLog.AddRange(listProLog);
            if (listALLProLog != null)
            {
                //这里去掉汇总审批时的虚拟节点同意日志
                HideOpinions.Value = JsonConvert.SerializeObject(listALLProLog.Where(p => p.NodeType != 7).OrderByDescending(p => p.FinishDateTime));
            }
            #endregion

            C_System c_System = StaticResource.Instance.SystemList.Where(x => x.GroupType == _BatchModel.BatchType).FirstOrDefault();
            hideSystemID.Value = c_System.ID.ToString();
            hideProcessCode.Value = StaticResource.Instance[c_System.ID, DateTime.Now].Configuration.Element("ProcessCode").Value;
            lblName.Text = _BatchModel.FinYear + "-" + _BatchModel.FinMonth + "月度经营报告";
            hideFinMonth.Value = _BatchModel.FinMonth.ToString();
            hideProType.Value = Request.QueryString["ProType"];
            hideBatchID.Value = _BatchModel.ID.ToString() ;
            //hideMonthReportID.Value = _BatchModel.ID.ToString();
            hideMonthReportID.Value = Guid.Empty.ToString();
            hideFinYear.Value = _BatchModel.FinYear.ToString();
        }


    }
}