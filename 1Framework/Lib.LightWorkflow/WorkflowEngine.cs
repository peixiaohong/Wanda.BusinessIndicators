using System;
using System.Transactions;
using System.Collections.Generic;
using System.Collections;
using System.Linq;


using Wanda.Lib.LightWorkflow.Entities;
using Wanda.Lib.LightWorkflow.Dal;

using Wanda.Lib.LightWorkflow.Tools;
using System.Collections;
using Wanda.Lib.LightWorkflow.Expression;
using Wanda.Lib.LightWorkflow.Services;
using Wanda.Lib.LightWorkflow.Configs;
using Lib.Data;
using Wanda.Lib.LightWorkflow.Filter;
using Wanda.HR.Common.Web;

using Lib.Config;


namespace Wanda.Lib.LightWorkflow
{
    public class WorkflowEngine
    {
        public WorkflowEngine()
        {
        }

        /// <summary>
        /// 引擎实例
        /// </summary>	
        public static WorkflowEngine WorkflowService
        {
            get
            {
                return _WorkflowService;
            }
        }
        private static WorkflowEngine _WorkflowService = new WorkflowEngine();

        /// <summary>
        /// 流程类型的表达式定义名称
        /// </summary>
        private string ProcessTypeName
        {
            get
            {
                if (_ProcessTypeName == string.Empty)
                {
                    WorkflowSetting setting = LightWorkflowSettings.Instance.WorkflowSettings["ProcessType"];
                    if (setting == null)
                        _ProcessTypeName = string.Empty;
                    else
                        _ProcessTypeName = setting.Value;
                }
                return _ProcessTypeName;

            }
        }
        private string _ProcessTypeName = string.Empty;

        /// <summary>
        /// 所有流程定义的名字列表，可以用ProcessCode进行查询
        /// </summary>
        public Dictionary<int, string> ProcessNameList
        {
            get
            {
                if (_ProcessNameList == null)
                {
                    _ProcessNameList = new Dictionary<int, string>();
                    List<Process> processList = ProcessAdapter.Instance.GetAllActivedProcesses();
                    foreach (Process process in processList)
                    {
                        _ProcessNameList.Add(process.ID, process.ProcessName);
                    }
                }
                return _ProcessNameList;
            }
        }
        private Dictionary<int, string> _ProcessNameList = null;

        /// <summary>
        /// 获取流程节点的定义列表
        /// </summary>
        /// <param name="processCode">流程编码</param>
        /// <param name="bizProcessContext">业务流程上下文环境数据</param>
        /// <returns></returns>
        public List<ProcessNode> GetProcessNodeList(string processCode, int CongID, Hashtable bizProcessContext)
        {
            int processID = ProcessAdapter.Instance.LoadByCode(processCode, CongID).ID;
            string processType = string.Empty;
            if (ProcessTypeName != string.Empty)
                processType = bizProcessContext["CompanyType"].ToString();
            List<ProcessNode> result = ProcessNodeAdapter.Instance.LoadList(processID, processType);

            //根据流程上下文计算流程节点
            if (bizProcessContext != null)
                result = CacluateProcessNodesByCondition(result, bizProcessContext);

            //校验流程节点列表是否合规
            CheckProcessNodes(result);

            return result;
        }

        /// <summary>
        /// 根据流程上下文计算流程节点
        /// </summary>
        /// <param name="nodeList"></param>
        /// <param name="bizProcessContext"></param>
        /// <returns></returns>
        public List<ProcessNode> CacluateProcessNodesByCondition(List<ProcessNode> nodeList, Hashtable bizProcessContext)
        {
            for (int i = nodeList.Count - 1; i >= 0; i--)
            {
                ProcessNode node = nodeList[i];
                //使用表达式分析加载流程上下文对象
                WorkflowExpressionParser parser = new WorkflowExpressionParser(bizProcessContext);
                //使用节点表达式计算，如果不符合表达式，从列表里面移除
                if (!parser.CacluateCondition(node.Expression))
                {
                    nodeList.Remove(node);
                }
            }
            return nodeList;
        }

        /// <summary>
        /// 校验流程节点列表是否合规
        /// </summary>
        /// <param name="result"></param>
        public void CheckProcessNodes(List<ProcessNode> nodeList)
        {
            int launchNodeCount = 0;
            int approvalNodeCount = 0;
            if (nodeList.Count == 0)
            {
                throw new Exception("无流程节点，无法发起流程。");
            }
            else if (nodeList.Count == 1)
            {
                throw new Exception("仅有一个流程节点，无法发起流程。");
            }
            foreach (ProcessNode node in nodeList)
            {
                if (node.NodeType == (int)CommonConsts.NodeType.Launch)
                    launchNodeCount++;
                else if (node.NodeType == (int)CommonConsts.NodeType.Review || node.NodeType == (int)CommonConsts.NodeType.Approval)
                    approvalNodeCount++;
            }
            if (launchNodeCount == 0)
            {
                throw new Exception("无发起人节点，无法发起流程。");
            }
            else if (launchNodeCount > 1)
            {
                throw new Exception("有多个发起人节点，无法发起流程。");
            }
            else if (approvalNodeCount == 0)
            {
                throw new Exception("无复核或审批人节点，无法发起流程。");
            }
        }

