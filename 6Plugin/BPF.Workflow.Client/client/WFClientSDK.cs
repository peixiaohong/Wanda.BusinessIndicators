using System;
using System.Collections.Generic;
using System.Text;
using BPF.Workflow.Object;
using Newtonsoft.Json;
using System.Web.UI;
using System.Web;

namespace BPF.Workflow.Client
{
    /// <summary>
    /// 工作流SDK
    /// </summary>
    public class WFClientSDK
    {
        #region 客户端注册事件处理
        /// <summary>
        /// 客户端注册事件
        /// </summary>
        private Action<WFClientSDK> OnCreateListener = p => { };
        internal void OnListen()
        {
            if (OnCreateListener != null)
                OnCreateListener(this);
        }
        /// <summary>
        /// BeforeExecute,SaveApplicationData的委托类型
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">参数</param>
        /// <returns></returns>
        public delegate bool WorkflowEventHandler(object sender, WFExecuteArgs e);
        /// <summary>
        /// 执行前(准备参数)
        /// </summary>
        public event WorkflowEventHandler BeforeExecute = delegate { return true; };
        /// <summary>
        /// 执行中，此处进行提交业务数据
        /// </summary>
        public event WorkflowEventHandler SaveApplicationData = delegate { return true; };
        /// <summary>
        /// 执行后（进行回滚或其它操作（例如跳转））
        /// </summary>
        public event EventHandler<WFExecuteArgs> AfterExecute;
        /// <summary>
        /// 页面实例
        /// </summary>
        internal Page PageInstance { get; private set; }
        /// <summary>
        /// 流程控件显示的DomID
        /// </summary>
        internal string DomID { get; private set; }
        #endregion

