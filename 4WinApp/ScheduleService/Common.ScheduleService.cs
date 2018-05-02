using Quartz;
using Quartz.Impl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace Common.ScheduleService
{
    public partial class ScheduleService : ServiceBase
    {
        private IScheduler scheduler;

        protected override void OnStart(string[] args)
        {
            Log.Instance.Info("LJTH Common Schedule Service starting...");
            scheduler.Start();
            Log.Instance.Info("LJTH Common Schedule Service started");
        }

        protected override void OnStop()
        {
            Log.Instance.Info("LJTH Common Schedule Service Shutdowning...");
            if (scheduler != null)
            {
                //停止全部正在运行的调度任务
                IList<IJobExecutionContext> jobs = scheduler.GetCurrentlyExecutingJobs();
                foreach (IJobExecutionContext job in jobs)
                {
                    if (job == null) continue;
                    IInterruptableJob interruptableJob = job.JobInstance as IInterruptableJob;
                    if (interruptableJob != null)
                    {
                        interruptableJob.Interrupt();
                    }
                }

                GC.Collect();
                //此处参数要选择false，否则本行代码会等待所有作业执行完成后才返回，导致服务停止操作报告失败
                scheduler.Shutdown(false);
            }
            Log.Instance.Info("LJTH Common Schedule Service Shutdown complete");
        }

        protected override void OnPause()
        {
            Log.Instance.Info("LJTH Common Schedule Service pausing...");
            scheduler.PauseAll();
            Log.Instance.Info("LJTH Common Schedule Service paused...");
        }

        protected override void OnContinue()
        {
            scheduler.ResumeAll();
        }

        public ScheduleService()
        {
            Log.Instance.Info("------- Service Initialize ----------------------");
            InitializeComponent();
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            scheduler = schedulerFactory.GetScheduler();
            Log.Instance.Info("------- Service Initialize Complete ------------");
        }


    }
}
