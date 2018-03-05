using System;
using System.Reflection;
using System.Web.Services.Protocols;
using Lib.Core.Properties;

namespace Lib.Core
{
    /// <summary>
    /// Exception工具，提供了TrueThrow和FalseThrow等方法
    /// </summary>
    /// <remarks>Exception工具，TrueThrow方法判断它的布尔参数值是否为true，若是则抛出异常；FalseThrow方法判断它的布尔参数值是否为false，若是则抛出异常。
    /// </remarks>
    public static class ExceptionHelper
    {
        /// <summary>
        /// 如果条件表达式boolExpression的结果值为真(true)，则抛出strMessage指定的错误信息
        /// </summary>
        /// <param name="parseExpressionResult">条件表达式</param>
        /// <param name="message">错误信息</param>
        /// <param name="messageParams">错误信息参数</param>
        /// <remarks>
        /// 如果条件表达式boolExpression的结果值为真(true)，则抛出strMessage指定的错误信息
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\ExceptionsTest.cs"  lang="cs" title="通过判断条件表达式boolExpression的结果值而判断是否抛出指定的异常信息" />
        /// <seealso cref="FalseThrow"/>
        /// <seealso cref="Lib.Core.Compression.ZipReader"/>
        /// </remarks>
        /// <example>
        /// <code>
        /// ExceptionTools.TrueThrow(name == string.Empty, "对不起，名字不能为空！");
        /// </code>
        /// </example>
        public static void TrueThrow(bool parseExpressionResult, string message, params object[] messageParams)
        {
            TrueThrow<SystemSupportException>(parseExpressionResult, message, messageParams);
        }

        /// <summary>
        /// 如果条件表达式boolExpression的结果值为真(true)，则抛出strMessage指定的错误信息
        /// </summary>
        /// <param name="parseExpressionResult">条件表达式</param>
        /// <param name="message">错误信息</param>
        /// <param name="messageParams">错误信息的参数</param>
        /// <typeparam name="T">异常的类型</typeparam>
        /// <remarks>
        /// 如果条件表达式boolExpression的结果值为真(true)，则抛出message指定的错误信息
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\ExceptionsTest.cs" region = "TrueThrowTest" lang="cs" title="通过判断条件表达式boolExpression的结果值而判断是否抛出指定的异常信息" />
        /// <seealso cref="FalseThrow"/>
        /// <seealso cref="Lib.Logging.LogEntity"/>
        /// </remarks>
        public static void TrueThrow<T>(bool parseExpressionResult, string message, params object[] messageParams) where T : System.Exception
        {
            Type exceptionType = typeof(T);

            if (parseExpressionResult)
            {
                if (message == null)
                    throw new ArgumentNullException("message");

                Object obj = Activator.CreateInstance(exceptionType);

                Type[] types = new Type[1];
                types[0] = typeof(string);

                ConstructorInfo constructorInfoObj = exceptionType.GetConstructor(
                    BindingFlags.Instance | BindingFlags.Public, null,
                    CallingConventions.HasThis, types, null);

                Object[] args = new Object[1];

                args[0] = string.Format(message, messageParams);

                constructorInfoObj.Invoke(obj, args);

                throw (Exception)obj;
            }
        }

        /// <summary>
        /// 如果条件表达式boolExpression的结果值为假（false），则抛出strMessage指定的错误信息
        /// </summary>
        /// <param name="parseExpressionResult">条件表达式</param>
        /// <param name="message">错误信息</param>
        /// <param name="messageParams">错误信息参数</param>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\ExceptionsTest.cs" region = "FalseThrowTest" lang="cs" title="通过判断条件表达式boolExpression的结果值而判断是否抛出指定的异常信息" />
        /// <seealso cref="TrueThrow"/>
        /// <seealso cref="Lib.Logging.LoggerFactory"/>
        /// <remarks>
        /// 如果条件表达式boolExpression的结果值为假（false），则抛出message指定的错误信息
        /// </remarks>
        /// <example>
        /// <code>
        /// ExceptionTools.FalseThrow(name != string.Empty, "对不起，名字不能为空！");
        /// </code>
        /// </example>
        public static void FalseThrow(bool parseExpressionResult, string message, params object[] messageParams)
        {
            TrueThrow(false == parseExpressionResult, message, messageParams);
        }

