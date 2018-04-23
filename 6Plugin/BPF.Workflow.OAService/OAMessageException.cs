
using System;
namespace BPF.OAMQServices
{

    /// <summary>
    /// 数据验证失败异常
    /// </summary>
    public class OAMessageException : ApplicationException
    {
        public OAMessageException():base()
        {
            
        }

        public OAMessageException(string message):base(message)
        {
            
        }
        public OAMessageException(string message, Exception innerException)
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
