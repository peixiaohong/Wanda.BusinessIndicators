
using Framework.Data;
using Framework.Web.Security.Authentication;
using Framework.Web.Utility;
using Lib.Model;
using Plugin.EmployeeTransfer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Plugin.EmployeeTransfer
{
    class Program
    {
        static void Main(string[] args)
        {
            WebHelper.Instance.GetUser += VirtualUser;
#if DEBUG
            DebugRun();
#else
            ServiceBase[] ServicesToRun = new ServiceBase[] { new ScheduleService() };
            ServiceBase.Run(ServicesToRun);
#endif
        }

        public static LoginUserInfo VirtualUser(string loginName)
        {
            return new LoginUserInfo()
            {
                CNName = loginName,
                LoginName = loginName
            };
        }

        private static void DebugRun()
        {
            MainEntry objMain = new MainEntry();
            objMain.Run();
        }
    }
}
