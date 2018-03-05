
namespace Lib.Core
{
    /// <summary>
    /// Json对象序列化/反序列化异常
    /// </summary>
    public class JsonConvertFailException : SystemSupportException
    {
        public JsonConvertFailException() { }
        public JsonConvertFailException(string message) : base(message) { }
        public JsonConvertFailException(string message, System.Exception innerException) : base(message, innerException) { }

    }
}
