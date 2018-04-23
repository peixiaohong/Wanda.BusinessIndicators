using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using System.Configuration;
using BPF.Workflow.Object;
using Newtonsoft.Json;
using System.Web.UI;

////缺少编译器要求的成员“System.Runtime.CompilerServices.ExtensionAttribute..ctor”
//namespace System.Runtime.CompilerServices
//{
//    /// <summary>
//    /// 解决Newtonsoft2.0报缺少编译器要求的成员“System.Runtime.CompilerServices.ExtensionAttribute..ctor”
//    /// </summary>
//    internal class ExtensionAttribute : Attribute { }
//}
namespace BPF.Workflow.Client
{
    /// <summary>
    /// BPF.Platform.WorkFlow.ClientComponent辅助工具类
    /// </summary>
    public static class SDKHelper
    {
        private const string virtualUserKey = AppSettingInfo.SETTING_VirtualUser;

        /// <summary>
        /// 获取CTX账号
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetUserName(HttpContext context)
        {
            if (context != null)
            {
                if (context.User != null)
                    if (context.User.Identity != null)
                        if (!string.IsNullOrEmpty(context.User.Identity.Name))
                            return context.User.Identity.Name;
            }
            return System.Configuration.ConfigurationManager.AppSettings[virtualUserKey];
        }

        internal static DateTime? ParseDate(string jsonTick)
        {
            if (string.IsNullOrEmpty(jsonTick))
            {
                DateTime d;
                if (DateTime.TryParse(jsonTick, out d))
                {
                    d = d.AddHours(8);
                    return d;
                }
            }
            return null;
        }
        /// <summary>
        /// 转换为Int
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        internal static int ToInt(string value, int defaultValue = 0)
        {
            int i = 0;
            if (int.TryParse(value, out i))
            {
                return i;
            }
            else
            {
                return defaultValue;
            }
        }

        internal static DateTime? ParseDate(IDictionary<string, System.Object> dic, string key)
        {
            if (dic.ContainsKey(key))
            {
                if (dic[key] != null)
                    return ParseDate(dic[key].ToString());
            }
            return null;
        }

        public static void RegisterJsonInfoToPage(WorkflowContext workflowContext, System.Web.UI.Page page)
        {
            if (page != null)
            {
                string processInfo = JsonConvert.SerializeObject(workflowContext);
                string script = "var " + AppSettingInfo.CONST_WorkflowContextJsonVarName + "=" + processInfo + ";";
                page.ClientScript.RegisterClientScriptBlock(page.GetType(), string.Empty, script, true);
            }
        }
        /// <summary>
        /// 构建WebService参数（callBackUrl为空）
        /// </summary>
        /// <param name="action"></param>
        /// <param name="methodName"></param>
        /// <param name="param"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Dictionary<string, object> BuildParamWebService(string action, string methodName, string token, string param, string callBackUrl = "")
        {
            var dicParamWebService = new Dictionary<string, object>();
            dicParamWebService.Add("appCode", AppSettingInfo.ApplicationCode);
            dicParamWebService.Add("action", action);
            dicParamWebService.Add("method", methodName);
            dicParamWebService.Add("token", token);
            dicParamWebService.Add("param", param);
            dicParamWebService.Add("callBackUrl", callBackUrl);
            return dicParamWebService;
        }
        /// <summary>
        /// 调用WebService请求（调用地址为Web.Config中配置的地址）
        /// </summary>
        /// <param name="Pars"></param>
        /// <returns></returns>
        public static string QueryPostWebService(Dictionary<string, object> Pars)
        {
            string workFlowServerFullURL = SDKHelper.GetWorkflowServerUrlFullPath(AppSettingInfo.WorkflowServerUrl);
            return QueryPostWebService(workFlowServerFullURL, AppSettingInfo.CONST_WorkflowServiceMethodName, Pars);
        }

