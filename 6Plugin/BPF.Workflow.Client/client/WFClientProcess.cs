using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using BPF.Workflow.Object;
using Newtonsoft.Json;
using System.Web.UI;

namespace BPF.Workflow.Client
{
    /// <summary>
    /// 流程操作的SDK
    /// </summary>
    internal class WFClientProcess
    {
        /// <summary>
        /// 创建流程返回WorkflowContext对象
        /// </summary>
        /// <param name="page">页面对象</param>
        /// <param name="startup">流程发起参数</param>
        /// <returns></returns>
        internal static WorkflowContext CreateProcess(Page page, WFStartupParameter startup)
        {
            var bizContext = ConvertToBizContext(startup);
            string result = ExecuteMethod(AppSettingInfo.CONST_ExecuteMethod_CreateProcess, AppSettingInfo.CONST_ExecuteMode_Zero, AppSettingInfo.CONST_WorkflowMethodVersion, bizContext);
            SDKHelper.ShowProcess(page, result);
            return JsonConvert.DeserializeObject<WorkflowContext>(result);
        }

        /// <summary>
        /// 获取流程信息
        /// </summary>
        /// <param name="page">页面对象</param>
        /// <param name="bizContext">业务系统BizContext</param>
        /// <returns></returns>
        internal static WorkflowContext GetProcess(Page page, BizContext bizContext)
        {
            string result = ExecuteMethod(AppSettingInfo.CONST_ExecuteMethod_GetProcess, AppSettingInfo.CONST_ExecuteMode_Zero, AppSettingInfo.CONST_WorkflowMethodVersion, bizContext);
            SDKHelper.ShowProcess(page, result);
            return JsonConvert.DeserializeObject<WorkflowContext>(result);
        }
        /// <summary>
        /// 查询审批日志
        /// </summary>
        /// <param name="businessID">业务ID</param>
        /// <returns></returns>
        internal static List<ProcessLog> GetProcessLogList(string businessID)
        {
            //将appCode，action，method，param（序列化后的dicParam）添加到dicParamWebService中调用WebService
            var dicParamWebService = SDKHelper.BuildParamWebService(AppSettingInfo.CONST_Action_ProcessOperator, AppSettingInfo.CONST_OtherMethod_QueryProcessLog, Guid.NewGuid().ToString(), Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                BusinessID = businessID,
            }));
            return PostWebServiceGetResult<List<ProcessLog>>(dicParamWebService);
        }
        /// <summary>
        /// 查询待办/已办
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <param name="methodName">方法名称</param>
        /// <returns></returns>
        internal static WFTaskQueryResult QureyTask(WFTaskQueryFilter condition, string methodName)
        {
            var dicParamWebService = SDKHelper.BuildParamWebService(AppSettingInfo.CONST_Action_ProcessOperator, methodName, Guid.NewGuid().ToString(), Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                Condition = condition,
            }));
            return PostWebServiceGetResult<WFTaskQueryResult>(dicParamWebService);
        }

        /// <summary>
        /// 根据业务ID查询待办数据
        /// </summary>
        /// <param name="businessID"></param>
        /// <returns></returns>
        public static WFTaskQueryResult QueryToDoByBusinessID(string businessID)
        {
            var dicParamWebService = SDKHelper.BuildParamWebService(AppSettingInfo.CONST_Action_ProcessOperator, AppSettingInfo.CONST_OtherMethod_QueryToDoByBusinessID, Guid.NewGuid().ToString(), Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                BusinessID = businessID,
            }));
            return PostWebServiceGetResult<WFTaskQueryResult>(dicParamWebService);
        }
        /// <summary>
        /// 根据业务ID查询已办数据
        /// </summary>
        /// <param name="businessID"></param>
        /// <returns></returns>
        public static WFTaskQueryResult QueryDoneByBusinessID(string businessID)
        {
            var dicParamWebService = SDKHelper.BuildParamWebService(AppSettingInfo.CONST_Action_ProcessOperator, AppSettingInfo.CONST_OtherMethod_QueryDoneByBusinessID, Guid.NewGuid().ToString(), Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                BusinessID = businessID,
            }));
            return PostWebServiceGetResult<WFTaskQueryResult>(dicParamWebService);
        }

        /// <summary>
        /// 查询已办（同一流程同一个人只会查询到一条最新的已办信息）
        /// </summary>
        /// <param name="condition">查询条件（PageSize和PageIndex同时为0表示查询所有）</param>
        /// <returns>已办查询结果(同一流程同一个人只会查询到一条最新的已办信息)</returns>
        public static WFTaskQueryResult QueryDoneDistinct(WFTaskQueryFilter condition)
        {
            var dicParamWebService = SDKHelper.BuildParamWebService(AppSettingInfo.CONST_Action_ProcessOperator, AppSettingInfo.CONST_OtherMethod_QueryDoneDistinct, Guid.NewGuid().ToString(), Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                Condition = condition,
            }));
            return PostWebServiceGetResult<WFTaskQueryResult>(dicParamWebService);
        }
        /// <summary>
        /// 根据业务ID查询已办数据（同一流程同一个人只会查询到一条最新的已办信息）
        /// </summary>
        /// <param name="businessID">业务ID</param>
        /// <returns>已办查询结果(同一流程同一个人只会查询到一条最新的已办信息)</returns>
        public static WFTaskQueryResult QueryDoneDistinctByBusinessID(string businessID)
        {
            var dicParamWebService = SDKHelper.BuildParamWebService(AppSettingInfo.CONST_Action_ProcessOperator, AppSettingInfo.CONST_OtherMethod_QueryDoneDistinctByBusinessID, Guid.NewGuid().ToString(), Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                BusinessID = businessID,
            }));
            return PostWebServiceGetResult<WFTaskQueryResult>(dicParamWebService);
        }

        /// <summary>
        /// 请求WebService返回结果
        /// </summary>
        /// <typeparam name="T">返回的结果类型</typeparam>
        /// <param name="dicParamWebService">WebService返回结果</param>
        /// <returns></returns>
        private static T PostWebServiceGetResult<T>(Dictionary<string, object> dicParamWebService)
        {
            string result = SDKHelper.QueryPostWebService(dicParamWebService);
            ResultBaseContext baseContext = JsonConvert.DeserializeObject<ResultBaseContext>(result);
            if (baseContext.StatusCode == ClientConstDefine.WORKFLOW_SERVICE_ERRORCODE_NOERROR)
            {
                return JsonConvert.DeserializeObject<T>((string)baseContext.Data);
            }
            else
            {
                throw new Exception(baseContext.StatusMessage, baseContext.LastException);
            }
        }

        /// <summary>
        /// 检查是否发起过流程
        /// </summary>
        /// <param name="businessID">业务ID</param>
        /// <returns></returns>
        internal static bool Exist(string businessID)
        {
            var dicParamWebService = SDKHelper.BuildParamWebService(AppSettingInfo.CONST_Action_ProcessOperator, AppSettingInfo.CONST_OtherMethod_ExistProcess, Guid.NewGuid().ToString(), Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                BusinessID = businessID,
            }));
            string result = SDKHelper.QueryPostWebService(dicParamWebService);
            ResultBaseContext baseContext = JsonConvert.DeserializeObject<ResultBaseContext>(result);
            if (baseContext.StatusCode == ClientConstDefine.WORKFLOW_SERVICE_ERRORCODE_NOERROR)
            {
                return (string)baseContext.Data == Boolean.TrueString;
            }
            else
            {
                throw new Exception(baseContext.StatusMessage, baseContext.LastException);
            }
        }

        #region 内部方法
        /// <summary>
        /// 将发起参数WFStartupParameter转换为BizContext
        /// </summary>
        /// <param name="startup"></param>
        /// <returns>BizContext</returns>
        private static BizContext ConvertToBizContext(WFStartupParameter startup)
        {
            BizContext bizContext = new BizContext();
            bizContext.FormParams = startup.FormParams;
            bizContext.DynamicRoleUserList = startup.DynamicRoleUserList;
            bizContext.FlowCode = startup.FlowCode;
            bizContext.ProcessMobileURL = startup.ProcessMobileURL;
            bizContext.ProcessTitle = startup.ProcessTitle;
            bizContext.ProcessURL = startup.ProcessURL;
            bizContext.CurrentUser = startup.CurrentUser;
            return bizContext;
        }
        /// <summary>
        /// 执行流程方法
        /// </summary>
        /// <param name="param"></param>
        /// <param name="methodMode"></param>
        /// <returns></returns>
        internal static string ExecuteMethod(WFExecuteParameter param, int methodMode)
        {
            return ExecuteMethod(param.MethodName, methodMode, param.Version, param.BizContext);
        }

        /// <summary>
        /// 执行流程方法
        /// </summary>
        /// <param name="methodName">方法名称</param>
        /// <param name="methodMode">方法类型（0：直接提交，1：第一步提交，2：第二步提交）</param>
        /// <param name="methodVersion">方法版本</param>
        /// <param name="bizContext">流程上下文参数</param>
        /// <returns></returns>
        internal static string ExecuteMethod(string methodName, int methodMode, string methodVersion, BizContext bizContext, string bizWFServiceURL = "")
        {
            //补充BizContext的信息（CurrentUser，AppCode）
            WFClientProcess.RepaireBizContext(bizContext);

            //将appCode，action，method，param（序列化后的dicParam）添加到dicParamWebService中调用WebService
            var dicParamWebService = new Dictionary<string, object>();
            dicParamWebService.Add("appCode", bizContext.AppCode);
            dicParamWebService.Add("action", "Process");
            dicParamWebService.Add("method", methodName);
            dicParamWebService.Add("token", bizContext.WFToken);
            dicParamWebService.Add("param", Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                BizContext = JsonConvert.SerializeObject(bizContext),
                Mode = methodMode,
                Version = methodVersion
            }));
            dicParamWebService.Add("callBackUrl", "");
            string url = AppSettingInfo.WorkflowServerUrl;
            if (!string.IsNullOrEmpty(bizWFServiceURL))
            {
                url = bizWFServiceURL;
            }
            string workFlowServerFullURL = SDKHelper.GetWorkflowServerUrlFullPath(url);
            string result = SDKHelper.QueryPostWebService(workFlowServerFullURL, AppSettingInfo.CONST_WorkflowServiceMethodName, dicParamWebService);
            return result;

        }

        /// <summary>
        /// 完善BizContext信息
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        internal static BizContext RepaireBizContext(BizContext context)
        {
            if (string.IsNullOrEmpty(context.AppCode))
            {
                context.AppCode = AppSettingInfo.ApplicationCode;
            }
            if (context.BusinessID == null)
            {
                context.BusinessID = "";
            }
            if (context.CurrentUser == null)
            {
                context.CurrentUser = new UserInfo() { UserLoginID = SDKHelper.GetUserName(HttpContext.Current) };
            }
            if (string.IsNullOrEmpty(context.WFToken))
            {
                context.WFToken = Guid.NewGuid().ToString();
            }
            return context;
        }
        #endregion
    }
}
