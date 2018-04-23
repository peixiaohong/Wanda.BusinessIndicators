using System;
using System.Collections.Generic;
using System.Text;
using BPF.Workflow.Object;

namespace BPF.Workflow.Client
{
    /// <summary>
    /// 流程执行的参数
    /// </summary>
    public class WFExecuteParameter
    {
        /// <summary>
        /// 设置流程参数
        /// </summary>
        /// <param name="formParams"></param>
        public void SetFormParam(Dictionary<string, object> formParams)
        {
            this.BizContext.FormParams = formParams;
        }
        /// <summary>
        /// 设置动态角色
        /// </summary>
        /// <param name="dynamicRole">动态角色信息</param>
        public void SetDynamicRoleUserList(Dictionary<string, List<UserInfo>> dynamicRole)
        {
            this.BizContext.DynamicRoleUserList = dynamicRole;
        }
        /// <summary>
        /// 设置流程发起或保存时的参数
        /// </summary>
        /// <param name="processTitle">流程标题</param>
        /// <param name="processURL">流程的URL</param>
        /// <param name="processMobileURL">流程手机审批URL</param>
        /// <param name="businessID">业务ID</param>
        public void SetStartOrSaveParam(string processTitle, string processURL, string processMobileURL, string businessID)
        {
            if (!string.IsNullOrEmpty(processTitle))
            {
                this.BizContext.ProcessTitle = processTitle;
            }
            if (!string.IsNullOrEmpty(processURL))
            {
                this.BizContext.ProcessURL = processURL;
            }
            if (!string.IsNullOrEmpty(processMobileURL))
            {
                this.BizContext.ProcessMobileURL = processMobileURL;
            }
            if (!string.IsNullOrEmpty(businessID))
            {
                this.BizContext.BusinessID = businessID;
            }
        }

        /// <summary>
        /// 操作类型
        /// </summary>
        public int OperatorType { get; set; }
        /// <summary>
        /// 操作方法的名称
        /// </summary>
        public string MethodName { get; set; }
        /// <summary>
        /// 版本
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// 流程执行需要的业务上下文参数
        /// </summary>
        public BizContext BizContext { get; set; }
        /// <summary>
        /// 流程信息
        /// </summary>
        public WorkflowContext WorkflowContext { get; set; }
    }

    /// <summary>
    /// 发起流程时需要的参数
    /// </summary>
    public class WFStartupParameter
    {
        /// <summary>
        /// 业务ID
        /// </summary>
        public string BusinessID
        {
            get { return _BusinessID; }
            set { _BusinessID = value; }
        }private string _BusinessID = String.Empty;
        /// <summary>
        /// 流程Code
        /// </summary>
        public string FlowCode
        {
            get { return _FlowCode; }
            set { _FlowCode = value; }
        }private string _FlowCode = String.Empty;

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
        /// 当前用户信息
        /// 用户信息默认从SSO中获取，此处提供模拟用户的接口
        /// 内部管理模块调用时使用
        /// </summary>
        public UserInfo CurrentUser
        {
            get { return _CurrentUser; }
            set { _CurrentUser = value; }
        }private UserInfo _CurrentUser = null;
    }
}
