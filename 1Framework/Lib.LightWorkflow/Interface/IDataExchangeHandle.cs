using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wanda.Lib.LightWorkflow.Entities;
using System.Collections;
namespace Wanda.Lib.LightWorkflow.Interface
{
    /// <summary>
    /// 外部数据交换服务接口，所有的外部数据扩展服务必须实现此接口
    /// </summary>
    public interface IDataExchangeHandle
    {
        /// <summary>
        /// 流程启动后事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnProcessStarted(object sender, WorkflowEventArgs e);

        /// <summary>
        /// 流程重新上报后事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnProcessReStarted(object sender, WorkflowEventArgs e);

        /// <summary>
        /// 流程审批完成后事件(流程未结束,状态:归档中)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnProcessApprovalCompleted(object sender, WorkflowEventArgs e);

        /// <summary>
        /// 流程完成后事件(流程结束,已归档)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnProcessCompleted(object sender, WorkflowEventArgs e);

        /// <summary>
        /// 节点进入时事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnNodeInitiated(object sender, WorkflowEventArgs e);
        /// <summary>
        /// 节点完成后事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnNodeCompleted(object sender, WorkflowEventArgs e);

        /// <summary>
        /// 流程退回后事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnProcessRejected(object sender, WorkflowEventArgs e);
        /// <summary>
        /// 流程转发后事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnProcessForwarded(object sender, WorkflowEventArgs e);
        /// <summary>
        /// 流程代理委托后事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnProcessEntrusted(object sender, WorkflowEventArgs e);
        /// <summary>
        /// 流程取消后事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnProcessCanceled(object sender, WorkflowEventArgs e);
    }

    /// <summary>
    /// 工作流事件
    /// </summary>
    [Serializable]
    public class WorkflowEventArgs : System.EventArgs
    {
        WorkflowInstance _WorkflowInstance = null;
        public WorkflowEventArgs()
            : base()
        {
        }

        public WorkflowEventArgs(WorkflowInstance workflowInstance)
            : base()
        {
            _WorkflowInstance = workflowInstance;
        }

        public WorkflowInstance WorkflowInstance
        {
            get
            {
                return _WorkflowInstance;
            }
        }

    }

    /// <summary>
    /// 工作流启动事件
    /// </summary>
    [Serializable]
    public class WorkflowStartEventArgs : System.EventArgs
    {
        public WorkflowStartEventArgs(string processCode, int congID, int bizProcessId, int projectId, string instanceName, int userId, string approvalNote, List<ProcessNodeInstance> nodes, Hashtable ctxData)
        {
            this.ProcessCode = processCode;
            this.CongID = congID;
            this.BizProcessId = bizProcessId;
            this.InstanceName = instanceName;
            this.ProjectId = projectId;
            this.UserId = userId;
            this.ApprovalNote = approvalNote;
            this.NodeInstanceList = nodes;
            this.ContextData = ctxData;
        }

        //protected int ProjectId
        //{
        //    get;
        //    private set;
        //}

        //protected int BizProcessId
        //{
        //    get;
        //    private set;
        //}

        protected int ProjectId
        {
            get;
            private set;
        }

        protected int BizProcessId
        {
            get;
            private set;
        }
        protected string ApprovalNote
        {
            get;
            private set;
        }

        protected string InstanceName
        {
            get;
            private set;
        }

        protected string ProcessCode
        {
            get;
            private set;
        }
        protected int CongID
        {
            get;
            private set;
        }
        protected int UserId
        {
            get;
            private set;
        }

        protected List<ProcessNodeInstance> NodeInstanceList
        {
            get;
            set;
        }

        protected Hashtable ContextData
        {
            get;
            set;
        }

        /// <summary>
        /// 保存工作流
        /// </summary>
        /// <param name="instanceName">流程名称</param>
        public void SaveWorkflow(string instanceName)
        {
            if (!string.IsNullOrEmpty(instanceName))
            {
                this.InstanceName = instanceName;
            }

            WorkflowInstance wf = WorkflowEngine.WorkflowService.GetWorkflowInstance(this.BizProcessId);
            if (wf == null)
            {
                WorkflowEngine.WorkflowService.StartProcess(this.ProcessCode, this.CongID, this.BizProcessId, this.ProjectId, this.InstanceName, this.ApprovalNote, this.NodeInstanceList, this.ContextData);//TODO马光辉：最后一个参数传递bizprocesscontext
            }
            else
            {
                wf.ReStart(this.ApprovalNote, this.NodeInstanceList, this.ContextData);
            }
        }
    }

    /// <summary>
    /// 工作流事件代理方法
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void WorkflowStartEventHandler(object sender, WorkflowStartEventArgs e);
}
