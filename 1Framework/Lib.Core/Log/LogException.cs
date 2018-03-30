using System;

namespace Lib.Log
{
    /// <summary>
    /// 日志异常类
    /// </summary>
    /// <remarks>
    /// 日志系统自定义异常类型
    /// </remarks>
    [Serializable]
    //说明：
    //		1、一个类虽然实现了 ISerializable接口，但是如果没有声明[Serializable]，仍然不会被序列化。
    //		2、.NET运行时将允许任何声明了 [Serializable]属性的对象进行序列化。
    //			如果通过.NET框架定义的缺省序列化方法能够使一个类被序列化，那么这个类的对象也必定被正确的序列化。
    //			如果类需要自定义序列化方法，那他就必须实现ISerializable接口，同时还必须声明[Serializable]属性。
    public sealed class LogException : ApplicationException
    {
        /// <summary>
        /// 缺省构造函数
        /// </summary>
        /// <remarks>
        /// <code source="..\Framework\src\DeluxeWorks.Library\Logging\Logger.cs" 
        /// lang="cs" region="Process Log" title="写日志的异常处理"></code>
        /// </remarks>
        public LogException()
            : base()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">异常消息</param>
        /// <remarks>
        /// 根据异常消息，生成日志异常类
        /// </remarks>
        public LogException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 重载的构造函数
        /// </summary>
        /// <param name="message">异常消息</param>
        /// <param name="exception">原始异常对象</param>
        /// <remarks>
        /// 将原始异常，转义成LogException
        /// <code source="..\Framework\src\DeluxeWorks.Library\Logging\Logger.cs" 
        /// lang="cs" region="Process Log" title="写日志的异常处理"></code>
        /// </remarks>
        public LogException(string message, Exception exception)
            : base(message, exception)
        {
        }
    }
}
