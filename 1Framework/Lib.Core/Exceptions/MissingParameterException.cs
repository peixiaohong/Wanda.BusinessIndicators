
namespace Lib.Core
{
    /// <summary>
    /// 方法执行时缺少参数异常
    /// </summary>
    public class MissingParameterException : SystemSupportException
    {
        public MissingParameterException()  { }
        public MissingParameterException(string message) : base(message) { }
        public MissingParameterException(string message, System.Exception innerException) : base(message, innerException) { }
        
    }
}
