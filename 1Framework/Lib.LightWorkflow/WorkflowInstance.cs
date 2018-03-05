using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using Wanda.Lib.LightWorkflow.Entities;
using Wanda.Lib.LightWorkflow.Dal;
using Wanda.Lib.LightWorkflow.Tools;
using Wanda.Lib.LightWorkflow.Interface;
using Wanda.Lib.LightWorkflow.Handle;
using System.Collections;
using System.Web.Script.Serialization;
using Lib.Data;
using Wanda.HR.Common.Web;

namespace Wanda.Lib.LightWorkflow
{
    /// <summary>
    /// 工作流实例
    /// </summary>
    [Serializable]
    public class WorkflowInstance
    {
        public WorkflowInstance()
        {
        }

        public WorkflowInstance(ProcessInstance instance)
        {
            ProcessInstance = instance;
            this._process = ProcessAdapter.Instance.Load(ProcessInstance.ProcessID);
        }

        public WorkflowInstance(int instanceID)
        {
            ProcessInstance = ProcessInstanceAdapter.Instance.Load(instanceID);
            this._process = ProcessAdapter.Instance.Load(ProcessInstance.ProcessID);
        }

        public string ProcessCode
        {
            get { return ProcessInstance.ProcessCode; }
        }

        private Hashtable _BizProcessContext = null;
        public Hashtable BizProcessContext
        {
            get
            {
                if (this.ProcessInstance.BizProcessContext == "")
                {
                    _BizProcessContext = null;
                }
                else if (_BizProcessContext == null)
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    _BizProcessContext = js.Deserialize<Hashtable>(this.ProcessInstance.BizProcessContext);
                }
                return _BizProcessContext;
            }
        }

        /// <summary>
        /// 流程实例
        /// </summary>
        private ProcessInstance _Instance = null;
        public ProcessInstance ProcessInstance
        {
            get
            {
                return _Instance;
            }
            set
            {
                _Instance = value;
            }
        }


        /// <summary>
        /// 流程实例
        /// </summary>
        private Process _process = null;
        public Process Process
        {
            get
            {
                return _process;
            }
        }

        #region 属性列表
        /// <summary>
        /// 待办列表
        /// </summary>
        private List<TodoWork> _todoWorks = null;
        public List<TodoWork> TodoWorks
        {
            get
            {
                if (_todoWorks == null)
                {
                    _todoWorks = TodoWorkAdapter.Instance.LoadListByProcessInstanceID(ProcessInstance.ID);
                }
                return _todoWorks;
            }
        }

        /// <summary>
        /// 审批流程节点列表
        /// </summary>
        private List<ProcessNodeInstance> _NodeInstances = null;
        public List<ProcessNodeInstance> NodeInstances
        {
            get
            {
                if (_NodeInstances == null)
                {
                    _NodeInstances = ProcessNodeInstanceAdapter.Instance.LoadList(ProcessInstance.ID);
                }
                return _NodeInstances;
            }
        }

        /// <summary>
        /// 流程干系人列表
        /// </summary>
        private List<StakeHolder> _stakeHolders = null;
        public List<StakeHolder> StakeHolders
        {
            get
            {
                if (_stakeHolders == null)
                {
                    _stakeHolders = StakeHolderAdapter.Instance.LoadList(ProcessInstance.ID);
                }
                return _stakeHolders;
            }
        }

        /// <summary>
        /// 审批日志列表（倒叙）
        /// </summary>
        private List<ApprovalLog> _approvalLogs = null;
        public List<ApprovalLog> ApprovalLogs
        {
            get
            {
                if (_approvalLogs == null)
                {
                    _approvalLogs = ApprovalLogAdapter.Instance.LoadList(ProcessInstance.ID);
                }
                return _approvalLogs;
            }
        }

        #endregion

        #region 获取操作权限
        /// <summary>
        /// 根据用户ID获取可以做的动作列表
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public List<CommonConsts.NodeOperationType> GetPermissionsList(int userID)
        {
            List<CommonConsts.NodeOperationType> result = new List<CommonConsts.NodeOperationType>();
            //如果用户不在干系人列表里面，说明没有权限
            StakeHolder currentHolder = StakeHolderAdapter.Instance.Load(this.ProcessInstance.ID, userID);
            if (currentHolder != null)
            {
                switch (this.ProcessInstance.Status)
                {
                    case (int)CommonConsts.ProcessInstanceStatus.Running:
                        CheckRunningPermissions(userID, result);
                        result.Add(CommonConsts.NodeOperationType.Forward);
                        break;

                    case (int)CommonConsts.ProcessInstanceStatus.Archiving:
                        CheckArchivingPermissions(userID, result);
                        result.Add(CommonConsts.NodeOperationType.Forward);
                        break;
                    case (int)CommonConsts.ProcessInstanceStatus.WidthDrawed:
                    case (int)CommonConsts.ProcessInstanceStatus.Rejected:
                        CheckRejectedPermissions(userID, result);
                        result.Add(CommonConsts.NodeOperationType.Forward);
                        break;

                    case (int)CommonConsts.ProcessInstanceStatus.Finished:
                        CheckFinishedPermissions(userID, result);
                        result.Add(CommonConsts.NodeOperationType.Forward);
                        break;

                    case (int)CommonConsts.ProcessInstanceStatus.Canceled:
                        break;
                    default:
                        throw new Exception("未知的流程状态");
                }
            }
            return result;
        }

        private void CheckRunningPermissions(int userID, List<CommonConsts.NodeOperationType> result)
        {
            TodoWork currentTodo = FindUserTodoWork(userID);
            //有待办
            if (currentTodo != null)
            {
                switch (currentTodo.NodeType)
                {
                    case (int)CommonConsts.NodeType.Approval:
                    case (int)CommonConsts.NodeType.Review:
                        result.Add(CommonConsts.NodeOperationType.Approved);
                        result.Add(CommonConsts.NodeOperationType.Reject);
                        result.Add(CommonConsts.NodeOperationType.Entrust);
                        break;

                    case (int)CommonConsts.NodeType.CC:
                        result.Add(CommonConsts.NodeOperationType.Comment);
                        break;

                    case (int)CommonConsts.NodeType.Forward:
                        result.Add(CommonConsts.NodeOperationType.Comment);
                        break;

                    case (int)CommonConsts.NodeType.Archive:
                    case (int)CommonConsts.NodeType.Entrust:
                    case (int)CommonConsts.NodeType.Launch:
                        throw new Exception("错误的待办状态");

                    default:
                        throw new Exception("未知的待办状态");
                }
            }
            //流程运行中，无待办并且当前用户ＩＤ为流程发起人ID，则添加撤回操作
            else
            {
                if (userID == this.ProcessInstance.UserID)
                {
                    result.Add(CommonConsts.NodeOperationType.WidthDraw);
                }
            }
        }

