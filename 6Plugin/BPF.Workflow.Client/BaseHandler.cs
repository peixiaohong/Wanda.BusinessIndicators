using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Web;
using BPF.Workflow.Client.Tools;
using BPF.Workflow.Object;

namespace BPF.Workflow.Client
{
    /// <summary>
    /// Hnadler基类
    /// </summary>
    public abstract class BaseHandler : IHttpHandler, System.Web.SessionState.IReadOnlySessionState
    {
        public bool IsReusable
        {
            get { return false; }
        }

        private List<string> IgnoreRequestParamArray = new List<string> { "RequestType", "Action", "Method", "RequestTime", "CallBackUrl" };
        /// <summary>
        /// 处理请求
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)
        {
            /*
             * 必须的参数：RequestType，Token
             *             RequestType = Request，必须的参数（Action，Method），非必须参数（RequestTime），其他自定义参数
             *             RequestType = CallBack，必须的参数（Result,AppCode）
             * 处理逻辑：
             *             RequestType = Request
             *             1、缓存中存在数据，则直接返回缓存中的数据
             *             2、缓存中不存在数据，判断RequestTime是否大于0。
             *             2.1、                RequestTime>0 ,判断缓存中是否存在上次请求时间（LastRequestTime）
             *             2.1.1、                             不存在，则缓存当前时间，并且请求WebService
             *             2.1.2、                             存  在，则判断缓存时间与当前时间差值是否大于等于RequestTime
             *             2.1.2.1                                     缓存时间差值 >= RequestTime ，则请求WebService
             *             2.1.2.2                                     缓存时间差值 <  RequestTime ，则直接返回
             *             2.2                  RequestTime<=0 请求WebService
             *             RequestType = CallBack，将Result直接存储在Cahce中。
             */
            //context.Response.ContentType = "text";
            string Result = "";
            try
            {
                string requestType = context.Request["RequestType"];//请求类型，Request/CallBack。
                string token = context.Request["Token"];//Token
                string appcode = AppSettingInfo.ApplicationCode;//配置的AppCode

                ExceptionHelper.TrueThrow(string.IsNullOrEmpty(requestType) || string.IsNullOrEmpty(token)
                    , ClientConstDefine.WORKFLOW_SERVICE_ERRORCODE_USERSELECT_SDKREQUESTTYPEORTOKENEMPTY
                    , ClientConstDefine.WORKFLOW_SERVICE_ERRORCONTENT_USERSELECT_ARGUMENTERROR);

                if (requestType == "CallBack")
                {
                    //CallBack，根据appcode和token，将Result值存储在Cache中
                    string result = context.Request["Result"];//Token
                    string strResult = JsonConvert.SerializeObject(new ResultBaseContext() { Data = result });
                    CacheHelper.Add(appcode, "Request", token, strResult, 0, 10, 0, 0);
                }
                else if (requestType == "Request")
                {
                    //Request，根据appcode和token获取缓存中的数据
                    var result = CacheHelper.GetValue(appcode, "Request", token);
                    if (result != null)
                    {
                        Result = result.ToString();
                    }
                    else
                    {
                        //不存在缓存数据，则调用RequestWebService处理
                        Result = RequestWebService(context, appcode, token);
                    }
                }
                else if (requestType == "AppSetting")
                {
                    string action = context.Request["Action"];//AppSetting Key;
                    ExceptionHelper.TrueThrow(string.IsNullOrEmpty(action)
               , ClientConstDefine.WORKFLOW_SERVICE_ERRORCODE_USERSELECT_SDKACTIONORMETHODEMPTY
               , ClientConstDefine.WORKFLOW_SERVICE_ERRORCONTENT_USERSELECT_ARGUMENTERROR);
                    string appSettingValue = "";//兼容JS传递的旧配置请求。
                    if (action == "ApplicationCode")
                    {
                        appSettingValue = AppSettingInfo.ApplicationCode;
                    }
                    else if (action == "WorkflowServerUrl")
                    {
                        appSettingValue = AppSettingInfo.WorkflowServerUrl;
                    }
                    else
                    {
                        appSettingValue = ConfigurationManager.AppSettings[action];
                    }
                    Result = JsonConvert.SerializeObject(new ResultBaseContext(appSettingValue));
                }
                else
                {
                    ExceptionHelper.TrueThrow(true
                   , ClientConstDefine.WORKFLOW_SERVICE_ERRORCODE_USERSELECT_SDKREQUESTTYPEERROR
                   , ClientConstDefine.WORKFLOW_SERVICE_ERRORCONTENT_USERSELECT_ARGUMENTERROR);
                }
            }
            catch (Exception ex)
            {
                Result = JsonConvert.SerializeObject(new ResultBaseContext()
                {
                    StatusCode = ExceptionHelper.GetExceptionCode(ex),
                    StatusMessage = ExceptionHelper.GetExceptionMessage(ex),
                });
            }
            context.Response.Write(Result);
        }

