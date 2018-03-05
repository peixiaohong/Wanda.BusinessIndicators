
using System;
namespace Lib.Core
{

    /// <summary>
    /// 数据验证失败异常
    /// </summary>
    public class ValidationException : SystemSupportException
    {
        public ValidationException():base()
        {
            
        }

        public ValidationException(string message):base(message)
        {
            
        }
        public ValidationException(string message, Exception innerException)
            : base(message,innerException)
        {
              
        }

        public override string Message
        {
            get
            {
                return "数据验证失败。"+base.Message;
            }
        }

     
    }
}