        private void CheckArchivingPermissions(int userID, List<CommonConsts.NodeOperationType> result)
        {
            TodoWork currentTodo = FindUserTodoWork(userID);
            //有待办
            if (currentTodo != null)
            {
                switch (currentTodo.NodeType)
                {
                    case (int)CommonConsts.NodeType.Archive:
                        result.Add(CommonConsts.NodeOperationType.Archive);
                        result.Add(CommonConsts.NodeOperationType.Entrust);
                        break;

                    case (int)CommonConsts.NodeType.CC:
                        result.Add(CommonConsts.NodeOperationType.Comment);
                        break;

                    case (int)CommonConsts.NodeType.Forward:
                        result.Add(CommonConsts.NodeOperationType.Comment);
                        break;

                    //20120407: 不再这样处理待办，所有被转发的人，收到的都是转发待办，只能做批注
                    //出现此种节点，应该是流程中曾经参与审批的人，又被转发后导致的，参考public void Forward()中的说明
                    case (int)CommonConsts.NodeType.Approval:
                    case (int)CommonConsts.NodeType.Review:
                    //    result.Add(Common.NodeOperationType.Approved);
                    //    break;
                    case (int)CommonConsts.NodeType.Entrust:
                    case (int)CommonConsts.NodeType.Launch:
                        throw new Exception("错误的待办状态");

                    default:
                        throw new Exception("未知的待办状态");
                }
            }
        }

        private void CheckRejectedPermissions(int userID, List<CommonConsts.NodeOperationType> result)
        {
            TodoWork currentTodo = FindUserTodoWork(userID);
            //有待办
            if (currentTodo != null)
            {
                switch (currentTodo.NodeType)
                {
                    case (int)CommonConsts.NodeType.Launch:
                        result.Add(CommonConsts.NodeOperationType.Entrust);
                        result.Add(CommonConsts.NodeOperationType.Launch);
                        result.Add(CommonConsts.NodeOperationType.Cancel);
                        break;
                    case (int)CommonConsts.NodeType.Forward:
                        result.Add(CommonConsts.NodeOperationType.Comment);
                        break;
                    case (int)CommonConsts.NodeType.CC:
                        result.Add(CommonConsts.NodeOperationType.Comment);
                        break;
                    case (int)CommonConsts.NodeType.Approval:
                    case (int)CommonConsts.NodeType.Review:
                    case (int)CommonConsts.NodeType.Entrust:
                    case (int)CommonConsts.NodeType.Archive:
                        throw new Exception("错误的待办状态");

                    default:
                        throw new Exception("未知的待办状态");
                }
            }
        }

        private void CheckFinishedPermissions(int userID, List<CommonConsts.NodeOperationType> result)
        {
            TodoWork currentTodo = FindUserTodoWork(userID);
            //有待办
            if (currentTodo != null)
            {
                switch (currentTodo.NodeType)
                {
                    case (int)CommonConsts.NodeType.CC:
                    case (int)CommonConsts.NodeType.Forward:
                        result.Add(CommonConsts.NodeOperationType.Comment);
                        break;

                    case (int)CommonConsts.NodeType.Approval:
                    case (int)CommonConsts.NodeType.Review:
                    case (int)CommonConsts.NodeType.Entrust:
                    case (int)CommonConsts.NodeType.Launch:
                    case (int)CommonConsts.NodeType.Archive:
                        throw new Exception("错误的待办状态");

                    default:
                        throw new Exception("未知的待办状态");
                }
            }
        }

        #endregion

        #region 流程动作

        /// <summary>
        /// 启动流程（创建流程实例）
        /// </summary>
        /// <param name="processID">流程定义ID</param>
        /// <param name="processCode">流程编码</param>
        /// <param name="bizProcessID">业务id</param>
        /// <param name="projectID">项目id</param>
        /// <param name="InstanceName">流程实例名称</param>
        /// <param name="approvalNote">提交意见</param>
        /// <param name="nodeInstanceList">节点列表</param>
        /// <param name="bizProcessContext">业务流程上下文</param>
        /// <returns></returns>
        public static WorkflowInstance Start(int processID, string processCode,
            int bizProcessID,
            int projectID, string instanceName,
            string approvalNote,
            List<ProcessNodeInstance> nodeInstanceList, Hashtable bizProcessContext)
        {
            WorkflowInstance workflowInstance = null;

            AddArchiveNode(nodeInstanceList);

            //创建流程实例
            ProcessInstance instance = new ProcessInstance();
            instance.ProcessID = processID;
            instance.InstanceName = instanceName;
            instance.Status = (int)CommonConsts.ProcessInstanceStatus.Running;
            instance.ProcessCode = processCode;
            instance.BizProcessID = bizProcessID;
            instance.ProjectID = projectID;
            ProcessNodeInstance node = nodeInstanceList[0];
            instance.cUserID = node.cUserID;
            instance.UserCode = node.UserCode;
            instance.UserID = node.UserID;
            instance.UserName = node.UserName;
            instance.LastUpdatedTime = WebHelper.DateTimeNow;
            instance.CreateTime = WebHelper.DateTimeNow;
            instance.CreatorName = WebHelper.GetCurrentUser().LoginName;
            instance.IsDeleted = false;
            instance.ModifyTime = WebHelper.DateTimeNow;
            instance.ModifierName = WebHelper.GetCurrentUser().LoginName;
            //add czq
            instance.StartTime = WebHelper.DateTimeNow;
            //instance.ID = Guid.NewGuid().ToString();
            if (bizProcessContext == null)
            {
                instance.BizProcessContext = "";
            }
            else
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                instance.BizProcessContext = js.Serialize(bizProcessContext);
            }

            try
            {
                using (TransactionScope trans = TransactionScopeFactory.Create())
                {

                    //添加流程实例
                    instance.ID = ProcessInstanceAdapter.Instance.Insert(instance);

                    //添加当前流程审批人员列表
                    AddNodeInstanceList(instanceName, approvalNote, nodeInstanceList, instance);
                    //调用事件委托方法
                    workflowInstance = new WorkflowInstance(instance);

                    //提交事务
                    trans.Complete();
                }
                DataExchangeEngine.DataExchangeService.RaiseProcessStarted(workflowInstance, new WorkflowEventArgs(workflowInstance));
                DataExchangeEngine.DataExchangeService.RaiseNodeInitiated(workflowInstance, new WorkflowEventArgs(workflowInstance));
            }
            catch
            {
                throw;
            }

