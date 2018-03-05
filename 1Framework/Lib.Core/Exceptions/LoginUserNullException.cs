using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lib.Core
{
    public class LoginUserNullException : ApplicationException
    {
        public LoginUserNullException():base()
        {
        }
    }
}