        #region 私有方法
        /// <summary>
        /// 处理请求
        /// </summary>
        /// <param name="context"></param>
        /// <param name="appCode"></param>
        /// <param name="token"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        private string RequestWebService(HttpContext context, string appCode, string token)
        {
            string action = context.Request["Action"];//类型
            string method = context.Request["Method"];//调用方法

            ExceptionHelper.TrueThrow(string.IsNullOrEmpty(action) || string.IsNullOrEmpty(method)
                , ClientConstDefine.WORKFLOW_SERVICE_ERRORCODE_USERSELECT_SDKACTIONORMETHODEMPTY
                , ClientConstDefine.WORKFLOW_SERVICE_ERRORCONTENT_USERSELECT_ARGUMENTERROR);

            string requestTime = context.Request["RequestTime"];//请求WebService频率
            double iRequestTime = 0;
            double.TryParse(requestTime, out iRequestTime);

            string Result = "";

            if (iRequestTime > 0)
            {
                //需要设置请求频率。
                object lastRequestTime = CacheHelper.GetValue(appCode, "LastRequestTime", token);
                if (lastRequestTime == null)
                {
                    CacheHelper.Add(appCode, "LastRequestTime", token, DateTime.Now, 0, 10, 0, 0);
                    //第一次请求WebService
                    Result = CallWebService(context, action, method, token);
                }
                else
                {
                    DateTime dtLastRequestTime = (DateTime)CacheHelper.GetValue(appCode, "LastRequestTime", token);
                    if ((DateTime.Now - dtLastRequestTime).Seconds > iRequestTime / 1000.0)
                    {
                        CacheHelper.Upsert(appCode, "LastRequestTime", token, DateTime.Now, 0, 10, 0, 0);
                        //超过请求频率
                        Result = CallWebService(context, action, method, token);
                    }
                    else
                    {
                        //未超过请求频率
                        Result = JsonConvert.SerializeObject(new ResultBaseContext() { StatusCode = ClientConstDefine.WORKFLOW_SERVICE_ERRORCODE_USERSELECT_ERRORSUCCESS });
                    }
                }
            }
            else
            {
                //直接请求WebService
                Result = CallWebService(context, action, method, token);
            }
            return Result;
        }
        /// <summary>
        /// 调用WebService请求数据
        /// </summary>
        /// <param name="context"></param>
        /// <param name="action"></param>
        /// <param name="method"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private string CallWebService(HttpContext context, string action, string method, string token)
        {
            string callBackUrl = context.Request.Url.ToString().Substring(0, context.Request.Url.ToString().IndexOf('?') - 1) + "/CommonHandler.ashx";
            var dicParamWebService = GetWebServiceParam(context, action, method, token, callBackUrl);
            try
            {

                string url = AppSettingInfo.WorkflowServerUrl;
                string bizWFServiceURL = context.Request.Form["BizWFURL"];
                if (!string.IsNullOrEmpty(bizWFServiceURL))
                {
                    url = bizWFServiceURL;
                }
                string workFlowServerFullURL = SDKHelper.GetWorkflowServerUrlFullPath(url);
                var result = SDKHelper.QueryPostWebService(workFlowServerFullURL, AppSettingInfo.CONST_WorkflowServiceMethodName, dicParamWebService);
                return result;
            }
            catch (Exception ex)
            {
                SDKSystemSupportException newEX = new SDKSystemSupportException(ClientConstDefine.WORKFLOW_SERVICE_ERRORCODE_USERSELECT_SERVERWEBSERVICEERROR
                    , ClientConstDefine.WORKFLOW_SERVICE_ERRORCONTENT_USERSELECT_SERVERWEBSERVICEERROR
                    );
                throw (Exception)ex;
            }

        }
        #endregion

        /// <summary>
        /// 构建WebService需要的参数
        /// </summary>
        /// <param name="context"></param>
        /// <param name="appCode"></param>
        /// <param name="action"></param>
        /// <param name="method"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private Dictionary<string, object> GetWebServiceParam(HttpContext context, string action, string method, string token, string callBackUrl)
        {
            //获取所有的参数信息，将除Action和Method参数以外的其它参数添加到dicParam字典中。
            var param = context.Request.Form;
            var dicParam = new Dictionary<string, string>();
            foreach (var item in param.AllKeys)
            {
                if (!IgnoreRequestParamArray.Contains(item))
                {
                    dicParam.Add(item, param[item]);
                }
            }
            return BuildParamWebService(action, method, token, callBackUrl, dicParam, context);
        }

        public abstract Dictionary<string, object> BuildParamWebService(string action, string method, string token, string callBackUrl, Dictionary<string, string> dicParam, HttpContext context);
    }
}