        /// <summary>
        /// 需要WebService支持Post调用
        /// </summary>
        public static string QueryPostWebService(String URL, String MethodName, Dictionary<string, object> Pars)
        {
            HttpWebRequest request = null;
            string responseData = string.Empty;
            try
            {
                URL = URL.EndsWith("/") ? URL : URL + "/";
                request = (HttpWebRequest)HttpWebRequest.Create(URL + "/" + MethodName);
                request.Method = "POST";
                request.ProtocolVersion = HttpVersion.Version10;
                request.KeepAlive = false;
                request.ServicePoint.ConnectionLimit = 65500;
                request.ContentType = "application/x-www-form-urlencoded";
                request.Credentials = CredentialCache.DefaultCredentials;
                ServicePointManager.DefaultConnectionLimit = 512;
                request.Timeout = 300000;
                //request.ServicePoint.Expect100Continue = false;
                byte[] data = Encoding.UTF8.GetBytes(ParsToString(Pars));
                request.ContentLength = data.Length;
                request.Proxy = null;
                using (Stream writer = request.GetRequestStream())
                {
                    writer.Write(data, 0, data.Length);
                    writer.Close();
                }
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        String retXml = sr.ReadToEnd();
                        sr.Close();
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(retXml);
                        responseData = doc.LastChild.InnerText;
                    }
                    response.Close();
                }
                return responseData;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 将Hashtable转换成WEB请求键值对字符串
        /// </summary>
        /// <param name="Pars"></param>
        /// <returns></returns>
        internal static String ParsToString(Dictionary<string, object> Pars)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string k in Pars.Keys)
            {
                if (sb.Length > 0)
                {
                    sb.Append("&");
                }
                sb.Append(HttpUtility.UrlEncode(k) + "=" + HttpUtility.UrlEncode(Pars[k].ToString()));
            }
            return sb.ToString();
        }
        /// <summary>
        /// 重定向到当前代办页
        /// </summary>
        /// <param name="process"></param>
        public static void Redirect(WorkflowContext context)
        {
            if (HttpContext.Current != null)
            {
                if (context.StatusCode == 0)
                {
                    HttpContext.Current.Response.Redirect(context.ProcessInstance.ProcessURL + "?BusinessID=" + context.BusinessID, false);
                }
                else
                {
                    throw context.LastException;
                }
            }
        }

        /// <summary>
        /// 显示流程
        /// </summary>
        /// <param name="page"></param>
        /// <param name="workflowContext"></param>
        public static void ShowProcess(Page page, string workflowContext)
        {
            SDKHelper.RegisterScript(page, AppSettingInfo.CONST_WorkflowContextJsonVarName, SDKHelper.FormateVarScript(AppSettingInfo.CONST_WorkflowContextJsonVarName, workflowContext));
            SDKHelper.RegisterScript(page, "showProcess", "wanda_wf_client.showProcess();");
        }
        /// <summary>
        /// 格式化变量信息的脚本
        /// </summary>
        /// <param name="varName"></param>
        /// <param name="varValue"></param>
        /// <returns></returns>
        internal static string FormateVarScript(string varName, string varValue)
        {
            string script = "var " + varName + "=" + varValue + ";";
            return script;
        }
        /// <summary>
        /// 注册脚本信息
        /// </summary>
        /// <param name="processInfo"></param>
        internal static void RegisterScript(Page page, string key, string script)
        {
            if (page != null)
            {
                page.ClientScript.RegisterStartupScript(page.GetType(), key, script, true);
            }
        }
        /// <summary>
        /// 获取请求的URL（Workflow.asmx）全地址
        /// </summary>
        /// <param name="WorkflowServerUrl"></param>
        /// <returns></returns>
        public static string GetWorkflowServerUrlFullPath(string WorkflowServerUrl)
        {
            WorkflowServerUrl = WorkflowServerUrl.EndsWith("/") ? WorkflowServerUrl : WorkflowServerUrl + "/";
            return WorkflowServerUrl + "RuntimeService/Workflow.asmx";
        }
    }
    /// <summary>
    /// 配置信息
    /// </summary>
    public class AppSettingInfo
    {
        /// <summary>
        /// WorkflowServerUrl地址
        /// </summary>
        public static readonly string WorkflowServerUrl = ConfigurationManager.AppSettings["BPF.Workflow.WorkflowServerUrl"];
        /// <summary>
        /// 应用Code（SchemaCode）
        /// </summary>
        public static readonly string ApplicationCode = ConfigurationManager.AppSettings["BPF.Workflow.ApplicationCode"];
        /// <summary>
        /// 虚拟用户配置项名称
        /// </summary>
        internal const string SETTING_VirtualUser = "BPF.Workflow.virtualUser";
        /// <summary>
        /// WorkflowServerUrl接口中对应的方法名称
        /// </summary>
        internal const string CONST_WorkflowServiceMethodName = "CommonHandler";
        /// <summary>
        /// 方法版本
        /// </summary>
        public const string CONST_WorkflowMethodVersion = "1.0";
        /// <summary>
        /// 前端注册的Json对象名称
        /// </summary>
        internal const string CONST_WorkflowContextJsonVarName = "MCS_WF_WORKFLOWCONTEXT_JSON";
        /// <summary>
        /// 前端按钮处理后POST回来的Form参数名
        /// </summary>
        internal const string CONST_PostOperaionInfoKey = "MCS_WF_OPERATIONJSON";
        ///// <summary>
        ///// 前端注册的DomID的对象名称
        ///// </summary>
        //public const string WorkflowContainerDomIDVarName = "MCS_WF_CONTAINER_DOMID";
        /// <summary>
        /// 获取流程时刷新扩展命令
        /// </summary>
        internal const string CONST_ExtensionCommond_GetProcessForceRefresh = "GetProcessForceRefresh";
        /// <summary>
        /// 其它查询操作的Action
        /// </summary>
        internal const string CONST_Action_ProcessOperator = "ProcessOperator";
        /// <summary>
        /// 查询待办方法名
        /// </summary>
        internal const string CONST_OtherMethod_QueryToDo = "QueryToDo";
        /// <summary>
        /// 查询已办方法名
        /// </summary>
        internal const string CONST_OtherMethod_QueryDone = "QueryDone";
        /// <summary>
        /// 查询已办(一个人在一只流程中只会得到最后一条数据)方法名
        /// </summary>
        internal const string CONST_OtherMethod_QueryDoneDistinct = "QueryDoneDistinct";
        /// <summary>
        /// 根据业务ID查询待办方法名
        /// </summary>
        internal const string CONST_OtherMethod_QueryToDoByBusinessID = "QueryToDoByBusinessID";
        /// <summary>
        /// 根据业务ID查询已办方法名
        /// </summary>
        internal const string CONST_OtherMethod_QueryDoneByBusinessID = "QueryDoneByBusinessID";
        /// <summary>
        /// 根据业务ID查询已办(一个人在一只流程中只会得到最后一条数据)方法名
        /// </summary>
        internal const string CONST_OtherMethod_QueryDoneDistinctByBusinessID = "QueryDoneDistinctByBusinessID";
        /// <summary>
        /// 查询审批日志方法名
        /// </summary>
        internal const string CONST_OtherMethod_QueryProcessLog = "QueryProcessLog";
        /// <summary>
        /// 检查是否存在流程方法名
        /// </summary>
        internal const string CONST_OtherMethod_ExistProcess = "ExistProcess";
        /// <summary>
        /// 获取流程方法名
        /// </summary>
        internal const string CONST_ExecuteMethod_GetProcess = "GetProcess";
        /// <summary>
        /// 创建流程方法名
        /// </summary>
        internal const string CONST_ExecuteMethod_CreateProcess = "CreateProcess";

        /// <summary>
        /// 一次性提交
        /// </summary>
        internal const int CONST_ExecuteMode_Zero = 0;
        /// <summary>
        /// 第一步提交
        /// </summary>
        internal const int CONST_ExecuteMode_One = 1;
        /// <summary>
        /// 第二步提交
        /// </summary>
        internal const int CONST_ExecuteMode_Two = 2;
    }
}
