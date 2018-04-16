using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plugin.OAMessage
{
    public class commonResult
    {
        public bool success { get; set; }
        public ResCode resCode { get; set; }
        public string message { get; set; }
        public object result { get; set; }
        public static commonResult Success(string resMsg)
        {
            commonResult result = new commonResult();
            if(resMsg.Contains("成功"))
            {
                result.success = true;
                result.resCode = ResCode.success;
                result.message = "";
                result.result = resMsg;
            }
            else
            {
                result.success = false;
                result.resCode = ResCode.oaerror;
                result.message = resMsg;
                result.result = resMsg;
            }
            return result;
        }
        public static commonResult NullParameter()
        {
            commonResult result = new commonResult();
            result.success = false;
            result.resCode = ResCode.parameter;
            result.message = "缺少必要的参数";
            result.result = null;
            return result;
        }
        public static commonResult OAError(Exception e)
        {
            commonResult result = new commonResult();
            result.success = false;
            result.resCode = ResCode.error;
            result.message = e.Message;
            result.result = null;
            return result;
        }
        public static commonResult Excepiton(Exception e)
        {
            commonResult result = new commonResult();
            result.success = false;
            result.resCode = ResCode.exception;
            result.message = e.Message;
            result.result = null;
            return result;
        }
    }

}