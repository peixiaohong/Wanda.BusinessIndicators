using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleService.Handler
{
    public class JobDemo:Quartz.IJob
    {
        public void Execute(Quartz.IJobExecutionContext context)
        {
            Common.ScheduleService.Log.Instance.Info("Your JobDemo runed in {0}...",DateTime.Now.ToString());
        }
    }
}