            return workflowInstance;
        }

        /// <summary>
        /// 添加当前流程审批人员列表
        /// </summary>
        /// <param name="instanceName"></param>
        /// <param name="approvalNote"></param>
        /// <param name="nodeInstanceList"></param>
        /// <param name="instance"></param>
        private static void AddNodeInstanceList(string instanceName, string approvalNote, List<ProcessNodeInstance> nodeInstanceList, ProcessInstance instance)
        {

            //int previousNIID = 0;
            int previousNIID = 0;
            for (int i = 0; i < nodeInstanceList.Count; i++)
            {
                ProcessNodeInstance nodeInstance = nodeInstanceList[i];
                nodeInstance.PreviousNodeInstanceID = previousNIID;
                nodeInstance.ProcessInstanceID = instance.ID;
                nodeInstance.CreateTime = WebHelper.DateTimeNow;
                nodeInstance.CreatorName = WebHelper.GetCurrentUser().LoginName;
                nodeInstance.IsDeleted = false;
                nodeInstance.ModifyTime = WebHelper.DateTimeNow;
                nodeInstance.ModifierName = WebHelper.GetCurrentUser().LoginName;
                if (i == 0)
                {
                    if (nodeInstance.NodeSeq >= 1000)
                    {
                        throw new Exception("非法流程：无发起人。");
                    }
                    //标记当前人员为已完成
                    nodeInstance.Status = (int)CommonConsts.NodeStatus.Executed;
                    nodeInstance.OperationType = (int)CommonConsts.NodeOperationType.Launch;
                    nodeInstance.LastUpdatedTime = WebHelper.DateTimeNow; ;
                }
                else if (i == 1)
                {
                    //标记下个节点的状态为执行中
                    if (nodeInstance.NodeSeq >= 1000)
                    {
                        throw new Exception("非法流程：无审批人。");
                    }
                    nodeInstance.Status = (int)CommonConsts.NodeStatus.Executing;

                }
                //添加流程人员实例
                nodeInstance.ID = ProcessNodeInstanceAdapter.Instance.Insert(nodeInstance);
                if (i == 0)
                {
                    //生成并添加已办日志
                    ApprovalLog log = WorkflowEngine.BuildApprovalLog(nodeInstance, instanceName, approvalNote);
                    log.ID = ApprovalLogAdapter.Instance.Insert(log);
                    //添加StakeHolder人员
                    StakeHolder holder = WorkflowEngine.BuildStakeHolder(nodeInstance);
                    try
                    {
                        holder.ID = StakeHolderAdapter.Instance.Insert(holder);
                    }
                    catch
                    {
                        //如果插入重复的，说明以前已经有了，唯一索引导致不能再次插入，可以忽略这个错误
                        //这里出现错误，有可能是发起人和第一个审批人是一个人导致的
                    }
                }
                else if (i == 1)
                {
                    //计算下个节点，生成待办
                    TodoWork todo = WorkflowEngine.BuildTodo(CommonConsts.TodoType.TODO, instance, nodeInstance);
                    WorkflowEngine.WorkflowService.CreateWorkflowTodo(todo);
                    //添加StakeHolder人员
                    StakeHolder holder = WorkflowEngine.BuildStakeHolder(nodeInstance);
                    try
                    {
                        holder.ID = StakeHolderAdapter.Instance.Insert(holder);
                    }
                    catch
                    {
                        //如果插入重复的，说明以前已经有了，唯一索引导致不能再次插入，可以忽略这个错误
                        //这里出现错误，有可能是发起人和第一个审批人是一个人导致的
                    }

                }
                previousNIID = nodeInstance.ID;
            }
        }

        /// <summary>
        /// 重新提交
        /// </summary>
        /// <param name="approvalNote">提交意见</param>
        /// <param name="nodeInstanceList">节点列表</param>
        /// <param name="bizProcessContext">业务流程上下文</param>
        public void ReStart(string approvalNote, List<ProcessNodeInstance> nodeInstanceList, Hashtable bizProcessContext)
        {
            ProcessNodeInstance node = nodeInstanceList[0];
            TodoWork todo = GetUserTodoWork(node.UserID);
            //获取当期节点实例
            ProcessNodeInstance currentNodeins = ProcessNodeInstanceAdapter.Instance.Load(todo.NodeInstanceID);
            if (currentNodeins.NodeSeq != CommonConsts.LaunchNodeSeqNumber)
            {
                throw new Exception("错误的重新上报动作。");
            }

            //添加归档节点
            AddArchiveNode(nodeInstanceList);

            try
            {
                using (TransactionScope trans = TransactionScopeFactory.Create())
                {
                    //删除当前人员待办
                    WorkflowEngine.WorkflowService.CloseWorkflowTodo(todo);

                    //删除原来的节点实例
                    ProcessNodeInstanceAdapter.Instance.DeleteList(this.ProcessInstance.ID);

                    //添加当前流程审批人员列表
                    AddNodeInstanceList(this.ProcessInstance.InstanceName, approvalNote, nodeInstanceList, this.ProcessInstance);

                    //更新流程状态
                    this.ProcessInstance.Status = (int)CommonConsts.ProcessInstanceStatus.Running;
                    if (bizProcessContext == null)
                    {
                        this.ProcessInstance.BizProcessContext = "";
                    }
                    else
                    {
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        this.ProcessInstance.BizProcessContext = js.Serialize(bizProcessContext);
                    }
                    this._BizProcessContext = null;
                    this.ProcessInstance.LastUpdatedTime = WebHelper.DateTimeNow; ;
                    ProcessInstanceAdapter.Instance.Update(this.ProcessInstance);

                    //提交事务
                    trans.Complete();
                }
                //调用事件委托方法
                DataExchangeEngine.DataExchangeService.RaiseNodeCompleted(this, new WorkflowEventArgs(this));
                DataExchangeEngine.DataExchangeService.RaiseProcessReStarted(this, new WorkflowEventArgs(this));
                DataExchangeEngine.DataExchangeService.RaiseNodeInitiated(this, new WorkflowEventArgs(this));
            }
            catch
            {
                throw;
            }
        }

        private static void AddArchiveNode(List<ProcessNodeInstance> nodeInstanceList)
        {
            //添加虚拟的归档节点
            ProcessNodeInstance node = nodeInstanceList[0];
            ProcessNodeInstance archiveNode = ObjectHelper.Clone<ProcessNodeInstance>(node);
            archiveNode.NodeName = "经办人";
            archiveNode.NodeSeq = CommonConsts.ArchiveNodeSeqNumber;
            archiveNode.Description = "归档节点";
            archiveNode.NodeType = (int)CommonConsts.NodeType.Archive;
            archiveNode.Status = (int)CommonConsts.NodeStatus.NotExecute;
            nodeInstanceList.Add(archiveNode);

        }

        /// <summary>
        /// 流程提交（审批）
        /// </summary>
        /// <param name="approvalNote">审批意见</param>
        /// <param name="userID">用户ID</param>
        public void Submit(string approvalNote, int userID, int todoWorkID)
        {
            TodoWork todo = GetUserTodoWork(userID, todoWorkID);

            //转发的待办或通知待办提交后的处理
            if (todo.NodeInstanceID <= 0)
            {
                if (todo.TodoType == (int)CommonConsts.TodoType.TODO)
                {
                    //待办的处理方式：
                    try
                    {
                        using (TransactionScope trans = TransactionScopeFactory.Create())
                        {
                            //删除待办
                            WorkflowEngine.WorkflowService.CloseWorkflowTodo(todo);
                            //生成已办日志
                            ApprovalLog log = new ApprovalLog();
                            log.NodeType = todo.NodeType;
                            log.ApprovalNote = approvalNote;
                            log.BizProcessID = todo.BizProcessID;
                            log.CompletedTime = WebHelper.DateTimeNow; ;
                            log.CreatedTime = todo.CreatedTime;
                            log.InstanceName = todo.InstanceName;
                            log.NodeInstanceID = todo.NodeInstanceID;//TODO:huwz
                            log.NodeName = todo.NodeName;
                            log.OperationType = (int)CommonConsts.NodeOperationType.Comment;
                            log.ProcessID = todo.ProcessID;
                            log.ProcessInstanceID = todo.ProcessInstanceID;
                            log.PreviousNodeInstanceID = todo.PreviousNodeInstanceID;
                            log.Status = 0;//保留字段
                            //log.cUserID = todo.cUserID;
                            //log.UserCode = todo.UserCode;
                            log.UserID = todo.UserID;
                            log.UserName = todo.UserName;
                            log.CreateTime = WebHelper.DateTimeNow;
                            log.CreatorName = WebHelper.GetCurrentUser().LoginName;
                            log.IsDeleted = false;
                            log.ModifyTime = WebHelper.DateTimeNow;
                            log.ModifierName = WebHelper.GetCurrentUser().LoginName;
                            log.ID = ApprovalLogAdapter.Instance.Insert(log);

                            trans.Complete();
                        }
                    }
                    catch
                    {
                        throw;
                    }
                }
                else if (todo.TodoType == (int)CommonConsts.TodoType.Notification)//通知这点代码可能基本不会执行
                {
                    //通知的处理方式：
                    try
                    {
                        using (TransactionScope trans = TransactionScopeFactory.Create())
                        {
                            //删除待办
                            WorkflowEngine.WorkflowService.CloseWorkflowTodo(todo);
                            //暂不生成已办日志
                            trans.Complete();
                        }
                    }
                    catch
                    {
                        throw;
                    }
                }
                else
                {
                    throw new Exception("无法处理未知的待办类型。");
                }
            }
            else
            {
                //获取当期节点实例
                ProcessNodeInstance currentNodeins = ProcessNodeInstanceAdapter.Instance.Load(todo.NodeInstanceID);
                try
                {
                    using (TransactionScope trans = TransactionScopeFactory.Create())
                    {
                        //删除当前人员待办
                        WorkflowEngine.WorkflowService.CloseWorkflowTodo(todo);

                        DoSubmit(approvalNote, currentNodeins);

                        //提交事务
                        trans.Complete();

                    }
                    //调用事件委托方法
                    DataExchangeEngine.DataExchangeService.RaiseNodeCompleted(this, new WorkflowEventArgs(this));
                    if (ProcessInstance.Status == (int)CommonConsts.ProcessInstanceStatus.Finished)
                    {
                        DataExchangeEngine.DataExchangeService.RaiseProcessCompleted(this, new WorkflowEventArgs(this));
                    }
                    else
                    {
                        //如果当前流转状态是归档中（可能是刚刚修改为的），而且当前节点不是抄送和归档（审批人），那么需要调用审批通过的消息事件
                        if (ProcessInstance.Status == (int)CommonConsts.ProcessInstanceStatus.Archiving && currentNodeins.NodeSeq < CommonConsts.CCNodeSeqNumber)
                            DataExchangeEngine.DataExchangeService.RaiseProcessApprovalCompleted(this, new WorkflowEventArgs(this));
                        DataExchangeEngine.DataExchangeService.RaiseNodeInitiated(this, new WorkflowEventArgs(this));
                    }
                }
                catch
                {
                    throw;
                }
            }
        }

        public void DoSubmit(string approvalNote, ProcessNodeInstance currentNodeins)
        {
            //标记当前人员为已完成
            currentNodeins.Status = (int)CommonConsts.NodeStatus.Executed;
            currentNodeins.OperationType = (int)CommonConsts.NodeOperationType.Approved;

            if (currentNodeins.NodeSeq == CommonConsts.ArchiveNodeSeqNumber)
            {
                if (this.ProcessInstance.Status != (int)CommonConsts.ProcessInstanceStatus.Archiving)
                {
                    throw new Exception("错误的流程状态。");
                }
                //操作类型为归档
                currentNodeins.OperationType = (int)CommonConsts.NodeOperationType.Archive;
                //如果是发起人归档节点（Seq=5000），标记流程状态为完成，同时调用回调函数
                this.ProcessInstance.Status = (int)CommonConsts.ProcessInstanceStatus.Finished;
                this.ProcessInstance.FinishTime = WebHelper.DateTimeNow; ;
            }
            else if (currentNodeins.NodeSeq >= CommonConsts.CCNodeSeqNumber && currentNodeins.NodeSeq < CommonConsts.ArchiveNodeSeqNumber)
            {
                //如果是抄送人员节点(1000<=Seq<5000)， 更新流程状态
                //操作类型为确认
                currentNodeins.OperationType = (int)CommonConsts.NodeOperationType.Comment;
            }
            else
            {
                //不应该有发起节点，报异常
                if (currentNodeins.NodeSeq == CommonConsts.LaunchNodeSeqNumber)
                {
                    ////当前节点为发起人节点：重新提交
                    //currentNodeins.OperationType = (int)Common.NodeOperationType.Launch;
                    ////流程状态修改为运行中
                    //this.ProcessInstance.Status = (int)Common.ProcessInstanceStatus.Running;
                    throw new Exception("当前为发起人节点，无法进行提交动作。");
                }
                //计算下个节点
                List<ProcessNodeInstance> nextNodeinsList = new List<ProcessNodeInstance>();

                for (int i = 0; i < this.NodeInstances.Count; i++)
                {
                    if (NodeInstances[i].ID == currentNodeins.ID)
                    {
                        //如果不是最后一个审批人节点，跳到下一步。
                        //如果是审批的最后一个节点，需要同时发给：发起人归档（Seq=5000），抄送人员(Seq=1000)发送待办，并修改流程状态为归档中
                        if (NodeInstances[i + 1].NodeSeq >= 1000)
                        {
                            for (int j = i + 1; j < this.NodeInstances.Count; j++)
                            {
                                //如果抄送人相同，或抄送人与归档人相同，避免重复插入待办
                                ProcessNodeInstance node = nextNodeinsList.Find(p => p.UserID == NodeInstances[j].UserID);
                                if (node != null)
                                    nextNodeinsList.Remove(node);
                                nextNodeinsList.Add(NodeInstances[j]);
                            }
                            this.ProcessInstance.Status = (int)CommonConsts.ProcessInstanceStatus.Archiving;
                        }
                        else
                        {
                            nextNodeinsList.Add(NodeInstances[i + 1]);
                        }
                        break;
                    }
                }
                if (nextNodeinsList.Count == 0)
                {
                    throw new Exception("流程出错：无法找到下一个节点。");
                }
                //更新节点信息，生成待办, 添加StakeHolder人员
                for (int i = 0; i < nextNodeinsList.Count; i++)
                {
                    ProcessNodeInstance nextNodeins = nextNodeinsList[i];
                    nextNodeins.Status = (int)CommonConsts.NodeStatus.Executing;
                    nextNodeins.LastUpdatedTime = WebHelper.DateTimeNow; ;
                    ProcessNodeInstanceAdapter.Instance.Update(nextNodeins);
                    TodoWork nextTodo = WorkflowEngine.BuildTodo(CommonConsts.TodoType.TODO, ProcessInstance, nextNodeins);
                    WorkflowEngine.WorkflowService.CreateWorkflowTodo(nextTodo);
                    //添加StakeHolder人员
                    StakeHolder holder = WorkflowEngine.BuildStakeHolder(nextNodeins);
                    try
                    {
                        holder.ID = StakeHolderAdapter.Instance.Insert(holder);
                    }
                    catch
                    {
                        //如果插入重复的，说明以前已经有了，唯一索引导致不能再次插入，可以忽略这个错误
                    }
                }
            }
            //更新当前节点状态
            currentNodeins.LastUpdatedTime = WebHelper.DateTimeNow; ;
            ProcessNodeInstanceAdapter.Instance.Update(currentNodeins);
            //生成已办日志
            ApprovalLog log = WorkflowEngine.BuildApprovalLog(currentNodeins, this.ProcessInstance.InstanceName, approvalNote);
            log.ID = ApprovalLogAdapter.Instance.Insert(log);
            //更新流程状态
            this.ProcessInstance.LastUpdatedTime = WebHelper.DateTimeNow; ;
            ProcessInstanceAdapter.Instance.Update(this.ProcessInstance);
        }

        /// <summary>
        /// 流程退回
        /// </summary>
        /// <param name="approvalNote"></param>
        /// <param name="userID"></param>
        public void Reject(string approvalNote, int userID)
        {
            TodoWork todo = GetUserTodoWork(userID);

            //转发的待办或通知待办无法进行退回操作，只有审批或审核人员才可以进行流程退回
            if (todo.TodoType == (int)CommonConsts.TodoType.Notification ||
                //todo.NodeInstanceID == null ||
                todo.NodeInstanceID == 0 ||
                (todo.NodeType != (int)CommonConsts.NodeType.Review && todo.NodeType != (int)CommonConsts.NodeType.Approval))
            {
                throw new Exception("错误的调用导致流程出错。");
            }

            //获取当期节点实例
            ProcessNodeInstance currentNodeins = ProcessNodeInstanceAdapter.Instance.Load(todo.NodeInstanceID);
            try
            {
                using (TransactionScope trans = TransactionScopeFactory.Create())
                {
                    //删除当前人员待办
                    WorkflowEngine.WorkflowService.CloseWorkflowTodo(todo);

                    //标记发起人的状态为待办，其他所有节点的状态都为未执行，并为发起人生成待办
                    for (int i = 0; i < this.NodeInstances.Count; i++)
                    {
                        ProcessNodeInstance nodeins = this.NodeInstances[i];
                        if (i == 0)
                        {
                            nodeins.Status = (int)CommonConsts.NodeStatus.Executing;
                            TodoWork nextTodo = WorkflowEngine.BuildTodo(CommonConsts.TodoType.TODO, ProcessInstance, nodeins);
                            WorkflowEngine.WorkflowService.CreateWorkflowTodo(nextTodo);
                        }
                        else
                        {
                            nodeins.Status = (int)CommonConsts.NodeStatus.NotExecute;
                            if (nodeins.ID == currentNodeins.ID)
                            {
                                nodeins.OperationType = (int)CommonConsts.NodeOperationType.Reject;
                            }
                        }
                        nodeins.LastUpdatedTime = WebHelper.DateTimeNow; ;
                        ProcessNodeInstanceAdapter.Instance.Update(nodeins);
                        //如果已经更新到当前节点，后续的节点肯定没执行，可以不再更新
                        if (nodeins.ID == currentNodeins.ID)
                        {
                            currentNodeins.LastUpdatedTime = nodeins.LastUpdatedTime;
                            currentNodeins.OperationType = nodeins.OperationType;
                            break;
                        }
                    }

                    //生成已办日志
                    ApprovalLog log = WorkflowEngine.BuildApprovalLog(currentNodeins, this.ProcessInstance.InstanceName, approvalNote);
                    log.ID = ApprovalLogAdapter.Instance.Insert(log);
                    //更新流程状态为退回
                    this.ProcessInstance.Status = (int)CommonConsts.ProcessInstanceStatus.Rejected;
                    this.ProcessInstance.LastUpdatedTime = WebHelper.DateTimeNow; ;
                    ProcessInstanceAdapter.Instance.Update(this.ProcessInstance);

                    //提交事务
                    trans.Complete();
                }
                //调用事件委托方法
                DataExchangeEngine.DataExchangeService.RaiseNodeCompleted(this, new WorkflowEventArgs(this));
                DataExchangeEngine.DataExchangeService.RaiseProcessRejected(this, new WorkflowEventArgs(this));
                DataExchangeEngine.DataExchangeService.RaiseNodeInitiated(this, new WorkflowEventArgs(this));
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 流程取消
        /// </summary>
        /// <param name="approvalNote"></param>
        /// <param name="userID"></param>
        public void Cancel(string approvalNote, int userID)
        {
            TodoWork todo = GetUserTodoWork(userID);

            //转发的待办或通知待办无法进行退回操作，只有发起人才可以进行流程取消
            if (todo.TodoType == (int)CommonConsts.TodoType.Notification ||
                todo.NodeInstanceID == 0 ||
                //todo.NodeInstanceID == 0 || 
                todo.NodeType != (int)CommonConsts.NodeType.Launch)
            {
                throw new Exception("错误的调用导致流程出错。");
            }

            //获取当期节点实例
            ProcessNodeInstance currentNodeins = ProcessNodeInstanceAdapter.Instance.Load(todo.NodeInstanceID);
            try
            {
                using (TransactionScope trans = TransactionScopeFactory.Create())
                {
                    //标记当前人员为已完成，动作为取消

                    currentNodeins.Status = (int)CommonConsts.NodeStatus.Executed;
                    currentNodeins.OperationType = (int)CommonConsts.NodeOperationType.Cancel;
                    currentNodeins.LastUpdatedTime = WebHelper.DateTimeNow; ;
                    ProcessNodeInstanceAdapter.Instance.Update(currentNodeins);
                    //删除当前流程所有待办
                    WorkflowEngine.WorkflowService.CloseWorkflowAllTodo(this);
                    //生成当前人员已办日志
                    ApprovalLog log = WorkflowEngine.BuildApprovalLog(currentNodeins, this.ProcessInstance.InstanceName, approvalNote);
                    log.NodeType = 0;
                    log.NodeName = "流程撤销";
                    log.ID = ApprovalLogAdapter.Instance.Insert(log);
                    //更新流程状态为取消
                    this.ProcessInstance.Status = (int)CommonConsts.ProcessInstanceStatus.Canceled;
                    this.ProcessInstance.LastUpdatedTime = WebHelper.DateTimeNow; ;
                    ProcessInstanceAdapter.Instance.Update(this.ProcessInstance);

                    //提交事务
                    trans.Complete();
                }

                //调用事件委托方法
                DataExchangeEngine.DataExchangeService.RaiseNodeCompleted(this, new WorkflowEventArgs(this));
                DataExchangeEngine.DataExchangeService.RaiseProcessCanceled(this, new WorkflowEventArgs(this));
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 强制撤销流程（该功能建议放在后台调用或管理界面中调用）
        /// </summary>
        /// <param name="cancelNote"></param>
        /// <param name="userID"></param>
        /// <param name="cUserID"></param>
        /// <param name="userName"></param>
        /// <param name="userCode"></param>
        public void ForceCancel(string cancelNote, int userID, int cUserID, string userName, string userCode)
        {
            try
            {
                using (TransactionScope trans = TransactionScopeFactory.Create())
                {
                    //生成操作人员已办日志
                    ApprovalLog log = new ApprovalLog();
                    log.NodeType = 0;
                    log.ApprovalNote = cancelNote;
                    log.BizProcessID = this.ProcessInstance.BizProcessID;
                    log.CompletedTime = WebHelper.DateTimeNow;
                    log.CreatedTime = WebHelper.DateTimeNow;
                    log.InstanceName = this.ProcessInstance.InstanceName;
                    log.NodeInstanceID = 0;
                    //log.NodeInstanceID = 0;
                    log.NodeName = "流程撤销";
                    log.OperationType = (int)CommonConsts.NodeOperationType.Cancel;
                    log.ProcessID = this.ProcessInstance.ProcessID;
                    log.ProcessInstanceID = this.ProcessInstance.ID;
                    //log.PreviousNodeInstanceID = 0;
                    log.PreviousNodeInstanceID = 0;
                    log.Status = 0;//保留字段
                    //log.cUserID = cUserID;
                    //log.UserCode = userCode;
                    log.UserID = userID;
                    log.UserName = userName;
                    log.CreateTime = WebHelper.DateTimeNow;
                    log.CreatorName = WebHelper.GetCurrentUser().LoginName;
                    log.IsDeleted = false;
                    log.ModifyTime = WebHelper.DateTimeNow;
                    log.ModifierName = WebHelper.GetCurrentUser().LoginName;
                    log.ID = ApprovalLogAdapter.Instance.Insert(log);
                    //删除当前流程所有待办
                    WorkflowEngine.WorkflowService.CloseWorkflowAllTodo(this);
                    //更新流程状态为取消
                    this.ProcessInstance.Status = (int)CommonConsts.ProcessInstanceStatus.Canceled;
                    this.ProcessInstance.LastUpdatedTime = WebHelper.DateTimeNow; ;
                    ProcessInstanceAdapter.Instance.Update(this.ProcessInstance);

                    //提交事务
                    trans.Complete();
                }

                //调用事件委托方法
                DataExchangeEngine.DataExchangeService.RaiseProcessCanceled(this, new WorkflowEventArgs(this));
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 流程代理委托
        /// </summary>
        /// <param name="entrustNote"></param>
        /// <param name="userID"></param>
        /// <param name="entrustUserID"></param>
        /// <param name="entrustUserName"></param>
        /// <param name="entrustcUserID"></param>
        /// <param name="entrustUserCode"></param>
        public void Entrust(string entrustNote, int userID, int entrustUserID, string entrustUserName, int entrustcUserID, string entrustUserCode)
        {
            TodoWork todo = GetUserTodoWork(userID);

            //转发的待办或通知待办、抄送的待办无法进行委托操作，只有发起人（归档）、审批人才可以
            if (todo.TodoType == (int)CommonConsts.TodoType.Notification ||
                todo.NodeInstanceID == 0 ||
                //todo.NodeInstanceID == 0 ||  
                todo.NodeType == (int)CommonConsts.NodeType.CC)
            {
                throw new Exception("错误的调用导致流程出错。");
            }

            //获取当期节点实例
            ProcessNodeInstance currentNodeins = ProcessNodeInstanceAdapter.Instance.Load(todo.NodeInstanceID);
            try
            {
                using (TransactionScope trans = TransactionScopeFactory.Create())
                {
                    //删除当前人员待办并增加代理委托日志
                    WorkflowEngine.WorkflowService.CloseWorkflowTodo(todo);
                    entrustNote = string.Format("{0} 将流程待办委托至 {1}：\r\n {2}", todo.UserName, entrustUserName, entrustNote);
                    ApprovalLog log = WorkflowEngine.BuildApprovalLog(currentNodeins, this.ProcessInstance.InstanceName, entrustNote);
                    log.NodeName = "待办委托";
                    log.NodeType = (int)CommonConsts.NodeType.Entrust;
                    log.OperationType = (int)CommonConsts.NodeOperationType.Entrust;
                    log.CreatedTime = WebHelper.DateTimeNow;
                    log.CompletedTime = WebHelper.DateTimeNow;

                    log.ID = ApprovalLogAdapter.Instance.Insert(log);

                    //更新流程节点内容
                    //currentNodeins.CreatedTime = WebHelper.DateTimeNow;;
                    currentNodeins.LastUpdatedTime = WebHelper.DateTimeNow;
                    currentNodeins.cUserID = entrustcUserID;
                    currentNodeins.UserCode = entrustUserCode;
                    currentNodeins.UserID = entrustUserID;
                    currentNodeins.UserName = entrustUserName;
                    ProcessNodeInstanceAdapter.Instance.Update(currentNodeins);

                    //如果以前参与过的人，首先判断当前有无待办，如果有待办，就不能够生成待办了
                    StakeHolder entrustHolder = StakeHolderAdapter.Instance.Load(this.ProcessInstance.ID, entrustUserID);
                    TodoWork entrustTodo = null;
                    if (entrustHolder != null)
                    {
                        entrustTodo = TodoWorkAdapter.Instance.Load(this.ProcessInstance.ID, entrustUserID);
                        if (entrustTodo != null)
                        {
                            throw new Exception(string.Format("用户{0}在当前流程有待办，无法为该用户再添加待办", entrustUserName));
                        }
                    }
                    //为被委托人增加待办
                    //增加stakeholder
                    entrustTodo = WorkflowEngine.BuildTodo(CommonConsts.TodoType.TODO, ProcessInstance, currentNodeins);
                    WorkflowEngine.WorkflowService.CreateWorkflowTodo(entrustTodo);
                    if (entrustHolder == null)
                    {
                        entrustHolder = WorkflowEngine.BuildStakeHolder(currentNodeins);
                        entrustHolder.ID = StakeHolderAdapter.Instance.Insert(entrustHolder);
                    }

                    //提交事务
                    trans.Complete();
                }
                //调用事件委托方法
                DataExchangeEngine.DataExchangeService.RaiseProcessEntrusted(this, new WorkflowEventArgs(this));
            }
            catch
            {
                throw;
            }
        }


        /// <summary>
        /// 转发流程
        /// </summary>
        /// <param name="forwardNote"></param>
        /// <param name="userID"></param>
        /// <param name="forwardUserIDs"></param>
        /// <param name="forwardUserNames"></param>
        /// <param name="forwardcUserIDs"></param>
        /// <param name="forwardUserCodes"></param>
        public void Forward(string forwardNote, int userID, string userName, int cUserID, string userCode,
            string[] forwardUserIDs, string[] forwardUserNames, string[] forwardcUserIDs, string[] forwardUserCodes)
        {
            //StakeHolder currentHolder = StakeHolderAdapter.Instance.Load(this.ProcessInstance.ID, userID);
            try
            {
                using (TransactionScope trans = TransactionScopeFactory.Create())
                {
                    //为当前人员增加转发日志
                    ApprovalLog log = new ApprovalLog();
                    log.NodeType = (int)CommonConsts.NodeType.Forward;
                    string strforwardUserNames = StringHelper.Join(forwardUserNames, "，");
                    log.ApprovalNote = string.Format("转发流程至{0}： {1}", strforwardUserNames, forwardNote);
                    log.BizProcessID = this.ProcessInstance.BizProcessID;
                    log.CompletedTime = WebHelper.DateTimeNow;
                    log.CreatedTime = WebHelper.DateTimeNow; ;
                    log.InstanceName = this.ProcessInstance.InstanceName;
                    log.NodeInstanceID = 0;
                    log.NodeName = "流程转发";
                    log.OperationType = (int)CommonConsts.NodeOperationType.Forward;
                    log.ProcessID = this.ProcessInstance.ProcessID;
                    log.ProcessInstanceID = this.ProcessInstance.ID;
                    log.PreviousNodeInstanceID = 0;
                    log.Status = 0;//保留字段
                    log.UserID = userID;
                    log.UserName = userName;
                    log.ID = 0;
                    log.CreateTime = WebHelper.DateTimeNow;
                    log.CreatorName = userName;
                    log.IsDeleted = false;
                    log.ModifyTime = WebHelper.DateTimeNow;
                    log.ModifierName = WebHelper.GetCurrentUser().LoginName;
                    log.ID = ApprovalLogAdapter.Instance.Insert(log);

                    //为被转发人员增加待办
                    for (int i = 0; i < forwardUserIDs.Length; i++)
                    {
                        //不能转发给自己
                        if (userID == int.Parse(forwardUserIDs[i]))
                        {
                            continue;
                        }

                        StakeHolder holder = StakeHolderAdapter.Instance.Load(this.ProcessInstance.ID, int.Parse(forwardUserIDs[i]));
                        TodoWork todo = new TodoWork();
                        todo.NodeType = (int)CommonConsts.NodeType.Forward;
                        if (holder != null)
                        {
                            //如果以前参与过的人，首先判断当前有无待办，如果有待办，就不需要生成待办了
                            TodoWork todoWork = TodoWorkAdapter.Instance.Load(this.ProcessInstance.ID, int.Parse(forwardUserIDs[i]));
                            if (todoWork != null)
                            {
                                continue;
                            }

                            //20120407: 不再这样处理待办，所有被转发的人，收到的都是转发待办，只能做批注
                            //需要根据当时的NodeType生成待办，如果是审核人或审批人，需要处理
                            //if (holder.NodeType == (int)Common.NodeType.Approval || holder.NodeType == (int)Common.NodeType.Review)
                            //{
                            //    //如果流程不是办结而且不是被退回的状态，需要处理
                            //    if (this.ProcessInstance.Status != (int)Common.ProcessInstanceStatus.Finished && this.ProcessInstance.Status != (int)Common.ProcessInstanceStatus.Rejected)
                            //    {
                            //        todo.NodeType = holder.NodeType;
                            //    }
                            //}
                        }
                        todo.BizProcessID = this.ProcessInstance.BizProcessID;
                        todo.CreatedTime = WebHelper.DateTimeNow; ;

                        todo.InstanceName = this.ProcessInstance.InstanceName;
                        todo.NodeInstanceID = 0;
                        todo.NodeName = "转发反馈";
                        todo.ProcessID = this.ProcessInstance.ProcessID;
                        todo.ProcessInstanceID = this.ProcessInstance.ID;
                        todo.PreviousNodeInstanceID = 0;
                        todo.ProcessCode = this.ProcessInstance.ProcessCode;
                        todo.Status = (int)CommonConsts.TodoStatus.Unread;
                        todo.TodoType = (int)CommonConsts.TodoType.TODO;
                        todo.UserID = int.Parse(forwardUserIDs[i]);
                        todo.UserName = forwardUserNames[i];

                        todo.CreateProcessTime = this.ProcessInstance.CreateTime;

                        todo.CreateProcessUserName = this.ProcessInstance.UserName;
                        todo.CreateProcessUserID = this.ProcessInstance.UserID;

                        todo.ProjectID = this.ProcessInstance.ProjectID;

                        //add czq 
                        //todo.ID = Guid.NewGuid().ToString();
                        todo.CreateTime = WebHelper.DateTimeNow;
                        todo.CreatorName = WebHelper.GetCurrentUser().LoginName;
                        todo.IsDeleted = false;
                        todo.ModifyTime = WebHelper.DateTimeNow;
                        todo.ModifierName = WebHelper.GetCurrentUser().LoginName;

                        WorkflowEngine.WorkflowService.CreateWorkflowTodo(todo);

                        //添加StakeHolder人员
                        if (holder == null)
                        {
                            holder = new StakeHolder();
                            holder.NodeType = (int)CommonConsts.NodeType.Forward;
                            holder.BizProcessID = this.ProcessInstance.BizProcessID;
                            holder.CreatedTime = WebHelper.DateTimeNow; ;
                            holder.ProcessID = this.ProcessInstance.ProcessID;
                            holder.ProcessInstanceID = this.ProcessInstance.ID;
                            //holder.cUserID = forwardcUserIDs[i];
                            //holder.UserCode = forwardUserCodes[i];
                            holder.UserID = int.Parse(forwardUserIDs[i]);
                            holder.UserName = forwardUserNames[i];
                            holder.CreateTime = WebHelper.DateTimeNow; ;
                            holder.CreatorName = WebHelper.GetCurrentUser().LoginName;
                            holder.IsDeleted = false;
                            holder.ModifyTime = WebHelper.DateTimeNow;
                            holder.ModifierName = WebHelper.GetCurrentUser().LoginName;
                            holder.ID = StakeHolderAdapter.Instance.Insert(holder);
                        }
                    }

                    trans.Complete();
                }
                //调用事件委托方法
                DataExchangeEngine.DataExchangeService.RaiseProcessForwarded(this, new WorkflowEventArgs(this));
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// 归档
        /// </summary>
        /// <param name="approvalNote"></param>
        /// <param name="userID"></param>
        /// <param name="todoWorkID"></param>
        public void Archiving(string approvalNote, int userID, int todoWorkID)
        {
            TodoWork todo = GetUserTodoWork(userID, todoWorkID);
            //获取当期节点实例
            ProcessNodeInstance currentNodeins = ProcessNodeInstanceAdapter.Instance.Load(todo.NodeInstanceID);
            try
            {
                //删除当前人员待办
                WorkflowEngine.WorkflowService.CloseWorkflowTodo(todo);
                //操作类型为归档
                currentNodeins.OperationType = (int)CommonConsts.NodeOperationType.Archive;
                //如果是发起人归档节点（Seq=5000），标记流程状态为完成，同时调用回调函数
                this.ProcessInstance.Status = (int)CommonConsts.ProcessInstanceStatus.Finished;
                this.ProcessInstance.FinishTime = WebHelper.DateTimeNow; ;
                //更新当前节点状态
                currentNodeins.LastUpdatedTime = WebHelper.DateTimeNow; ;
                currentNodeins.Status = (int)Wanda.Lib.LightWorkflow.CommonConsts.NodeStatus.Executed;
                ProcessNodeInstanceAdapter.Instance.Update(currentNodeins);
                //生成已办日志
                ApprovalLog log = WorkflowEngine.BuildApprovalLog(currentNodeins, this.ProcessInstance.InstanceName, approvalNote);
                log.ID = ApprovalLogAdapter.Instance.Insert(log);
                //更新流程状态
                this.ProcessInstance.LastUpdatedTime = WebHelper.DateTimeNow; ;
                ProcessInstanceAdapter.Instance.Update(this.ProcessInstance);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 撤回流程
        /// </summary>
        /// <param name="approvalNote"></param>
        /// <param name="p"></param>
        public void WithDraw(string approvalNote, WorkflowInstance wfinstance)
        {
            try
            {
                TodoWork todo= wfinstance.GetCurrentTodo();
                if (todo == null)
                {
                    throw new Exception("用户无权限或该待办不存在导致流程无法执行。");
                }
                //获取当期节点实例
                ProcessNodeInstance currentNodeins = ProcessNodeInstanceAdapter.Instance.Load(todo.NodeInstanceID);
                using (TransactionScope trans = TransactionScopeFactory.Create())
                {
                    //标记当前人员为已完成，动作为取消

                    currentNodeins.Status = (int)CommonConsts.NodeStatus.Executed;
                    currentNodeins.OperationType = (int)CommonConsts.NodeOperationType.Cancel;
                    currentNodeins.LastUpdatedTime = WebHelper.DateTimeNow; ;
                    ProcessNodeInstanceAdapter.Instance.Update(currentNodeins);
                    //删除当前流程所有待办
                    WorkflowEngine.WorkflowService.CloseWorkflowAllTodo(this);
                    //生成当前人员已办日志
                    ApprovalLog log = WorkflowEngine.BuildApprovalLog(currentNodeins, this.ProcessInstance.InstanceName, approvalNote);
                    log.NodeType = currentNodeins.NodeType;
                    log.NodeName = "流程撤销";
                    log.ID = ApprovalLogAdapter.Instance.Insert(log);
                    //更新流程状态为取消
                    this.ProcessInstance.Status = (int)CommonConsts.ProcessInstanceStatus.WidthDrawed;
                    this.ProcessInstance.LastUpdatedTime = WebHelper.DateTimeNow; ;
                    ProcessInstanceAdapter.Instance.Update(this.ProcessInstance);

                    //提交事务
                    trans.Complete();
                }

                //调用事件委托方法
                DataExchangeEngine.DataExchangeService.RaiseNodeCompleted(this, new WorkflowEventArgs(this));
                DataExchangeEngine.DataExchangeService.RaiseProcessWidthDrawed(this, new WorkflowEventArgs(this));
                DataExchangeEngine.DataExchangeService.RaiseNodeInitiated(this, new WorkflowEventArgs(this));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private TodoWork GetUserTodoWork(int userID, int todoWorkID = -1)
        {
            TodoWork todo = FindUserTodoWork(userID, todoWorkID);
            if (todo == null)
            {
                throw new Exception("用户无权限或该待办不存在导致流程无法执行。");
            }
            return todo;
        }

        private TodoWork FindUserTodoWork(int userID, int todoWorkID = -1)
        {
            TodoWork todo = null;
            if (this.TodoWorks != null)
            {
                for (int i = 0; i < this.TodoWorks.Count; i++)
                {
                    if (todoWorkID == -1)
                    {
                        if (TodoWorks[i].UserID == userID)
                        {
                            todo = TodoWorks[i];
                            break;
                        }
                    }
                    else
                    {
                        if (TodoWorks[i].ID == todoWorkID && TodoWorks[i].UserID == userID)
                        {
                            todo = TodoWorks[i];
                            break;
                        }
                    }
                }
            }
            return todo;
        }
        /// <summary>
        /// 获取当前的流程待办节点
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        private TodoWork GetCurrentTodo()
        {
            TodoWork todo = null;
            if (this.TodoWorks != null)
            {
                ProcessNodeInstance node = this.NodeInstances.Find(p => p.Status == (int)CommonConsts.NodeStatus.Executing);
                if (node == null)
                    return todo;
                return this.TodoWorks.Find(p => p.NodeInstanceID == node.ID);
            }
            return todo;
        }
        #endregion
    }
}