        #region 公开方法
        /// <summary>
        /// 初始化设置
        /// </summary>
        /// <param name="page">页面</param>
        /// <param name="domID">流程渲染控件DomID</param>
        /// <param name="onCreateListener">注册的事件</param>
        /// <param name="otherSetting">其他设置</param>
        public static void InitSetting(Page page, string domID, Action<WFClientSDK> onCreateListener, Dictionary<string, object> otherSetting = null)
        {
            WFClientSDK listener = new WFClientSDK();
            listener.PageInstance = page;
            listener.DomID = domID;
            listener.OnCreateListener = onCreateListener;
            if (otherSetting != null && otherSetting.Count > 0)
            {
                var jsonOtherSetting = JsonConvert.SerializeObject(otherSetting);
                SDKHelper.RegisterScript(page, "init", "wanda_wf_client.initPostSetting(\"" + domID + "\"," + jsonOtherSetting + ");");
            }
            else
            {
                SDKHelper.RegisterScript(page, "init", "wanda_wf_client.initPostSetting(\"" + domID + "\");");
            }

            var str_op = HttpContext.Current.Request[AppSettingInfo.CONST_PostOperaionInfoKey];
            if (!string.IsNullOrEmpty(str_op))
            {
                listener.OnListen();
                var param = InitExecuteParameter(str_op);
                listener.Execute(listener, param);
            }
        }
        /// <summary>
        /// 创建流程返回WorkflowContext对象
        /// </summary>
        /// <param name="page">页面</param>
        /// <param name="startup">发起流程的参数</param>
        /// <returns>流程信息</returns>
        public static WorkflowContext CreateProcess(Page page, WFStartupParameter startup)
        {
            return WFClientProcess.CreateProcess(page, startup);
        }
        /// <summary>
        /// 获取流程WorkflowContext对象(加载已有流程信息)
        /// </summary>
        /// <param name="page">页面对象</param>
        /// <param name="businessID">业务ID</param>
        /// <param name="userLoginID">用户CTX账号</param>
        /// <returns>流程信息</returns>
        public static WorkflowContext GetProcess(Page page, string businessID, string userLoginID = null)
        {
            BizContext bizContext = new BizContext();
            bizContext.BusinessID = businessID;
            if (!string.IsNullOrEmpty(userLoginID))
            {
                bizContext.CurrentUser = new UserInfo() { UserLoginID = userLoginID };
            }
            return WFClientProcess.GetProcess(page, bizContext);
        }
        /// <summary>
        /// 获取流程WorkflowContext对象(加载已有流程信息)
        /// </summary>
        /// <param name="page">页面对象</param>
        /// <param name="businessID">业务ID</param>
        /// <param name="userInfo">用户信息</param>
        /// <returns>流程信息</returns>
        public static WorkflowContext GetProcess(Page page, string businessID, UserInfo userInfo)
        {
            BizContext bizContext = new BizContext();
            bizContext.BusinessID = businessID;
            if (userInfo == null)
            {
                bizContext.CurrentUser = new UserInfo() { UserLoginID = SDKHelper.GetUserName(HttpContext.Current) };
            }
            else
            {
                bizContext.CurrentUser = userInfo;
            }
            return WFClientProcess.GetProcess(page, bizContext);
        }
        /// <summary>
        /// 获取流程信息
        /// </summary>
        /// <param name="page">页面对象</param>
        /// <param name="bizContext">业务系统BizContext</param>
        /// <returns></returns>
        public static WorkflowContext GetProcess(Page page, BizContext bizContext)
        {
            return WFClientProcess.GetProcess(page, bizContext);
        }
        /// <summary>
        /// 获取流程WorkflowContext对象(根据参数刷新流程信息)
        /// </summary>
        /// <param name="page">页面对象</param>
        /// <param name="businessID">业务ID</param>
        /// <param name="formParam">流程参数</param>
        /// <param name="DynamicRoleUserList">动态角色</param>
        /// <param name="userLoginID">用户UserLoginID（为空或null则从SSO获取）</param>
        /// <returns>刷新后的流程信息</returns>
        public static WorkflowContext GetProcessForceRefresh(Page page, string businessID, Dictionary<string, object> formParam, Dictionary<string, List<UserInfo>> DynamicRoleUserList, string userLoginID = null)
        {
            BizContext bizContext = new BizContext();
            bizContext.BusinessID = businessID;
            bizContext.FormParams = formParam;
            bizContext.DynamicRoleUserList = DynamicRoleUserList;
            if (!string.IsNullOrEmpty(userLoginID))
            {
                bizContext.CurrentUser = new UserInfo() { UserLoginID = userLoginID };
            }
            bizContext.ExtensionCommond = new Dictionary<string, string>();
            bizContext.ExtensionCommond.Add(AppSettingInfo.CONST_ExtensionCommond_GetProcessForceRefresh, bool.TrueString);
            return WFClientProcess.GetProcess(page, bizContext);
        }
        /// <summary>
        /// 获取审批记录
        /// </summary>
        /// <param name="businessID">业务ID</param>
        /// <returns>按照倒序排列的审批记录</returns>
        public static List<ProcessLog> GetProcessLogList(string businessID)
        {
            return WFClientProcess.GetProcessLogList(businessID);
        }
        /// <summary>
        /// 检查是否发起过流程
        /// </summary>
        /// <param name="businessID">业务ID</param>
        /// <returns>true，表示已发起过流程。false表示未发起过流程</returns>
        public static bool Exist(string businessID)
        {
            return WFClientProcess.Exist(businessID);
        }
        /// <summary>
        /// 执行流程动作
        /// </summary>
        /// <param name="methodName">操作命令</param>
        /// <param name="bizContext">业务系统上下文</param>
        /// <returns>执行流程后返回的流程信息</returns>
        public static WorkflowContext ExecuteMethod(string methodName, BizContext bizContext)
        {
            if (string.IsNullOrEmpty(methodName))
            {
                throw new Exception("操作命令不能为空");
            }
            string result = WFClientProcess.ExecuteMethod(methodName, 0, AppSettingInfo.CONST_WorkflowMethodVersion, bizContext);
            return JsonConvert.DeserializeObject<WorkflowContext>(result);
        }
        /// <summary>
        /// 执行流程动作（分步执行）
        /// </summary>
        /// <param name="methodName">操作命令</param>
        /// <param name="methodMode">方法类型（1：第一步提交，2：第二步提交）</param>
        /// <param name="bizContext">业务系统上下文</param>
        /// <returns></returns>
        public static WorkflowContext ExecuteMethod(string methodName, int methodMode, BizContext bizContext)
        {
            if (string.IsNullOrEmpty(methodName))
            {
                throw new Exception("操作命令不能为空");
            }
            string result = WFClientProcess.ExecuteMethod(methodName, methodMode, AppSettingInfo.CONST_WorkflowMethodVersion, bizContext);
            return JsonConvert.DeserializeObject<WorkflowContext>(result);
        }
        #endregion