        /// <summary>
        /// 获取流程实例节点列表
        /// </summary>
        /// <param name="bizProcessID">业务ID</param>
        /// <returns></returns>
        public List<ProcessNodeInstance> GetProcessNodeInstanceList(int bizProcessID)
        {
            List<ProcessNodeInstance> result = ProcessNodeInstanceAdapter.Instance.LoadListByBizID(bizProcessID);

            return result;
        }

        /// <summary>
        /// 启动流程（创建流程实例）
        /// </summary>
        /// <param name="processCode">流程编码</param>
        /// <param name="bizProcessID">业务id</param>
        /// <param name="projectID">项目id</param>
        /// <param name="InstanceName">流程实例名称</param>
        /// <param name="approvalNote">提交意见</param>
        /// <param name="nodeInstanceList">节点列表</param>
        /// <param name="bizProcessContext">业务流程上下文</param>
        /// <returns></returns>
        public WorkflowInstance StartProcess(string processCode, int CongID, int bizProcessID, int projectID, string instanceName, string approvalNote, List<ProcessNodeInstance> nodeInstanceList, Hashtable bizProcessContext)
        {
            int processID = ProcessAdapter.Instance.LoadByCode(processCode, CongID).ID;
            return WorkflowInstance.Start(processID, processCode, bizProcessID, projectID, instanceName, approvalNote, nodeInstanceList, bizProcessContext);
        }


        /// <summary>
        /// 不需要发送OA待办的特殊流程及流程
        /// 默认值为false不发送
        /// </summary>
        public Hashtable OAMessageExceptionList
        {
            get
            {
                if (_OAMessageExceptionList.Count<=0)
                {
                    WorkflowSetting setting = LightWorkflowSettings.Instance.WorkflowSettings["OAMessageExceptionList"];
                    string list = setting == null ? null : setting.Value;
                    if (!string.IsNullOrEmpty(list))
                    {
                        string[] pns = list.Split('|');
                        foreach (string s in pns)
                        {
                            if (s.Split(',').Length == 2)
                            {
                                _OAMessageExceptionList.Add(s.Split(',')[0].ToLower().Trim(), s.Split(',')[1].ToLower().Trim());
                            }
                        }

                    }
                }
                return _OAMessageExceptionList;
            }
        }private Hashtable _OAMessageExceptionList = new System.Collections.Hashtable();


