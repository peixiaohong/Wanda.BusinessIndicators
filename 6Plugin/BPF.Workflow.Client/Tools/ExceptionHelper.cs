using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Web.Services.Protocols;
using BPF.Workflow.Object;

namespace BPF.Workflow.Client.Tools
{
    /// <summary>
    /// Exception工具，提供了TrueThrow和FalseThrow等方法
    /// </summary>
    /// <remarks>Exception工具，TrueThrow方法判断它的布尔参数值是否为true，若是则抛出异常；FalseThrow方法判断它的布尔参数值是否为false，若是则抛出异常。
    /// </remarks>
    public static class ExceptionHelper
    {
        #region 获取异常信息
        /// <summary>
        /// 获取异常消息
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string GetExceptionMessage(Exception ex)
        {
            Exception innerEx = ex.InnerException;
            if (innerEx == null)
            {
                if (ex is SDKSystemSupportException)
                {
                    return ex.Message;
                }
                else
                {
                    return ex.ToString();
                }
            }
            else
            {
                if (innerEx is SDKSystemSupportException)
                {
                    return ((SDKSystemSupportException)innerEx).Message;
                }
                else
                {
                    return innerEx.ToString();
                }
            }
        }
        /// <summary>
        /// 获取异常Code
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static int GetExceptionCode(Exception ex)
        {
            Exception innerEx = ex.InnerException;
            if (innerEx == null)
            {
                if (ex is SDKSystemSupportException)
                {
                    return ((SDKSystemSupportException)ex).ErrorCode;
                }
                else
                {
                    return ClientConstDefine.WORKFLOW_SERVICE_ERRORCODE_USERSELECT_ERROR;
                }
            }
            else
            {
                if (innerEx is SDKSystemSupportException)
                {
                    return ((SDKSystemSupportException)innerEx).ErrorCode;
                }
                else
                {
                    return ClientConstDefine.WORKFLOW_SERVICE_ERRORCODE_USERSELECT_ERROR;
                }
            }
        }
        #endregion

        #region 抛异常
        /// <summary>
        /// 抛异常
        /// </summary>
        /// <param name="parseExpressionResult"></param>
        /// <param name="code">异常代码值</param>
        /// <param name="message">客户端显示的异常信息，不一定是异常代码对应的信息</param>
        public static void TrueThrow(bool parseExpressionResult, int code, string message)
        {
            if (parseExpressionResult)
            {
                SDKSystemSupportException ex = new SDKSystemSupportException(code, message);
                throw (Exception)ex;
            }
        }
        #endregion
    }
}
