using ScheduleService.Handler;
using System.ServiceProcess;

namespace Common.ScheduleService
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            string systemid = "DB3E4D6E-A272-48D8-9CE9-AA1DBD4472F2";
            int year = 2018;
            int month = 3;
            new ProjectMergeProcess().AddMonthlyReport(new System.Guid(systemid), year, month);
            return;

            //#if DEBUG
            //            DebugRun();
            //#else
            ServiceBase[] ServicesToRun = new ServiceBase[] {
                new ScheduleService()
            };
            ServiceBase.Run(ServicesToRun);
            //#endif

        }

        private static void DebugRun()
        {
            MainEntry objMain = new MainEntry();
            objMain.Run();
        }
    }
}
