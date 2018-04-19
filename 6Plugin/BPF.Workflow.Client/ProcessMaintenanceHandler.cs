using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using BPF.Workflow.Object;

namespace BPF.Workflow.Client
{
    /// <summary>
    /// 维护Handler处理
    /// </summary>
    public class ProcessMaintenanceHandler : BaseHandler
    {
        public override Dictionary<string, object> BuildParamWebService(string action, string method, string token, string callBackUrl, Dictionary<string, string> dicParam, HttpContext context)
        {
            string version = "1.0";
            if (dicParam.ContainsKey("Version"))
            {
                version = dicParam["Version"];
            }
            string paramStr = string.Empty;
            if (dicParam.ContainsKey("Param"))
            {
                paramStr = dicParam["Param"];
            }
            //将appCode，action，method，param（序列化后的dicParam）添加到dicParamWebService中调用WebService
            return SDKHelper.BuildParamWebService(action, method, token, Newtonsoft.Json.JsonConvert.SerializeObject(
                new
                {
                    Version = version,
                    Param = paramStr,
                    CurrentUserLoginID = SDKHelper.GetUserName(context),
                    BizAppCode = AppSettingInfo.ApplicationCode
                }), callBackUrl);
        }
    }
}