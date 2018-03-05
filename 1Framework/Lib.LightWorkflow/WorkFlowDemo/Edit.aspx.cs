using Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Wanda.LightWorkflow.Dal;
using Wanda.LightWorkflow.Entities;
using Wanda.LightWorkflow;
using System.Collections;
using Wanda.LightWorkflow.Interface;
namespace WorkFlowDemo
{
    public partial class Edit : System.Web.UI.Page
    {
        private const string PROCESSCODE = "BP01";
        private const string PROCESTYPE = "1";
        private string bizPid = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            //打开即创建BizProcess
            if (string.IsNullOrEmpty(Request.QueryString["bid"]))
            {
                NewBizProcess(out bizPid);
            }
            else
            {
                bizPid = Request.QueryString["bid"];
                //TODO：这里需要处理业务数据的加载。
            }
        }
        //加载Process
        private Process LoadProcess(string processCode)
        {
            return ProcessAdapter.Instance.LoadByCode(processCode);
        }
        //创建BizProcess
        private int NewBizProcess(out string id)
        {
            id = Guid.NewGuid().ToString();
            BizProcess bp = new BizProcess()
            {
                AlarmAction = 1,
                AlarmPlanSplitDate = DateTime.Now.AddHours(40),
                CreateTime = DateTime.Now,
                CreatorName = userN.InnerText,
                IsCurrentVersion = true,
                IsDeleted = false,
                ID = id,
                ParentID = 0,
                ProcessType = 1,
                Status = 1,//状态:1未上报；2运行中；4审批完成，归档中；8被退回；流程结束；16流程已取消；32已归档；
                TaskID = 0,
                Title = string.Format("{0}_{1}", scope.Items[scope.SelectedIndex].Text, year.Items[year.SelectedIndex].Text)
            };
            //BizProcessAdapter
            return new BizProcessAdapter().Insert(bp);
        }
        //更新BizProcess信息
        private int UpdateBizProceee(BizProcess bizProcess)
        {
            return new BizProcessAdapter().Update(bizProcess);
        }
        //构建NodeInstance
        private List<ProcessNodeInstance> CreateNodeInstance(string bizPid)
        {
            Process p = LoadProcess(PROCESSCODE);
            List<ProcessNode> nodeList = ProcessNodeAdapter.Instance.LoadList(p.ID, PROCESTYPE);
            List<ProcessNodeInstance> list = new List<ProcessNodeInstance>();
            string guid = string.Empty;
            int index = 1;
            for (int i = 0; i < nodeList.Count; i++)
            {
                guid = Guid.NewGuid().ToString();
                list.Add(new ProcessNodeInstance()
                {
                    ID = guid,
                    BizProcessID = bizPid,
                    CreatedTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    CreatorName = userN.InnerText,
                    cUserID = index.ToString(),
                    Description = string.Format("DESC_{0}", index),
                    Expression = "1==1",
                    IsDeleted = false,
                    IsHandSign = false,
                    LastUpdatedTime = DateTime.Now,
                    NodeID = p.ID,
                    NodeInstanceID = guid,
                    NodeName = nodeList[i].NodeName,
                    NodeSeq = nodeList[i].NodeSeq,
                    NodeType = nodeList[i].NodeType,
                    OperationType = nodeList[i].NodeSeq == 1 ? 1 : nodeList[i].NodeSeq == 3 ? 5 : 2,
                    PreviousNodeInstanceID = "",
                    ProcessID = p.ID,
                    ProcessInstanceID = "",
                    ProcessType = PROCESTYPE,
                    RoleID = nodeList[i].RoleID,
                    Status = nodeList[i].NodeSeq == 1 ? 2 : 4,
                    UserCode = index.ToString(),
                    UserID = index,
                    UserName = string.Format("N_{0}", index)
                });
                index++;
            }
            return list;
        }
        //上报
        protected void Apply_Click(object sender, EventArgs e)
        {
            string piid = Guid.NewGuid().ToString();
            ////string bizPid = "";
            string instanceName = string.Format("{0}_{1}", scope.Items[scope.SelectedIndex].Text, year.Items[year.SelectedIndex].Text);
            BizProcess bizPro = new BizProcessAdapter().GetModelByID(bizPid);
            bizPro.Title = bizPro.Title + "UDP";
            bizPro.Status = 2;//状态:1未上报；2运行中；4审批完成，归档中；8被退回；流程结束；16流程已取消；32已归档；
            UpdateBizProceee(bizPro);
            List<ProcessNodeInstance> list = CreateNodeInstance(bizPid);
            Hashtable ht = new Hashtable();
            WorkflowStartEventArgs startEvt = new WorkflowStartEventArgs(PROCESSCODE, bizPid, "", instanceName, 1, note.InnerText, list, ht);
            startEvt.SaveWorkflow(instanceName);
            //WorkflowInstance.Start(piid, PROCESSCODE, bizPid, string.Empty, instanceName, note.InnerText, list, ht);
        }
        //保存
        protected void SaveDraft_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(bizPid))
            {
                return;
            }
            
            BizProcess bizPro = new BizProcessAdapter().GetModelByID(bizPid);
            bizPro.Title = bizPro.Title + "UDP";
            bizPro.Status = 1;//状态:1未上报；2运行中；4审批完成，归档中；8被退回；流程结束；16流程已取消；32已归档；
            UpdateBizProceee(bizPro);


            //TODO：这里需要处理业务数据的保存。

            //string piid = Guid.NewGuid().ToString();
            //string bizPid = "";
            ////string instanceName = string.Format("{0}_{1}", scope.Items[scope.SelectedIndex].Text, year.Items[year.SelectedIndex].Text);
            //List<ProcessNodeInstance> list = CreateNodeInstance(bizPid);
            //Hashtable ht = new Hashtable();
            //WorkflowStartEventArgs startEvt = new WorkflowStartEventArgs(PROCESSCODE, bizPid, "", instanceName, 1, note.InnerText, list, ht);
            //startEvt.SaveWorkflow(instanceName);
        }
    }
}