using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.ViewModel;
using Newtonsoft.Json;
using BPF.Workflow.Client;
using BPF.Workflow.Object;

namespace ScheduleService.Handler
{
    [Quartz.DisallowConcurrentExecution]
    [Quartz.PersistJobDataAfterExecution]
    public class ProjectCompleteProcess: Quartz.IJob
    {
        public string CurrentUser
        {
            //get { return "虚拟"; }
            get { return System.Configuration.ConfigurationManager.AppSettings["virtualUser"]; }
        }

        private string userName = System.Configuration.ConfigurationManager.AppSettings["virtualUser"];
        public void Execute(Quartz.IJobExecutionContext context)
        {
            Common.ScheduleService.Log.Instance.Info("提交虚拟暂挂节点处理开始！");
            bool IsChildrenSubmit=false;
            int month = DateTime.Now.Month - 2;
            List<B_SystemBatch> listBSB= B_SystemBatchOperator.Instance.GetSystemBatchList(month).ToList();

            foreach (B_SystemBatch Bsystembatch in listBSB)
            {
                try
                {
                    if (BPF.Workflow.Client.WFClientSDK.Exist(Bsystembatch.ID.ToString()))
                    {
                        WorkflowContext SummaryProcessWorkflow = WFClientSDK.GetProcess(null, Bsystembatch.ID.ToString(), new UserInfo() { UserCode = "$VirtualUserCode$" + userName });
                        //WorkflowContext SummaryProcessWorkflow = WFClientSDK.GetProcess(null, Bsystembatch.ID.ToString(), new UserInfo() { UserLoginID = "fanbing" });
                        if (SummaryProcessWorkflow.ProcessInstance.Status == 3)
                        {
                            List<V_SubReport> listVs = JsonConvert.DeserializeObject<List<V_SubReport>>(Bsystembatch.SubReport);
                            foreach (V_SubReport vs in listVs)
                            {
                                if (BPF.Workflow.Client.WFClientSDK.Exist(vs.ReportID.ToString()))
                                {
                                    WorkflowContext ChildrenWorkflow = WFClientSDK.GetProcess(null, vs.ReportID.ToString(), new UserInfo() { UserCode = "$VirtualUserCode$" + userName });
                                    // WorkflowContext ChildrenWorkflow = WFClientSDK.GetProcess(null, vs.ReportID.ToString(), new UserInfo() { UserLoginID = "fanbing" });
                                    if (ChildrenWorkflow.ProcessInstance.Status != 3)
                                    {
                                        Common.ScheduleService.Log.Instance.Info("项目系统子流程,流程开始提交！ID=" + vs.ReportID.ToString());
                                        Dictionary<string, object> formParams = new Dictionary<string, object>();
                                        formParams.Add("ReportName", ChildrenWorkflow.ProcessInstance.ProcessTitle);
                                        formParams.Add("ProcessKey", ChildrenWorkflow.ProcessInstance.FlowCode);
                                        BizContext bizContext = new BizContext();
                                        bizContext.NodeInstanceList = ChildrenWorkflow.NodeInstanceList;
                                        bizContext.ProcessRunningNodeID = ChildrenWorkflow.ProcessInstance.RunningNodeID;
                                        bizContext.BusinessID = vs.ReportID.ToString();
                                        bizContext.FlowCode = ChildrenWorkflow.ProcessInstance.FlowCode;
                                        bizContext.ApprovalContent = "同意";
                                        bizContext.CurrentUser = new UserInfo() { UserCode = "$VirtualUserCode$" + CurrentUser };
                                        bizContext.ProcessURL = "/BusinessReport/TargetApprove.aspx";
                                        bizContext.FormParams = formParams;
                                        bizContext.ExtensionCommond = new Dictionary<string, string>();
                                        bizContext.ExtensionCommond.Add("RejectNode", Guid.Empty.ToString());
                                        WorkflowContext wfc = WFClientSDK.ExecuteMethod("SubmitProcess", bizContext);
                                        if (wfc.StatusCode != 0)
                                        {
                                            //throw wfc.LastException;
                                            Common.ScheduleService.Log.Instance.Info(Bsystembatch.BatchType+"版块分支流程提交失败，原因："+wfc.StatusMessage+"！ID=" + vs.ReportID.ToString());
                                        }
                                        else
                                        {
                                            Common.ScheduleService.Log.Instance.Info(Bsystembatch.BatchType + "版块分支流程提交成功！ID=" + vs.ReportID.ToString());
                                        }
                                    }
                                    else
                                    {
                                        IsChildrenSubmit = true;
                                    }
                                }
                            }
                        }
                        if (IsChildrenSubmit)
                        {
                            Common.ScheduleService.Log.Instance.Info(Bsystembatch.BatchType + "版块子流程,没有子流程提交！");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Common.ScheduleService.Log.Instance.Error(Bsystembatch.BatchType+ "版块子流程,流程提交失败！" + ex.ToString());
                }
            }
            Common.ScheduleService.Log.Instance.Info("提交虚拟暂挂节点处理结束！");
        }
    }
}
