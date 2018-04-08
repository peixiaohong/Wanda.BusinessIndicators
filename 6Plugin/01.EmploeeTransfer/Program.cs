
using Plugin.EmploeeTransfer.Models;
using System.ServiceProcess;

namespace Plugin.EmployeeTransfer
{
    class Program
    {
        static void Main(string[] args)
        {
            //WebHelper.Instance.GetUser += VirtualUser;
//#if DEBUG
//            DebugRun();
//#else
            ServiceBase[] ServicesToRun = new ServiceBase[] { new ScheduleService() };
            ServiceBase.Run(ServicesToRun);
//#endif
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