        /// <summary>
        /// 创建工作流待办
        /// </summary>
        /// <param name="todo"></param>
        public void CreateWorkflowTodo(TodoWork todo)
        {
            try
            {
                OAMQMessages msg = OAService.OAServiceInstance.BuildMessage(todo);
                using (TransactionScope trans = TransactionScopeFactory.Create())
                {
                    todo.ID = TodoWorkAdapter.Instance.Insert(todo);
                    msg.TodoID = todo.ID;
                    bool sendMessage = true;
                    if (OAMessageExceptionList.ContainsKey(todo.ProcessCode.ToLower().Trim()))
                    {
                        if (OAMessageExceptionList[todo.ProcessCode.ToLower().Trim()].ToString() == todo.NodeType.ToString())
                        {
                            sendMessage = false; ;
                        }
                    }
                    if (sendMessage)
                        OAService.OAServiceInstance.CreateOATodo(msg);
                    trans.Complete();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 关闭工作流待办
        /// </summary>
        /// <param name="todo"></param>
        public void CloseWorkflowTodo(TodoWork todo)
        {
            try
            {
                OAMQMessages msg = OAService.OAServiceInstance.BuildMessage(todo);
                using (TransactionScope trans = TransactionScopeFactory.Create())
                {
                    //删除通知待办
                    TodoWorkAdapter.Instance.Delete(todo.ID);
                    bool sendMessage = true;
                    bool needDoneMessage = true;
                    if (OAMessageExceptionList.ContainsKey(todo.ProcessCode.ToLower().Trim()))
                    {
                        needDoneMessage = false;
                        if (OAMessageExceptionList[todo.ProcessCode.ToLower().Trim()].ToString() == todo.NodeType.ToString())
                        {
                            sendMessage = false; ;
                        }
                    }
                    if (sendMessage)
                        OAService.OAServiceInstance.CloseOATodo(msg, needDoneMessage);
                    trans.Complete();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 删除ProcessInstanceID相关的所有待办
        /// </summary>
        /// <param name="instance">流程实例</param>
        public void CloseWorkflowAllTodo(WorkflowInstance instance)
        {
            bool needDoneMessage = true;
            if (OAMessageExceptionList.ContainsKey(instance.ProcessCode.ToLower().Trim()))
            {
                needDoneMessage = false;
            }
                try
            {
                List<OAMQMessages> msgList = new List<OAMQMessages>();
                foreach (TodoWork todo in instance.TodoWorks)
                {
                    bool sendmessage = true;
                    if (OAMessageExceptionList.ContainsKey(instance.ProcessCode.ToLower().Trim()))
                    {
                        if (OAMessageExceptionList[todo.ProcessCode.ToLower().Trim()].ToString()== todo.NodeType.ToString()) 
                        {
                            sendmessage = false;
                        }
                    }
                    if (sendmessage)
                    {
                        msgList.Add(OAService.OAServiceInstance.BuildMessage(todo));
                    }
                }
                using (TransactionScope trans = TransactionScopeFactory.Create())
                {
                    //循环获取所有需要删除的待办列表，然后统一发到OA里面删除
                    OAService.OAServiceInstance.CloseOATodo(msgList, needDoneMessage);
                    //删除通知待办
                    TodoWorkAdapter.Instance.DeleteAll(instance.ProcessInstance.ID);
                    trans.Complete();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 创建通知类的待办
        /// </summary>
        /// <param name="bizProcessID">业务相关ID</param>
        /// <param name="todoTitle">待办标题</param>
        /// <param name="receiveUserID">接收用户ID</param>
        /// <param name="receiveUserName">接收用户姓名</param>
        /// <param name="receiveUserCode">接收用户代码</param>
        /// <param name="receivecUserID">接收用户cUserID</param>
        /// <param name="createUserID">创建者ID</param>
        /// <param name="createUserName">创建者姓名</param>
        /// <param name="createUserCode">创建者代码</param>
        /// <param name="createcUserID">创建者cUserID</param>
        public void CreateNotificationTodo(int bizProcessID, string todoTitle,
            int receiveUserID, string receiveUserName, string receiveUserCode, int receivecUserID,
            int createUserID, string createUserName, string createUserCode, int createcUserID, int projectID)
        {
            TodoWork todo = new TodoWork();
            todo.NodeType = 0;
            todo.BizProcessID = bizProcessID;
            todo.InstanceName = todoTitle;
            //todo.NodeInstanceID = 0;
            todo.NodeInstanceID = 0;
            todo.NodeName = "";
            //todo.ProcessID = 0;
            //todo.ProcessInstanceID = 0;
            //todo.PreviousNodeInstanceID = 0;
            todo.ProcessID = 0;
            todo.ProcessInstanceID = 0;
            todo.PreviousNodeInstanceID = 0;
            todo.ProcessCode = "";
            todo.Status = (int)CommonConsts.TodoStatus.Unread;
            todo.TodoType = (int)CommonConsts.TodoType.Notification;
            todo.cUserID = receivecUserID;
            todo.UserCode = receiveUserCode;
            todo.UserID = receiveUserID;
            todo.UserName = receiveUserName;

            todo.CreatedTime = WebHelper.DateTimeNow;
            todo.ModifyTime = WebHelper.DateTimeNow;

            todo.CreateProcessTime = WebHelper.DateTimeNow;

            todo.CreateProcessUserName = createUserName;
            todo.ProjectID = projectID;
            try
            {
                OAMQMessages msg = OAService.OAServiceInstance.BuildMessage(todo);
                using (TransactionScope trans = new TransactionScope())
                {
                    TodoWorkAdapter.Instance.Insert(todo);
                    OAService.OAServiceInstance.CreateOATodo(msg);
                    trans.Complete();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 关闭通知类待办
        /// </summary>
        /// <param name="bizProcessID"></param>
        /// <param name="userID"></param>
        /// <param name="todoWorkID"></param>
        /// <param name="projectID"></param>
        public void CloseNotificationTodo(int bizProcessID, int userID, int todoWorkID, int projectID)
        {
            TodoWork todo = TodoWorkAdapter.Instance.Load(todoWorkID);
            if (todo == null)
            {
                throw new Exception("没有找到指定的待办。");
            }
            else if (todo.BizProcessID != bizProcessID || todo.ProjectID != projectID || todo.UserID != userID)
            {
                throw new Exception("用户无权限或该待办不存在。");
            }
            else if (todo.TodoType != (int)CommonConsts.TodoType.Notification)
            {
                throw new Exception("无法处理未知的待办类型。");
            }
            else
            {
                try
                {
                    OAMQMessages msg = OAService.OAServiceInstance.BuildMessage(todo);
                    using (TransactionScope trans = TransactionScopeFactory.Create())
                    {
                        //删除通知待办,暂不生成已办日志
                        TodoWorkAdapter.Instance.Delete(todo.ID);
                        OAService.OAServiceInstance.CloseOATodo(msg);
                        trans.Complete();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public void RemoveApprovalLog(ProcessInstance pInstance)
        {
            ApprovalLogAdapter.Instance.RemoveApprovalLog(pInstance.ID);
        }

        public void RemoveStakeholder(ProcessInstance pInstance)
        {
            StakeHolderAdapter.Instance.DeleteStakeholder(pInstance.ID);
        }


        public void RemoveStakeholder(WorkflowInstance wInstance)
        {
            RemoveStakeholder(wInstance.ProcessInstance);
        }

        public void RemoveApprovalLog(WorkflowInstance wInstance)
        {
            RemoveStakeholder(wInstance.ProcessInstance);
        }

        /// <summary>
        /// 根据传递的bizprocessid获取流程对象
        /// </summary>
        /// <param name="bizProcessID"></param>
        /// <returns></returns>
        public WorkflowInstance GetWorkflowInstance(int bizProcessID)
        {
            ProcessInstance instance = ProcessInstanceAdapter.Instance.LoadByBizProcessID(bizProcessID);
            if (instance == null)
                return null;
            else
                return new WorkflowInstance(instance);
        }

        /// <summary>
        /// 更新节点使用
        /// </summary>
        /// <param name="bizID">BizProcessID</param>
        /// <param name="nodeID">NodeInstanceID</param>
        /// <param name="uid">UserID</param>
        /// <param name="userName">UserName</param>
        /// <returns></returns>
        public int UpdateNodeInstance(int bizID, int nodeID, int uid, string userName)
        {
            WorkflowInstance instance = WorkflowEngine.WorkflowService.GetWorkflowInstance(bizID);
            List<ProcessNodeInstance> nodeInstanceList = ProcessNodeInstanceAdapter.Instance.LoadList(instance.ProcessInstance.ID);
            ProcessNodeInstance nodeInstance = nodeInstanceList.Find(p => p.ID == nodeID);
            TodoWork todo = null;
            int result = 0;
            foreach (var item in instance.TodoWorks)
            {
                if (item.TodoType == (int)Wanda.Lib.LightWorkflow.CommonConsts.TodoType.TODO)
                {
                    todo = item;
                    using (TransactionScope trans = TransactionScopeFactory.Create())
                    {
                        //待办的处理方式：
                        //删除待办
                        WorkflowEngine.WorkflowService.CloseWorkflowTodo(todo);
                        //生成已办日志
                        ApprovalLog log = new ApprovalLog();
                        log.NodeType = todo.NodeType;
                        log.ApprovalNote = string.Format("{0}将节点更新至{1}", userName, nodeInstance.UserName);
                        log.BizProcessID = todo.BizProcessID;
                        log.CompletedTime = WebHelper.DateTimeNow;
                        log.CreatedTime = todo.CreatedTime;
                        log.InstanceName = todo.InstanceName;
                        log.NodeInstanceID = todo.NodeInstanceID;//TODO:huwz
                        log.NodeName = todo.NodeName;
                        log.OperationType = (int)Wanda.Lib.LightWorkflow.CommonConsts.NodeOperationType.ChangeNode;
                        log.ProcessID = todo.ProcessID;
                        log.ProcessInstanceID = todo.ProcessInstanceID;
                        log.PreviousNodeInstanceID = todo.PreviousNodeInstanceID;
                        log.Status = 0;//保留字段
                        //log.cUserID = todo.cUserID;
                        //log.UserCode = todo.UserCode;
                        log.CreateTime = WebHelper.DateTimeNow;
                        log.CreatorName = userName;
                        log.UserID = todo.UserID;
                        log.UserName = todo.UserName;
                        log.ID = 0;

                        log.ModifyTime = WebHelper.DateTimeNow;
                        log.ModifierName = userName;

                        ApprovalLogAdapter.Instance.Insert(log);

                        //计算下个节点，生成待办
                        TodoWork todoNew = WorkflowEngine.BuildTodo(Wanda.Lib.LightWorkflow.CommonConsts.TodoType.TODO, instance.ProcessInstance, nodeInstance);
                        WorkflowEngine.WorkflowService.CreateWorkflowTodo(todoNew);

                        //更新流程节点
                        int curSqu = -1;
                        foreach (var temp in nodeInstanceList)
                        {
                            if (temp.NodeSeq > nodeInstance.NodeSeq)
                            {
                                temp.Status = (int)Wanda.Lib.LightWorkflow.CommonConsts.NodeStatus.NotExecute;
                                ProcessNodeInstanceAdapter.Instance.Update(temp);
                                continue;
                            }

                            if (temp.NodeSeq < nodeInstance.NodeSeq)
                            {
                                temp.Status = (int)Wanda.Lib.LightWorkflow.CommonConsts.NodeStatus.Executed;
                                if (temp.ID == todo.NodeInstanceID)
                                {
                                    curSqu = temp.NodeSeq;
                                }
                                if (curSqu != -1 && temp.NodeSeq >= curSqu)
                                {
                                    temp.OperationType = (int)Wanda.Lib.LightWorkflow.CommonConsts.NodeOperationType.ChangeNode;
                                    ProcessNodeInstanceAdapter.Instance.Update(temp);
                                }
                            }
                            else
                            {
                                temp.Status = (int)Wanda.Lib.LightWorkflow.CommonConsts.NodeStatus.Executing;
                                temp.OperationType = (int)Wanda.Lib.LightWorkflow.CommonConsts.NodeOperationType.ChangeNode;
                                ProcessNodeInstanceAdapter.Instance.Update(temp);
                            }

                        }
                        if (nodeInstance.NodeType == (int)CommonConsts.NodeType.Archive)
                        {
                            ProcessInstance p = instance.ProcessInstance;
                            p.Status = (int)CommonConsts.ProcessInstanceStatus.Archiving;
                            ProcessInstanceAdapter.Instance.Update(p);
                        }
                        //添加StakeHolder人员
                        StakeHolder holder = WorkflowEngine.BuildStakeHolder(nodeInstance);

                        StakeHolderAdapter.Instance.Insert(holder);
                        result = nodeID;
                        trans.Complete();
                    }
                    break;
                }
            }
            return result;
        }

        #region 调用Adapter方法
        /// <summary>
        /// 获取BBizProcess
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BBizProcess GetBizProcessByID(int id)
        {
            return new BizProcessAdapter().GetModelByID(id);
        }
        /// <summary>
        /// 更新BBizProcess
        /// </summary>
        /// <param name="bizPro"></param>
        /// <returns></returns>
        public int InsertBizProcess(BBizProcess bizPro)
        {
            return new BizProcessAdapter().Insert(bizPro);
        }
        /// <summary>
        /// 更新BBizProcess
        /// </summary> 
        /// <param name="bizPro"></param>
        /// <returns></returns>
        public int UpdateBizProcess(BBizProcess bizPro)
        {
            return new BizProcessAdapter().Update(bizPro);
        }


        /// <summary>
        /// 获取ProcessNodeInstance
        /// </summary>
        /// <param name="niid"></param>
        /// <returns></returns>
        public ProcessNodeInstance LoadProcessNodeInstance(int niid)
        {
            return ProcessNodeInstanceAdapter.Instance.GetModelByID(niid);
        }
        /// <summary>
        /// 获取ProcessNode
        /// </summary>
        /// <param name="processID"></param>
        /// <param name="processType"></param>
        /// <returns></returns>
        public List<ProcessNode> LoadProcessNodeList(int processID, string processType)
        {
            return ProcessNodeAdapter.Instance.LoadList(processID, processType);
        }
        /// <summary>
        /// 加载ProcessNode
        /// </summary>
        /// <param name="processID"></param>
        /// <returns></returns>
        public List<ProcessNode> LoadProcessNodeList(int processID)
        {
            return ProcessNodeAdapter.Instance.LoadList(processID);
        }
        /// <summary>
        /// 获取ProcessNode 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public ProcessNode LoadProcessNodeByID(int id)
        {
            return ProcessNodeAdapter.Instance.GetModelByID(id);
        }
        /// <summary>
        /// 插入ProcessNode
        /// </summary>
        /// <param name="processNode"></param>
        /// <returns></returns>
        public int InsertProcessNode(ProcessNode processNode)
        {
            return ProcessNodeAdapter.Instance.Insert(processNode);
        }

        /// <summary>
        /// 更新ProcessNode
        /// </summary>
        /// <param name="processID"></param>
        /// <returns></returns>
        public int UpdateProcessNode(ProcessNode processNode)
        {
            return ProcessNodeAdapter.Instance.Update(processNode);
        }
        /// <summary>
        /// 删除ProcessNode
        /// </summary>
        /// <param name="id"></param>
        public void RemoveProcessNode(int id)
        {
            ProcessNodeAdapter.Instance.Delete(id);
        }
        /// <summary>
        /// 根据processID删除ProcessNode
        /// </summary>
        /// <param name="processID"></param>
        public void RemoveProcessNodeByProcessID(int processID)
        {
            ProcessNodeAdapter.Instance.RemoveByProcessID(processID);
        }

        /// <summary>
        /// 获取Process
        /// </summary>
        /// <param name="processCode"></param>
        /// <param name="congID"></param>
        /// <returns></returns>
        public Process LoadProcessByCode(string processCode, int congID)
        {
            return ProcessAdapter.Instance.LoadByCode(processCode, congID);
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public int InsertProcess(Process process)
        {
            return ProcessAdapter.Instance.Insert(process);
        }
        /// <summary>
        /// 获取Process
        /// </summary>
        /// <param name="processCode"></param>
        /// <param name="congID"></param>
        /// <returns></returns>
        public Process LoadProcessByID(int id)
        {
            return ProcessAdapter.Instance.GetModelByID(id);
        }
        /// <summary>
        /// 获取Process
        /// </summary>
        /// <param name="processCode"></param>
        /// <param name="congID"></param>
        /// <returns></returns>
        public int UpdateProcess(Process process)
        {
            return ProcessAdapter.Instance.Update(process);
        }
        /// <summary>
        /// 删除Process
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public void RemoveProcess(int id)
        {
            ProcessAdapter.Instance.Delete(id);
        }
        /// <summary>
        /// 更新Process状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public int UpdateProcessByStatus(int id, bool status)
        {
            Process process = LoadProcessByID(id);
            process.IsActived = status;
            UpdateProcess(process);
            return id;
        }
        /// <summary>
        /// 获取ProcessInstance
        /// </summary>
        /// <param name="bizProcessID"></param>
        /// <returns></returns>
        public ProcessInstance LoadProcessInstanceByBizProcessID(int bizProcessID)
        {
            return ProcessInstanceAdapter.Instance.LoadByBizProcessID(bizProcessID);
        }
        /// <summary>
        /// 更新ProcessInstance
        /// </summary>
        /// <param name="bizProcessID"></param>
        /// <returns></returns>
        public int UpdateProcessInstance(ProcessInstance pInstance)
        {
            return ProcessInstanceAdapter.Instance.Update(pInstance);
        }
        /// <summary>
        /// 获取ProcessInstance
        /// </summary>
        /// <param name="ProcessInstanceID"></param>
        /// <returns></returns>
        public ProcessInstance LoadProcessInstanceByProcessInstanceID(int ProcessInstanceID)
        {
            return ProcessInstanceAdapter.Instance.Load(ProcessInstanceID);
        }


        #endregion

        #region build对象方法
        /// <summary>
        /// 构建审批日志
        /// </summary>
        /// <param name="nodeInstance"></param>
        /// <param name="instanceName"></param>
        /// <param name="approvalNote"></param>
        /// <returns></returns>
        public static ApprovalLog BuildApprovalLog(ProcessNodeInstance nodeInstance, string instanceName, string approvalNote)
        {
            ApprovalLog log = new ApprovalLog();
            log.NodeType = nodeInstance.NodeType;
            log.ApprovalNote = approvalNote;
            log.BizProcessID = nodeInstance.BizProcessID;
            log.CompletedTime = nodeInstance.LastUpdatedTime;
            log.CreatedTime = nodeInstance.CreatedTime;
            log.InstanceName = instanceName;
            log.NodeInstanceID = nodeInstance.ID; //TODO:huwz
            log.NodeName = nodeInstance.NodeName + ObjectHelper.EnumDescription((CommonConsts.NodeType)log.NodeType);
            log.OperationType = nodeInstance.OperationType;
            log.ProcessID = nodeInstance.ProcessID;
            log.ProcessInstanceID = nodeInstance.ProcessInstanceID;
            log.PreviousNodeInstanceID = nodeInstance.PreviousNodeInstanceID;
            log.Status = 0;//保留字段 
            log.UserID = nodeInstance.UserID;
            log.UserName = nodeInstance.UserName;
            //add czq
            log.CreateTime = WebHelper.DateTimeNow;
            log.CreatorName = WebHelper.GetCurrentUser().LoginName;
            log.IsDeleted = false;
            log.ModifyTime = WebHelper.DateTimeNow;
            log.ModifierName = WebHelper.GetCurrentUser().LoginName;
            return log;
        }
        /// <summary>
        /// 构建流程干系人
        /// </summary>
        /// <param name="nodeInstance"></param>
        /// <returns></returns>
        public static StakeHolder BuildStakeHolder(ProcessNodeInstance nodeInstance)
        {
            StakeHolder holder = new StakeHolder();
            holder.NodeType = nodeInstance.NodeType;
            holder.BizProcessID = nodeInstance.BizProcessID;
            holder.CreatedTime = WebHelper.DateTimeNow;
            holder.ProcessID = nodeInstance.ProcessID;
            holder.ProcessInstanceID = nodeInstance.ProcessInstanceID;
            holder.cUserID = nodeInstance.cUserID == 0 ? 0 : nodeInstance.cUserID;
            holder.UserCode = string.IsNullOrEmpty(nodeInstance.UserCode) ? "" : nodeInstance.UserCode;
            holder.UserID = nodeInstance.UserID;
            holder.UserName = nodeInstance.UserName;
            //add czq
            holder.CreateTime = WebHelper.DateTimeNow;
            holder.CreatorName = WebHelper.GetCurrentUser().LoginName;
            holder.IsDeleted = false;
            holder.ModifyTime = WebHelper.DateTimeNow;
            holder.ModifierName = WebHelper.GetCurrentUser().LoginName;
            return holder;
        }
        /// <summary>
        /// 构建待办
        /// </summary>
        /// <param name="todoType"></param>
        /// <param name="process"></param>
        /// <param name="nodeInstance"></param>
        /// <returns></returns>
        public static TodoWork BuildTodo(CommonConsts.TodoType todoType, ProcessInstance process, ProcessNodeInstance nodeInstance)
        {
            TodoWork todo = new TodoWork();
            todo.NodeType = nodeInstance.NodeType;
            todo.BizProcessID = nodeInstance.BizProcessID;
            todo.CreatedTime = WebHelper.DateTimeNow;
            todo.ModifyTime = WebHelper.DateTimeNow;
            todo.InstanceName = process.InstanceName;
            todo.NodeInstanceID = nodeInstance.ID;
            todo.NodeName = nodeInstance.NodeName;
            todo.ProcessID = nodeInstance.ProcessID;
            todo.ProcessInstanceID = nodeInstance.ProcessInstanceID;
            todo.PreviousNodeInstanceID = nodeInstance.PreviousNodeInstanceID;
            todo.ProcessCode = process.ProcessCode;
            todo.Status = (int)CommonConsts.TodoStatus.Unread;
            todo.TodoType = (int)todoType;
            todo.cUserID = nodeInstance.cUserID == 0 ? 0 : nodeInstance.cUserID;
            todo.UserCode = string.IsNullOrEmpty(nodeInstance.UserCode) ? "" : nodeInstance.UserCode;
            todo.UserID = nodeInstance.UserID;
            todo.UserName = nodeInstance.UserName;


            todo.CreateProcessTime = process.CreateTime;

            todo.CreateProcessUserName = process.UserName;

            todo.CreateProcessUserID = process.UserID;
            todo.ProjectID = process.ProjectID;
            //add czq 
            todo.CreateTime = WebHelper.DateTimeNow;
            todo.CreatorName = WebHelper.GetCurrentUser().LoginName;
            todo.IsDeleted = false;
            todo.ModifyTime = WebHelper.DateTimeNow;
            todo.ModifierName = WebHelper.GetCurrentUser().LoginName;
            return todo;
        }
        /// <summary>
        /// 构建NodeInstance
        /// </summary>
        /// <param name="bizProcessID"></param>
        /// <param name="node"></param>
        /// <param name="userID"></param>
        /// <param name="cUserID"></param>
        /// <param name="userName"></param>
        /// <param name="userCode"></param>
        /// <returns></returns>
        public static ProcessNodeInstance BuildNodeInstance(int bizProcessID, ProcessNode node, int userID, string userName, string userCode)
        {
            ProcessNodeInstance nodeInstance = new ProcessNodeInstance();
            nodeInstance.ProcessID = node.ProcessID;
            nodeInstance.NodeID = node.ID;
            nodeInstance.ProcessType = node.ProcessType;
            nodeInstance.NodeSeq = node.NodeSeq;
            nodeInstance.NodeName = node.NodeName;
            nodeInstance.NodeType = node.NodeType;
            nodeInstance.Expression = node.Expression;
            nodeInstance.IsHandSign = node.IsHandSign;
            nodeInstance.RoleID = node.RoleID;
            nodeInstance.Description = node.Description;
            nodeInstance.CreatedTime = WebHelper.DateTimeNow;
            nodeInstance.LastUpdatedTime = WebHelper.DateTimeNow;
            nodeInstance.BizProcessID = bizProcessID;
            nodeInstance.UserID = userID;
            nodeInstance.UserCode = userCode;
            nodeInstance.UserName = userName;

            nodeInstance.Status = (int)CommonConsts.NodeStatus.NotExecute;
            nodeInstance.CreateTime = WebHelper.DateTimeNow;
            nodeInstance.CreatorName = WebHelper.GetCurrentUser().LoginName;
            nodeInstance.IsDeleted = false;
            nodeInstance.ModifyTime = WebHelper.DateTimeNow;
            nodeInstance.ModifierName = WebHelper.GetCurrentUser().LoginName;

            return nodeInstance;
        }
        /// <summary>
        /// 发送审核RTX消息
        /// </summary>
        /// <param name="pInstance"></param>
        public static void BuildAuditRtxMessage(WorkflowInstance wInstance)
        {
            DataExchangeProviderInfo info = LightWorkflowSettings.Instance.DataExchangeProviderInfos[wInstance.ProcessInstance.ProcessCode];
            string content = LightWorkflowSettings.Instance.WorkflowSettings["RTXAuditMessage"] == null ? "" : LightWorkflowSettings.Instance.WorkflowSettings["RTXAuditMessage"].Value;

            if (info != null && bool.Parse(info.NeedSendRTXMessage) && !string.IsNullOrEmpty(info.RTXReceiver))
            {
                string[] list = info.RTXReceiver.Split('#');
                foreach (var item in list)
                {
                    if (item.Contains(wInstance.Process.CongID.ToString()))
                    {
                        string[] temp = item.Split('|');
                        if (temp.Length > 1)
                        {
                            TSM_MessageAdapter.Instance.InsertRTXMessage(temp[1].Split(';'), content);
                        }
                    }
                }

            }
        }
        #endregion


        #region 方法重构 使用独立Filter实现查询 add czq 2013-06-17
        /// <summary>
        /// 获取流程列表
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        //public PartlyCollection<Process> GetProcess(ProcessFilter filter)
        //{
        //    return ProcessAdapter.Instance.GetProcess(filter);
        //}
        /// <summary>
        /// 根据条件查询待办项（我的待办）
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="whereString">查询条件</param>
        /// <param name="recordCount">记录条数</param>
        /// <returns></returns>
        public PartlyCollection<VTodoWorkAndUser> GetTodoWorkList(TodoWorkFilter filter)
        {
            return VTodoWorkAndUserAdapter.Instance.LoadList(filter);
        }
        /// <summary>
        /// 根据查询条件获取流程列表（我的流程）
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="whereString">查询条件</param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public PartlyCollection<WorkflowInstance> GetWorkflowInstanceListByCreateUserID(WorkFlowFilter filter)
        {
            filter.ExcludeProcessCode = GetExcludeProcessCode();
            PartlyCollection<ProcessInstance> processInstanceList = ProcessInstanceAdapter.Instance.LoadListByCreateUserID(filter);
            PartlyCollection<WorkflowInstance> result = new PartlyCollection<WorkflowInstance>();
            foreach (ProcessInstance processInstance in processInstanceList)
            {
                result.Add(new WorkflowInstance(processInstance));
            }
            return result;
        }
        /// <summary>
        /// 根据条件获取流程列表（查询流程）
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="whereString">查询条件</param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public PartlyCollection<VPInstanceAndStakeHolder> GetWorkflowInstanceList(WorkFlowFilter filter)
        {
            filter.ExcludeProcessCode = GetExcludeProcessCode();
            PartlyCollection<VPInstanceAndStakeHolder> processInstanceList = VPinstaneAndStakeHolderAdapter.Instance.LoadList(filter);
            return processInstanceList;
        }
        /// <summary>
        /// 根据条件获取流程列表（查询流程）
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="whereString">查询条件</param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public PartlyCollection<VAdminPInstanceAndStakeHolder> GetWorkflowInstanceListForAdmin(WorkFlowFilter filter)
        {
            filter.ExcludeProcessCode = GetExcludeProcessCode();
            PartlyCollection<VAdminPInstanceAndStakeHolder> processInstanceList = VAdminPinstaneAndStakeHolderAdapter.Instance.LoadList(filter);
            return processInstanceList;
        }
        /// <summary>
        /// 根据条件查询我的已办
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="dt">查询条件</param>
        /// <returns></returns>
        public PartlyCollection<VPInstanceAndApprovalLog> GetWorkflowInstanceListByRelatedUserID(WorkFlowFilter filter)
        {
            // 排除一些既定的流程不显示
            filter.ExcludeProcessCode = GetExcludeProcessCode();

            PartlyCollection<VPInstanceAndApprovalLog> VPInstanceAndApprovalLogList = VPInstanceAndApprovalLogAdapter.Instance.LoadList(filter);
            return VPInstanceAndApprovalLogList;
        }

        /// <summary>
        /// 从配置文件中读取要排除的流程
        /// </summary>
        /// <returns></returns>
        private List<string> GetExcludeProcessCode()
        {
            string PAISONG_PROCESS_CODES = AppSettingConfig.GetSetting("PaiSongProcess", "");
            if (string.IsNullOrEmpty(PAISONG_PROCESS_CODES))
            {
                return null;
            }

            return PAISONG_PROCESS_CODES.Split(",;".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
        }
        /// <summary>
        /// 根据条件查询草稿箱
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="dt">查询条件</param>
        /// <param name="recordCount">记录条数</param>
        /// <returns></returns>
        public PartlyCollection<BBizProcess> GetWorkflowDraftByRelatedUserID(WorkFlowFilter filter)
        {
            filter.ExcludeProcessCode = GetExcludeProcessCode();
            PartlyCollection<BBizProcess> processInstanceList = VBizProcessDraftAdapter.Instance.LoadList(filter);

            return processInstanceList;
        }

        public PartlyCollection<VOAMQMeaages> GetOAMQList(OAMQFilter filter)
        {
            PartlyCollection<VOAMQMeaages> OAMQMessages = VOAMQListAdapter.Instance.GetList(filter);

            return OAMQMessages;
        }
        public List<OAMQMessages> GetOAMQList(int count)
        {
            return OAMQMessagesAdapter.Instance.LoadList(count);
        }

        public void UpdateOAMQMessage(OAMQMessages message)
        {
            OAMQMessagesAdapter.Instance.Update(message);
        }

        #endregion


        public void InsertRTXMessage(string receiver, string errorMessage)
        {
            TSM_MessageAdapter.Instance.InsertRTXMessage(receiver, errorMessage);
        }


    }


}