        #region 私有/内部方法
        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="host"></param>
        /// <param name="param"></param>
        internal void Execute(WFClientSDK host, WFExecuteParameter param)
        {
            string workflowContextPost = JsonConvert.SerializeObject(param.WorkflowContext);
            //TODO 执行流程操作
            WFExecuteArgs args = new WFExecuteArgs(param);

            if (args.ExecuteParameter.OperatorType == 7)
            {
                //如果执行的是撤回操作，则直接执行工作流方法，然后调用After。
                ExecuteMethodOnlyCallAfter(host, param);
                return;
            }

            bool result = true;
            if (BeforeExecute != null)
            {
                result = BeforeExecute(this, args);
            }
            if (result == false)
            {
                //如果Before出错则把POST回来的WorkflowContext再返回回去
                SDKHelper.ShowProcess(host.PageInstance, workflowContextPost);
                return;
            }
            string workflowContext = WFClientProcess.ExecuteMethod(args.ExecuteParameter, 1);
            WorkflowContext workflowContextOne = JsonConvert.DeserializeObject<WorkflowContext>(workflowContext);
            ProcessReturn(args, workflowContextOne);
            if (args.ExecuteParameter.WorkflowContext.StatusCode != 0)
            {
                SDKHelper.ShowProcess(host.PageInstance, JsonConvert.SerializeObject(args.ExecuteParameter.WorkflowContext));
                return;
            }
            if (SaveApplicationData != null)
            {
                result = SaveApplicationData(this, args);
            }
            if (result)
            {
                workflowContext = WFClientProcess.ExecuteMethod(args.ExecuteParameter, 2);
                args.ExecuteParameter.WorkflowContext = JsonConvert.DeserializeObject<WorkflowContext>(workflowContext);
                if (AfterExecute != null)
                {
                    AfterExecute(this, args);
                }
                SDKHelper.ShowProcess(host.PageInstance, workflowContext);
            }
            else
            {
                SDKHelper.ShowProcess(host.PageInstance, workflowContextPost);
            }
        }
        /// <summary>
        /// 执行工作流的方法（只调用After）
        /// </summary>
        /// <param name="host"></param>
        /// <param name="param"></param>
        private void ExecuteMethodOnlyCallAfter(WFClientSDK host, WFExecuteParameter param)
        {
            string workflowContextPost = JsonConvert.SerializeObject(param.WorkflowContext);
            //TODO 执行流程操作
            WFExecuteArgs args = new WFExecuteArgs(param);
            string workflowContext = WFClientProcess.ExecuteMethod(args.ExecuteParameter, 0);
            args.ExecuteParameter.WorkflowContext = JsonConvert.DeserializeObject<WorkflowContext>(workflowContext);
            if (AfterExecute != null)
            {
                AfterExecute(this, args);
            }
            SDKHelper.ShowProcess(host.PageInstance, workflowContext);
        }

        private void ProcessReturn(WFExecuteArgs args, WorkflowContext resultWorkflowContext)
        {
            if (resultWorkflowContext.StatusCode == 0 || resultWorkflowContext.StatusCode == 11 || resultWorkflowContext.StatusCode == 21)
            {
                //如果11和21的错误和成功，直接返回
                args.ExecuteParameter.WorkflowContext = resultWorkflowContext;
            }
            else
            {
                //其它错误，把错误代码赋给POST回来的WorkflowContext。然后返回
                args.ExecuteParameter.WorkflowContext.StatusCode = resultWorkflowContext.StatusCode;
                args.ExecuteParameter.WorkflowContext.StatusMessage = resultWorkflowContext.StatusMessage;
                args.ExecuteParameter.WorkflowContext.LastException = resultWorkflowContext.LastException;
            }
        }

        /// <summary>
        /// 反序列化前端POST的BizContext的Json串
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        private static WFExecuteParameter InitExecuteParameter(string json)
        {
            return JsonConvert.DeserializeObject<WFExecuteParameter>(json);
        }
        #endregion
    }
}
