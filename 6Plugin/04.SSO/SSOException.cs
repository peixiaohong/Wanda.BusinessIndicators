
using System;
namespace Plugin.Core
{

    /// <summary>
    /// 数据验证失败异常
    /// </summary>
    public class SSOException : ApplicationException
    {
        public SSOException():base()
        {
            
        }

        public SSOException(string message):base(message)
        {
            
        }
        public SSOException(string message, Exception innerException)
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
