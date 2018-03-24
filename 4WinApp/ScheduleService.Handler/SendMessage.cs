using Lib.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.BLL.BizBLL;
using LJTH.BusinessIndicators.Common;
using LJTH.BusinessIndicators.Model;

namespace ScheduleService.Handler
{
    [Quartz.DisallowConcurrentExecution]
    [Quartz.PersistJobDataAfterExecution]
    public class SendMessage : Quartz.IJob
    {
        public void Execute(Quartz.IJobExecutionContext context)
        {
            Common.ScheduleService.Log.Instance.Info("Service begin execute");
            //查出上报月所有订阅了的系统以及人
            List<B_Subscription> result = B_SubscriptionOperator.Instance.GetallSubscriptionList().ToList();
            int times = 60;
            try
            {
                string tim = ConfigurationManager.AppSettings["duration"];
                if (tim != null && tim != "")
                {
                    times = int.Parse(tim);
                }
                Common.ScheduleService.Log.Instance.Info("在App.Config读取时间间隔成功{0}", times);
            }
            catch (Exception)
            {

                Common.ScheduleService.Log.Instance.Error("在App.Config内读取时间间隔错误,请确保其为数字,没有空格.而且其单位为分钟");
            }



            if (result.Count > 0)
            {
                try
                {
                    //循环所有订阅了系统的人
                    for (int i = 0; i < result.Count; i++)
                    {
                        //循环获得每个系统下每个订阅过的人的Action(要求时间内)
                        List<B_MonthlyReportAction> ReportAction = B_MonthlyReportActionOperator.Instance.GetMonthlyReportActionOneHour(result[i].SystemID, result[i].FinMonth, result[i].FinYear, times);
                        if (ReportAction != null)
                        {
                            if (ReportAction.Count > 0)
                            {
                                string system = C_SystemOperator.Instance.GetSystem(ReportAction[0].SystemID).SystemName;
                                Common.ScheduleService.Log.Instance.Info("读取{0}在{1}年{2}月{3}系统下所有的操作消息成功", result[i].CreatorName, result[i].FinYear, result[i].FinMonth, system);
                                TSM_Messages model = new TSM_Messages();
                                model.ID = new Guid();
                                
                                model.Target = result[i].CreatorName;//用户RTX账号
                                model.Title = "经营系统上报消息提示";//消息标题
                             
                                model.Priority = 0; //优先级
                                model.MessageType = 1; //消息类型
                                model.CreateTime = DateTime.Now;//创建时间
                                model.SendTime = DateTime.Now;//发送时间
                                model.Status = 0;
                                model.TryTimes = 0;

                                string starttime = DateTime.Now.AddMinutes(times*-1).ToLocalTime().ToString();
                                string endtime = DateTime.Now.ToLocalTime().ToString();
                                string stringtime = ConfigurationManager.AppSettings["ContextFormatTime"];
                                string stringvalue = ConfigurationManager.AppSettings["ContextFormatValue"];
                                string Content = string.Format("亲爱的用户,在{0}到{1}之间,{2}", starttime, endtime, system);
                                if (stringtime != null && stringtime != "")
                                {
                                    if (stringtime.Trim()!="")
                                    {
                                        Content = string.Format(stringtime, starttime, endtime, system);
                                        Common.ScheduleService.Log.Instance.Info("在App.Config读取拼接ContextFormatTime字符串成功'{0}'", stringtime);
                                    }
                                    else
                                    {
                                        Common.ScheduleService.Log.Instance.Error("在App.Config读取拼接ContextFormatTime字符串为空,取默认值");
                                    }
                                   
                                }
                                else
                                {
                                    Common.ScheduleService.Log.Instance.Error("在App.Config读取拼接ContextFormatTime字符串失败,取默认值");
                                }
                               
                                //循环操作枚举
                                for (int a = 1; a <= EnumUtil.GetItems(typeof(MonthlyReportLogActionType)).Count; a++)
                                {
                                    int sum = 0;
                                    string ActionEnum = EnumHelper.GetEnumDescription(typeof(MonthlyReportLogActionType), a);

                                    //循环Action,若该action中的操作位当前ActionEnum,则i++,最后统计出该操作总共做了多少次
                                    for (int s = 0; s < ReportAction.Count; s++)
                                    {
                                        if (ReportAction[s].Action == ActionEnum)
                                        {
                                            sum++;
                                        }
                                    }
                                    if (sum > 0)//如果该操作操作次数为0,则跳过
                                    {
                                        if (stringvalue != null && stringvalue != "")
                                        {
                                            if (stringvalue.Trim()!="")
                                            {
                                                Common.ScheduleService.Log.Instance.Info("在App.Config读取拼接ContextFormatValue字符串成功'{0}'", stringvalue);
                                                Content += string.Format(stringvalue, ActionEnum, sum);
                                            }
                                            else
                                            {
                                                Common.ScheduleService.Log.Instance.Info("在App.Config读取拼接ContextFormatValue字符串为空,取默认值");
                                            }
                                        }
                                        else
                                        {
                                            Content += string.Format("有{0}操作{1}个,", ActionEnum, sum);
                                            Common.ScheduleService.Log.Instance.Info("在App.Config读取拼接ContextFormatValue字符串失败,取默认值");
                                        }
                                       
                                    }

                                }
                                model.Content = Content.TrimEnd(',');//去掉最后一个""
                                TSM_MessagesOperator.Instance.AddTSM_Messages(model);
                                Common.ScheduleService.Log.Instance.Info("发送信息成功");

                            }
                        }
                    }
                }
                catch (Exception)
                {
                    Common.ScheduleService.Log.Instance.Error("在编写发送消息时出错");
                }
            }
            Common.ScheduleService.Log.Instance.Info("Service execute finished");

        }
    }
}
