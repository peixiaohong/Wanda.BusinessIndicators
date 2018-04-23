using System;
using System.Collections.Generic;
using System.Text;

namespace BPF.Workflow.Object
{
    /// <summary>
    /// 工作流Context
    /// </summary>
    public class WorkflowContext : BaseServeContext
    {
        /// <summary>
        /// 应用Code
        /// </summary>
        public string AppCode
        {
            get { return _AppCode; }
            set { _AppCode = value; }
        }private string _AppCode = String.Empty;
        /// <summary>
        /// 应用ID(SchemaID)
        /// </summary>
        public string AppID
        {
            get { return _AppID; }
            set { _AppID = value; }
        }private string _AppID = String.Empty;
        /// <summary>
        /// 业务单据编号
        /// </summary>
        public string BusinessID
        {
            get { return _BusinessID; }
            set { _BusinessID = value; }
        }private string _BusinessID = string.Empty;
        /// <summary>
        /// 当前用户ID
        /// </summary>
        public UserInfo CurrentUser
        {
            get { return _CurrentUser; }
            set { _CurrentUser = value; }
        }private UserInfo _CurrentUser = null;
        /// <summary>
        /// 工作流回话ID
        /// 每次回话时客户端sdk创建
        /// </summary>
        public string WFToken
        {
            get { return _WFToken; }
            set { _WFToken = value; }
        }private string _WFToken = string.Empty;
        /// <summary>
        /// 流程实例
        /// </summary>
        public Process ProcessInstance
        {
            get { return _ProcessInstance; }
            set { _ProcessInstance = value; }
        }private Process _ProcessInstance = null;
        /// <summary>
        /// 审批流程的节点实例列表
        /// 使用NodeID索引
        /// </summary>
        public Dictionary<string, Node> NodeInstanceList
        {
            get { return _NodeInstanceList; }
            set { _NodeInstanceList = value; }
        }private Dictionary<string, Node> _NodeInstanceList = null;
        /// <summary>
        /// 抄送节点实例列表
        /// 使用NodeID索引
        /// </summary>
        public Dictionary<string, Node> CcNodeInstanceList
        {
            get { return _CcNodeInstanceList; }
            set { _CcNodeInstanceList = value; }
        }private Dictionary<string, Node> _CcNodeInstanceList = null;
        /// <summary>
        /// 审批日志信息列表
        /// 默认按照创建日期倒序
        /// </summary>
        public List<ProcessLog> ProcessLogList
        {
            get { return _ProcessLogList; }
            set { _ProcessLogList = value; }
        }private List<ProcessLog> _ProcessLogList = null;
        /// <summary>
        /// 当前节点ID，与当前用户关联显示用户特定信息
        /// 如果为空，表示当前用户无节点信息
        /// 注意：该节点并非流程当前执行到的节点（Process.RunningNodeID）
        /// </summary>
        public string CurrentUserNodeID
        {
            get { return _CurrentUserNodeID; }
            set { _CurrentUserNodeID = value; }
        }private string _CurrentUserNodeID = string.Empty;
        /// <summary>
        /// 当前用户的场景设置
        /// </summary>
        public SceneSetting CurrentUserSceneSetting
        {
            get { return _CurrentUserSceneSetting; }
            set { _CurrentUserSceneSetting = value; }
        }private SceneSetting _CurrentUserSceneSetting = null;
        /// <summary>
        /// 当前用户有待办
        /// </summary>
        public bool CurrentUserHasTodoTask
        {
            get { return _CurrentUserHasTodoTask; }
            set { _CurrentUserHasTodoTask = value; }
        }private bool _CurrentUserHasTodoTask = false;
        /// <summary>
        /// 当前用户的待办是否已经阅读过
        /// 只有CurrentUserHasTodoTask=true是才有意义
        /// </summary>
        public bool CurrentUserTodoTaskIsRead
        {
            get { return _CurrentUserTodoTaskIsRead; }
            set { _CurrentUserTodoTaskIsRead = value; }
        }private bool _CurrentUserTodoTaskIsRead = false;
        /// <summary>
        /// 当前用户所有执行过的流程定义的节点扩展属性，使用NodeID索引
        /// 供业务系统自行判断使用
        /// </summary>
        public Dictionary<string, List<ActivityProperty>> CurrentUserActivityPropertiesList
        {
            get { return _ActivityPropertiesList; }
            set { _ActivityPropertiesList = value; }
        }private Dictionary<string, List<ActivityProperty>> _ActivityPropertiesList = null;

        /// <summary>
        /// 扩展信息
        /// key：信息名称
        /// value：信息内容
        /// </summary>
        public Dictionary<string, string> ExtensionInfos
        {
            get { return _ExtensionInfos; }
            set { _ExtensionInfos = value; }
        }private Dictionary<string, string> _ExtensionInfos = null;
    }
}
