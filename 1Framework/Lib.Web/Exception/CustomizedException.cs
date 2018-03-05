using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lib.Web.Exceptions
{
    public class CustomizedException : ApplicationException
    {
        public CustomizedException()
            : base()
        {

        }

        public CustomizedException(string message)
            : base(message)
        {

        }
        public CustomizedException(string message, System.Exception innerException)
            : base(message, innerException)
        {

        }

        public override string Message
        {
            get
            {
                return   base.Message;
            }
        }
    }

    public class PreparationException : CustomizedException { 
      public PreparationException()
            : base()
        {

        }

        public PreparationException(string message)
            : base(message)
        {

        }
        public PreparationException(string message, System.Exception innerException)
            : base(message, innerException)
        {

        }


        public override string Message
        {
            get
            {
                return "数据操作出错:" + base.Message;
            }
        }
    }
}
