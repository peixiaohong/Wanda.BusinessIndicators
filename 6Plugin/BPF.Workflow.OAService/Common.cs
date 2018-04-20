using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPF.OAMQServices
{
    internal static class Common
    {
        private static Lvl.LogAdapter log = Lvl.LogAdapter.GetLogger("OAMQService");

        public static Lvl.LogAdapter Log
        {
            get
            {
                return log;
            }
        }
    }
}
