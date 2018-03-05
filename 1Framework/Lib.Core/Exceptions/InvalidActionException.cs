
namespace Lib.Core
{
    /// <summary>
    /// 无法找到对应的操作异常
    /// </summary>
    public class InvalidActionException : SystemSupportException
    {   public InvalidActionException()  { }
        public InvalidActionException(string message) : base(message) { }
        public InvalidActionException(string message, System.Exception innerException) : base(message, innerException) { }
       
    }
}
