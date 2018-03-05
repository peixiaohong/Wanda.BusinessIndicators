using Lib.Core;

namespace Lib.Cache
{
    /// <summary>
    /// Dependency失效，导致Cache的Key访问失效所使用的异常
    /// </summary>
    public class DependencyChangedException : SystemSupportException
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        public DependencyChangedException()
            : base()
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="message">异常信息</param>
        public DependencyChangedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="message">异常信息</param>
        /// <param name="innerException">异常对象</param>
        public DependencyChangedException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
