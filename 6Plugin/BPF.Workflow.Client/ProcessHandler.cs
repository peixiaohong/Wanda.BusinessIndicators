using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using BPF.Workflow.Object;
using Newtonsoft.Json;

namespace BPF.Workflow.Client
{
    /// <summary>
    /// 流程操作Handler处理
    /// </summary>
    public class ProcessHandler : IHttpHandler, System.Web.SessionState.IReadOnlySessionState
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text";
            string bizContext = context.Request.Form["BizContext"];
            string methodName = context.Request.Form["MethodName"];
            string methodMode = context.Request.Form["MethodMode"];
            string methodVersion = context.Request.Form["Version"];
            string bizWFURL = context.Request.Form["BizWFURL"];
            if (string.IsNullOrEmpty(bizContext))
            {
                bizContext = context.Request.QueryString["BizContext"];
            }
            if (string.IsNullOrEmpty(methodName))
            {
                methodName = context.Request.QueryString["MethodName"];
            }
            if (string.IsNullOrEmpty(methodMode))
            {
                methodMode = context.Request.QueryString["MethodMode"];
            }
            if (string.IsNullOrEmpty(methodVersion))
            {
                methodVersion = context.Request.QueryString["Version"];
            }
            if (string.IsNullOrEmpty(methodName))
            {
                throw new Exception("参数MethodName为空");
            }
            if (string.IsNullOrEmpty(bizContext))
            {
                throw new Exception("参数BizContext为空");
            }
            //反序列化得到BizContext
            BizContext bizContextObj = JsonConvert.DeserializeObject<BizContext>(bizContext);
            string result = WFClientProcess.ExecuteMethod(methodName, SDKHelper.ToInt(methodMode), methodVersion, bizContextObj, bizWFURL);
            context.Response.Write(result);
        }
    }
}