        /// <summary>
        /// 如果条件表达式boolExpression的结果值为假（false），则抛出message指定的错误信息
        /// </summary>
        /// <typeparam name="T">异常的类型</typeparam>
        /// <param name="parseExpressionResult">条件表达式</param>
        /// <param name="message">错误信息</param>
        /// <param name="messageParams">错误信息参数</param>
        /// <remarks>
        /// 如果条件表达式boolExpression的结果值为假（false），则抛出strMessage指定的错误信息
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\ExceptionsTest.cs" region="FalseThrowTest" lang="cs" title="通过判断条件表达式boolExpression的结果值而判断是否抛出指定的异常信息" />
        /// <seealso cref="TrueThrow"/>
        /// <seealso cref="Lib.Core.EnumItemDescriptionAttribute"/>
        /// </remarks>
        /// <example>
        /// <code>
        /// ExceptionTools.FalseThrow(name != string.Empty, typeof(ApplicationException), "对不起，名字不能为空！");
        /// </code>
        /// </example>
        public static void FalseThrow<T>(bool parseExpressionResult, string message, params object[] messageParams) where T : System.Exception
        {
            TrueThrow<T>(false == parseExpressionResult, message, messageParams);
        }

        /// <summary>
        /// 检查字符串参数是否为Null或空串，如果是，则抛出异常
        /// </summary>
        /// <param name="data">字符串参数值</param>
        /// <param name="paramName">字符串名称</param>
        /// <remarks>
        /// 若字符串参数为Null或空串，抛出ArgumentException异常
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\ExceptionsTest.cs" region="CheckStringIsNullOrEmpty" lang="cs" title="检查字符串参数是否为Null或空串，若是，则抛出异常" />
        /// </remarks>
        public static void CheckStringIsNullOrEmpty(string data, string paramName)
        {
            if (string.IsNullOrEmpty(data))
                throw new ArgumentException(string.Format(Resource.StringParamCanNotBeNullOrEmpty, paramName));
        }

        /// <summary>
        /// 从Exception对象中，获取真正发生错误的错误对象。
        /// </summary>
        /// <param name="ex">Exception对象</param>
        /// <returns>真正发生错误的错误对象</returns>
        public static Exception GetRealException(Exception ex)
        {
            System.Exception lastestEx = ex;

            if (ex is SoapException)
            {
                lastestEx = new SystemSupportException(GetSoapExceptionMessage(ex), ex);
            }
            else
            {
                while (ex != null &&
                    (ex is System.Web.HttpUnhandledException || ex is System.Web.HttpException || ex is TargetInvocationException))
                {
                    if (ex.InnerException != null)
                        lastestEx = ex.InnerException;
                    else
                        lastestEx = ex;

                    ex = ex.InnerException;
                }
            }

            return lastestEx;
        }

        /// <summary>
        /// 得到SoapException中的错误信息
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string GetSoapExceptionMessage(Exception ex)
        {
            string strNewMsg = ex.Message;

            if (ex is SoapException)
            {
                int i = strNewMsg.LastIndexOf("--> ");

                if (i > 0)
                {
                    strNewMsg = strNewMsg.Substring(i + 4);
                    i = strNewMsg.IndexOf(": ");

                    if (i > 0)
                    {
                        strNewMsg = strNewMsg.Substring(i + 2);

                        i = strNewMsg.IndexOf("\n   ");

                        strNewMsg = strNewMsg.Substring(0, i);
                    }
                }
            }

            return strNewMsg;
        }
    }
}
