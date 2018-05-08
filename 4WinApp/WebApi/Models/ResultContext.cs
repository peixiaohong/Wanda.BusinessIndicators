using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    public class ResultContext
    {
        public ResultContext()
        {
            this.IsSuccess = true;
            this.StatusCode = (int)StatusCodeEnum.isTrue;
        }
        public ResultContext(object Data)
        {
            this.Data = Data;
            this.IsSuccess = true;
            this.StatusCode = (int)StatusCodeEnum.isTrue;
        }
        public ResultContext(int StatusCode,string Statusmessage)
        {
            this.IsSuccess = false;
            this.StatusCode = StatusCode;
            this.StatusMessage = StatusMessage;
        }
        public bool IsSuccess { get; }
        public object Data { get; set; }
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
    }

    public enum StatusCodeEnum
    {
        //请求成功  
        isTrue = 200,
        //逻辑错误
        isFalse = 300,
        //系统错误
        isCatch = 305
    }
}