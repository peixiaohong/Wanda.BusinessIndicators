using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using BPF.Workflow.Client.Tools;
using Newtonsoft.Json;
using BPF.Workflow.Object;

namespace BPF.Workflow.Client
{
    /// <summary>
    /// 通用Handler处理
    /// </summary>
    public class CommonHandler : BaseHandler
    {
        public override Dictionary<string, object> BuildParamWebService(string action, string method, string token, string callBackUrl, Dictionary<string, string> dicParam, HttpContext context)
        {
            return SDKHelper.BuildParamWebService(action, method, token, Newtonsoft.Json.JsonConvert.SerializeObject(dicParam), callBackUrl);
        }
    }
}
