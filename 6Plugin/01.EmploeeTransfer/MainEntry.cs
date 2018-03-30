using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Threading;
using Quartz;
using Quartz.Impl;
using System.IO;

namespace Plugin.EmployeeTransfer
{
    class MainEntry
    {
        static IScheduler sched = null;
        public void Run()
        {
            try
            {
                LogMgnt.Instance["*"].Info("------- Service Initialize ----------------------");
                ISchedulerFactory sf = new StdSchedulerFactory();
                sched = sf.GetScheduler();
                // all jobs and triggers are now in scheduler
                // Start up the scheduler (nothing can actually run until the 
                // scheduler has been started
                sched.Start();
                try
                {
                    Thread.Sleep(30 * 1000);
                }
                catch (ThreadInterruptedException)
                {
                }
                LogMgnt.Instance["*"].Info("------- Service Initialize Complete ------------");
            }
            catch (Exception ex)
            {
                LogMgnt.Instance["*"].Error(ex.ToString());
            }
        }
    }
}
