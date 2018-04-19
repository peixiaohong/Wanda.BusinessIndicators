using System;
using System.Collections.Generic;
using System.Text;

namespace BPF.Workflow.Object
{
}

namespace BPF.Workflow.Object
{
    /// <summary>
    /// 定制的异常类
    /// </summary>
    /// <remarks>定制的异常类,这种异常类会提醒前端程序显示出技术支持信息的提示信息，该类继承自ApplicationException类。
    /// </remarks>
    public class SDKSystemSupportException : ApplicationException
    {
        /// <summary>
        /// 错误代码
        /// </summary>
        public int ErrorCode { get; set; }
        /// <summary>
        /// 异常信息（日志记录）目前暂未记录到日志中
        /// </summary>
        public string RealErrorMessage { get; set; }

        //默认的前端错误提示信息
        const string defaultMessage = ClientConstDefine.WORKFLOW_SERVICE_ERRORCONTENT_USERSELECT_ERROR;

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public SDKSystemSupportException() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="errorCode">错误代码</param>
        public SDKSystemSupportException(int errorCode)
            : this(errorCode, defaultMessage)
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="errorCode">错误代码</param>
        /// <param name="message">返回客户端的错误信息</param>
        public SDKSystemSupportException(int errorCode, string message)
            : base("【错误代码：" + errorCode + "】" + message)
        {
            this.ErrorCode = errorCode;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="errorCode">错误代码</param>
        /// <param name="message">返回客户端的错误信息</param>
        /// <param name="realErrorMessage">服务器端错误信息</param>
        public SDKSystemSupportException(int errorCode, string message, string realErrorMessage)
            : this(errorCode, message)
        {
            this.RealErrorMessage = realErrorMessage;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="errorCode">错误代码</param>
        /// <param name="message">返回客户端的错误信息</param>
        /// <param name="realErrorMessage">服务器端错误信息</param>
        /// <param name="innerException">异常信息</param>
        public SDKSystemSupportException(int errorCode, string message, string realErrorMessage, Exception innerException)
            : base("【错误代码：" + errorCode + "】" + message, innerException)
        {
            this.RealErrorMessage = realErrorMessage;
        }
        #endregion
    }
}
