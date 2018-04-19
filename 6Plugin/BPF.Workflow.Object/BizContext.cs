using System;
using System.Collections.Generic;
using System.Text;
namespace BPF.Workflow.Object
{
    /// <summary>
    /// 业务系统Context
    /// </summary>
    public class BizContext
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
        /// 流程Code
        /// </summary>
        public string FlowCode
        {
            get { return _FlowCode; }
            set { _FlowCode = value; }
        }private string _FlowCode = String.Empty;
        /// <summary>
        /// 业务单据编号
        /// </summary>
        public string BusinessID
        {
            get { return _BusinessID; }
            set { _BusinessID = value; }
        }private string _BusinessID = string.Empty;
        
        /// <summary>
        /// 工作流会话ID
        /// 每次回话时客户端sdk创建
        /// </summary>
        public string WFToken
        {
            get { return _WFToken; }
            set { _WFToken = value; }
        }private string _WFToken = string.Empty;
        /// <summary>
        /// 业务系统表单参数
        /// 用户可以使用表单object或者Hashtable、Dictionary<string, object>定义参数
        /// 不建议使用原接口的Dictionary<string, string>对象（表达式解析器无法识别对象类型）
        /// </summary>
        public Dictionary<string, object> FormParams
        {
            get { return _FormParams; }
            set { _FormParams = value; }
        }private Dictionary<string, object> _FormParams = null;
        /// <summary>
        /// 动态角色指定的用户列表
        /// 每个动态角色指定一组用户，默认使用UserInfo中的UserCode查询用户，如果UserCode为空，使用UserLoginID，其他属性不支持。
        /// </summary>
        public Dictionary<string, List<UserInfo>> DynamicRoleUserList
        {
            get { return _DynamicRoleUserList; }
            set { _DynamicRoleUserList = value; }
        }private Dictionary<string, List<UserInfo>> _DynamicRoleUserList = null;
        /// <summary>
        /// 当前用户信息
        /// 用户信息默认从SSO中获取，此处提供模拟用户的接口
        /// 内部管理模块调用时使用
        /// </summary>
        public UserInfo CurrentUser
        {
            get { return _CurrentUser; }
            set { _CurrentUser = value; }
        }private UserInfo _CurrentUser = null;

        
        /// <summary>
        /// 是否检查用户是否在流程中（默认为true），由业务系统决定是否要强制校验用户有无流程权限
        /// true：CurrentUser如果不在流程用户中，查询流程时，返回错误
        /// false：CurrentUser无论在不在流程中，都可以返回流程信息
        /// </summary>
        public bool CheckUserInProcess
        {
            get { return _JudgeUserInProcess; }
            set { _JudgeUserInProcess = value; }
        }private bool _JudgeUserInProcess = true;
        /// <summary>
        /// 流程当前运行节点的ID
        /// 必须返回，用作服务器端重复提交的检测和用户占用的冲突检测
        /// </summary>
        public string ProcessRunningNodeID
        {
            get { return _ProcessRunningNodeID; }
            set { _ProcessRunningNodeID = value; }
        }private string _ProcessRunningNodeID = string.Empty;
        /// <summary>
        /// 流程标题
        /// </summary>		
        public string ProcessTitle
        {
            get
            {
                return _ProcessTitle;
            }
            set
            {
                _ProcessTitle = value;
            }
        }private string _ProcessTitle = string.Empty;

        /// <summary>
        /// 流程的URL
        /// </summary>		
        public string ProcessURL
        {
            get
            {
                return _ProcessURL;
            }
            set
            {
                _ProcessURL = value;
            }
        }private string _ProcessURL = string.Empty;

        /// <summary>
        /// 流程的移动端URL
        /// </summary>		
        public string ProcessMobileURL
        {
            get
            {
                return _ProcessMobileURL;
            }
            set
            {
                _ProcessMobileURL = value;
            }
        }private string _ProcessMobileURL = string.Empty;
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
        /// 用户录入的审批内容
        /// </summary>
        public string ApprovalContent
        {
            get { return _ApprovalContent; }
            set { _ApprovalContent = value; }
        }private string _ApprovalContent = string.Empty;
        /// <summary>
        /// 扩展命令
        /// key：命令名称
        /// value：命令参数
        /// </summary>
        public Dictionary<string, string> ExtensionCommond
        {
            get { return _ExtensionCommond; }
            set { _ExtensionCommond = value; }
        }private Dictionary<string, string> _ExtensionCommond = null;
    }
}
