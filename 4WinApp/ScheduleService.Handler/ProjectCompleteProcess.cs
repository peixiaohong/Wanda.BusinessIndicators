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
    public class ProjectCompleteProcess : Quartz.IJob
    {
        public string CurrentUser
        {
            //get { return "虚拟"; }
            get { return System.Configuration.ConfigurationManager.AppSettings["virtualUser"]; }
        }

        private string userName = System.Configuration.ConfigurationManager.AppSettings["virtualUser"];
        public void Execute(Quartz.IJobExecutionContext context)
        {
            var reportTime = StaticResource.Instance.GetReportDateTime();
            Common.ScheduleService.Log.Instance.Info("提交虚拟暂挂节点处理开始！");
            bool IsChildrenSubmit = false;
            int month = reportTime.Month;
            List<B_SystemBatch> listBSB = B_SystemBatchOperator.Instance.GetSystemBatchList(month).ToList();

            foreach (B_SystemBatch Bsystembatch in listBSB)
            {
                try
                {
                    if (BPF.Workflow.Client.WFClientSDK.Exist(Bsystembatch.ID.ToString()))
                    {
                        WorkflowContext SummaryProcessWorkflow = WFClientSDK.GetProcess(null, Bsystembatch.ID.ToString(), new UserInfo() { UserCode = "$VirtualUserCode$" + userName });
                        if (SummaryProcessWorkflow.ProcessInstance.Status == 3)
                        {
                            List<V_SubReport> listVs = JsonConvert.DeserializeObject<List<V_SubReport>>(Bsystembatch.SubReport);
                            foreach (V_SubReport vs in listVs)
                            {
                                if (BPF.Workflow.Client.WFClientSDK.Exist(vs.ReportID.ToString()))
                                {
                                    WorkflowContext ChildrenWorkflow = WFClientSDK.GetProcess(null, vs.ReportID.ToString(), new UserInfo() { UserCode = "$VirtualUserCode$" + userName });

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
                                            Common.ScheduleService.Log.Instance.Info(Bsystembatch.BatchType + "版块分支流程提交失败，原因：" + wfc.StatusMessage + "！ID=" + vs.ReportID.ToString());
                                        }
                                        else
                                        {
                                            Update(vs);
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
                    Common.ScheduleService.Log.Instance.Error(Bsystembatch.BatchType + "版块子流程,流程提交失败！" + ex.ToString());
                }
            }
            Common.ScheduleService.Log.Instance.Info("提交虚拟暂挂节点处理结束！");
        }

        public void Update(V_SubReport p)
        {
            List<NavigatActivity1> lstna = GetProcessIntance(p.ReportID.ToString(), new UserInfo { UserCode = "$VirtualUserCode$" + CurrentUser });
            string Json = Newtonsoft.Json.JsonConvert.SerializeObject(lstna);
            B_MonthlyreportOperator.Instance.UpdateReportApprove(p.ReportID,Json); //??

        }
        public List<NavigatActivity1> GetProcessIntance(string BusinessID, UserInfo UserLonginID)
        {

            var p = BPF.Workflow.Client.WFClientSDK.GetProcess(null, BusinessID, UserLonginID);
            List<NavigatActivity1> listna = new List<NavigatActivity1>();
            NavigatActivity1 na1 = null;
            Dictionary<string, BPF.Workflow.Object.Node> list = new Dictionary<string, Node>();
            string strNextNodeID = p.ProcessInstance.StartNodeID;
            foreach (var p1 in p.NodeInstanceList)
            {
                if (!string.IsNullOrEmpty(strNextNodeID))
                {
                    var CurrentNode = p.NodeInstanceList[strNextNodeID];
                    list.Add(strNextNodeID, CurrentNode);
                    strNextNodeID = CurrentNode.NextNodeID;
                }
            }
            foreach (var p1 in list)
            {
                if (string.IsNullOrEmpty(p1.Value.ParentNodeID))
                {
                    na1 = new NavigatActivity1();
                    na1.ActivityID = p1.Value.NodeID;
                    na1.ActivityName = p1.Value.NodeName;
                    na1.ActivityType = p1.Value.NodeType;
                    na1.RunningStatus = (p1.Value.Status > 1 ? 3 : p1.Value.Status);
                    List<ClientOpinion1> listclientOp = new List<ClientOpinion1>();
                    listclientOp.Add(new ClientOpinion1()
                    {
                        CreateDate = p1.Value.FinishDateTime
                    });
                    na1.Opinions = listclientOp;

                    List<NavigatCandidate1> listnc = new List<NavigatCandidate1>();

                    var lstNode = p.NodeInstanceList.Where(s => s.Value.ParentNodeID == p1.Value.NodeID);
                    if (lstNode.Count() > 0)
                    {
                        foreach (var itme in lstNode)
                        {
                            listnc.Add(new NavigatCandidate1()
                            {
                                Id = itme.Value.User.UserID,
                                Name = itme.Value.User.UserName,
                                Title = itme.Value.User.UserJobName,
                                DeptName = itme.Value.User.UserOrgPathName,
                                Completed = itme.Value.Status == 2 ? true : false
                            });
                        }
                    }
                    else
                    {

                        listnc.Add(new NavigatCandidate1()
                        {
                            Id = p1.Value.User.UserID,
                            Name = p1.Value.User.UserName,
                            Title = p1.Value.User.UserJobName,
                            DeptName = p1.Value.User.UserOrgPathName,
                            Completed = p1.Value.Status == 2 ? true : false
                        });
                    }
                    na1.Candidates = listnc;
                    listna.Add(na1);
                }

            }
            return listna;
        }

        public void debugg(string BusinessID)
        {
            B_SystemBatch sysBatch = B_SystemBatchOperator.Instance.GetSystemBatch(new Guid(BusinessID));
            //获取批次
            List<V_SubReport> BatchRptList = JsonConvert.DeserializeObject<List<V_SubReport>>(sysBatch.SubReport);

            //批次更新
            BatchRptList.ForEach(p =>
            {
                Update(p);
            });
        }
    
    }
}
