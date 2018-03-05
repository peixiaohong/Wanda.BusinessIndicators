using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wanda.BusinessIndicators.BLL;
using Wanda.BusinessIndicators.Model;
using Wanda.BusinessIndicators.Engine;

namespace ScheduleService.Handler
{
    [Quartz.DisallowConcurrentExecution]
    [Quartz.PersistJobDataAfterExecution]
    public class Recalculation : Quartz.IJob
    {
        void Quartz.IJob.Execute(Quartz.IJobExecutionContext context)
        {
            Common.ScheduleService.Log.Instance.Info("重新计算 Service begin execute");

            //查出上报月所有订阅了的系统以及人
            //List<B_Subscription> result = B_SubscriptionOperator.Instance.GetallSubscriptionList().ToList();

           

            int times = 30;
            try
            {
                string tim = ConfigurationManager.AppSettings["RecalculationDuration"];
                if (tim != null && tim != "")
                {
                    times = int.Parse(tim);
                }
                Common.ScheduleService.Log.Instance.Info("重新计算服务,在App.Config读取时间间隔成功{0}", times);
            }
            catch (Exception)
            {

                Common.ScheduleService.Log.Instance.Error("重新计算服务,在App.Config内读取时间间隔错误,请确保其为数字,没有空格.而且其单位为分钟");
            }

            List<A_TargetPlan> result = A_TargetplanOperator.Instance.GetListByRecalculation(times).ToList();


            int DataSum = 0;

            //重新计算，根据现有A表的实际数，计算A表计划的，得到新的counter的值

            try
            {

                if (result.Count() > 0)
                {
                    //首先看，有几个系统修改。
                    result.ForEach(p =>
                    {

                        //根据A_TargetPlan 获取明细表中的数据
                        List<A_TargetPlanDetail> TargetPlanDetail = A_TargetplandetailOperator.Instance.GetTargetplandetailList(p.ID).ToList();

                        //获取A表中实际上报的数据
                        List<A_MonthlyReportDetail> MonthlyReportDetailList = A_MonthlyreportdetailOperator.Instance.GetAMonthlyreportdetailList(p.SystemID, p.FinYear).ToList();

                        //将修改的计划表的计划值，赋值到填报A表的数据中，这里只更新累计计划指标
                        MonthlyReportDetailList = MonthlyReportDetailList.Select(D =>
                        {
                            var TargetSum = TargetPlanDetail.FindAll(t => t.FinMonth <= D.FinMonth && t.CompanyID == D.CompanyID && t.TargetID == D.TargetID && t.FinYear == D.FinYear ).Sum(s => s.Target);
                            D.NAccumulativePlanAmmount = TargetSum;
                            return D;
                        }).ToList();



                        int _month =  MonthlyReportDetailList.Max(M => M.FinMonth);

                        List<A_MonthlyReportDetail> Temp_ReportDetailList;

                        for (int i = 1; i <= _month; i++)
                        {

                            Temp_ReportDetailList = MonthlyReportDetailList.FindAll(MD => MD.FinYear == p.FinYear && MD.FinMonth == i);
                       
                            //重新计算，数据的counter的数
                            List<A_MonthlyReportDetail> A_MonthlyReportDetailList = ResetCalculationEvationEngine.ResetCalculationEvaluationService.ResetCalculation(Temp_ReportDetailList, MonthlyReportDetailList);


                            //这里Update A表的数据
                            //A_MonthlyReportDetailList 是新指标后，通过重新计算得到的数值，
                            //这里我们只需要更新NewCounter字段，所以这里需要重新组装下
                            //A_MonthlyReportDetailList


                            //首先获取原表数据
                            List<A_MonthlyReportDetail> MonthlyReportDetail = A_MonthlyreportdetailOperator.Instance.GetAMonthlyreportdetailList(p.SystemID, p.FinYear).ToList().FindAll(mr=>mr.FinMonth == i);

                            MonthlyReportDetail.ForEach(m =>
                            {
                                var tempModel = A_MonthlyReportDetailList.Find(f => f.CompanyID == m.CompanyID && f.FinYear == m.FinYear && f.FinMonth == m.FinMonth && f.TargetID == m.TargetID);

                                m.NewCounter = tempModel.NewCounter;

                            });


                            int count = A_MonthlyreportdetailOperator.Instance.UpdateMonthlyreportDetailList(MonthlyReportDetail);

                            DataSum = DataSum + count;

                            C_System _sys = StaticResource.Instance[p.SystemID,DateTime.Now];

                            if (count > 0)
                            {
                                Common.ScheduleService.Log.Instance.Info("Recalculation服务执行更新" + _sys.SystemName  + "系统数据" + count.ToString() + "条");
                            }
                            else
                            {
                                Common.ScheduleService.Log.Instance.Info("Recalculation服务执行更新数据0条");
                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Common.ScheduleService.Log.Instance.Info("Recalculation服务重新计算逻辑报错:"+ex.ToString());
            }
            Common.ScheduleService.Log.Instance.Info("Recalculation服务本次执行更新数据"+DataSum.ToString()+"条");

            Common.ScheduleService.Log.Instance.Info("Service execute finished");
        }
    }
}
