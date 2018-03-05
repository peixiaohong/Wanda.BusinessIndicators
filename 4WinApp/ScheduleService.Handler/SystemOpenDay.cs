using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wanda.BusinessIndicators.BLL;
using Wanda.BusinessIndicators.Model;

namespace ScheduleService.Handler
{
    [Quartz.DisallowConcurrentExecution]
    [Quartz.PersistJobDataAfterExecution]
    public class SystemOpenDay : Quartz.IJob
    {

        public void Execute(Quartz.IJobExecutionContext context)
        {
            Common.ScheduleService.Log.Instance.Info("Service begin execute");
            //查出上报月所有订阅了的系统以及人
           // List<B_Subscription> result = B_SubscriptionOperator.Instance.GetallSubscriptionList().ToList();

            int times = 60;
            try
            {
                string tim = ConfigurationManager.AppSettings["duration"];
                if (tim != null && tim != "")
                {
                    times = int.Parse(tim);
                }
                Common.ScheduleService.Log.Instance.Info("系统开放日服务，在App.Config读取时间间隔成功{0}", times);
            }
            catch (Exception)
            {

                Common.ScheduleService.Log.Instance.Error("系统开放日服务，在App.Config内读取时间间隔错误,请确保其为数字,没有空格.而且其单位为分钟");
            }

            //拿出系统开放实体
            C_ReportTime SysConfigModel = C_ReportTimeOperator.Instance.GetReportTime();

            Common.ScheduleService.Log.Instance.Info("系统开放日服务，执行逻辑业务开始");

            try
            {

                if (SysConfigModel.SysOpenDay != null)
                {
                    if (SysConfigModel.SysOpenDay.Value.ToString("yyyy-MM-dd") == DateTime.Now.ToString("yyyy-MM-dd"))
                    {
                        SysConfigModel.ReportTime = SysConfigModel.WantTime;
                        SysConfigModel.OpenStatus = "1";
                        C_ReportTimeOperator.Instance.UpdateReportTime(SysConfigModel);
                    }
                }

                Common.ScheduleService.Log.Instance.Info("系统开放日服务，执行逻辑业务结束");
            }
            catch (Exception ex)
            {
                Common.ScheduleService.Log.Instance.Error("系统开放日服务，执行逻辑业务报错：" + ex.ToString());
            }



        }
    }
}
