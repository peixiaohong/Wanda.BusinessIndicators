using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace Common.ScheduleService
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
#if DEBUG
            DebugRun();
#else
            ServiceBase[] ServicesToRun = new ServiceBase[] { new ScheduleService() };
            ServiceBase.Run(ServicesToRun);
#endif
        }

        private static void DebugRun()
        {
            MainEntry objMain = new MainEntry();
            objMain.Run();
        }
    }
}
